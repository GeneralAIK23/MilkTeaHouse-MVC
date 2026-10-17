using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilkTeaShopMVC.Models {
 [Table("PromotionCodes")]
 public class PromotionCode {
  public int Id {get;set;}
  [Required, StringLength(40)] public string Code {get;set;}
  [Required, StringLength(160)] public string Description {get;set;}
  [Required, StringLength(10)] public string DiscountType {get;set;} // Percent, Fixed
  public decimal DiscountValue {get;set;}
  public decimal MinimumOrder {get;set;}
  public decimal? MaximumDiscount {get;set;}
  public DateTime StartAt {get;set;}
  public DateTime EndAt {get;set;}
  public int? UsageLimit {get;set;}
  public int UsedCount {get;set;}
  public bool IsActive {get;set;}
 }
}
