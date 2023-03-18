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
    public class GetRoleByNameQueryHandler : IRequestHandler<GetRoleByNameQuery, RoleDto?>
    {
        private readonly ModsenLibGateWayContext _context;
        private readonly IMapper _mapper;

        public GetRoleByNameQueryHandler(ModsenLibGateWayContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<RoleDto?> Handle(GetRoleByNameQuery request,
            CancellationToken cts)
        {
            if (_context.Roles != null && _context.Roles.Any())
            {
                var role = await _context.Roles.FirstOrDefaultAsync(role => role.Name != null
                                                && role.Name.Equals(request.RoleName));
                return _mapper.Map<RoleDto>(role);
            }
            return null;
        }
    }
}
