using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraceWPF.Domain.Models
{
    public class TzzDto
    {
        public string ts { get; set; }
        public string tzzlist { get; set; }
        public int failureid { get; set; }
        public string acquistnumber { get; set; }
        public string channelnumber { get; set; }
        public int deviceid { get; set; }
        public int diagnosepointid { get; set; }
    }
}
