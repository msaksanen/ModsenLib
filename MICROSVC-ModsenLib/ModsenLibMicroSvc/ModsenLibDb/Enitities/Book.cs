using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModsenLibDb.Enitities
{
    public class Book : IBaseEntity
    {
        public Guid Id { get; set; } 
        public BookPassport? BookPassport { get; set; }    
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Description { get; set; }
        public string? Genre { get; set; }
        public string? ISBN { get; set; }
        public bool? IsTaken { get; set; }
        public DateTime? CreationDate { get; set; }

    }
}
