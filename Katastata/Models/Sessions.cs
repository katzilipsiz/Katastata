using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Katastata.Models
{
    public class Sessions
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ApplicationId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan Duration { get; set; }


        public TimeSpan GetDuration()
        {
            return EndTime.HasValue ? EndTime.Value - StartTime : DateTime.Now - StartTime;
        }
    }
}
