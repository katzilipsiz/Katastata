using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Katastata.Models
{
    public class Applications
    {
        public int id { get; set; }
        public string title { get; set; } = string.Empty;
        public string process { get; set; } = string.Empty;
        public int category_id { get; set; } = 1;
        public string? icon_path { get; set; }
        public string? description { get; set; }
        public string? notes { get; set; }
    }
}