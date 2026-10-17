using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using MilkTeaShopMVC.Models;
namespace MilkTeaShopMVC.Controllers {
 [AdminOnly]
 public class PromotionsAdminController:Controller {
  private readonly MilkTeaDb db=new MilkTeaDb();
  public ActionResult Index(){return View(db.PromotionCodes.OrderByDescending(p=>p.Id).ToList());}
  [HttpPost,ValidateAntiForgeryToken]
  public ActionResult Create(PromotionInput input) {
   var code=PromotionRules.Normalize(input.Code);
   if(!Regex.IsMatch(code,@"^[A-Z0-9-]{3,40}$")) ModelState.AddModelError("Code","Mã chỉ gồm chữ A-Z, số và dấu -, dài 3–40 ký tự.");
   if(!new[]{"Percent","Fixed"}.Contains(input.DiscountType)) ModelState.AddModelError("DiscountType","Chọn kiểu giảm giá hợp lệ.");
   if(input.DiscountValue<=0 || input.DiscountValue>1000000m || (input.DiscountType=="Percent"&&input.DiscountValue>100m)) ModelState.AddModelError("DiscountValue","Giá trị giảm không hợp lệ.");
   if(input.MinimumOrder<0 || input.MinimumOrder>100000000m) ModelState.AddModelError("MinimumOrder","Giá trị đơn tối thiểu không hợp lệ.");
   if(input.MaximumDiscount.HasValue && (input.MaximumDiscount.Value<=0 || input.MaximumDiscount.Value>10000000m)) ModelState.AddModelError("MaximumDiscount","Giá trị giới hạn giảm không hợp lệ.");
   if(input.StartAt==default(DateTime) || input.EndAt<=input.StartAt) ModelState.AddModelError("EndAt","Ngày kết thúc phải sau ngày bắt đầu.");
   if(input.UsageLimit.HasValue && (input.UsageLimit.Value<1||input.UsageLimit.Value>1000000)) ModelState.AddModelError("UsageLimit","Số lượt sử dụng không hợp lệ.");
   if(db.PromotionCodes.Any(p=>p.Code==code)) ModelState.AddModelError("Code","Mã này đã tồn tại.");
   if(!ModelState.IsValid){TempData["Error"]=string.Join(" | ",ModelState.Values.SelectMany(v=>v.Errors).Select(e=>e.ErrorMessage));return RedirectToAction("Index");}
   db.PromotionCodes.Add(new PromotionCode{Code=code,Description=input.Description.Trim(),DiscountType=input.DiscountType,
    DiscountValue=input.DiscountValue,MinimumOrder=input.MinimumOrder,MaximumDiscount=input.MaximumDiscount,
    StartAt=input.StartAt,EndAt=input.EndAt,UsageLimit=input.UsageLimit,UsedCount=0,IsActive=true});
   db.SaveChanges();TempData["Success"]="Đã tạo mã "+code;return RedirectToAction("Index");
  }
  [HttpPost,ValidateAntiForgeryToken]
  public ActionResult Toggle(int id) {
   var promo=db.PromotionCodes.Find(id);if(promo==null)return HttpNotFound();
   promo.IsActive=!promo.IsActive;db.SaveChanges();return RedirectToAction("Index");
  }
  protected override void Dispose(bool disposing){if(disposing)db.Dispose();base.Dispose(disposing);}
 }
}
