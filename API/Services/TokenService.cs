using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        // symmetric key same key is to used for both signing and reading encrrypted msgs
        // _key lives on the server
        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<AppUser> _userManager;

        public TokenService(
            IConfiguration config,
            UserManager<AppUser> userManager
            )
        {
            //is this the signature? rendomly generated?
            // what is "TokenKEy"
            // _key = signature
            _key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(config["TokenKey"]));
            _userManager = userManager;
        }

        public async Task<string>CreateToken(AppUser user)
        {
            // identifyung what claims are put in the token
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
                
            };

            var roles = await _userManager.GetRolesAsync(user);
            //passing role
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));



            //encrypts _key
            var cred = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = cred
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}