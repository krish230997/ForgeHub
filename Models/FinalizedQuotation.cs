using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForgeHub.Models
{
    public class FinalizedQuotation
    {
        [Key]
        public int FinalId { get; set; }

        public DateTime? FinalizedDate { get; set; } = DateTime.Now;

        // Navigation
        [ForeignKey("RFQ")]
        public int RFQId { get; set; }
        public RFQ RFQ { get; set; }

        [ForeignKey("RFQQuotation")]
        public int QuotationId { get; set; }
        public RFQQuotation RFQQuotation { get; set; }
    }
}
