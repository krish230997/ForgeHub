using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForgeHub.Models
{
    public class RFQItem
    {
        [Key]
        public int ItemId { get; set; }

        [ForeignKey("RFQ")]
        public int RFQId { get; set; }
        public RFQ RFQ { get; set; }

        public string RFQLineNo { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public int ReqQty { get; set; }
        public string UOM { get; set; }
        public DateTime ReqDeliveryDate { get; set; }
        public string? DeliveryLocation { get; set; }
        public string? Description { get; set; }
        public string? FactoryCode { get; set; }
    }
}
