using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MilkTeaShopMVC.Models;

namespace MilkTeaShopMVC.Controllers {
 public class CartController : Controller {
  private readonly MilkTeaDb db = new MilkTeaDb();
  public ActionResult Index() { return View(CartStore.Get(Session)); }

  [HttpPost, ValidateAntiForgeryToken]
  public ActionResult Add(int productId, string size, int quantity = 1,
     int[] toppingIds = null, int[] toppingQuantities = null, int? toppingId = null) {
   var product = db.Products.Find(productId);
   if(product == null || !product.IsAvailable) return HttpNotFound();
   if(size != "M" && size != "L") return new HttpStatusCodeResult(400, "Size invalid");
   if(quantity < 1 || quantity > 30) return new HttpStatusCodeResult(400, "Quantity invalid");

   var selected = new List<CartToppingChoice>();
   if(toppingIds != null || toppingQuantities != null) {
    if(toppingIds == null || toppingQuantities == null ||
       toppingIds.Length != toppingQuantities.Length || toppingIds.Length > 30)
       return new HttpStatusCodeResult(400, "Invalid topping data");
    var seen = new HashSet<int>();
    for(int i=0; i<toppingIds.Length; i++) {
     int id = toppingIds[i], amount = toppingQuantities[i];
     if(id < 1 || amount < 0 || amount > 5 || !seen.Add(id))
       return new HttpStatusCodeResult(400, "Invalid topping selection");
     if(amount == 0) continue;
     var t = db.Toppings.Find(id);
     if(t == null || !t.IsAvailable)
       return new HttpStatusCodeResult(400, "Topping unavailable");
     selected.Add(new CartToppingChoice {
      ToppingId=t.Id, Name=t.Name, Price=t.Price, Quantity=amount
     });
    }
   } else if(toppingId.HasValue) {
    // Ho tro POST tu giao dien cu trong luc dang nang cap.
    var t=db.Toppings.Find(toppingId.Value);
    if(t==null || !t.IsAvailable)
      return new HttpStatusCodeResult(400,"Topping unavailable");
    selected.Add(new CartToppingChoice { ToppingId=t.Id,
     Name=t.Name, Price=t.Price, Quantity=1 });
   }
   if(selected.Sum(t=>t.Quantity) > 20)
     return new HttpStatusCodeResult(400,"Maximum 20 toppings per cup");

   var newLine = new CartLine {
    ProductId=product.Id, ProductName=product.Name, Emoji=product.Emoji,
    Size=size, Quantity=quantity, ToppingSelections=selected,
    UnitPrice=product.Price+(size=="L" ? 7000 : 0)+
       selected.Sum(t=>t.Price*t.Quantity)
   };
   var lines=CartStore.Get(Session);
   var line=lines.FirstOrDefault(x=>x.Key == newLine.Key);
   if(line!=null) line.Quantity=Math.Min(30,line.Quantity+quantity);
   else lines.Add(newLine);
   TempData["Success"]="Đã thêm vào giỏ hàng!";
   return RedirectToAction("Index");
  }

  [HttpPost, ValidateAntiForgeryToken]
  public ActionResult Update(string key, int quantity) {
   var items=CartStore.Get(Session);
   var line=items.FirstOrDefault(x=>x.Key==key);
   if(line!=null) {
    if(quantity<=0) items.Remove(line);
    else line.Quantity=Math.Min(quantity,30);
   }
   return RedirectToAction("Index");
  }
  [HttpPost, ValidateAntiForgeryToken]
  public ActionResult Remove(string key) {
   var items=CartStore.Get(Session);
   var line=items.FirstOrDefault(x=>x.Key==key);
   if(line!=null) items.Remove(line);
   return RedirectToAction("Index");
  }
  protected override void Dispose(bool disposing) {
   if(disposing)db.Dispose(); base.Dispose(disposing);
  }
 }
}
