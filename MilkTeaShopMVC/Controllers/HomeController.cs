using System.Linq;
using System.Web.Mvc;
using MilkTeaShopMVC.Models;
namespace MilkTeaShopMVC.Controllers {
 public class HomeController:Controller {
  private readonly MilkTeaDb db=new MilkTeaDb();
  public ActionResult Index(int? categoryId,string search) {
   var q=db.Products.Include("Category").Where(p=>p.IsAvailable);
   if(categoryId.HasValue) q=q.Where(p=>p.CategoryId==categoryId.Value);
   if(!string.IsNullOrWhiteSpace(search)) q=q.Where(p=>p.Name.Contains(search));
   return View(new ShopViewModel{Products=q.OrderBy(p=>p.Id).ToList(),Categories=db.Categories.OrderBy(c=>c.Name).ToList(),CategoryId=categoryId,Search=search});
  }
  public ActionResult About(){return View();}
  protected override void Dispose(bool disposing){if(disposing)db.Dispose();base.Dispose(disposing);}
 }
}
