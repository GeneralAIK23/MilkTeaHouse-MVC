using System;
using System.Data.SqlClient;
using System.Web.Mvc;
using MilkTeaShopMVC.Models;

namespace MilkTeaShopMVC.Controllers
{
    public class FranchiseController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View(new FranchiseRequestForm());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Submit(FranchiseRequestForm model)
        {
            // Bot trap; do not store automated junk.
            if (!string.IsNullOrWhiteSpace(model.Website))
                return RedirectToAction("Index");

            if (!ModelState.IsValid) return View("Index", model);
            try
            {
                FranchiseRequestsStore.Add(model);
                TempData["FranchiseSuccess"] = "MilkTeaShop đã tiếp nhận yêu cầu tư vấn. Cảm ơn bạn!";
                return RedirectToAction("Index");
            }
            catch (SqlException)
            {
                ModelState.AddModelError("", "Chưa thể lưu yêu cầu vào SQL Server. Vui lòng thử lại.");
                return View("Index", model);
            }
        }
    }
}
