using MediatR;
using ModsenLibGateWayAbstractions.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModsenLibGateWayCQS.Roles.Queries
{
    public class GetRoleByNameQuery : IRequest<RoleDto?>
    {
        public string? RoleName { get; set; }
    }
}
