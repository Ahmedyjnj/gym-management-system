using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication2.Dtos
{
   public class PaymentRowDto
    {
        public int? Id { get; set; }
        public string SubscriberName { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime? PaymentDate { get; set; } 
    }
}
