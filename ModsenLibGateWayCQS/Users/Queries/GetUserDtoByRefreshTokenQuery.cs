using MediatR;
using ModsenLibGateWayAbstractions.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModsenLibGateWayCQS.Users.Queries
{
    public class GetUserDtoByRefreshTokenQuery : IRequest<UserDto?>
    {
        public Guid? RefreshToken { get; set; }
    }
}
