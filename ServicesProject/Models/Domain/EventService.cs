using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesProject.Models.Domain {
    public class EventService {
        public int EventServiceId { get; set; }
        public int EventId { get; set; }
        public int ServiceId { get; set; }
        public int Quantity { get; set; }
    }
}
