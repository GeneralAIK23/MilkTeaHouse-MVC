using System;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using MilkTeaShopMVC.Models;

namespace MilkTeaShopMVC.Controllers {
 [CustomerOnly]
 public class PaymentController : Controller {
  private readonly MilkTeaDb db=new MilkTeaDb();
  public ActionResult Index() {
   int customerId=CustomerAuth.CustomerId(HttpContext);
   ViewBag.Ledger=db.DemoPaymentLedgers.Include(x=>x.DemoPaymentAccount)
    .Where(x=>x.DemoPaymentAccount.CustomerAccountId==customerId)
    .OrderByDescending(x=>x.Id).Take(30).ToList();
   return View(db.DemoPaymentAccounts.Where(x=>x.CustomerAccountId==customerId).OrderBy(x=>x.Method).ToList());
  }
  [HttpPost,ValidateAntiForgeryToken]
  public ActionResult Open(string method,string pin) {
   try {DemoPaymentService.OpenAccount(db,CustomerAuth.CustomerId(HttpContext),method,pin);
    TempData["PaymentInfo"]="Đã mở tài khoản "+MilkTeaStatusVi.Method(method)+" mô phỏng với số dư thử nghiệm.";}
   catch(Exception ex) when(ex is InvalidOperationException || ex is System.Data.Entity.Infrastructure.DbUpdateException) {
    TempData["PaymentError"]=ex is InvalidOperationException ? ex.Message : "Không thể mở tài khoản. Vui lòng tải lại trang.";
   }
   return RedirectToAction("Index");
  }
  public ActionResult Pay(int id) {
   int customerId=CustomerAuth.CustomerId(HttpContext);
   var order=db.Orders.FirstOrDefault(x=>x.Id==id && x.CustomerAccountId==customerId);
   if(order==null)return HttpNotFound();
   if(!DemoPaymentService.IsOnline(order.PaymentMethod))return RedirectToAction("Success","Order",new{id});
   ViewBag.Wallet=db.DemoPaymentAccounts.FirstOrDefault(x=>x.CustomerAccountId==customerId && x.Method==order.PaymentMethod);
   return View(order);
  }
  [HttpPost,ValidateAntiForgeryToken,ActionName("Pay")]
  public ActionResult PayPost(int id,string pin) {
   int customerId=CustomerAuth.CustomerId(HttpContext);
   try {DemoPaymentService.Pay(db,customerId,id,pin);
    TempData["PaymentInfo"]="Thanh toán mô phỏng thành công. Đơn đang chờ hoàn tất để cộng điểm.";
    return RedirectToAction("Success","Order",new{id});}
   catch(InvalidOperationException ex) {TempData["PaymentError"]=ex.Message;return RedirectToAction("Pay",new{id});}
  }
  protected override void Dispose(bool disposing) {if(disposing)db.Dispose();base.Dispose(disposing);}
 }
}
