using System;
using System.Linq;
using System.Web.Mvc;
using MilkTeaShopMVC.Models;
namespace MilkTeaShopMVC.Controllers {
 public class OrderController:Controller {
  private readonly MilkTeaDb db=new MilkTeaDb();
  [CustomerOnly] public ActionResult Checkout(){ var customer=db.CustomerAccounts.Find(CustomerAuth.CustomerId(HttpContext)); if(customer==null || !customer.IsActive)return new HttpStatusCodeResult(403); var items=CartStore.Get(Session);if(!items.Any())return RedirectToAction("Index","Cart");return View(new CheckoutViewModel{Items=items,CustomerName=customer.FullName,Phone=customer.Phone,Address=customer.Address});}
  [CustomerOnly, HttpPost, ValidateAntiForgeryToken]
  public ActionResult Checkout(CheckoutViewModel model){
   var items=CartStore.Get(Session);if(!items.Any())ModelState.AddModelError("","Giỏ hàng trống.");
   if(!ModelState.IsValid){model.Items=items;return View(model);}
   // Revalidate live products/toppings and recalculate prices server-side.
   var order=new Order{CustomerAccountId=CustomerAuth.CustomerId(HttpContext),CustomerName=model.CustomerName,Phone=model.Phone,Address=model.Address,Note=model.Note,CreatedAt=DateTime.Now,Status="Pending"};
   foreach(var x in items){var p=db.Products.Find(x.ProductId);if(p==null||!p.IsAvailable)return new HttpStatusCodeResult(400,"Sản phẩm không còn bán. Vui lòng cập nhật giỏ hàng.");
    if(x.Size!="M"&&x.Size!="L"||x.Quantity<1||x.Quantity>30)return new HttpStatusCodeResult(400,"Giỏ hàng không hợp lệ");
    Topping t=null;if(x.ToppingId.HasValue){t=db.Toppings.Find(x.ToppingId.Value);if(t==null||!t.IsAvailable)return new HttpStatusCodeResult(400,"Topping không còn bán");}
    decimal price=p.Price+(x.Size=="L"?7000:0)+(t?.Price??0);
    order.Lines.Add(new OrderLine{ProductId=p.Id,ProductName=p.Name,Size=x.Size,Toppings=t?.Name,Quantity=x.Quantity,UnitPrice=price});order.Total+=price*x.Quantity;
   }
   db.Orders.Add(order);db.SaveChanges();Session.Remove("cart");return RedirectToAction("Success",new{id=order.Id});
  }
  [CustomerOnly] public ActionResult Success(int id){var customerId = CustomerAuth.CustomerId(HttpContext); var order = db.Orders.FirstOrDefault(x => x.Id == id && x.CustomerAccountId == customerId);if(order==null)return HttpNotFound();return View(order);}
  [CustomerOnly]
  public ActionResult MyOrders(){
    var id=CustomerAuth.CustomerId(HttpContext);
    return View(db.Orders.Where(x=>x.CustomerAccountId==id).OrderByDescending(x=>x.CreatedAt).ToList());
  }
  protected override void Dispose(bool disposing){if(disposing)db.Dispose();base.Dispose(disposing);}
 }
}
