using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace MilkTeaShopMVC.Models {
 public static class CartStore {
  public static List<CartLine> Get(HttpSessionStateBase session) {
   var lines=session["cart"] as List<CartLine>;
   if(lines==null){lines=new List<CartLine>();session["cart"]=lines;}
   return lines;
  }
  public static decimal Total(HttpSessionStateBase session)=>Get(session).Sum(x=>x.Subtotal);
  public static int Count(HttpSessionStateBase session)=>Get(session).Sum(x=>x.Quantity);
 }
}
