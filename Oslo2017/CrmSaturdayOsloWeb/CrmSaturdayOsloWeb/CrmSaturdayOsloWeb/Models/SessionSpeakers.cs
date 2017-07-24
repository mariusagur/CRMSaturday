using System;
using System.Collections.Generic;

namespace CrmSaturdayOsloWeb.Models
{
    public partial class SessionSpeakers
    {
        public int SpeakerId { get; set; }
        public int SessionId { get; set; }

        public virtual Sessions Session { get; set; }
        public virtual Speakers Speaker { get; set; }
    }
}
