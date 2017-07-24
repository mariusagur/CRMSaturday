using System;
using System.Collections.Generic;

namespace CrmSaturdayOsloWeb.Models
{
    public partial class Speakers
    {
        public Speakers()
        {
            SessionSpeakers = new HashSet<SessionSpeakers>();
        }

        public int SpeakerId { get; set; }
        public byte[] ProfilePicture { get; set; }
        public string ProfilePictureExtension { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string BlogUrl { get; set; }
        public string Titles { get; set; }
        public string Bio { get; set; }
        public string TwitterHandle { get; set; }

        public virtual ICollection<SessionSpeakers> SessionSpeakers { get; set; }
    }
}
