using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForgeHub.Models
{
    public class VendorRFQMapping
    {
        [Key]
        public int Id { get; set; }


        [ForeignKey("RFQ")]
        public int RFQId { get; set; }
        public RFQ RFQ { get; set; }


        [ForeignKey("Vendor")]
        public int VendorId { get; set; }
        public User Vendor { get; set; }
    }
}
