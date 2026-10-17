# MILKTEA HOUSE — ASP.NET MVC 5 + Entity Framework 6

## Mở và chạy
1. Giải nén toàn bộ ZIP (giữ nguyên cấu trúc thư mục).
2. Dùng **Visual Studio 2026 Community** hoặc Visual Studio 2022, mở **MilkTeaShopMVC.sln**.
3. Cài workload `ASP.NET and web development` và `.NET Framework 4.8 development tools`.
4. Bật/ cài **SQL Server Express LocalDB** (`MSSQLLocalDB`) trong Visual Studio Installer nếu máy chưa có.
5. Chuột phải solution → **Restore NuGet Packages**; nếu các thư viện chưa tải, kiểm tra mạng/NuGet.org rồi Restore lại.
6. Đặt `MilkTeaShopMVC` là startup project → `Ctrl+F5`. Lần chạy đầu EF sẽ tự tạo database **MilkTeaShopMVC** và 8 sản phẩm mẫu trong SQL Server LocalDB.
7. Trang chủ: `/` — Quản trị: `/Admin/Login`.

**Admin mẫu (chỉ demo ở localhost):** `admin@milktea.local` / `Demo@12345`.
**CẢNH BÁO:** PHẢI đổi thông tin đăng nhập admin và thay đổi phương án cấp phát credential trước khi đưa web lên Internet. Không deploy public khi còn tài khoản mẫu; không sử dụng dữ liệu thật.

## Có gì trong demo?
- Trang chủ / thực đơn theo danh mục, tìm kiếm, chi tiết món
- Chọn size M/L, topping, giỏ hàng Session, cập nhật giỏ
- Checkout, lưu đơn & chi tiết đơn vào database, xác nhận đơn demo
- Đăng nhập quản trị qua forms auth, mật khẩu hash PBKDF2
- Dashboard, CRUD sản phẩm (ẩn thay vì xóa vật lý), thêm danh mục, quản lý đơn hàng
- Giao diện responsive, CSS và emoji minh họa sản phẩm

## Các chức năng chưa làm
- Đăng ký/đăng nhập người mua, thanh toán online, giao nhận thật, quản lý tồn kho, cập nhật/xóa danh mục, thay đổi password từ giao diện, audit logs.
- Không dùng production ngay; cần cấu hình security HTTPS, cookie secure, lỗi chi tiết, DB credentials riêng, luồng quản lý admin và kiểm thử thêm.

## SQL Server / Entity Framework
Cấu hình tại `MilkTeaShopMVC/Web.config` → `connectionStrings` key `MilkTeaDb`.
Mặc định `Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=MilkTeaShopMVC;Integrated Security=True;MultipleActiveResultSets=True`.
Code First + `CreateDatabaseIfNotExists` chỉ tạo mới nếu chưa tồn tại. Sau khi đổi model, cần thực hiện EF Migrations hoặc dùng DB development mới; không xóa DB đang có dữ liệu.
Kiểm tra LocalDB bằng terminal `sqllocaldb info`.

## Hosting của khoa
Tài liệu khoa nói SolidCP Windows Server, `hosting.kcntt.edu.vn`, SQL Server 2019, thư mục `wwwroot`.
1. Xác nhận ASP.NET MVC 5 / .NET Framework 4.8 có hỗ trợ trên hosting và thông số SQL Server được cấp.
2. Publish dạng **Folder** từ Visual Studio (Release, Web Deploy/Folder publish) rồi upload **NỘI DUNG THƯ MỤC ĐÃ PUBLISH** vào wwwroot, không phải cả solution chưa build.
3. Thiết lập `Web.config` connection string của database hosting, tạo schema qua EF migration / provision dữ liệu (xác nhận DB permission).
4. Điều chỉnh `compilation debug=false`, `customErrors=RemoteOnly` hoặc On, bật HTTPS và `requireSSL=true` cho forms authentication sau khi hosting SSL hoạt động.
5. Xóa/đổi tài khoản demo, quản lý secrets riêng; kiểm thử lại các URL /Home, /Cart, /Order/Checkout, /Admin.

## Cấu trúc
`Controllers/` xử lý HTTP; `Models/` chứa entity EF/DbContext; `Views/` giao diện Razor; `Content/site.css` stylesheet; `App_Start/RouteConfig.cs` route MVC.

## Vì sao tách solution?
Bạn chưa tải source project học trên máy lên chat, nên đây là solution mới độc lập. Không ghi đè vào project `2311555809_NguyenThaiHoc`. Có thể giữ cả hai và nộp solution MilkTeaShopMVC khi đã kiểm thử trên máy.
