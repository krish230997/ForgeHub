using ForgeHub.Data;
using ForgeHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace ForgeHub.Controllers
{
    public class AddUserController : Controller
    {
        ApplicationDbContext db;
        public AddUserController(ApplicationDbContext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AddUsers(User u)
        {

            db.Users.Add(u);
            db.SaveChanges();
            return Json(u);
        }

        public IActionResult FetchUsers()
        {
            var data = db.Users.ToList();
            return Json(data);

        }

        public IActionResult EditUsers(int id)
        {
            var data = db.Users.Find(id);
            return Json(data);
        }
        public IActionResult UpdateUsers(User u)
        {
            db.Users.Update(u);
            db.SaveChanges();
            return Json("");
        }

        public IActionResult DeleteUser(int id)
        {
            var data = db.Users.Find(id);
            db.Users.Remove(data);
            db.SaveChanges();
            return Json("");

        }

        public IActionResult SearchUser(string mydata)
        {
            if (mydata != null)
            {
                var data = db.Users.Where(x => x.FullName.Contains(mydata)).ToList();
                return Json(data);
            }
            else
            {
                var data = db.Users.ToList();
                return Json(data);
            }

        }


    }
}