using System;
using System.Linq;
using System.Web.Mvc;
using MilkTeaShopMVC.Models;

namespace MilkTeaShopMVC.Controllers {
 [CustomerOnly]
 public class LoyaltyController:Controller {
  private readonly MilkTeaDb db=new MilkTeaDb();
  public ActionResult Index() {
   int id=CustomerAuth.CustomerId(HttpContext);
   var customer=db.CustomerAccounts.Find(id);
   if(customer==null || !customer.IsActive) return new HttpStatusCodeResult(403);
   ViewBag.PointsBalance=customer.PointsBalance;
   ViewBag.CustomerName=customer.FullName;
   return View(db.LoyaltyPointTransactions.Where(x=>x.CustomerAccountId==id)
    .OrderByDescending(x=>x.Id).Take(100).ToList());
  }
  protected override void Dispose(bool disposing) {if(disposing)db.Dispose();base.Dispose(disposing);}
 }
}
