using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForgeHub.Models
{
    public class RFQQuotation
    {
        [Key]

        public int QuotationId { get; set; }

        public string? BidNo { get; set; }
        [Precision(18, 4)]
        public decimal? QuotedAmount { get; set; }
        public DateTime DeliveryDate { get; set; }

        public string? PaymentTerms { get; set; }
        public string? Remarks { get; set; }


        public string? Status { get; set; } // "Accepted", "Rejected", "Pending"
        public DateTime? SubmittedDate { get; set; } = DateTime.Now;
        // Navigation
        [ForeignKey("RFQ")]
        public int RFQId { get; set; }
        public RFQ RFQ { get; set; }

        [ForeignKey("Vendor")]
        public int VendorId { get; set; }
        public User Vendor { get; set; }


        public FinalizedQuotation FinalizedQuotation { get; set; }
    }
}
