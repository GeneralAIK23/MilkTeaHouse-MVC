using System;
using System.Linq;
using System.Collections.Generic;
namespace MilkTeaShopMVC.Models {
 public static class PromoCartCalculator {
  public static decimal GetFreshSubtotal(MilkTeaDb db, List<CartLine> items) {
   if(items==null||items.Count==0) throw new InvalidOperationException("Giỏ hàng trống.");
   decimal total=0m;
   foreach(var line in items) {
    var product=db.Products.Find(line.ProductId);
    if(product==null||!product.IsAvailable) throw new InvalidOperationException("Sản phẩm không còn bán.");
    if((line.Size!="M" && line.Size!="L")||line.Quantity<1||line.Quantity>30) throw new InvalidOperationException("Giỏ hàng không hợp lệ.");
    decimal price=product.Price+(line.Size=="L"?7000m:0m);
    var selections=(line.ToppingSelections??new List<CartToppingChoice>()).Where(t=>t.Quantity>0).ToList();
    if(selections.Count==0 && line.ToppingId.HasValue) selections.Add(new CartToppingChoice{ToppingId=line.ToppingId.Value,Quantity=1});
    if(selections.Count>30||selections.Any(t=>t.Quantity<1||t.Quantity>5)||selections.Sum(t=>t.Quantity)>20||selections.Select(t=>t.ToppingId).Distinct().Count()!=selections.Count)
     throw new InvalidOperationException("Lựa chọn topping không hợp lệ.");
    foreach(var t in selections){
     var topping=db.Toppings.Find(t.ToppingId);
     if(topping==null||!topping.IsAvailable)throw new InvalidOperationException("Topping hiện không còn bán.");
     price+=topping.Price*t.Quantity;
    }
    total+=price*line.Quantity;
   }
   return total;
  }
 }
}
