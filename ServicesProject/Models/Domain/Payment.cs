using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesProject.Models.Domain {
    public class Payment {
        public int PaymentId { get; set; }
        public int ClientId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public int PaymentMethodId { get; set; }
    }
}
