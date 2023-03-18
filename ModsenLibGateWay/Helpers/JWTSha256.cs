using MediatR;
using Microsoft.IdentityModel.Tokens;
using ModsenLibGateWay.Models;
using ModsenLibGateWayAbstractions.DataTransferObjects;
using ModsenLibGateWayCQS.Tokens.Commands;
using ModsenLibGateWayCQS.Tokens.Queries;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ModsenLibGateWay.Helpers
{

    /// <summary>
    /// JWTSha256 Helper
    /// </summary>
    public class JWTSha256
    {
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;

        /// <summary>
        /// JWTSha256 Ctor
        /// </summary>
        public JWTSha256(IConfiguration configuration,
            IMediator mediator)
        {
            _configuration = configuration;
            _mediator = mediator;
        }

        internal async Task<TokenResponse?> GenerateTokenAsync(UserDto dto, IEnumerable<RoleDto> roleList, string UId = "")
        {
            return await GenTokenAsync(dto, roleList, UId);
        }
        private async Task<TokenResponse?> GenTokenAsync(UserDto dto, IEnumerable<RoleDto> roleList, string UId = "")
        {
            string isfullBlocked = "false";
            var jwtSecret = _configuration["Token:JwtSecret"];
            if (jwtSecret == null) return null;
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var nowUtc = DateTime.UtcNow;
            var expiryMinutes = _configuration["Token:ExpiryMinutes"];
            if (expiryMinutes == null) return null;
            var exp = nowUtc.AddMinutes(double.Parse(expiryMinutes))
                .ToUniversalTime();
            if (dto.Email != null && roleList != null)
            {
                if (dto.IsFullBlocked == true)
                    isfullBlocked = "true";

                string id = dto.Id.ToString();
                if (UId.Equals(string.Empty))
                    UId = id;

                var claims = new List<Claim>()
               {
                 new Claim(JwtRegisteredClaimNames.Sub, dto.Email),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("D")), //jwt unique id from spec
                 new Claim(ClaimTypes.NameIdentifier, dto.Id.ToString("D")),
                 new Claim(ClaimTypes.Email, dto.Email),
                 new Claim("UId",id),
                 new Claim("FullBlocked", isfullBlocked),
                 //new Claim(ClaimsIdentity.DefaultNameClaimType, dto.Username)
               };

                foreach (var role in roleList)
                    if (role.Name != null)
                        claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Name));

                var jwtToken = new JwtSecurityToken(_configuration["Token:Issuer"],
                _configuration["Token:Issuer"],
                claims,
                expires: exp,
                signingCredentials: credentials);

                var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

                var refreshTokenValue = Guid.NewGuid();

                await _mediator.Send(new AddRefreshTokenCommand()
                {
                    UserId = dto.Id,
                    TokenValue = refreshTokenValue
                });

                return new TokenResponse()
                {
                    AccessToken = accessToken,
                    Roles = roleList.Select(r => r.Name).ToArray(),
                    TokenExpiration = jwtToken.ValidTo,
                    UserId = dto.Id,
                    RefreshToken = refreshTokenValue
                };
            }

            return null;
        }

        internal async Task RemoveRefreshTokenAsync(Guid userId, Guid? requestRefreshToken)
        {
            await DeleteRefreshTokenAsync(userId, requestRefreshToken);
        }
        private async Task DeleteRefreshTokenAsync(Guid userId, Guid? requestRefreshToken)
        {

            await _mediator.Send(new RemoveRefreshTokenCommand() { TokenValue = requestRefreshToken });
        }

        internal async Task<Guid?> GetRefreshToken(Guid id)
        {
            var token = await _mediator.Send(new GetRefreshTokenQuery() { Id = id });
            return token;
        }

        /// <summary>
        /// RemoveOldRefreshToken
        /// </summary>
        /// <param name="expHours int "></param>
        /// <returns></returns>
        public async Task<int> RemoveOldRefreshToken(int expHours)
        {
            var res = await _mediator.Send(new RemoveOldRefreshTokensCommand() { ExpHours = expHours });

            return res;
        }


    }
}
