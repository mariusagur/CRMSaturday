using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrmSaturdayOsloWeb.Models
{
    public class ScheduleDTO
    {
        public ScheduleDTO() { }
        public ScheduleDTO(Sessions Session)
        {
            this.Session = Session;
        }
        public ScheduleDTO(Sessions Session, params Speakers[] speakers)
        {
            this.Session = Session;
            this.Speakers = speakers;
        }

        public Speakers[] Speakers { get; set; }
        public Sessions Session { get; set; }
    }
}
