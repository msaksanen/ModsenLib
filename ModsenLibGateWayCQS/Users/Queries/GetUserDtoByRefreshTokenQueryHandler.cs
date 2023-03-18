using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ModsenLibGateWayAbstractions.DataTransferObjects;
using ModsenLibGateWayDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModsenLibGateWayCQS.Users.Queries
{
    public class GetUserDtoByRefreshTokenQueryHandler : IRequestHandler<GetUserDtoByRefreshTokenQuery, UserDto?>
    {
        private readonly ModsenLibGateWayContext _context;
        private readonly IMapper _mapper;

        public GetUserDtoByRefreshTokenQueryHandler(ModsenLibGateWayContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserDto?> Handle(GetUserDtoByRefreshTokenQuery request,
            CancellationToken cts)
        {
            if (_context.RefreshTokens != null && _context.RefreshTokens.Any())
            {
                var refreshToken = await _context.RefreshTokens
                                         .Include(token => token.User)
                                         .AsNoTracking()
                                         .FirstOrDefaultAsync(token => token.Token.Equals(request.RefreshToken), cts);

                if (refreshToken != null && refreshToken.User != null)
                {
                    var user = refreshToken.User;
                    return _mapper.Map<UserDto>(user);
                }
            }
            return null;
        }
    }
}
