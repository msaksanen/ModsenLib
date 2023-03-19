using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModsenLibCQS.Books.Commands
{
    public class ChangeBookUserDataCommand : IRequest<int?>
    {
        public Guid? BookId { get; set; }
        public Guid? UserId { get; set; }
        public string? UserEmail { get; set; }
        public int BookPeriod { get; set; }
    }
}
