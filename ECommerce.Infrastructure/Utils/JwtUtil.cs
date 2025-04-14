using ECommerce.Shared.Models;
using ECommerce.Shared.Payload.Request.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerce.Infrastructure.Utils
{
    public class JwtUtil
    {
        // Hằng số cho cấu hình JWT
        private const string ISSUER = "Issuer";
        private const string ACCESS_TOKEN_SECRET = "SuperStrongSecretKeyForJwtToken123!";
        private const string REFRESH_TOKEN_SECRET = "AnotherSuperSecretKeyForRefreshToken!";
        private const int ACCESS_TOKEN_HOURS = 6;
        private const int REFRESH_TOKEN_DAYS = 7;

        public class JwtResponse
        {
            public string AccessToken { get; set; }
            public DateTime AccessTokenExpires { get; set; }
            public string RefreshToken { get; set; }
            public DateTime RefreshTokenExpires { get; set; }
        }

        public static JwtResponse GenerateJwtToken(User user, string claimType = null, Guid? claimValue = null)
        {
            // Tạo khóa và thông tin đăng nhập
            var accessKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ACCESS_TOKEN_SECRET));
            var refreshKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(REFRESH_TOKEN_SECRET));
            var accessCredentials = new SigningCredentials(accessKey, SecurityAlgorithms.HmacSha256Signature);
            var refreshCredentials = new SigningCredentials(refreshKey, SecurityAlgorithms.HmacSha256Signature);

            // Thời gian hết hạn
            var accessTokenExpires = DateTime.UtcNow.AddHours(ACCESS_TOKEN_HOURS);
            var refreshTokenExpires = DateTime.UtcNow.AddDays(REFRESH_TOKEN_DAYS);

            // Tạo claims cơ bản
            var baseClaims = CreateBaseClaims(user);

            // Thêm claim tùy chỉnh nếu được cung cấp
            if (!string.IsNullOrEmpty(claimType) && claimValue.HasValue)
            {
                baseClaims.Add(new Claim(claimType, claimValue.Value.ToString()));
            }

            // Tạo token truy cập và token làm mới
            var accessToken = CreateToken(ISSUER, new List<Claim>(baseClaims), accessTokenExpires, accessCredentials);
            var refreshToken = CreateToken(ISSUER, new List<Claim>(baseClaims), refreshTokenExpires, refreshCredentials);

            return new JwtResponse
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                AccessTokenExpires = accessTokenExpires,
                RefreshToken = new JwtSecurityTokenHandler().WriteToken(refreshToken),
                RefreshTokenExpires = refreshTokenExpires
            };
        }

        public static JwtResponse RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var tokenValidationParams = new TokenValidationParameters
                {
                    ValidIssuer = ISSUER,
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(REFRESH_TOKEN_SECRET)),
                    RoleClaimType = ClaimTypes.Role
                };

                // Xác thực token
                var principal = tokenHandler.ValidateToken(refreshTokenRequest.RefreshToken, tokenValidationParams, out var validatedToken);

                if (validatedToken is not JwtSecurityToken jwtToken)
                {
                    throw new SecurityTokenException("Invalid refresh token");
                }

                // Xác thực thông tin người dùng
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = principal.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role) ||
                    !Guid.TryParse(userId, out var parsedUserId) ||
                    parsedUserId != refreshTokenRequest.UserId)
                {
                    throw new SecurityTokenException("Invalid refresh token data");
                }

                // Tạo token mới
                var user = new User
                {
                    Id = refreshTokenRequest.UserId,
                    Role = role
                };

                return GenerateJwtToken(user, "UserId", user.Id);
            }
            catch (Exception)
            {
                throw new SecurityTokenException("Invalid or expired refresh token");
            }
        }

        // Phương thức phụ trợ

        private static List<Claim> CreateBaseClaims(User user)
        {
            return new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
        }

        private static JwtSecurityToken CreateToken(
            string issuer,
            List<Claim> claims,
            DateTime expires,
            SigningCredentials credentials)
        {
            return new JwtSecurityToken(
                issuer: issuer,
                audience: null,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: credentials
            );
        }
    }
}