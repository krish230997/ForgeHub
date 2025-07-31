using ForgeHub.Data;
using ForgeHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForgeHub.Controllers
{
    [Authorize(Roles = "Vendor")]
    public class VendorController : Controller
    {
        ApplicationDbContext db;
        public VendorController(ApplicationDbContext db) 
        {
            this.db = db;
        }   
        public IActionResult Dashboard()
        {
            int vendorId = (int)(HttpContext.Session.GetInt32("UserId"));


            var rfqs = db.RFQVendors.Where(v => v.VendorId == vendorId).Include(v => v.RFQ).Include(r => r.RFQ.RFQItems).Include(r => r.RFQ.users).Select(v => v.RFQ).ToList();
            return View(rfqs);
        }
        [HttpPost]
        public IActionResult SubmitQuotation(RFQQuotation model)
        {
           

            int vendorId = (int)(HttpContext.Session.GetInt32("UserId"));

            var quotation = new RFQQuotation
            {
                RFQId = model.RFQId,
                VendorId = vendorId,
                BidNo = model.BidNo,
                QuotedAmount = model.QuotedAmount,
                DeliveryDate = model.DeliveryDate,
                PaymentTerms = model.PaymentTerms,
                Remarks = model.Remarks,
                SubmittedDate = DateTime.Now,
                Status = model.Status == "Accepted" ? "Accepted" : "Rejected"
            };

            db.RFQQuotations.Add(quotation);

            var rfqVendor = db.RFQVendors.FirstOrDefault(x => x.RFQId == model.RFQId && x.VendorId == vendorId);
            if (rfqVendor != null)
            {
                db.RFQVendors.Remove(rfqVendor);
            }

            db.SaveChanges();

            return RedirectToAction("Dashboard");
        }
        public IActionResult MySubmittedQuotations()
        {
            int vendorId = (int)(HttpContext.Session.GetInt32("UserId"));

            var quotations = db.RFQQuotations.Include(q => q.RFQ).Where(q => q.VendorId == vendorId).OrderByDescending(q => q.SubmittedDate).ToList();

            return View(quotations);
        }
        public IActionResult MyFinalizedQuotations()
        {
            int UserId = (int)(HttpContext.Session.GetInt32("UserId"));

            var data = db.FinalizedQuotations.Include(f => f.RFQ).Include(x=>x.RFQ.users)    .Include(f => f.RFQQuotation).Include(q => q.RFQQuotation.Vendor).Where(f => f.RFQQuotation.VendorId == UserId).ToList();

             return View(data);
         }

    }
}

