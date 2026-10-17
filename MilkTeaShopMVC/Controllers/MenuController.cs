using System.Linq;
using System.Web.Mvc;
using MilkTeaShopMVC.Models;

namespace MilkTeaShopMVC.Controllers
{
    // Trang thuc don doc lap voi trang chu, giu nguyen EF6 va database hien tai.
    public class MenuController : Controller
    {
        private readonly MilkTeaDb db = new MilkTeaDb();

        public ActionResult Index(int? categoryId, string search)
        {
            var query = db.Products.Include("Category").Where(p => p.IsAvailable);
            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(p => p.Name.Contains(search));
            }

            var vm = new ShopViewModel
            {
                Products = query.OrderBy(p => p.Id).ToList(),
                Categories = db.Categories.OrderBy(c => c.Name).ToList(),
                CategoryId = categoryId,
                Search = search
            };
            return View(vm);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
