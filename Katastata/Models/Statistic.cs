using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Katastata.Models
{
    public class Statistic
    {
        public int id { get; set; }
        public int app_id{ get; set; }
        public int user_id{ get; set; }
        public DateTime last_launch_at {  get; set; }
        public TimeSpan total_time{ get; set; }

        public void UpdateFromSession(Sessions sessions)
        {
            total_time += sessions.GetDuration();
            last_launch_at = sessions.begin_at;
        }
    }
}
