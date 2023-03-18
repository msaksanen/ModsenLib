using MediatR;
using ModsenLibAbstractions.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModsenLibCQS.Books.Commands
{
    public class RemoveBookCommand: IRequest<int?>
    {
        public Guid? BookId { get; set; }
}
}
