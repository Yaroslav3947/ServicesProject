using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesProject.Models.Domain {
    public class Event {
        public int EventId { get; set; }
        public int ClientId { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string Venue { get; set; }
        public int TotalGuests { get; set; }
    }
}
