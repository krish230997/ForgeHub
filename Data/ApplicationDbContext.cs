using ForgeHub.Models;
using Microsoft.EntityFrameworkCore;

namespace ForgeHub.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<RFQ> RFQs { get; set; }
        public DbSet<RFQQuotation> RFQQuotations { get; set; }
        public DbSet<FinalizedQuotation> FinalizedQuotations { get; set; }
        public DbSet<VendorRFQMapping> RFQVendors { get; set; }
        public DbSet<RFQItem> RFQItem { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           
            modelBuilder.Entity<RFQ>()
                .HasOne(r => r.users)
                .WithMany(u => u.RFQs)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            
            modelBuilder.Entity<RFQQuotation>()
                .HasOne(q => q.Vendor)
                .WithMany(u => u.RFQQuotations)
                .HasForeignKey(q => q.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

       
            modelBuilder.Entity<RFQQuotation>()
                .HasOne(q => q.RFQ)
                .WithMany(r => r.RFQQuotations)
                .HasForeignKey(q => q.RFQId)
                .OnDelete(DeleteBehavior.Restrict);

          
            modelBuilder.Entity<RFQ>()
                .HasOne(r => r.FinalizedQuotation)
                .WithOne(f => f.RFQ)
                .HasForeignKey<FinalizedQuotation>(f => f.RFQId)
                .OnDelete(DeleteBehavior.Restrict);

            
            modelBuilder.Entity<RFQQuotation>()
                .HasOne(q => q.FinalizedQuotation)
                .WithOne(f => f.RFQQuotation)
                .HasForeignKey<FinalizedQuotation>(f => f.QuotationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VendorRFQMapping>()
                .HasOne(rv => rv.RFQ)
                .WithMany(r => r.RFQVendors)
                .HasForeignKey(rv => rv.RFQId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VendorRFQMapping>()
                .HasOne(rv => rv.Vendor)
                .WithMany()
                .HasForeignKey(rv => rv.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RFQItem>()
            .HasOne(r => r.RFQ)
            .WithMany(r => r.RFQItems)
            .HasForeignKey(r => r.RFQId)
             .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
