using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace MilkTeaShopMVC.Models {
 public class ShopViewModel {public List<Product> Products {get;set;} public List<Category> Categories {get;set;} public int? CategoryId {get;set;} public string Search {get;set;} }
 public class ProductDetailViewModel {public Product Product {get;set;} public List<Topping> Toppings {get;set;} }
 public class CheckoutViewModel {
  [Required(ErrorMessage="Vui lòng nhập họ tên"),StringLength(100)] public string CustomerName {get;set;}
  [Required(ErrorMessage="Vui lòng nhập số điện thoại"),RegularExpression(@"^[0-9+(). -]{9,20}$",ErrorMessage="Số điện thoại không hợp lệ")] public string Phone {get;set;}
  [Required(ErrorMessage="Vui lòng nhập địa chỉ"),StringLength(250)] public string Address {get;set;}
  [StringLength(500)] public string Note {get;set;}
  [StringLength(40)] public string PromoCode {get;set;}
  [Range(0,1000000,ErrorMessage="Số điểm không hợp lệ")] public int PointsToRedeem {get;set;}
  public List<CartLine> Items {get;set;} = new List<CartLine>();
 }
 public class LoginViewModel {[Required,EmailAddress] public string Email {get;set;} [Required,DataType(DataType.Password)] public string Password {get;set;} }
}
