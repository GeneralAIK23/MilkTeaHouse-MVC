using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilkTeaShopMVC.Models {
 public class DemoPaymentAccount {
  public int Id {get;set;}
  public int CustomerAccountId {get;set;}
  [ForeignKey("CustomerAccountId")] public virtual CustomerAccount CustomerAccount {get;set;}
  [Required,StringLength(16)] public string Method {get;set;}
  public decimal Balance {get;set;}
  [Required,StringLength(500)] public string PinHash {get;set;}
  [Required,StringLength(500)] public string PinSalt {get;set;}
  public DateTime CreatedAt {get;set;}
 }
 public class PaymentTransaction {
  public int Id {get;set;}
  public int OrderId {get;set;}
  [ForeignKey("OrderId")] public virtual Order Order {get;set;}
  public int? CustomerAccountId {get;set;}
  [Required,StringLength(16)] public string Method {get;set;}
  public decimal Amount {get;set;}
  [Required,StringLength(16)] public string Status {get;set;}
  public DateTime CreatedAt {get;set;}
  public DateTime? PaidAt {get;set;}
 }
 public class DemoPaymentLedger {
  public int Id {get;set;}
  public int DemoPaymentAccountId {get;set;}
  [ForeignKey("DemoPaymentAccountId")] public virtual DemoPaymentAccount DemoPaymentAccount {get;set;}
  public int? OrderId {get;set;}
  [Required,StringLength(20)] public string EntryType {get;set;}
  public decimal Amount {get;set;}
  public decimal BalanceAfter {get;set;}
  public DateTime CreatedAt {get;set;}
  [StringLength(220)] public string Description {get;set;}
 }
 public class DemoPaymentPageViewModel {
  public Order Order {get;set;}
  public DemoPaymentAccount Wallet {get;set;}
  public string Error {get;set;}
 }
}
