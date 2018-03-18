using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SharedKnowledgeAPI.Models;
using SharedKnowledgeAPI.Models.AccountViewModels;
using static SharedKnowledgeAPI.Services.CustomAuthorizationHelper;

namespace SharedKnowledgeAPI.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration; 

        public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
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
                var appUser = _userManager.Users.SingleOrDefault(r => r.Email == model.Email);
                return await GenerateJwtToken(model.Email, appUser);
            }

            throw new ApplicationException("Invalid Login Attempt");
        }

        [HttpPost]
        public async Task<object> Register([FromBody] RegisterViewModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return await GenerateJwtToken(model.Email, user);
            }

            throw new ApplicationException("Invalid Registration Attempt");
        }

        private async Task<object> GenerateJwtToken(string email, IdentityUser user)
        {
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
            return Ok(new { token = formattedToken, secret = user.SecurityStamp });
        }

        public List<LoginViewModel> GetFakeData()
        {
            List<LoginViewModel> logins = new List<LoginViewModel>();
            logins.Add(new LoginViewModel()
            {
                Email = "bob@home.com",
                Password = "password"
            });
            logins.Add(new LoginViewModel()
            {
                Email = "fakeuser@home.com",
                Password = "password"
            });
            return logins;
        }

        // This Action method does not require authentication.

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ClaimRequirement("Custom Validator", "Logged In")]
        [HttpGet]
        public IEnumerable<LoginViewModel> Public()
        {
            return GetFakeData();
        }


    }
}