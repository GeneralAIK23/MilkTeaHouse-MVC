using System;
using System.Linq;
using System.Web.Mvc;
using MilkTeaShopMVC.Models;
namespace MilkTeaShopMVC.Controllers {
 public class CartController:Controller {
  private readonly MilkTeaDb db=new MilkTeaDb();
  public ActionResult Index(){return View(CartStore.Get(Session));}
  [HttpPost,ValidateAntiForgeryToken]
  public ActionResult Add(int productId,string size,int? toppingId,int quantity=1) {
   var product=db.Products.Find(productId);if(product==null||!product.IsAvailable)return HttpNotFound();
   if(size!="M"&&size!="L")return new HttpStatusCodeResult(400,"Size invalid");
   if(quantity<1||quantity>30)return new HttpStatusCodeResult(400,"Quantity invalid");
   Topping topping=null;if(toppingId.HasValue){topping=db.Toppings.Find(toppingId.Value);if(topping==null||!topping.IsAvailable)return new HttpStatusCodeResult(400,"Topping invalid");}
   var lines=CartStore.Get(Session);string key=productId+"-"+size+"-"+(toppingId.HasValue?toppingId.Value.ToString():"0");
   var line=lines.FirstOrDefault(x=>x.Key==key);
   if(line!=null)line.Quantity=Math.Min(30,line.Quantity+quantity);
   else lines.Add(new CartLine{ProductId=product.Id,ProductName=product.Name,Emoji=product.Emoji,Size=size,ToppingId=toppingId,ToppingName=topping?.Name,UnitPrice=product.Price+(size=="L"?7000:0)+(topping?.Price??0),Quantity=quantity});
   TempData["Success"]="Đã thêm vào giỏ hàng!";return RedirectToAction("Index");
  }
  [HttpPost,ValidateAntiForgeryToken] public ActionResult Update(string key,int quantity) {
   var items=CartStore.Get(Session);var p=items.FirstOrDefault(x=>x.Key==key);if(p!=null){if(quantity<=0)items.Remove(p);else p.Quantity=Math.Min(quantity,30);}return RedirectToAction("Index");
  }
  [HttpPost,ValidateAntiForgeryToken] public ActionResult Remove(string key){var items=CartStore.Get(Session);var p=items.FirstOrDefault(x=>x.Key==key);if(p!=null)items.Remove(p);return RedirectToAction("Index");}
  protected override void Dispose(bool disposing){if(disposing)db.Dispose();base.Dispose(disposing);}
 }
}
