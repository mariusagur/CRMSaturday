using System;
using System.Collections.Generic;

namespace CrmSaturdayOsloWeb.Models
{
    public partial class Assessments
    {
        public int AssessmentId { get; set; }
        public string Attendee { get; set; }
        public int SessionId { get; set; }
        public string SpeakerFeedback { get; set; }
        public string SessionFeedback { get; set; }
        public byte? Rating { get; set; }

        public virtual Sessions Session { get; set; }
    }
}
