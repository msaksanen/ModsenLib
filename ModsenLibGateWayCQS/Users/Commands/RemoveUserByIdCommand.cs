using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModsenLibGateWayCQS.Users.Commands
{
    public class RemoveUserByIdCommand : IRequest<int>
    {
        public Guid? Id { get; set; }
    }
}
