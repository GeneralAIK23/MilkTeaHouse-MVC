using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MilkTeaShopMVC.Models;
namespace MilkTeaShopMVC.Controllers {
 public class AdminController:Controller {
  private readonly MilkTeaDb db=new MilkTeaDb();
  [AllowAnonymous] public ActionResult Login(){return View(new LoginViewModel());}
  [HttpPost,AllowAnonymous,ValidateAntiForgeryToken] public ActionResult Login(LoginViewModel model){
   if(!ModelState.IsValid)return View(model);
   var user=db.AdminUsers.FirstOrDefault(a=>a.Email==model.Email);
   if(user==null||!PasswordHelper.Verify(model.Password,user.PasswordSalt,user.PasswordHash)){ModelState.AddModelError("","Email hoặc mật khẩu không đúng.");return View(model);}
   FormsAuthentication.SetAuthCookie(user.Email,false);return RedirectToAction("Index");
  }
  [Authorize,HttpPost,ValidateAntiForgeryToken] public ActionResult Logout(){FormsAuthentication.SignOut();return RedirectToAction("Index","Home");}
  [Authorize] public ActionResult Index(){ViewBag.UnreadContactCount=ContactInboxStore.UnreadCount();ViewBag.ProductCount=db.Products.Count();ViewBag.OrderCount=db.Orders.Count();ViewBag.Revenue=db.Orders.Where(o=>o.Status!="Cancelled").Select(o=>(decimal?)o.Total).Sum()??0m;return View(db.Orders.OrderByDescending(o=>o.CreatedAt).Take(6).ToList());}
  [Authorize] public ActionResult Products(){return View(db.Products.Include(p=>p.Category).OrderByDescending(p=>p.Id).ToList());}
  [Authorize] public ActionResult Create(){ViewBag.Categories=new SelectList(db.Categories,"Id","Name");return View("EditProduct",new Product{IsAvailable=true,Emoji="🧋",Theme="caramel"});}
  [Authorize] public ActionResult Edit(int id){var p=db.Products.Find(id);if(p==null)return HttpNotFound();ViewBag.Categories=new SelectList(db.Categories,"Id","Name",p.CategoryId);return View("EditProduct",p);}
  [Authorize,HttpPost,ValidateAntiForgeryToken]
  public ActionResult Save(Product model){
   if(!db.Categories.Any(c=>c.Id==model.CategoryId))ModelState.AddModelError("CategoryId","Danh mục không hợp lệ");
   if(!ModelState.IsValid){ViewBag.Categories=new SelectList(db.Categories,"Id","Name",model.CategoryId);return View("EditProduct",model);}
   if(model.Id==0){db.Products.Add(new Product{Name=model.Name,Description=model.Description,Price=model.Price,Emoji=model.Emoji,Theme=model.Theme,IsAvailable=model.IsAvailable,CategoryId=model.CategoryId});}
   else {var p=db.Products.Find(model.Id);if(p==null)return HttpNotFound();p.Name=model.Name;p.Description=model.Description;p.Price=model.Price;p.Emoji=model.Emoji;p.Theme=model.Theme;p.CategoryId=model.CategoryId;p.IsAvailable=model.IsAvailable;}
   db.SaveChanges();TempData["Success"]="Đã lưu sản phẩm.";return RedirectToAction("Products");
  }
  [Authorize,HttpPost,ValidateAntiForgeryToken]
  public ActionResult Delete(int id){var p=db.Products.Find(id);if(p!=null){p.IsAvailable=false;db.SaveChanges();}return RedirectToAction("Products");}
  [Authorize] public ActionResult Categories(){return View(db.Categories.OrderBy(c=>c.Id).ToList());}
  [Authorize,HttpPost,ValidateAntiForgeryToken] public ActionResult AddCategory(string name){if(!string.IsNullOrWhiteSpace(name)&&name.Length<=80){db.Categories.Add(new Category{Name=name.Trim()});db.SaveChanges();}return RedirectToAction("Categories");}
  [Authorize] public ActionResult Orders(){return View(db.Orders.OrderByDescending(o=>o.CreatedAt).ToList());}
  [Authorize] public ActionResult OrderDetail(int id){var o=db.Orders.Include(x=>x.Lines).FirstOrDefault(x=>x.Id==id);if(o==null)return HttpNotFound();return View(o);}
  [Authorize,HttpPost,ValidateAntiForgeryToken] public ActionResult UpdateStatus(int id,string status){
   if(!new[]{"Pending","Confirmed","Delivering","Completed","Cancelled"}.Contains(status))return new HttpStatusCodeResult(400);
   var o=db.Orders.Find(id);if(o==null)return HttpNotFound();o.Status=status;db.SaveChanges();return RedirectToAction("OrderDetail",new{id});
  }
  [Authorize]
  public ActionResult Contacts() {
    return View(ContactInboxStore.Latest(200));
  }
  [Authorize]
  public ActionResult ContactDetail(int id) {
    var item = ContactInboxStore.ById(id);
    if (item == null) return HttpNotFound();
    return View(item);
  }
  [Authorize, HttpPost, ValidateAntiForgeryToken]
  public ActionResult MarkContactRead(int id) {
    ContactInboxStore.MarkRead(id);
    return RedirectToAction("ContactDetail", new { id = id });
  }

  protected override void Dispose(bool disposing){if(disposing)db.Dispose();base.Dispose(disposing);}
 }
}
