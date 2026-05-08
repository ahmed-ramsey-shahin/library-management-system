using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Users.Dtos;
using Lms.Domain.Common.Results;
using Lms.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Lms.Infrastructure.Identity
{
    public class TokenProvider(IConfiguration configuration, IAppDbContext db) : ITokenProvider
    {
        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }

        private async Task<Result<TokenDto>> CreateAsync(UserDto user, CancellationToken cancellationToken)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var key = jwtSettings["Secret"];
            var expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["TokenExpirationInMinutes"]!));
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(ClaimTypes.Role, user.Role.ToString()),
            };
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)),
                    SecurityAlgorithms.HmacSha256Signature
                ),
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(descriptor);
            var oldRefreshToken = await db.RefreshTokens
                .Where(token => token.UserId == user.UserId)
                .ExecuteDeleteAsync(cancellationToken);
            var refreshTokenResult = RefreshToken.Create(
                Guid.NewGuid(),
                GenerateRefreshToken(),
                user.UserId,
                DateTimeOffset.UtcNow.AddDays(7)
            );

            if (refreshTokenResult.IsError)
            {
                return refreshTokenResult.Errors!;
            }

            var refreshToken = refreshTokenResult.Value;
            db.RefreshTokens.Add(refreshToken);
            await db.SaveChangesAsync(cancellationToken);
            return new TokenDto
            {
                ExpiresOn = expires,
                RefreshToken = refreshToken.Token,
                AccessToken = tokenHandler.WriteToken(securityToken),
            };
        }

        public async Task<Result<TokenDto>> GenerateJwtTokenAsync(UserDto user, CancellationToken cancellationToken)
        {
            var tokenResult = await CreateAsync(user, cancellationToken);

            if (tokenResult.IsError)
            {
                return tokenResult.Errors!;
            }

            return tokenResult.Value;
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Secret"]!)),
                ValidateIssuer = true,
                ValidIssuer = configuration["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["JwtSettings:Audience"],
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
