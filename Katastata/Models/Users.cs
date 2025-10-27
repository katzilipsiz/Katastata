using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Katastata.Models
{
    public class Users
    {
        public int id { get; set; }
        public string loginLogin { get; set; } = string.Empty;
        public string hash_password{ get; set; } = string.Empty;
        public DateTime created_at { get; set; } = DateTime.Now;
        public int system_id { get; set; }


    }
}
