// MILKTEAHOUSE STATUS LABELS V1
// Keep database values unchanged. Translate only when displaying on screen.
namespace MilkTeaShopMVC.Models {
    public static class MilkTeaStatusVi {
        public static string Order(string value) {
            switch ((value ?? "").Trim()) {
                case "Pending": return "Chờ xử lý";
                case "Confirmed": return "Đã xác nhận";
                case "Delivering": return "Đang giao hàng";
                case "Completed": return "Hoàn thành";
                case "Cancelled": return "Đã hủy";
                default: return value ?? "";
            }
        }
        public static string Payment(string value) {
            switch ((value ?? "").Trim()) {
                case "Unpaid": return "Chưa thanh toán";
                case "Paid": return "Đã thanh toán";
                case "Pending": return "Chờ thanh toán";
                case "Processing": return "Đang xử lý thanh toán";
                case "Failed": return "Thanh toán thất bại";
                case "Refunded": return "Đã hoàn tiền";
                case "Cancelled": return "Đã hủy";
                default: return value ?? "";
            }
        }
        public static string Method(string value) {
            switch ((value ?? "").Trim().ToUpperInvariant()) {
                case "COD": return "Thanh toán khi nhận hàng";
                case "MOMO": return "Ví MoMo";
                case "CARD": return "Thẻ ngân hàng";
                case "BANKTRANSFER": return "Chuyển khoản ngân hàng";
                case "VNPAY": return "Thanh toán qua VNPay";
                default: return value ?? "";
            }
        }
        public static string PointType(string value) {
            switch ((value ?? "").Trim()) {
                case "Earn": return "Tích điểm";
                case "Redeem": return "Sử dụng điểm";
                case "RefundRedeem": return "Hoàn điểm";
                default: return value ?? "";
            }
        }
    }
}