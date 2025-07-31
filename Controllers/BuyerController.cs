using ForgeHub.Data;
using ForgeHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForgeHub.Controllers
{
    [Authorize(Roles = "Buyer")]
    public class BuyerController : Controller
    {
        ApplicationDbContext db;
        public BuyerController(ApplicationDbContext db)
        { 
            this.db = db;           
        }
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult AddRFQ()
        {
            var vendors = db.Users.Where(u => u.Role == "Vendor").ToList();
            ViewBag.Vendors = vendors;

            int currentYear = DateTime.Now.Year % 100;
            int currentMonth = DateTime.Now.Month;

            int existingCount = db.RFQs.Count();
            int nextNumber = existingCount + 1;

            string rfqNo = $"RFQ/{currentYear}/00000{nextNumber}";
            string indentNo = $"IND/{currentYear}/{currentMonth}/000{nextNumber}";

            var model = new RFQ
            {
                RFQNo = rfqNo,
                IndentNo = indentNo
            };

            return View(model);
        }
        [HttpPost]
        public IActionResult AddRFQ(RFQ rfq, List<int> SelectedVendors)
        {
            int currentYear = DateTime.Now.Year % 100;
            int currentMonth = DateTime.Now.Month;

            int existingCount = db.RFQs.Count();
            int nextNumber = existingCount + 1;

            rfq.RFQNo = $"RFQ/{currentYear}/00000{nextNumber}";
            rfq.IndentNo = $"IND/{currentYear}/{currentMonth}/000{nextNumber}";
            rfq.UserId = (int)(HttpContext.Session.GetInt32("UserId"));

            db.RFQs.Add(rfq);
            db.SaveChanges();



            foreach (var vendorId in SelectedVendors)
            {
                var mapping = new VendorRFQMapping
                {
                    RFQId = rfq.RFQId,
                    VendorId = vendorId
                };
                db.RFQVendors.Add(mapping);
            }

            db.SaveChanges();



            ViewBag.Vendors = db.Users.Where(u => u.Role == "Vendor").ToList();
            return RedirectToAction("FetchRFQ");
        }

        public IActionResult FetchRFQ()
        {
            int id = (int)HttpContext.Session.GetInt32("UserId");
            var data = db.RFQs.Include(x => x.users).Where(x => x.users.Role == "Buyer" && x.users.UserId == id && x.Status=="Open").ToList();
            
            return View(data);
        }
        public IActionResult FetchMyQuotations()
        {
            int UserId = (int)HttpContext.Session.GetInt32("UserId");

            var data = db.RFQQuotations.Include(q => q.Vendor).Include(q => q.RFQ).Where(q => q.RFQ.UserId == UserId && q.RFQ.Status == "Open").ToList();

            return View(data);
        }

        public IActionResult FetchMyFinalizedQuitations()
        {
            int userId = (int)HttpContext.Session.GetInt32("UserId");

            var data = db.FinalizedQuotations.Include(f => f.RFQ).Include(f => f.RFQQuotation).Include(q => q.RFQQuotation.Vendor) .Where(f => f.RFQ.UserId == userId).ToList();

            return View(data);
        }

    }
}

