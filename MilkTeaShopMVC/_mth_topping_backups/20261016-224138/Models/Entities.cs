using System;
using System.Collections.Generic;
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
  public int Quantity {get;set;} public decimal UnitPrice {get;set;}
 }
 public class CartLine {
  public int ProductId {get;set;} public string ProductName {get;set;} public string Emoji {get;set;}
  public string Size {get;set;} public int? ToppingId {get;set;} public string ToppingName {get;set;}
  public decimal UnitPrice {get;set;} public int Quantity {get;set;}
  public decimal Subtotal => UnitPrice * Quantity;
  public string Key => ProductId+"-"+Size+"-"+(ToppingId.HasValue ? ToppingId.Value.ToString() : "0");
 }
}
