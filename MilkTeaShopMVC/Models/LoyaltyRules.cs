using System;
using System.Linq;
using System.Data;
using MilkTeaShopMVC.Models;

namespace MilkTeaShopMVC.Models {
 public static class LoyaltyRules {
  public const int VndPerEarnedPoint=10000;
  public const int VndPerRedeemedPoint=200;
  public static int Earned(decimal paidAmount) {
   if(paidAmount<=0) return 0;
   return checked((int)decimal.Floor(paidAmount/VndPerEarnedPoint));
  }
  public static decimal RedeemValue(int points) {return points* (decimal)VndPerRedeemedPoint;}
 }

 // Quan tri trang thai don hang. Moi phep doi trang thai la mot giao dich SQL.
 // KHONG cong diem khi khach chi bam Dat hang.
 public static class LoyaltyOrderService {
  public static void UpdateStatus(MilkTeaDb db,int orderId,string target) {
   if(!new[]{"Pending","Confirmed","Delivering","Completed","Cancelled"}.Contains(target))
    throw new InvalidOperationException("Trạng thái không hợp lệ.");
   using(var tx=db.Database.BeginTransaction(IsolationLevel.Serializable)) {
    // Gianh khoa de hai request khong the cong/hoan diem 2 lan.
    var order=db.Orders.SqlQuery(
      "SELECT * FROM dbo.Orders WITH (UPDLOCK, HOLDLOCK) WHERE Id=@p0",orderId).SingleOrDefault();
    if(order==null) throw new InvalidOperationException("Không tìm thấy đơn hàng.");
    if(order.Status==target) {tx.Commit();return;}
    if(order.Status=="Cancelled" || order.Status=="Completed")
      throw new InvalidOperationException("Đơn đã kết thúc; không thể đổi trạng thái qua màn hình này.");

    if(target=="Completed") {
     // Admin chi chon Completed SAU KHI THUC SU DA NHAN TIEN (COD o giai doan nay).
     // Sau nay, MoMo/the phai xac nhan bang webhook duoc kiem chung.
     DemoPaymentService.OnOrderCompleted(db,order);
     order.Status="Completed";
     if(order.CustomerAccountId.HasValue) {
      var customer=db.CustomerAccounts.SqlQuery(
       "SELECT * FROM dbo.CustomerAccounts WITH (UPDLOCK, HOLDLOCK) WHERE Id=@p0",order.CustomerAccountId.Value).SingleOrDefault();
      if(customer!=null) {
       int earned=LoyaltyRules.Earned(order.Total);
       if(earned>0) {
        customer.PointsBalance=checked(customer.PointsBalance+earned);
        order.PointsEarned=earned;
        db.LoyaltyPointTransactions.Add(new LoyaltyPointTransaction {
          CustomerAccountId=customer.Id,OrderId=order.Id,Type="Earn",
          PointsDelta=earned,CreatedAt=DateTime.Now,
          Description="Tích điểm đơn hàng đã thanh toán #"+order.Id
        });
       }
      }
     }
    } else if(target=="Cancelled") {
     DemoPaymentService.OnOrderCancelled(db,order);
     order.Status="Cancelled";
     if(order.CustomerAccountId.HasValue && order.PointsRedeemed>0) {
      var customer=db.CustomerAccounts.SqlQuery(
       "SELECT * FROM dbo.CustomerAccounts WITH (UPDLOCK, HOLDLOCK) WHERE Id=@p0",order.CustomerAccountId.Value).SingleOrDefault();
      if(customer==null) throw new InvalidOperationException("Không tìm thấy thành viên để hoàn điểm.");
      customer.PointsBalance=checked(customer.PointsBalance+order.PointsRedeemed);
      db.LoyaltyPointTransactions.Add(new LoyaltyPointTransaction {
       CustomerAccountId=customer.Id,OrderId=order.Id,Type="RefundRedeem",
       PointsDelta=order.PointsRedeemed,CreatedAt=DateTime.Now,
       Description="Hoàn điểm do hủy đơn #"+order.Id
      });
     }
     // Tra luot su dung ma khi huy don; luon nam trong cung transaction.
     if(!string.IsNullOrWhiteSpace(order.PromotionCode)) {
      db.Database.ExecuteSqlCommand(
       "UPDATE dbo.PromotionCodes SET UsedCount=UsedCount-1 WHERE Code=@p0 AND UsedCount>0",order.PromotionCode);
     }
    } else {
     order.Status=target;
    }
    db.SaveChanges();
    tx.Commit();
   }
  }
 }
}
