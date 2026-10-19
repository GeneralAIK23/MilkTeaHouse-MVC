# Chuẩn bị và bàn giao database MilkTeaShopMVC

## Vì sao cần script SQL?

Project dùng `Database.SetInitializer<MilkTeaDb>(null)` nên **không tự sinh đầy đủ bảng SQL**. `Web.config` chỉ là nơi cấu hình kết nối, **không phải dữ liệu và cấu trúc database**.

## Cách xuất schema bằng SSMS (trên máy đang chạy website)

1. Mở SQL Server Management Studio (SSMS), kết nối đúng instance của ứng dụng; ví dụ `localhost` với Windows Authentication.
2. Trong Object Explorer: **Databases → MilkTeaShopMVC**.
3. Nhấp phải database → **Tasks → Generate Scripts...**.
4. Chọn **Script entire database and all database objects** (hoặc chọn toàn bộ các bảng, khóa và đối tượng cần dùng). Bấm **Next**.
5. Ở **Set Scripting Options**, chọn **Save to file**, lưu file SQL vào một thư mục riêng.
6. Mở **Advanced...** và kiểm tra:
   - **Types of data to script = Schema only** để xuất cấu trúc, không đưa khách hàng/đơn hàng thật lên GitHub.
   - Bật **Script Indexes**, **Script Primary Keys**, **Script Foreign Keys**, **Script Check Constraints** và các ràng buộc cần thiết nếu có lựa chọn tương ứng.
   - Nếu thầy dùng SQL Server cũ hơn, cân nhắc **Script for Server Version** phù hợp và kiểm tra tương thích.
7. Bấm **Next → Next → Finish**, kiểm tra script tạo đủ bảng.
8. Đặt file SQL đã kiểm tra vào `database/MilkTeaShopMVC_schema.sql` trong repository, **sau khi đảm bảo không chứa thông tin bí mật**.

## Dữ liệu mẫu — tùy chọn nhưng hữu ích khi demo

- Tạo dữ liệu mẫu mới (Categories, Products, Toppings, tài khoản demo mới) từ dữ liệu giả, **không xuất nguyên bảng CustomerAccounts, Orders, PaymentTransactions** nếu có dữ liệu cá nhân.
- Chỉ khi dữ liệu sạch, có thể lưu `database/MilkTeaShopMVC_demo_data.sql` và ghi rõ thứ tự chạy.
- Nếu chưa có dữ liệu mẫu đã chuẩn hóa, vẫn có thể nộp schema nhưng giảng viên sẽ cần tạo dữ liệu trước khi kiểm thử giao diện sản phẩm.

## Trên máy cần chạy đồ án

1. Mở SSMS → New Query, mở file `MilkTeaShopMVC_schema.sql` đã được xuất và chạy (F5) trên instance cần sử dụng.
2. Nếu có `MilkTeaShopMVC_demo_data.sql`, chạy sau schema.
3. Xác nhận database và bảng xuất hiện bằng:

```sql
USE MilkTeaShopMVC;
GO
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
```

4. Trong `MilkTeaShopMVC/Web.config`, chỉnh `Data Source` đúng instance và `Initial Catalog=MilkTeaShopMVC`.
5. Nếu SQL script chưa tạo câu `CREATE DATABASE`, tạo database `MilkTeaShopMVC` trong SSMS trước rồi chạy script trên đúng database. Tránh chạy schema sang `master`.

## Về file backup (.bak)

- Có thể dùng **Tasks → Back Up...** và **Restore Database...** để chuyển cơ sở dữ liệu trong môi trường riêng tư.
- Không commit `.bak`, `.mdf`, `.ldf` hoặc dump chứa thông tin cá nhân vào GitHub.
- Với SQL Server đích phiên bản **cũ hơn**, restore từ backup của SQL Server mới hơn có thể không được hỗ trợ; script SQL tương thích thường linh hoạt hơn.

> Ghi chú: README này **không tự nhận** đã xuất database trên máy người dùng. Người tạo dự án cần thực hiện các bước SSMS để có file SQL hoàn chỉnh.
