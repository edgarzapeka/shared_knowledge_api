using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using SharedKnowledgeAPI.Data;
using SharedKnowledgeAPI.Models;
using SharedKnowledgeAPI.Models.AccountViewModels;
using SharedKnowledgeAPI.Services;
using static SharedKnowledgeAPI.Services.CustomAuthorizationHelper;

namespace SharedKnowledgeAPI.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IConfiguration configuration, ApplicationDbContext context, IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
            _emailSender = emailSender;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<object> Login([FromBody] LoginViewModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

            if (result.Succeeded)
            {
                ApplicationUser appUser = _context.ApplicationUser.SingleOrDefault(r => r.Email == model.Email);

                return await GenerateUserState(model.Email);
            }
            else
            {
                return new JsonResult(new { message = "Wrong Credentials" });
            }
        }

        [HttpPost]
        public async Task<object> Register([FromBody] RegisterViewModel model)
        {
            var result = await _userManager.CreateAsync(new ApplicationUser {
                UserName = model.Email,
                Email = model.Email,
                Karma = 0,
            }, model.Password);

            if (result.Succeeded)
            {
                ApplicationUser appUser = _context.ApplicationUser.SingleOrDefault(r => r.Email == model.Email);
                appUser.UserRole = "User";
                appUser.CustomUserName = "User";
                _context.SaveChanges();

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
                var callbackUrl = Url.EmailConfirmationLink(appUser.Id, code, Request.Scheme);
                await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                return await GenerateUserState(model.Email);
            }
            else
            {
                return new JsonResult(new { message = "Wrong Credentials" });
            }
        }

        private async Task<object> GenerateUserState(string email)
        {
            ApplicationUser user = _context.ApplicationUser.SingleOrDefault(r => r.Email == email);
            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["TokenInformation:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //var expires = DateTime.Now.AddDays(Convert.ToDouble(1));
            var expires = DateTime.Now.AddMonths(1);
            var token = new JwtSecurityToken(
                _configuration["TokenInformation:Issuer"],
                _configuration["TokenInformation:Audience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );
            var formattedToken = new JwtSecurityTokenHandler().WriteToken(token);
            //return Ok(new { token = formattedToken, secret = user.SecurityStamp, id = user.Id });
            return Ok(new UserState() { Id = user.Id, Email = user.Email, Name = user.CustomUserName, Karma = user.Karma, PhoneNumber = user.PhoneNumber, Token = formattedToken, Secret = user.SecurityStamp, UserRole = user.UserRole });
        }

        [HttpPost]
        public Task<object> UpdateUser([FromBody]UserState data)
        {
            ApplicationUser user = _context.ApplicationUser.Where(au => au.Id == data.Id).FirstOrDefault();
            if (user != null)
            {
                user.Email = data.Email;
                user.CustomUserName = data.Name;
                user.PhoneNumber = data.PhoneNumber;
                Task<IdentityResult> result = _userManager.UpdateAsync(user);
                if (result.Result.Succeeded)
                {
                    return GenerateUserState(user.Email);
                }
            }

            throw new ApplicationException("Invalid Registration Attempt");
        }

        [HttpGet]
        public async Task<IActionResult> GetResetPasswordLink(string email)
        {
            ApplicationUser user = _context.ApplicationUser.Where(au => au.Email == email).FirstOrDefault();
            if (user != null)
            {
                string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                try
                {
                    MailMessage mailMsg = new MailMessage();

                    // To
                    mailMsg.To.Add(new MailAddress(user.Email, user.CustomUserName));

                    // From
                    mailMsg.From = new MailAddress("sharedknowledge@thebest.com", "Dear Administration");

                    // Subject and multipart/alternative Body
                    mailMsg.Subject = "Shared Knowledge | Reset Password";
                    string text = String.Format("Yo %s this is your link to rest password", user.CustomUserName);
                    string html = String.Format(@"<a href='http://localhost:3000/resetpassword/{0}/{1}'>Reset password</a>", user.Email, System.Web.HttpUtility.UrlEncode(passwordResetToken));
                    mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
                    mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

                    // Init SmtpClient and send
                    SmtpClient smtpClient = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
                    System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("thebardaland", "Bcit123!");
                    smtpClient.Credentials = credentials;

                    smtpClient.Send(mailMsg);

                    return new OkResult();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return new NotFoundResult();
                }
            }
            return new NotFoundResult();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string email)
        {
            ApplicationUser user = _context.ApplicationUser.Where(au => au.Email == email).FirstOrDefault();
            if (user != null)
            {
                try
                {
                    MailMessage mailMsg = new MailMessage();

                    // To
                    mailMsg.To.Add(new MailAddress(user.Email, user.CustomUserName));

                    // From
                    mailMsg.From = new MailAddress("sharedknowledge@thebest.com", "Dear Administration");

                    // Subject and multipart/alternative Body
                    mailMsg.Subject = "Shared Knowledge | Reset Password";
                    string text = String.Format("Yo %s this is your link to rest password", user.CustomUserName);
                    string html = String.Format(@"<a href='http://localhost:3000/confirmemail/{0}/'>Reset password</a>", user.Email);
                    mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
                    mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

                    // Init SmtpClient and send
                    SmtpClient smtpClient = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
                    System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("thebardaland", "Bcit123!");
                    smtpClient.Credentials = credentials;

                    smtpClient.Send(mailMsg);

                    return new OkResult();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return new NotFoundResult();
                }
            }
            return new NotFoundResult();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmailAction(string email)
        {
            ApplicationUser user = _context.ApplicationUser.Where(au => au.Email == email).FirstOrDefault();
            if (user != null)
            {
                string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await _userManager.ConfirmEmailAsync(user, token);
                return new OkResult();
            }

            return new NotFoundResult();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody]JObject json)
        {
            string email = json.GetValue("email").ToString();
            string token = System.Web.HttpUtility.UrlDecode(json.GetValue("token").ToString());
            string newPassword = json.GetValue("newPassword").ToString();
            string confirmPassword = json.GetValue("confirmPassword").ToString();
            
            ApplicationUser user = _context.ApplicationUser.Where(au => au.Email == email).FirstOrDefault();
            if (user != null)
            {
                token = await _userManager.GeneratePasswordResetTokenAsync(user);
                IdentityResult result = await _userManager.ResetPasswordAsync(user, token, newPassword);
                return new OkResult();
            }

            return new NotFoundResult();
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            return new JsonResult(_context.ApplicationUser.ToList());
        }

        [HttpPost]
        public IActionResult ChangeUserRole([FromBody]JObject json)
        {
            string email = json.GetValue("email").ToString();
            string userRole = json.GetValue("userRole").ToString();

            ApplicationUser user = _context.ApplicationUser.Where(au => au.Email == email).FirstOrDefault();
            if (user != null)
            {
                user.UserRole = userRole;
                _context.SaveChanges();
                return new JsonResult(user);
            }

            return new JsonResult(new{  message = "User not found" });
        }
    }
}