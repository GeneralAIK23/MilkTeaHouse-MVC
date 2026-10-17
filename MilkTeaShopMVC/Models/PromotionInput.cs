using System;
using System.ComponentModel.DataAnnotations;
namespace MilkTeaShopMVC.Models {
 public class PromotionInput {
  [Required, StringLength(40)] public string Code {get;set;}
  [Required, StringLength(160)] public string Description {get;set;}
  [Required] public string DiscountType {get;set;}
  public decimal DiscountValue {get;set;}
  public decimal MinimumOrder {get;set;}
  public decimal? MaximumDiscount {get;set;}
  public DateTime StartAt {get;set;}
  public DateTime EndAt {get;set;}
  public int? UsageLimit {get;set;}
 }
}
