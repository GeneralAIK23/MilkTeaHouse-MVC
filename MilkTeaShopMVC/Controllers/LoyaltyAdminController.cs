using System.Linq;
using System.Web.Mvc;
using MilkTeaShopMVC.Models;

namespace MilkTeaShopMVC.Controllers {
 [AdminOnly]
 public class LoyaltyAdminController:Controller {
  private readonly MilkTeaDb db=new MilkTeaDb();
  public ActionResult Index() {
   ViewBag.PointsIssued=db.LoyaltyPointTransactions.Where(x=>x.Type=="Earn")
     .Select(x=>(int?)x.PointsDelta).Sum()??0;
   return View(db.LoyaltyPointTransactions.OrderByDescending(x=>x.Id)
     .Take(200).ToList());
  }
  protected override void Dispose(bool disposing) {if(disposing)db.Dispose();base.Dispose(disposing);}
 }
}
