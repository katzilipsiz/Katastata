using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Katastata.Models
{
    public class Application
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Path { get; set; } = string.Empty;
        public int CategoryId { get; set; }
    }
}