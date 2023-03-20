using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModsenLibAbstractions.DataTransferObjects
{
    public class BookDto
    {
        public Guid Id { get; set; }
        public BookPassportDto? BookPassportDto { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Description { get; set; }
        public string? Genre { get; set; }
        public string? ISBN { get; set; }
        public DateTime? CreationDate { get; set; }
        public bool? IsTaken { get; set; }
    }
}
