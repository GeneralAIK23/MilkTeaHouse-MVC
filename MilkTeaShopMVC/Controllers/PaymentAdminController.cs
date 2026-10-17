using System;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using MilkTeaShopMVC.Models;
namespace MilkTeaShopMVC.Controllers {
 [AdminOnly]
 public class PaymentAdminController:Controller {
  private readonly MilkTeaDb db=new MilkTeaDb();
  public ActionResult Index() {
   ViewBag.Transactions=db.PaymentTransactions.Include(x=>x.Order).OrderByDescending(x=>x.Id).Take(80).ToList();
   ViewBag.Ledger=db.DemoPaymentLedgers.OrderByDescending(x=>x.Id).Take(50).ToList();
   return View(db.DemoPaymentAccounts.Include(x=>x.CustomerAccount).OrderByDescending(x=>x.Id).ToList());
  }
  [HttpPost,ValidateAntiForgeryToken]
  public ActionResult TopUp(int accountId,decimal amount) {
   try {DemoPaymentService.TopUp(db,accountId,amount);TempData["PaymentInfo"]="Đã nạp số dư demo.";}
   catch(InvalidOperationException ex) {TempData["PaymentError"]=ex.Message;}
   return RedirectToAction("Index");
  }
  protected override void Dispose(bool disposing) {if(disposing)db.Dispose();base.Dispose(disposing);}
 }
}
