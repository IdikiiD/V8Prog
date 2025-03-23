using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using v8proj.BissnessLogic.Infrastructure.Abstractions;
using v8proj.BissnessLogic.Interfaces.JWT;
using v8proj.Core.Model.DTO.User;

namespace v8proj.BissnessLogic.Services.JWTService
{
    public class JwtService: IJwtService
    {
        private readonly ICookiesService _cookiesService;

        public JwtService(ICookiesService cookiesService)
        {
            _cookiesService = cookiesService;
        }

        public async Task<string> GenerateJwtToken(UserDto user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyHashing = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["JWTSecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim("Email", user.Email),
                    new Claim("RoleId", user.UserType.ToString()),
                }), 
                Expires = DateTime.UtcNow.AddHours(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(keyHashing),
                    SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<int> GetUserIdFromToken( )
        {
            var token = _cookiesService.getCookie("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return 0;
            }

            var idClaim = GetClaim("id", token);
            return idClaim == null ? 0 : int.Parse(idClaim.Value);
            
        }

        public async Task<int> GetUserRoleIdFromToken()
        {
            var token = _cookiesService.getCookie("JWTToken");
            
            if (string.IsNullOrEmpty(token))
                return 0;

            var roleClaim = GetClaim("RoleId", token);

            return roleClaim == null ? 0 : int.Parse(roleClaim.Value);
        }

        public async Task<string> GetUserEmailFromToken()
        {
            var token = _cookiesService.getCookie("JWTToken");
            
            if (string.IsNullOrEmpty(token))
                return "";

            var emailClaim = GetClaim("Email", token);

            return emailClaim == null ? "" : emailClaim.Value;
        }
        
        public Task<bool> IsTokenValid(string token)
        {
            if (string.IsNullOrEmpty(token))
                return Task.FromResult(false);
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyHashing = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["JwtSecretKey"]);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyHashing),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var expiration = jwtToken.ValidTo;

                return Task.FromResult(expiration > DateTime.UtcNow);
            }
            catch
            {
                return Task.FromResult(true);
            }
        }
        
        public async Task<bool> IsTokenValid() 
            => await IsTokenValid(_cookiesService.getCookie("jwt"));
        private Claim GetClaim(string claimType, string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.Claims.FirstOrDefault(claim => claim.Type == claimType);
        }
    }
}