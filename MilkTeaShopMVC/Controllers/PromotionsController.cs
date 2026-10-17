using System;
using System.Linq;
using System.Web.Mvc;
using MilkTeaShopMVC.Models;

namespace MilkTeaShopMVC.Controllers {
 public class PromotionsController : Controller {
  private readonly MilkTeaDb db = new MilkTeaDb();
  public ActionResult Index() {
   var now=DateTime.Now;
   var promos=db.PromotionCodes.Where(p=>p.IsActive && p.StartAt<=now && p.EndAt>=now &&
     (!p.UsageLimit.HasValue || p.UsedCount<p.UsageLimit.Value)).OrderBy(p=>p.MinimumOrder).ToList();
   return View(promos);
  }
  [CustomerOnly, HttpPost, ValidateAntiForgeryToken]
  public ActionResult Preview(string promoCode) {
   var code=PromotionRules.Normalize(promoCode);
   if(code.Length==0) return Json(new {valid=false,message="Hãy nhập mã khuyến mãi."});
   if(code.Length>40) return Json(new {valid=false,message="Mã không hợp lệ."});
   try {
    var subtotal=PromoCartCalculator.GetFreshSubtotal(db,CartStore.Get(Session));
    var promo=db.PromotionCodes.FirstOrDefault(p=>p.Code==code);
    string error;
    if(!PromotionRules.IsValid(promo,subtotal,DateTime.Now,out error))
     return Json(new {valid=false,message=error});
    decimal discount=PromotionRules.Discount(promo,subtotal);
    return Json(new {valid=true,message="Áp dụng được: "+promo.Description,
       subtotal=subtotal,discount=discount,total=subtotal-discount});
   }
   catch(InvalidOperationException e) {return Json(new {valid=false,message=e.Message});}
  }
  protected override void Dispose(bool disposing) {if(disposing)db.Dispose();base.Dispose(disposing);}
 }
}
