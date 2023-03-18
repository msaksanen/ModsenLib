using AutoMapper;
using MediatR;
using ModsenLibGateWayDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModsenLibGateWayCQS.Tokens.Commands
{
    public class RemoveOldRefreshTokensCommandHandler : IRequestHandler<RemoveOldRefreshTokensCommand, int>
    {
        private readonly ModsenLibGateWayContext _context;

        public RemoveOldRefreshTokensCommandHandler(ModsenLibGateWayContext context, IMapper mapper)
        {
            _context = context;
        }

        public async Task<int> Handle(RemoveOldRefreshTokensCommand command, CancellationToken token)
        {
            if (_context.RefreshTokens != null && _context.RefreshTokens.Any())
            {
                var oldTokens = _context.RefreshTokens.Where(t => t.CreationDate == null ||
                                t.CreationDate.Value.AddHours(command.ExpHours) < DateTime.Now);
                _context.RefreshTokens.RemoveRange(oldTokens);
            }

            var res = await _context.SaveChangesAsync(token);
            return res;
        }
    }
}
