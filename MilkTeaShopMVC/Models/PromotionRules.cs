using System;
using System.Linq;
using System.Collections.Generic;

namespace MilkTeaShopMVC.Models {
 public static class PromotionRules {
  public static string Normalize(string code) {
   return (code ?? "").Trim().ToUpperInvariant();
  }
  public static bool IsValid(PromotionCode promo, decimal subtotal, DateTime now, out string error) {
   error = null;
   if(promo == null) { error = "Mã khuyến mãi không tồn tại."; return false; }
   if(!promo.IsActive) { error = "Mã hiện đã tạm ngừng."; return false; }
   if(now < promo.StartAt || now > promo.EndAt) { error = "Mã chưa đến ngày áp dụng hoặc đã hết hạn."; return false; }
   if(promo.UsageLimit.HasValue && promo.UsedCount >= promo.UsageLimit.Value) { error = "Mã đã hết lượt sử dụng."; return false; }
   if(subtotal < promo.MinimumOrder) { error = "Đơn hàng chưa đạt giá trị tối thiểu " + promo.MinimumOrder.ToString("N0") + " đ."; return false; }
   if(promo.DiscountValue <= 0 || !new[]{"Percent","Fixed"}.Contains(promo.DiscountType)) {error="Cấu hình mã không hợp lệ.";return false;}
   if(promo.DiscountType == "Percent" && promo.DiscountValue > 100) {error="Cấu hình mã không hợp lệ.";return false;}
   return true;
  }
  public static decimal Discount(PromotionCode promo, decimal subtotal) {
   if(promo == null || subtotal <= 0) return 0m;
   decimal result = promo.DiscountType == "Percent"
    ? decimal.Round(subtotal * promo.DiscountValue / 100m, 0, MidpointRounding.AwayFromZero)
    : promo.DiscountValue;
   if(promo.MaximumDiscount.HasValue) result=Math.Min(result,promo.MaximumDiscount.Value);
   return Math.Max(0m,Math.Min(subtotal,result));
  }
 }
}
