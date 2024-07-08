using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace SocialNetwork.Filters
{
    public class Authentication : Attribute, IAuthorizationFilter
    {
        private IConfiguration _configuration;
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            _configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();

            try
            {
                string JWT_Token = context.HttpContext.Request.Headers["Authorization"].ToString();
                var claim = VerifyJwtToken(JWT_Token.Split(' ')[1]);
                var a = claim;
            }
            catch
            {
                context.Result = new StatusCodeResult(401);
                return;
            }
        }

        private ClaimsPrincipal VerifyJwtToken(string jwtToken)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportFromPem(_configuration["PublicKey"]);
                RsaSecurityKey rsaSecurityKey = new RsaSecurityKey(rsa) { CryptoProviderFactory = new CryptoProviderFactory() { CacheSignatureProviders = false } };

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "ArticleApp",
                    ValidAudience = "CommentsApp",
                    IssuerSigningKey = rsaSecurityKey
                };

                var handler = new JwtSecurityTokenHandler();
                var claimsPrincipal = handler.ValidateToken(jwtToken, validationParameters, out SecurityToken validatedToken);
                return claimsPrincipal;
            }

        }
    }
}
