using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RedirectTest.Models.DbModels;
using System.Security.Claims;

namespace RedirectTest.Filters
{
    public class Authentication : Attribute, IAuthorizationFilter
    {
        private CircleScribeDbContext _db;

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                _db = context.HttpContext.RequestServices.GetService<CircleScribeDbContext>();

                if (context.HttpContext.User.Identity == null || !context.HttpContext.User.Identity.IsAuthenticated)
                {
                    context.Result = new StatusCodeResult(401);
                    context.HttpContext.Response.Headers.Add("Location", new Microsoft.Extensions.Primitives.StringValues("/RedirectTest/Accounts/Login"));
                    return;
                }

                List<Claim> claims = context.HttpContext.User.Claims.ToList();
                int userId = Convert.ToInt32(claims[0].Value);
                string email = claims[1].Value;

                User user = _db.Users.Where(u => u.Email == email).FirstOrDefault();
                if (user == null)
                {
                    context.Result = new StatusCodeResult(401);
                    return;
                }

                context.HttpContext.Items["Nickname"] = user.Nickname;
                context.HttpContext.Items["ProfileImageName"] = user.ProfileImageName;
            }
            catch
            {
                context.Result = new StatusCodeResult(500);
                return;
            }
        }
    }
}
