using System;
using System.Collections.Generic;

namespace CrmSaturdayOsloWeb.Models
{
    public partial class Sessions
    {
        public Sessions()
        {
            Assessments = new HashSet<Assessments>();
            SessionSpeakers = new HashSet<SessionSpeakers>();
        }

        public int SessionId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? Schedule { get; set; }
        public string HandoutsUrl { get; set; }
        public int? Track { get; set; }

        public virtual ICollection<Assessments> Assessments { get; set; }
        public virtual ICollection<SessionSpeakers> SessionSpeakers { get; set; }
    }
}
