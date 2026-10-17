using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MilkTeaShopMVC.Models {
 public class Category { public int Id {get;set;} [Required,StringLength(80)] public string Name {get;set;} public virtual ICollection<Product> Products {get;set;} = new List<Product>(); }
 public class Product {
    public string ImageUrl { get; set; }
  public int Id {get;set;}
  [Required,StringLength(120)] public string Name {get;set;}
  [Required,StringLength(700)] public string Description {get;set;}
  [Range(1000,1000000)] public decimal Price {get;set;}
  [StringLength(30)] public string Emoji {get;set;}
  [StringLength(30)] public string Theme {get;set;}
  public int CategoryId {get;set;}
  [ForeignKey("CategoryId")] public virtual Category Category {get;set;}
  public bool IsAvailable {get;set;} = true;
 }
 public class Topping { public int Id {get;set;} [Required,StringLength(100)] public string Name {get;set;} [Range(0,100000)] public decimal Price {get;set;} public bool IsAvailable {get;set;}=true; }
 public class AdminUser {
  public int Id {get;set;} [Required,StringLength(120)] public string Email {get;set;}
  [Required] public string PasswordHash {get;set;}
  [Required] public string PasswordSalt {get;set;}
 }
 public class Order {
  public int Id {get;set;}
  public int? CustomerAccountId {get;set;} [System.ComponentModel.DataAnnotations.Schema.ForeignKey("CustomerAccountId")] public virtual CustomerAccount CustomerAccount {get;set;}
  [Required,StringLength(100)] public string CustomerName {get;set;}
  [Required,StringLength(20)] public string Phone {get;set;}
  [Required,StringLength(250)] public string Address {get;set;}
  [StringLength(500)] public string Note {get;set;}
  public DateTime CreatedAt {get;set;}=DateTime.Now;
  [Required,StringLength(24)] public string Status {get;set;}="Pending";
  public decimal Total {get;set;}
  public virtual ICollection<OrderLine> Lines {get;set;}=new List<OrderLine>();
 }
 public class OrderLine {
  public int Id {get;set;} public int OrderId {get;set;} [ForeignKey("OrderId")] public virtual Order Order {get;set;}
  public int? ProductId {get;set;} [ForeignKey("ProductId")] public virtual Product Product {get;set;}
  [Required,StringLength(120)] public string ProductName {get;set;}
  [StringLength(20)] public string Size {get;set;}
  [StringLength(200)] public string Toppings {get;set;}
  public virtual ICollection<OrderLineTopping> ToppingDetails {get;set;} = new List<OrderLineTopping>();
  public int Quantity {get;set;} public decimal UnitPrice {get;set;}
 }
 // Chi tiet topping: luu don gia tai thoi diem dat hang (snapshot).
 public class OrderLineTopping {
  public int Id {get;set;}
  public int OrderLineId {get;set;}
  [ForeignKey("OrderLineId")] public virtual OrderLine OrderLine {get;set;}
  public int? ToppingId {get;set;}
  [Required, StringLength(100)] public string ToppingName {get;set;}
  [Range(1,5)] public int Quantity {get;set;}
  [Range(typeof(decimal),"0","100000")] public decimal UnitPrice {get;set;}
 }
 // Du lieu topping di kem 1 ly trong gio hang (session).
 public class CartToppingChoice {
  public int ToppingId {get;set;}
  public string Name {get;set;}
  public decimal Price {get;set;}
  public int Quantity {get;set;}
 }
 public class CartLine {
  public int ProductId {get;set;} public string ProductName {get;set;} public string Emoji {get;set;}
  public string Size {get;set;}
  // Du lai 2 thuoc tinh cu de ho tro gio hang cua session truoc nang cap.
  public int? ToppingId {get;set;} public string ToppingName {get;set;}
  public List<CartToppingChoice> ToppingSelections {get;set;} = new List<CartToppingChoice>();
  public decimal UnitPrice {get;set;} public int Quantity {get;set;}
  public decimal Subtotal => UnitPrice * Quantity;
  public string ToppingSummary {
   get {
    if(ToppingSelections != null && ToppingSelections.Count > 0)
     return string.Join("; ", ToppingSelections.OrderBy(t=>t.ToppingId)
        .Select(t=>t.Name + " x" + t.Quantity));
    return ToppingId.HasValue ? ToppingName : "";
   }
  }
  public string Key {
   get {
    if(ToppingSelections != null && ToppingSelections.Count > 0) {
     var suffix=string.Join(",",ToppingSelections.OrderBy(t=>t.ToppingId)
      .Select(t=>t.ToppingId+"x"+t.Quantity));
     return ProductId+"-"+Size+"-"+suffix;
    }
    return ProductId+"-"+Size+"-"+(ToppingId.HasValue ? ToppingId.Value.ToString() : "0");
   }
  }
 }
}
