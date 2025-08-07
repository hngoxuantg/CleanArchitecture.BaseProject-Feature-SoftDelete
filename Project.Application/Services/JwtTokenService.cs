using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Project.Application.Interfaces.IServices;
using Project.Common.Options;
using Project.Domain.Entities;
using Project.Domain.Interfaces.IRepositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Project.Application.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly AppSettings _appSettings;
        private readonly UserManager<User> _userManager;
        public JwtTokenService(IOptions<AppSettings> appSettings, UserManager<User> userManager)
        {
            _appSettings = appSettings.Value;
            _userManager = userManager;
        }
        public async Task<string> GenerateJwtTokenAsync(User user, CancellationToken cancellation = default)
        {
            JwtSecurityTokenHandler jwtTokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_appSettings.JwtConfig.Secret);

            IList<string> roles = await _userManager.GetRolesAsync(user);

            List<Claim> claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Aud, _appSettings.JwtConfig.ValidAudience),
                new Claim(JwtRegisteredClaimNames.Iss, _appSettings.JwtConfig.ValidIssuer),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
            };

            for(int i = 0; i < roles.Count; i++) 
                claims.Add(new Claim(ClaimTypes.Role, roles[i]));

            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_appSettings.JwtConfig.TokenExpirationMinutes)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = jwtTokenHandler.CreateToken(securityTokenDescriptor);
            string jwtToken = jwtTokenHandler.WriteToken(token);
            return jwtToken;
        }
        public string GenerateRefreshToken()
        {
            byte[] randomBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }
    }
}
