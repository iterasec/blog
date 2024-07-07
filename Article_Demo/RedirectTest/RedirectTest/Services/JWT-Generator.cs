using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace RedirectTest.Services
{
    public class JWT_Generator
    {

        private IConfiguration _configuration;

        public JWT_Generator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(int UserId,string UserName,int articleId)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportFromPem(_configuration["PrivateKey"]);
                RsaSecurityKey rsaPrivateKey = new RsaSecurityKey(rsa);

                SigningCredentials credentials = new SigningCredentials(rsaPrivateKey, SecurityAlgorithms.RsaSha256) { CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false } };

                List<Claim> claims = new List<Claim>{
                new Claim("UserId",UserId.ToString()),
                new Claim("Username",UserName),
                new Claim("ArticleId",articleId.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),ClaimValueTypes.Integer64)
                };

                JwtSecurityToken token = new JwtSecurityToken(
                    issuer: "ArticleApp",
                    audience: "CommentsApp",
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(60),
                    signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }          
        }
    }
}
