using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Katastata.Models
{
    public class SystemInfo
    {
        public int id { get; set; }
        public string gpu { get; set; } = string.Empty;
        public string cpu { get; set; } = string.Empty;
        public string ram { get; set; } = string.Empty;
        public string mb { get; set; } = string.Empty;
        public string oc { get; set; } = string.Empty;
    }
}
