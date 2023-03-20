using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModsenLibAbstractions.DataTransferObjects
{
    public class Result
    {
        public int? IntResult { get; set; }
        public string? Message { get; set; }

        public int? SaveChangesResult { get; set; }
    }
}
