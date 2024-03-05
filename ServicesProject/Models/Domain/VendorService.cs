using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesProject.Models.Domain {
    public class VendorService {
        public int VendorServiceId { get; set; }
        public int VendorId { get; set; }
        public int ServiceId { get; set; }
    }
}
