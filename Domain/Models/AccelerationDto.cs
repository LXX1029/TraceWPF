using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraceWPF.Domain.Models
{
    public class AccelerationDto
    {
        public string ts { get; set; }
        public double orginalvalue { get; set; }
        public string acquistnumber { get; set; }
        public string channelnumber { get; set; }
        public int deviceId { get; set; }
        public string diagnosepointId { get; set; }

    }
}
