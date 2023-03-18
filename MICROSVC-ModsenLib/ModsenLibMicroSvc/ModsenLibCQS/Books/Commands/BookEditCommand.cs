using MediatR;
using ModsenLibAbstractions.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModsenLibCQS.Books.Commands
{
    public class BookEditCommand : IRequest<int?>
    {
        public BookDto? BookDto { get; set; }
    }
}
