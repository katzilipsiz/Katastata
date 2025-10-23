using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Katastata.Models
{
    public class Applications
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Process { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public string IconPath { get; set; } = string.Empty;
        public int CategoryId { get; set; } = 1;
    }
}