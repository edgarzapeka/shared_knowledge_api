using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharedKnowledgeAPI.Data;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SharedKnowledgeAPI.Services
{
    public class CustomAuthorizationHelper
    {
        public class ClaimRequirementAttribute : TypeFilterAttribute
        {
            public ClaimRequirementAttribute(string claimType, string claimValue) : base(typeof(ClaimRequirementFilter))
            {
                Arguments = new object[] { new Claim(claimType, claimValue) };
            }
        }

        // The authorization filter allows us to handle custom authorization tasks.
        public class ClaimRequirementFilter : IAuthorizationFilter
        {
            readonly Claim _claim;
            ApplicationDbContext _dbcontext;

            public ClaimRequirementFilter() { }
            public ClaimRequirementFilter(Claim claim, ApplicationDbContext dbcontext)
            {
                _claim = claim;
                this._dbcontext = dbcontext;
            }

            public void OnAuthorization(AuthorizationFilterContext context)
            {
                // This 'if' statement is not actually needed. I just want to show that 
                // you can re-use the customizable ClaimRequirementFilter for many different types of 
                // authentication scenarios such as roles checking or even for distinguishing between
                // web and mobile users. This is in case you want to handle different types of users 
                // differently.
                if (_claim.Type == "Custom Validator" && _claim.Value == "Logged In")
                {

                    var headers = context.HttpContext.Request.Headers["HeaderAuthorization"];

                    // We are not actually using the token for anything here but this is how
                    // you can read it if needed.
                    var token = context.HttpContext.Request.Headers["Authorization"]
                                        .ToString().Replace("Bearer ", "");

                    // The secret is received from the client. It should match the 
                    // value in the SecurityStamp column of the identity table.
                    var secret = context.HttpContext.Request.Headers["secret"].ToString();

                    // These instructions convert token into plain text so you can read the token's
                    // data.
                    var handler = new JwtSecurityTokenHandler();
                    var tokenS = handler.ReadJwtToken(token) as JwtSecurityToken;
                    var userName = tokenS.Claims.First(claim => claim.Type == "sub").Value;


                    var validUser = _dbcontext.Users.Where(u => u.UserName == userName
                                                        && u.SecurityStamp == secret);

                    if (validUser.ToList().Count == 0)
                    {
                        context.Result = new ForbidResult();
                    }
                }
            }
        }

    }
}
