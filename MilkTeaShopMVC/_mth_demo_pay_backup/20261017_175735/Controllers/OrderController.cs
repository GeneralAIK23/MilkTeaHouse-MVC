using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MilkTeaShopMVC.Models;

namespace MilkTeaShopMVC.Controllers {
 public class OrderController : Controller {
  private readonly MilkTeaDb db=new MilkTeaDb();

  [CustomerOnly]
  public ActionResult Checkout() {
   var customer=db.CustomerAccounts.Find(CustomerAuth.CustomerId(HttpContext));
   if(customer==null || !customer.IsActive) return new HttpStatusCodeResult(403);
   var items=CartStore.Get(Session);
   if(!items.Any()) return RedirectToAction("Index","Cart");
   ViewBag.PointsBalance=customer.PointsBalance;return View(new CheckoutViewModel {Items=items,CustomerName=customer.FullName,
       Phone=customer.Phone,Address=customer.Address});
  }

  [CustomerOnly, HttpPost, ValidateAntiForgeryToken]
  public ActionResult Checkout(CheckoutViewModel model) {
   var items=CartStore.Get(Session); ViewBag.PointsBalance=db.Database.SqlQuery<int>("SELECT PointsBalance FROM dbo.CustomerAccounts WHERE Id=@p0",CustomerAuth.CustomerId(HttpContext)).SingleOrDefault();
   if(!items.Any()) ModelState.AddModelError("","Giỏ hàng trống.");
   if(!ModelState.IsValid) {model.Items=items;return View(model);}

   int customerId=CustomerAuth.CustomerId(HttpContext);
   var customer=db.CustomerAccounts.Find(customerId);
   if(customer==null || !customer.IsActive) return new HttpStatusCodeResult(403);

   var order=new Order { CustomerAccountId=customerId,
    CustomerName=model.CustomerName, Phone=model.Phone, Address=model.Address,
    Note=model.Note, CreatedAt=DateTime.Now, Status="Pending" };

   foreach(var x in items) {
    var p=db.Products.Find(x.ProductId);
    if(p==null || !p.IsAvailable)
      return new HttpStatusCodeResult(400,"Sản phẩm không còn bán. Vui lòng cập nhật giỏ hàng.");
    if((x.Size!="M" && x.Size!="L") || x.Quantity<1 || x.Quantity>30)
      return new HttpStatusCodeResult(400,"Giỏ hàng không hợp lệ");

    var choices=(x.ToppingSelections ?? new List<CartToppingChoice>())
       .Where(s=>s.Quantity>0).ToList();
    if(choices.Count==0 && x.ToppingId.HasValue)
      choices.Add(new CartToppingChoice {ToppingId=x.ToppingId.Value,Quantity=1});
    if(choices.Count>30 || choices.Any(s=>s.Quantity<1 || s.Quantity>5) ||
       choices.Sum(s=>s.Quantity)>20 ||
       choices.Select(s=>s.ToppingId).Distinct().Count()!=choices.Count)
      return new HttpStatusCodeResult(400,"Topping không hợp lệ");

    decimal price=p.Price+(x.Size=="L" ? 7000 : 0);
    var snapshots=new List<OrderLineTopping>();
    foreach(var choice in choices) {
     var topping=db.Toppings.Find(choice.ToppingId);
     if(topping==null || !topping.IsAvailable)
       return new HttpStatusCodeResult(400,"Topping không còn bán");
     price+=topping.Price*choice.Quantity;
     snapshots.Add(new OrderLineTopping {
      ToppingId=topping.Id, ToppingName=topping.Name,
      Quantity=choice.Quantity, UnitPrice=topping.Price
     });
    }
    string summary=string.Join("; ",snapshots.Select(s=>s.ToppingName+" x"+s.Quantity));
    if(summary.Length>200)
      return new HttpStatusCodeResult(400,"Tên topping vượt quá giới hạn lưu trữ");

    var detail=new OrderLine { ProductId=p.Id,ProductName=p.Name,Size=x.Size,
     Toppings=summary,Quantity=x.Quantity,UnitPrice=price };
    foreach(var snapshot in snapshots) detail.ToppingDetails.Add(snapshot);
    order.Lines.Add(detail);
    order.Total+=price*x.Quantity;
   }

   // Luu don hang, chi tiet tung ly va tung topping trong 1 lan SaveChanges.
   // Recalculate price, coupon and points under ONE SQL transaction.
   var code=PromotionRules.Normalize(model.PromoCode);
   order.OriginalTotal=order.Total;
   order.DiscountAmount=0m;
   order.PointsRedeemed=0;
   order.PointsDiscount=0m;
   order.PaymentMethod="COD";
   order.PaymentStatus="Unpaid";
   if(model.PointsToRedeem<0 || model.PointsToRedeem>1000000) {
    ModelState.AddModelError("PointsToRedeem","Số điểm không hợp lệ.");
    model.Items=items;return View(model);
   }
   if(code.Length>40) {
    ModelState.AddModelError("PromoCode","Mã không hợp lệ.");
    model.Items=items;return View(model);
   }
   using(var tx=db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable)) {
    // Khóa tài khoản để không thể dùng số điểm đó hai lần đồng thời.
    var member=db.CustomerAccounts.SqlQuery(
     "SELECT * FROM dbo.CustomerAccounts WITH (UPDLOCK, HOLDLOCK) WHERE Id=@p0",customerId)
     .SingleOrDefault();
    if(member==null || !member.IsActive) return new HttpStatusCodeResult(403);
    if(code.Length>0) {
     var promo=db.PromotionCodes.SqlQuery(
      "SELECT * FROM dbo.PromotionCodes WITH (UPDLOCK, HOLDLOCK) WHERE Code=@p0",code)
      .SingleOrDefault();
     string error;
     if(!PromotionRules.IsValid(promo,order.OriginalTotal.Value,DateTime.Now,out error)) {
      ModelState.AddModelError("PromoCode",error);model.Items=items;return View(model);
     }
     order.PromotionCode=promo.Code;
     order.DiscountAmount=PromotionRules.Discount(promo,order.OriginalTotal.Value);
     promo.UsedCount+=1;
    }
    decimal amountAfterCoupon=order.OriginalTotal.Value-order.DiscountAmount;
    var pointsValue=LoyaltyRules.RedeemValue(model.PointsToRedeem);
    if(model.PointsToRedeem>member.PointsBalance || pointsValue>amountAfterCoupon) {
     ModelState.AddModelError("PointsToRedeem","Điểm vượt quá số dư hoặc số tiền còn phải thanh toán.");
     model.Items=items;return View(model);
    }
    order.PointsRedeemed=model.PointsToRedeem;
    order.PointsDiscount=pointsValue;
    order.Total=amountAfterCoupon-pointsValue;
    if(model.PointsToRedeem>0) member.PointsBalance-=model.PointsToRedeem;
    db.Orders.Add(order);
    db.SaveChanges();
    if(model.PointsToRedeem>0) {
     db.LoyaltyPointTransactions.Add(new LoyaltyPointTransaction {
      CustomerAccountId=member.Id,OrderId=order.Id,Type="Redeem",
      PointsDelta=-model.PointsToRedeem,CreatedAt=DateTime.Now,
      Description="Sử dụng điểm cho đơn hàng #"+order.Id
     });
     db.SaveChanges();
    }
    tx.Commit();
   }
   Session.Remove("cart");return RedirectToAction("Success",new {id=order.Id});
  }

  [CustomerOnly]
  public ActionResult Success(int id) {
   var customerId=CustomerAuth.CustomerId(HttpContext);
   var order=db.Orders.Include("Lines.ToppingDetails")
      .FirstOrDefault(x=>x.Id==id && x.CustomerAccountId==customerId);
   if(order==null) return HttpNotFound();
   return View(order);
  }

  [CustomerOnly]
  public ActionResult MyOrders() {
   var customerId=CustomerAuth.CustomerId(HttpContext);
   return View(db.Orders.Where(x=>x.CustomerAccountId==customerId)
     .OrderByDescending(x=>x.CreatedAt).ToList());
  }

  protected override void Dispose(bool disposing) {
   if(disposing)db.Dispose(); base.Dispose(disposing);
  }
 }
}
