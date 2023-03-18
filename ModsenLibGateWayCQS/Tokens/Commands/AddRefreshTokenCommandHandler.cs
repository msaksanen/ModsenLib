using AutoMapper;
using MediatR;
using ModsenLibGateWayDb;
using ModsenLibGateWayDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModsenLibGateWayCQS.Tokens.Commands
{
    public class AddRefreshTokenCommandHandler
     : IRequestHandler<AddRefreshTokenCommand, int?>
    {
        private readonly ModsenLibGateWayContext _context;
        private readonly IMapper _mapper;

        public AddRefreshTokenCommandHandler(ModsenLibGateWayContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int?> Handle(AddRefreshTokenCommand command, CancellationToken token)
        {
            if (command.TokenValue == null || command.UserId == null)
                return null;
            var rt = new RefreshToken()
            {
                Id = Guid.NewGuid(),
                Token = (Guid)command.TokenValue,
                UserId = (Guid)command.UserId,
                CreationDate = DateTime.Now
            };

            if (_context.RefreshTokens != null)
                await _context.RefreshTokens.AddAsync(rt, token);
            var res = await _context.SaveChangesAsync(token);
            return res;
        }
    }
}
