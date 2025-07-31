using ForgeHub.Data;
using ForgeHub.JwtToken;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OtpNet;
using QRCoder;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;

namespace ForgeHub.Controllers
{
    public class AuthController : Controller
    {
        ApplicationDbContext db;
        JwtTokenGenerator jwt;
        public AuthController(ApplicationDbContext db , JwtTokenGenerator jwt)
        {
            this.jwt = jwt;
            this.db = db;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string email, string password)
        {

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Email and password are required.";
                return View();
            }


            var user = db.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == password);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("FullName", user.FullName);

            if (user.IsFirstTimeLogin)
            {
                var key = KeyGeneration.GenerateRandomKey(20);
                string secret = Base32Encoding.ToString(key);

                HttpContext.Session.SetString("Secret", secret);


                user.SecretKey = secret;
                db.SaveChanges();

                return RedirectToAction("ShowQR");
            }
            else
            {
                HttpContext.Session.SetString("Secret", user.SecretKey);
                return RedirectToAction("VerifyOtp");
            }

        }

        public IActionResult ShowQR()
        {
            string email = HttpContext.Session.GetString("Email");
            string secret = HttpContext.Session.GetString("Secret");
            string issuer = "ForgeHub";

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(secret))
            {
                ViewBag.Error = "Session expired. Please login again.";
                return RedirectToAction("Login");
            }

            string uri = $"otpauth://totp/{issuer}:{email}?secret={secret}&issuer={issuer}";

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
            Base64QRCode qrCode = new Base64QRCode(qrCodeData);
            ViewBag.QRCodeImage = "data:image/png;base64," + qrCode.GetGraphic(5);

            return View();
        }


        public IActionResult VerifyOtp()
        {
    
            return View();
           
        }

        [HttpPost]
        public IActionResult VerifyOtp(string otp)
        {
            string secret = HttpContext.Session.GetString("Secret");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (string.IsNullOrEmpty(secret) || userId == null)
            {
                ViewBag.Error = "Secret expired or not found.";
                return RedirectToAction("Login");
            }

            var totp = new Totp(Base32Encoding.ToBytes(secret));

            if (totp.VerifyTotp(otp, out long timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay))
            {
                var user = db.Users.Find(userId);

                if (user == null)
                {
                    ViewBag.Error = "User not found.";
                    return RedirectToAction("Login");
                }

                if (user.IsFirstTimeLogin)
                {
                    user.IsFirstTimeLogin = false;
                    db.SaveChanges();
                }


                string jwtToken = jwt.GenerateToken(user.UserId, user.Email, user.FullName, user.Role);

                Response.Cookies.Append("JwtToken", jwtToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                });


                if (user.Role == "Admin")
                {
                    return RedirectToAction("FetchRFQ", "Admin");
                }
                else if (user.Role == "Buyer")
                {
                    return RedirectToAction("FetchRFQ", "Buyer");
                }
                else
                {
                    return RedirectToAction("Dashboard", "Vendor");
                }

            }

            ViewBag.Error = "Invalid OTP";
            return View();

        }
        public IActionResult LostOtP()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LostOtp(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Error = "Email is required";
                return View();
            }

            
            var key = KeyGeneration.GenerateRandomKey(20);
            string secret = Base32Encoding.ToString(key);

            var totp = new Totp(Base32Encoding.ToBytes(secret));
            var otp = totp.ComputeTotp();

          
            HttpContext.Session.SetString("LostOtpSecret", secret);
            HttpContext.Session.SetString("LostOtpEmail", email);

     
            SendEmail(email, "Your OTP", $"Your OTP is: {otp}");

            return RedirectToAction("VerifyLostOtp");
        }

        public void SendEmail(string to, string subject, string body)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                Port = 587,
                Credentials = new NetworkCredential("sahnisha161@gmail.com", "wlzwiwnqcrcuohwl"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("sahnisha161@gmail.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = false,
            };
            mailMessage.To.Add(to);
            smtpClient.Send(mailMessage);
        }
        public IActionResult VerifyLostOtp()
        {
            return View();
        }


        [HttpPost]
        public IActionResult VerifyLostOtp(string otp)
        {
            string secret = HttpContext.Session.GetString("LostOtpSecret");
            int? userId = HttpContext.Session.GetInt32("LostOtpUserId");


            if (string.IsNullOrEmpty(secret))
            {
                ViewBag.Error = "OTP expired. Please try again.";
                return RedirectToAction("LostOtp");
            }

            var totp = new Totp(Base32Encoding.ToBytes(secret));
            if (totp.VerifyTotp(otp, out long _, VerificationWindow.RfcSpecifiedNetworkDelay))
            {
                return RedirectToAction("ShowQR");
            }

            ViewBag.Error = "Invalid OTP.";
            return View();
        }
        public IActionResult Logout()
        {
            Response.Cookies.Delete("JwtToken");
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }



    }
}
