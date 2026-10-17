using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using MilkTeaShopMVC.Models;

namespace MilkTeaShopMVC.Controllers
{
    public class ContactController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View(new ContactFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ContactFormViewModel model)
        {
            if (!model.Consent)
                ModelState.AddModelError("Consent", "Bạn cần đồng ý gửi thông tin liên hệ.");

            if (!ModelState.IsValid)
                return View(model);

            // Bỏ qua yêu cầu do bot điền vào trường ẩn.
            if (!string.IsNullOrWhiteSpace(model.Website))
            {
                TempData["ContactSuccess"] = "Cảm ơn bạn! Yêu cầu đã được tiếp nhận.";
                return RedirectToAction("Index");
            }

            try
            {
                var connStr = ConfigurationManager.ConnectionStrings["MilkTeaDb"].ConnectionString;
                using (var connection = new SqlConnection(connStr))
                using (var command = new SqlCommand(@"
INSERT INTO dbo.ContactMessages (FullName, Email, Phone, Address, Message)
VALUES (@FullName, @Email, @Phone, @Address, @Message);", connection))
                {
                    command.Parameters.Add("@FullName", SqlDbType.NVarChar, 120).Value = model.FullName.Trim();
                    command.Parameters.Add("@Email", SqlDbType.NVarChar, 200).Value = model.Email.Trim();
                    command.Parameters.Add("@Phone", SqlDbType.NVarChar, 25).Value =
                        string.IsNullOrWhiteSpace(model.Phone) ? (object)DBNull.Value : model.Phone.Trim();
                    command.Parameters.Add("@Address", SqlDbType.NVarChar, 250).Value =
                        string.IsNullOrWhiteSpace(model.Address) ? (object)DBNull.Value : model.Address.Trim();
                    command.Parameters.Add("@Message", SqlDbType.NVarChar, 3000).Value = model.Message.Trim();

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                TempData["ContactSuccess"] = "Cảm ơn bạn! MilkTeaShop đã ghi nhận lời nhắn.";
                return RedirectToAction("Index");
            }
            catch (SqlException)
            {
                ModelState.AddModelError("", "Chưa thể lưu lời nhắn vào cơ sở dữ liệu. Vui lòng thử lại sau.");
                return View(model);
            }
        }
    }
}
