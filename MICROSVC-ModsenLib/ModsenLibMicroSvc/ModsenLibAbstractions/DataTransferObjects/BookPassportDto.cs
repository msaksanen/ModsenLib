using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModsenLibAbstractions.DataTransferObjects
{
    public class BookPassportDto
    {
        public Guid Id { get; set; }
        public BookDto? Book { get; set; }
        public Guid? BookId { get; set; }
        public Guid? UserId { get; set; }
        public string? UserEmail { get; set; }
        public DateTime? TakenDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool? IsTaken { get; set; }
    }
}
