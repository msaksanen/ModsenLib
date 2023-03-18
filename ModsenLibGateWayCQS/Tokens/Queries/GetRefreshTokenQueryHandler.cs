using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ModsenLibGateWayDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModsenLibGateWayCQS.Tokens.Queries
{
    public class GetRefreshTokenQueryHandler : IRequestHandler<GetRefreshTokenQuery, Guid?>
    {
        private readonly ModsenLibGateWayContext _context;

        public GetRefreshTokenQueryHandler(ModsenLibGateWayContext context, IMapper mapper)
        {
            _context = context;
        }

        public async Task<Guid?> Handle(GetRefreshTokenQuery request,
            CancellationToken cts)
        {
            if (_context.RefreshTokens != null && _context.RefreshTokens.Any())
            {
                var refreshToken = await _context.RefreshTokens
                                         .FirstOrDefaultAsync(t => t.UserId.Equals(request.Id), cts);

                if (refreshToken != null)
                {

                    return refreshToken.Token;
                }
            }
            return null;
        }
    }
}
