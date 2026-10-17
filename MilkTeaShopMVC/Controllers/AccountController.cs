using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using MilkTeaShopMVC.Models;

namespace MilkTeaShopMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly MilkTeaDb db = new MilkTeaDb();

        [HttpGet]
        public ActionResult Register(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new CustomerRegisterViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Register(CustomerRegisterViewModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (!ModelState.IsValid) return View(model);
            var email = model.Email.Trim().ToLowerInvariant();
            if (db.CustomerAccounts.Any(x => x.Email == email) || db.AdminUsers.Any(x => x.Email == email))
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng.");
                return View(model);
            }
            string salt, hash;
            PasswordHelper.CreateHash(model.Password, out salt, out hash);
            var customer = new CustomerAccount
            {
                FullName = model.FullName.Trim(), Email = email,
                PasswordSalt = salt, PasswordHash = hash, IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            db.CustomerAccounts.Add(customer);
            try { db.SaveChanges(); }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                ModelState.AddModelError("Email", "Không thể tạo tài khoản. Email có thể đã được sử dụng.");
                return View(model);
            }
            CustomerAuth.SignInCustomer(Response, customer);
            return SafeReturn(returnUrl);
        }

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new CustomerLoginViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(CustomerLoginViewModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (!ModelState.IsValid) return View(model);
            var email = model.Email.Trim().ToLowerInvariant();
            var customer = db.CustomerAccounts.FirstOrDefault(x => x.Email == email && x.IsActive);
            if (customer == null || !PasswordHelper.Verify(model.Password, customer.PasswordSalt, customer.PasswordHash))
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
                return View(model);
            }
            CustomerAuth.SignInCustomer(Response, customer);
            return SafeReturn(returnUrl);
        }

        [CustomerOnly, HttpPost, ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [CustomerOnly, HttpGet]
        public ActionResult Profile()
        {
            var customer = db.CustomerAccounts.Find(CustomerAuth.CustomerId(HttpContext));
            if (customer == null || !customer.IsActive) { FormsAuthentication.SignOut(); return RedirectToAction("Login"); }
            return View(new CustomerProfileViewModel { Email=customer.Email, FullName=customer.FullName, Phone=customer.Phone, Address=customer.Address });
        }

        [CustomerOnly, HttpPost, ValidateAntiForgeryToken]
        public ActionResult Profile(CustomerProfileViewModel model)
        {
            var customer = db.CustomerAccounts.Find(CustomerAuth.CustomerId(HttpContext));
            if (customer == null || !customer.IsActive) return new HttpStatusCodeResult(403);
            model.Email = customer.Email; // Email cannot be changed through the form.
            ModelState.Remove("Email");
            if (!ModelState.IsValid) return View(model);
            customer.FullName = model.FullName.Trim();
            customer.Phone = string.IsNullOrWhiteSpace(model.Phone) ? null : model.Phone.Trim();
            customer.Address = string.IsNullOrWhiteSpace(model.Address) ? null : model.Address.Trim();
            db.SaveChanges();
            TempData["Success"] = "Đã cập nhật thông tin tài khoản.";
            return RedirectToAction("Profile");
        }

        private ActionResult SafeReturn(string returnUrl)
        {
            return Url.IsLocalUrl(returnUrl) ? (ActionResult)Redirect(returnUrl) : RedirectToAction("Index", "Home");
        }
        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }
    }
}
