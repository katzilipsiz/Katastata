using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Katastata.Models
{
    public class Statistic
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ApplicationId { get; set; }
        public TimeSpan TotalTime { get; set; }
        public DateTime LastLaunched {  get; set; }

        public void UpdateFromSession(Sessions sessions)
        {
            TotalTime += sessions.GetDuration();
            LastLaunched = sessions.StartTime;
        }
    }
}
