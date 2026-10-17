using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System;
namespace MilkTeaShopMVC.Models {
 public class MilkTeaDb : DbContext {
  public MilkTeaDb():base("MilkTeaDb"){}
  public DbSet<Category> Categories {get;set;}
  public DbSet<Product> Products {get;set;}
  public DbSet<Topping> Toppings {get;set;}
  public DbSet<Order> Orders {get;set;}
  public DbSet<OrderLine> OrderLines {get;set;}
  public DbSet<OrderLineTopping> OrderLineToppings {get;set;}
  public DbSet<AdminUser> AdminUsers {get;set;}
  public DbSet<CustomerAccount> CustomerAccounts {get;set;}
  protected override void OnModelCreating(DbModelBuilder modelBuilder) {
   modelBuilder.Entity<Product>().Property(x=>x.Price).HasPrecision(18,0);
   modelBuilder.Entity<Topping>().Property(x=>x.Price).HasPrecision(18,0);
   modelBuilder.Entity<Order>().Property(x=>x.Total).HasPrecision(18,0);
   modelBuilder.Entity<OrderLine>().Property(x=>x.UnitPrice).HasPrecision(18,0);
   modelBuilder.Entity<OrderLineTopping>().Property(x=>x.UnitPrice).HasPrecision(18,0);
   modelBuilder.Entity<OrderLineTopping>().HasRequired(x=>x.OrderLine)
    .WithMany(x=>x.ToppingDetails).HasForeignKey(x=>x.OrderLineId).WillCascadeOnDelete(true);
   modelBuilder.Entity<OrderLine>().HasOptional(x=>x.Product).WithMany().HasForeignKey(x=>x.ProductId).WillCascadeOnDelete(false);
   modelBuilder.Entity<OrderLine>().HasRequired(x=>x.Order).WithMany(x=>x.Lines).HasForeignKey(x=>x.OrderId).WillCascadeOnDelete(true);
   base.OnModelCreating(modelBuilder);
  }
 }
 public class MilkTeaInitializer:CreateDatabaseIfNotExists<MilkTeaDb> {
  protected override void Seed(MilkTeaDb db) {
   var c1=new Category{Name="Trà sữa"};var c2=new Category{Name="Trà trái cây"};var c3=new Category{Name="Đá xay"};
   db.Categories.AddRange(new[]{c1,c2,c3});
   db.Products.AddRange(new [] {
    new Product{Name="Trà sữa trân châu đường đen",Description="Trà sữa thơm béo, trân châu nấu đường đen, vị ngọt đậm đà.",Price=39000,Category=c1,Emoji="🧋",Theme="caramel"},
    new Product{Name="Trà sữa matcha",Description="Matcha thanh mát hòa cùng sữa tươi dịu nhẹ.",Price=45000,Category=c1,Emoji="🍵",Theme="matcha"},
    new Product{Name="Trà sữa ô long",Description="Hương trà ô long rang nhẹ, hậu vị tinh tế.",Price=42000,Category=c1,Emoji="🧋",Theme="oolong"},
    new Product{Name="Trà đào cam sả",Description="Trà đào ngọt dịu với cam tươi và sả thơm.",Price=43000,Category=c2,Emoji="🍑",Theme="peach"},
    new Product{Name="Trà vải lài",Description="Hương hoa lài nhẹ nhàng quyện cùng vải mọng.",Price=40000,Category=c2,Emoji="🍹",Theme="lychee"},
    new Product{Name="Trà chanh dây",Description="Chua ngọt tự nhiên, mát lành ngày nóng.",Price=38000,Category=c2,Emoji="🍋",Theme="lemon"},
    new Product{Name="Matcha đá xay",Description="Matcha đá xay mịn, phủ kem sữa béo nhẹ.",Price=52000,Category=c3,Emoji="🥤",Theme="matcha"},
    new Product{Name="Chocolate đá xay",Description="Chocolate đậm vị, mát lạnh và sánh mịn.",Price=54000,Category=c3,Emoji="🍫",Theme="cocoa"}
   });
   db.Toppings.AddRange(new[]{new Topping{Name="Trân châu đen",Price=7000},new Topping{Name="Thạch phô mai",Price=9000},new Topping{Name="Pudding trứng",Price=8000},new Topping{Name="Kem cheese",Price=10000}});
   string salt, hash; PasswordHelper.CreateHash("Demo@12345",out salt,out hash);
   db.AdminUsers.Add(new AdminUser{Email="admin@milktea.local",PasswordSalt=salt,PasswordHash=hash});
   db.SaveChanges();base.Seed(db);
  }
 }
}
