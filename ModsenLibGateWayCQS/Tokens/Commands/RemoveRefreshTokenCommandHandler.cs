using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ModsenLibGateWayDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModsenLibGateWayCQS.Tokens.Commands
{
    public class RemoveRefreshTokenCommandHandler
     : IRequestHandler<RemoveRefreshTokenCommand, int?>
    {
        private readonly ModsenLibGateWayContext _context;
        private readonly IMapper _mapper;

        public RemoveRefreshTokenCommandHandler(ModsenLibGateWayContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int?> Handle(RemoveRefreshTokenCommand command, CancellationToken token)
        {
            if (_context.RefreshTokens != null && _context.RefreshTokens.Any())
            {
                var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => command.TokenValue.Equals(rt.Token),
                    token);
                if (refreshToken != null)
                    _context.RefreshTokens.Remove(refreshToken);
            }

            var res = await _context.SaveChangesAsync(token);
            return res;
        }
    }
}
