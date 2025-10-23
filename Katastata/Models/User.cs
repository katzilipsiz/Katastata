using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Katastata.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; } = DateTime.Now;
        public int SettingId { get; set; }
        public int SystemId { get; set; }


    }
}
