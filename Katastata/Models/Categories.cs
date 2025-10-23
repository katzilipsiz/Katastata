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
        public int Id { get; set; }
        public string Title { get; set; } = "Новая категория";
        public string Description { get; set; } = string.Empty;
        public string IconPath { get; set; } = string.Empty;
    }
}