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

namespace ModsenLibGateWayCQS.Roles.Queries
{
    public class GetRoleListByUserIdQueryHandler : IRequestHandler<GetRoleListByUserIdQuery, IEnumerable<RoleDto>?>
    {
        private readonly ModsenLibGateWayContext _context;
        private readonly IMapper _mapper;

        public GetRoleListByUserIdQueryHandler(ModsenLibGateWayContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RoleDto>?> Handle(GetRoleListByUserIdQuery request,
            CancellationToken cts)
        {
            if (_context.Roles != null && _context.Roles.Any())
            {
                var list = await _context.Roles
                           .Include(r => r.Users)
                           .AsNoTracking()
                           .ToListAsync();

                var lst = from role in list
                          from user in role.Users!
                          where user.Id.Equals(request.UserId)
                          select _mapper.Map<RoleDto>(role);
                return lst;
            }
            return null;
        }
    }
}
