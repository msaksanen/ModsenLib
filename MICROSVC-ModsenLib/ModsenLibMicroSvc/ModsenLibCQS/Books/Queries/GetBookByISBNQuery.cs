using MediatR;
using ModsenLibAbstractions.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModsenLibCQS.Books.Queries
{
    public class GetBookByISBNQuery : IRequest<BookDto?>
    {
        public string? BookISBN { get; set; }
    }
}
