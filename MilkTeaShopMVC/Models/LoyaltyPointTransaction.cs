using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilkTeaShopMVC.Models {
 [Table("LoyaltyPointTransactions")]
 public class LoyaltyPointTransaction {
  public int Id {get;set;}
  public int CustomerAccountId {get;set;}
  public int OrderId {get;set;}
  [Required,StringLength(20)] public string Type {get;set;}
  public int PointsDelta {get;set;}
  public DateTime CreatedAt {get;set;}
  [StringLength(250)] public string Description {get;set;}
 }
}
