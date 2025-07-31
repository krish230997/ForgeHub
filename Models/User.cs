using System.ComponentModel.DataAnnotations;

namespace ForgeHub.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string Role { get; set; } // "Buyer" or "Vendor"


        public bool IsFirstTimeLogin { get; set; } = true;

        public string? SecretKey { get; set; }
        // Navigation
        public List<RFQ> RFQs { get; set; } // For Buyers
        public List<RFQQuotation> RFQQuotations { get; set; } // For Vendors
    }
}
