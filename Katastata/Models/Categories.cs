using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Katastata.Models
{
    public class Categories
    {
        public int id { get; set; }
        public string title { get; set; } = "Новая категория";
        public string description { get; set; } = string.Empty;
        public string icon_path { get; set; } = string.Empty;
    }
}