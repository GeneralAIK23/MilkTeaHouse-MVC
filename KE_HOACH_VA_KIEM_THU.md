# Kế hoạch demo & kiểm thử (môn Lập trình Web)
## Luồng demo 5 phút
1. Trang chủ, tìm "matcha" và lọc danh mục.
2. Mở trà sữa → chọn size L + pudding → thêm vào giỏ; cập nhật số lượng.
3. Checkout bằng thông tin mẫu → mã đơn được tạo.
4. Đăng nhập admin → xem số đơn / doanh thu → mở đơn → cập nhật trạng thái.
5. Thêm sản phẩm từ Admin/Products → mở trang chủ kiểm tra; sau đó ẩn sản phẩm.

## Test cases
| ID | Thao tác | Kỳ vọng |
|---|---|---|
| T01 | Mở trang chủ | Hiển thị 8 sản phẩm khởi tạo |
| T02 | Lọc danh mục "Trà sữa" | Chỉ có sản phẩm trong danh mục |
| T03 | Tìm "matcha" | Hiện món chứa matcha |
| T04 | Chọn size L, topping | Giá bằng giá gốc + 7k + topping |
| T05 | Thêm giỏ, cập nhật SL 2 | Giỏ cộng đúng tiền |
| T06 | Xác nhận checkout trống họ tên | Không cho tạo đơn |
| T07 | Checkout thông tin hợp lệ | Đơn lưu SQL và có mã |
| T08 | Truy cập /Admin khi chưa login | Chuyển login |
| T09 | Sai mật khẩu admin | Không đăng nhập được |
| T10 | Admin đổi trạng thái Completed | Đơn được cập nhật |
| T11 | Ẩn sản phẩm | Không còn hiển thị trên trang chủ |
| T12 | URL /Product/Details/99999 | HTTP 404 |

## Lưu ý cho báo cáo
Tên đề tài, use case, ERD, MVC, CSDL, giao diện, kiểm thử, triển khai. Quy trình đặt hàng ở đây là **mô phỏng**, không có cổng thanh toán hay gửi đơn cho tiệm thật.
