using System;
using System.Data;
using System.Linq;

namespace MilkTeaShopMVC.Models {
 // This is a COURSE-DEMO internal ledger. No external MoMo / card transfer occurs.
 public static class DemoPaymentService {
  public const decimal InitialDemoBalance = 500000m;
  public static bool IsOnline(string method) {return method=="MOMO" || method=="CARD";}
  public static bool IsMethod(string method) {return method=="COD" || IsOnline(method);}

  public static void OpenAccount(MilkTeaDb db,int customerId,string method,string pin) {
   if(!IsOnline(method)) throw new InvalidOperationException("Phương thức không hợp lệ.");
   if(string.IsNullOrEmpty(pin) || pin.Length!=6 || pin.Any(x=>x<'0'||x>'9'))
    throw new InvalidOperationException("Mã PIN demo phải gồm đúng 6 chữ số.");
   using(var tx=db.Database.BeginTransaction(IsolationLevel.Serializable)) {
    // Serialized by customer row; unique SQL index backs up uniqueness.
    var customer=db.CustomerAccounts.SqlQuery(
     "SELECT * FROM dbo.CustomerAccounts WITH (UPDLOCK, HOLDLOCK) WHERE Id=@p0",customerId).SingleOrDefault();
    if(customer==null || !customer.IsActive) throw new InvalidOperationException("Tài khoản không hợp lệ.");
    if(db.DemoPaymentAccounts.Any(x=>x.CustomerAccountId==customerId && x.Method==method))
     throw new InvalidOperationException("Bạn đã có tài khoản demo này.");
    string salt,hash;PasswordHelper.CreateHash(pin,out salt,out hash);
    var wallet=new DemoPaymentAccount {CustomerAccountId=customerId,Method=method,
     Balance=InitialDemoBalance,PinHash=hash,PinSalt=salt,CreatedAt=DateTime.Now};
    db.DemoPaymentAccounts.Add(wallet);db.SaveChanges();
    db.DemoPaymentLedgers.Add(new DemoPaymentLedger {DemoPaymentAccountId=wallet.Id,
     EntryType="Initial",Amount=InitialDemoBalance,BalanceAfter=wallet.Balance,
     CreatedAt=DateTime.Now,Description="Số dư khởi tạo để thực hành (không phải tiền thật)"});
    db.SaveChanges();tx.Commit();
   }
  }

  public static void Pay(MilkTeaDb db,int customerId,int orderId,string pin) {
   using(var tx=db.Database.BeginTransaction(IsolationLevel.Serializable)) {
    var order=db.Orders.SqlQuery(
     "SELECT * FROM dbo.Orders WITH (UPDLOCK, HOLDLOCK) WHERE Id=@p0",orderId).SingleOrDefault();
    if(order==null || order.CustomerAccountId!=customerId)
     throw new InvalidOperationException("Không tìm thấy đơn hàng của bạn.");
    if(!IsOnline(order.PaymentMethod)) throw new InvalidOperationException("Đơn hàng không dùng ví/thẻ demo.");
    if(order.Status=="Cancelled") throw new InvalidOperationException("Đơn hàng đã hủy.");
    if(order.PaymentStatus=="Paid") {tx.Commit();return;}
    if(order.PaymentStatus!="Unpaid") throw new InvalidOperationException("Trạng thái thanh toán không hợp lệ.");
    var payment=db.PaymentTransactions.SqlQuery(
     "SELECT * FROM dbo.PaymentTransactions WITH (UPDLOCK, HOLDLOCK) WHERE OrderId=@p0",orderId).SingleOrDefault();
    if(payment==null || payment.Method!=order.PaymentMethod || payment.Amount!=order.Total || payment.Status!="Pending")
     throw new InvalidOperationException("Giao dịch không khớp đơn hàng.");
    var wallet=db.DemoPaymentAccounts.SqlQuery(
     "SELECT * FROM dbo.DemoPaymentAccounts WITH (UPDLOCK, HOLDLOCK) WHERE CustomerAccountId=@p0 AND Method=@p1",
      customerId,order.PaymentMethod).SingleOrDefault();
    if(wallet==null) throw new InvalidOperationException("Bạn chưa mở tài khoản demo tương ứng.");
    if(string.IsNullOrEmpty(pin) || !PasswordHelper.Verify(pin,wallet.PinSalt,wallet.PinHash))
     throw new InvalidOperationException("Mã PIN demo không đúng.");
    if(wallet.Balance<order.Total) throw new InvalidOperationException("Số dư demo không đủ. Hãy đề nghị Admin nạp tiền thử nghiệm.");
    wallet.Balance-=order.Total;
    order.PaymentStatus="Paid";
    payment.Status="Paid";payment.PaidAt=DateTime.Now;
    if(order.Total>0) db.DemoPaymentLedgers.Add(new DemoPaymentLedger {
     DemoPaymentAccountId=wallet.Id,OrderId=order.Id,EntryType="Debit",
     Amount=-order.Total,BalanceAfter=wallet.Balance,CreatedAt=DateTime.Now,
     Description="Thanh toán demo đơn hàng #"+order.Id
    });
    db.SaveChanges();tx.Commit();
   }
  }

