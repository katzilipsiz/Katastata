using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Katastata.Models
{
    public class Sessions
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int app_id { get; set; }
        public DateTime begin_at { get; set; }
        public DateTime? end_at { get; set; }
        public TimeSpan duration { get; set; }


        public TimeSpan GetDuration()
        {
            return end_at.HasValue ? end_at.Value - begin_at : DateTime.Now - begin_at;
        }
    }
}
