using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModsenLibGateWayCQS.Tokens.Queries
{
    public class GetRefreshTokenQuery : IRequest<Guid?>
    {
        public Guid Id { get; set; }
    }
}
