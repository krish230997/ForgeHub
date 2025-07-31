using ForgeHub.Data;
using ForgeHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ForgeHub.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        ApplicationDbContext db;
        public AdminController(ApplicationDbContext db) 
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
        public IActionResult AddRFQ(RFQ rfq, List<int> SelectedVendors, string RFQItemsJson)
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

            if (!string.IsNullOrEmpty(RFQItemsJson))
            {
                var items = JsonSerializer.Deserialize<List<RFQItem>>(RFQItemsJson);

                foreach (var item in items)
                {
                    item.RFQId = rfq.RFQId; 
                    db.RFQItem.Add(item);
                }

                db.SaveChanges();
            }

           
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

            return RedirectToAction("FetchRFQ");
        }

        public IActionResult FetchRFQ()
        {
            var data = db.RFQs.Include(r => r.users).Where(r => r.Status == "Open").ToList();

            var rfqVendorGroups = db.RFQVendors.Include(v => v.Vendor).Where(v => data.Select(r => r.RFQId).Contains(v.RFQId)).ToList().GroupBy(v => v.RFQId).ToList(); 

            ViewBag.RFQVendorGroups = rfqVendorGroups;

            return View(data);
        }

        public IActionResult ViewVendorQuotations()
        {
            var quotations = db.RFQQuotations.Include(q => q.RFQ).Include(q => q.Vendor).Where(q => (q.Status == "Accepted" || q.Status == "Rejected") && q.RFQ.Status == "Open").OrderByDescending(q => q.SubmittedDate).ToList();

            return View(quotations);
        }
        [HttpPost]
        public IActionResult ApproveQuotation(int quotationId)
        {
  
            var quotation =db.RFQQuotations.Include(q => q.RFQ).FirstOrDefault(q => q.QuotationId == quotationId);

            

            var finalized = new FinalizedQuotation
            {
                RFQId = quotation.RFQId,
                QuotationId = quotation.QuotationId,
                FinalizedDate = DateTime.Now
            };

            db.FinalizedQuotations.Add(finalized);

       
            quotation.RFQ.Status = "Closed";

            db.SaveChanges();

            return RedirectToAction("ViewVendorQuotations"); 
        }


        public IActionResult EditRFQ(int id)
        {
            var rfq = db.RFQs.FirstOrDefault(x => x.RFQId == id);
          

            ViewBag.Vendors = db.Users.Where(u => u.Role == "Vendor").ToList();
            var selectedVendorIds = db.RFQVendors .Where(v => v.RFQId == id).Select(v => v.VendorId).ToList();
            ViewBag.SelectedVendorIds = selectedVendorIds;
            return View(rfq);
        }

        [HttpPost]
        public IActionResult EditRFQ(RFQ r, List<int> SelectedVendors)
        {
            r.UserId = (int)(HttpContext.Session.GetInt32("UserId"));
            db.RFQs.Update(r);

            
            var existingMappings = db.RFQVendors.Where(m => m.RFQId == r.RFQId);
            db.RFQVendors.RemoveRange(existingMappings);
            db.SaveChanges();

 
            foreach (var vendorId in SelectedVendors)
            {
                var mapping = new VendorRFQMapping
                {
                    RFQId = r.RFQId,
                    VendorId = vendorId
                };
                db.RFQVendors.Add(mapping);
            }

            db.SaveChanges();
            return RedirectToAction("FetchRFQ");
        }
        public IActionResult DeleteRFQ(int id) 
        {
            var data = db.RFQs.Find(id);
            db.RFQs.Remove(data);
            db.SaveChanges();
            return RedirectToAction("FetchRFQ");
        }

        public IActionResult FetchFinalizedQuotaions()
        {
                var data = db.FinalizedQuotations.Include(f => f.RFQ).Include(f => f.RFQQuotation).Include(q => q.RFQQuotation.Vendor).ToList();
            return View(data);
        }

    }
}
