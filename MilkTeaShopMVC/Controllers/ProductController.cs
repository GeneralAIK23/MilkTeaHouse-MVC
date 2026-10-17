using System.Linq;
using System.Web.Mvc;
using MilkTeaShopMVC.Models;
namespace MilkTeaShopMVC.Controllers {
 public class ProductController:Controller {
  private readonly MilkTeaDb db=new MilkTeaDb();
  public ActionResult Details(int id){var p=db.Products.FirstOrDefault(x=>x.Id==id&&x.IsAvailable);if(p==null)return HttpNotFound();return View(new ProductDetailViewModel{Product=p,Toppings=db.Toppings.Where(t=>t.IsAvailable).OrderBy(t=>t.Id).ToList()});}
  protected override void Dispose(bool disposing){if(disposing)db.Dispose();base.Dispose(disposing);}
 }
}