  // Called INSIDE the Admin order-status transaction before its commit.
  public static void OnOrderCompleted(MilkTeaDb db,Order order) {
   var payment=db.PaymentTransactions.SqlQuery(
    "SELECT * FROM dbo.PaymentTransactions WITH (UPDLOCK, HOLDLOCK) WHERE OrderId=@p0",order.Id).SingleOrDefault();
   if(payment==null) {
    // Legacy orders placed before the add-on: existing COD functionality remains usable.
    if(order.PaymentMethod!="COD" || order.PaymentStatus!="Unpaid")
     throw new InvalidOperationException("Không có giao dịch thanh toán hợp lệ.");
    db.PaymentTransactions.Add(new PaymentTransaction {OrderId=order.Id,CustomerAccountId=order.CustomerAccountId,
     Method="COD",Amount=order.Total,Status="Paid",CreatedAt=DateTime.Now,PaidAt=DateTime.Now});
    order.PaymentStatus="Paid";return;
   }
   if(payment.Method!=order.PaymentMethod || payment.Amount!=order.Total)
    throw new InvalidOperationException("Số tiền giao dịch không khớp đơn hàng.");
   if(order.PaymentMethod=="COD") {
    if(order.PaymentStatus!="Unpaid" || payment.Status!="Pending")
     throw new InvalidOperationException("Đơn COD đã ghi nhận thanh toán.");
    payment.Status="Paid";payment.PaidAt=DateTime.Now;order.PaymentStatus="Paid";
   } else if(IsOnline(order.PaymentMethod)) {
    if(order.PaymentStatus!="Paid" || payment.Status!="Paid")
     throw new InvalidOperationException("Ví/thẻ demo chưa thanh toán. Không thể hoàn tất đơn.");
   } else throw new InvalidOperationException("Phương thức thanh toán không hợp lệ.");
  }

  // Called INSIDE the same serializable Admin cancellation transaction.
  public static void OnOrderCancelled(MilkTeaDb db,Order order) {
   var payment=db.PaymentTransactions.SqlQuery(
    "SELECT * FROM dbo.PaymentTransactions WITH (UPDLOCK, HOLDLOCK) WHERE OrderId=@p0",order.Id).SingleOrDefault();
   if(IsOnline(order.PaymentMethod) && order.PaymentStatus=="Paid") {
    if(payment==null || payment.Status!="Paid" || !order.CustomerAccountId.HasValue)
     throw new InvalidOperationException("Không thể xác minh giao dịch để hoàn tiền.");
    var wallet=db.DemoPaymentAccounts.SqlQuery(
     "SELECT * FROM dbo.DemoPaymentAccounts WITH (UPDLOCK, HOLDLOCK) WHERE CustomerAccountId=@p0 AND Method=@p1",
      order.CustomerAccountId.Value,order.PaymentMethod).SingleOrDefault();
    if(wallet==null) throw new InvalidOperationException("Không tìm thấy ví/thẻ demo để hoàn tiền.");
    wallet.Balance+=order.Total;
    if(order.Total>0) db.DemoPaymentLedgers.Add(new DemoPaymentLedger {
     DemoPaymentAccountId=wallet.Id,OrderId=order.Id,EntryType="Refund",Amount=order.Total,
     BalanceAfter=wallet.Balance,CreatedAt=DateTime.Now,Description="Hoàn tiền demo khi hủy đơn #"+order.Id
    });
    payment.Status="Refunded";order.PaymentStatus="Refunded";
   } else {
    if(order.PaymentStatus=="Paid") throw new InvalidOperationException("Không thể hủy đơn đã thu tiền mặt.");
    order.PaymentStatus="Unpaid";
    if(payment!=null && payment.Status=="Pending") payment.Status="Cancelled";
   }
  }

  public static void TopUp(MilkTeaDb db,int accountId,decimal amount) {
   if(amount<10000 || amount>2000000 || amount!=decimal.Truncate(amount))
    throw new InvalidOperationException("Chỉ cho phép nạp thử từ 10.000đ đến 2.000.000đ.");
   using(var tx=db.Database.BeginTransaction(IsolationLevel.Serializable)) {
    var wallet=db.DemoPaymentAccounts.SqlQuery(
     "SELECT * FROM dbo.DemoPaymentAccounts WITH (UPDLOCK, HOLDLOCK) WHERE Id=@p0",accountId).SingleOrDefault();
    if(wallet==null) throw new InvalidOperationException("Không tìm thấy tài khoản demo.");
    wallet.Balance+=amount;
    db.DemoPaymentLedgers.Add(new DemoPaymentLedger {DemoPaymentAccountId=wallet.Id,
     EntryType="TopUp",Amount=amount,BalanceAfter=wallet.Balance,
     CreatedAt=DateTime.Now,Description="Admin nạp số dư thử nghiệm"});
    db.SaveChanges();tx.Commit();
   }
  }
 }
}
