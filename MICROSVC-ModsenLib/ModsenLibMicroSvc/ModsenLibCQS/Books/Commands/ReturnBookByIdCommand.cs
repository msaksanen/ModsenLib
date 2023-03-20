using MediatR;
using ModsenLibAbstractions.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModsenLibCQS.Books.Commands
{
    public class ReturnBookByIdCommand : IRequest<Result?>
    {
        public Guid? BookId { get; set; }
        public Guid? UserId { get; set; }
    }
}
