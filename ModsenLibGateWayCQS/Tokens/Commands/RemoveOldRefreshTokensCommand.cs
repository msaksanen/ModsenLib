using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModsenLibGateWayCQS.Tokens.Commands
{
    public class RemoveOldRefreshTokensCommand : IRequest<int>
    {
        public int ExpHours { get; set; }
    }
}
