using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesProject.Models.Domain {
    public class Vendor {
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string ContactPerson { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
