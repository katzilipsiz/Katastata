using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Katastata.Models
{
    public class SystemInfo
    {
        public int Id { get; set; }
        public string Gpu { get; set; } = string.Empty;
        public string Cpu { get; set; } = string.Empty;
        public string MotherBoard { get; set; } = string.Empty;
        public string Ram { get; set; } = string.Empty;
        public string OC { get; set; } = string.Empty;
    }
}
