# MilkTeaHouse — Website bán và quản lý trà sữa

> **Đồ án môn Lập trình Web** — ASP.NET MVC 5, C#, Entity Framework 6, SQL Server.  
> Phần thanh toán MoMo/thẻ trong đồ án là **mô phỏng với tài khoản và số dư trong SQL Server**, không kết nối hệ thống thanh toán thật.

## 1. Giới thiệu

MilkTeaHouse là website phục vụ khách hàng đặt đồ uống và nhân viên quản trị theo dõi hoạt động bán hàng. Các chức năng trong phiên bản dự án có thể bao gồm:

- Xem danh mục, thực đơn và chi tiết sản phẩm; chọn kích thước và nhiều topping.
- Giỏ hàng, tài khoản khách hàng và lịch sử đơn hàng.
- Mã khuyến mãi, tích/đổi điểm thành viên.
- Thanh toán khi nhận hàng và tài khoản ví/thẻ **demo**, ghi nhận giao dịch trong SQL Server.
- Giao diện quản trị sản phẩm, đơn hàng, khuyến mãi, điểm, thanh toán mô phỏng và yêu cầu liên hệ/nhượng quyền.

**Lưu ý phiên bản:** Danh sách tính năng trên dựa vào source đồ án và các bản nâng cấp đã thực hiện. Người dùng cần Build và kiểm thử chức năng trên bản commit đang xem; source được cập nhật theo tiến độ môn học.

## 2. Công nghệ

| Thành phần | Công nghệ |
| --- | --- |
| Backend | C#, ASP.NET MVC 5, .NET Framework 4.8 |
| ORM | Entity Framework 6 |
| Database | Microsoft SQL Server; database `MilkTeaShopMVC` |
| Frontend | Razor (.cshtml), HTML, CSS, JavaScript, Bootstrap |
| Công cụ | Visual Studio, SSMS, Git/GitHub, IIS Express |

## 3. Cấu trúc source code

```text
MilkTeaHouse-MVC/
├── MilkTeaShopMVC.sln
├── MilkTeaShopMVC/
│   ├── Controllers/                  # Xử lý yêu cầu HTTP
│   ├── Models/                       # Entity, ViewModel, DbContext, nghiệp vụ
│   │   ├── MilkTeaDb.cs             # Kết nối EF6, DbSet
│   │   ├── Entities.cs              # Các đối tượng dữ liệu cơ bản
│   │   ├── DemoPaymentService.cs    # Nghiệp vụ thanh toán mô phỏng (nếu đã cài)
│   │   └── ...
│   ├── Views/                        # Razor Views (.cshtml)
│   │   └── Web.config               # Cấu hình Razor; KHÔNG là chuỗi kết nối SQL
│   ├── Content/                      # CSS, ảnh, tài nguyên tĩnh
│   ├── App_Start/                    # Khai báo route MVC
│   ├── Global.asax.cs                # Khởi động ứng dụng
│   ├── packages.config              # Các NuGet package
│   ├── Web.config.example           # Bản cấu hình mẫu (đưa lên Git)
│   └── Web.config                   # Cấu hình thật trên máy (KHÔNG commit)
└── database/
    └── README.md                    # Hướng dẫn xuất và khôi phục DB
```

> Tên website là **MilkTeaHouse**; project/namespace và tên database dùng **MilkTeaShopMVC**. Đây là chủ ý, không phải lỗi.

## 4. Yêu cầu trước khi chạy

1. Windows, **Visual Studio 2022 hoặc 2026**, có workload **ASP.NET and web development**, .NET Framework **4.8 Developer Pack/Targeting Pack** và IIS Express.
2. SQL Server (có thể dùng SQL Server 2025 hay bản tương thích), **SQL Server Management Studio (SSMS)**.
3. Git, kết nối Internet để khôi phục thư viện NuGet khi cần.
4. **Database `MilkTeaShopMVC` có đầy đủ schema của phiên bản code đang chạy**. Xem mục 6 và `database/README.md`.

## 5. Tải source và chuẩn bị project

**5.1. Clone repository** (repository Private yêu cầu được cấp quyền):

```powershell
git clone https://github.com/GeneralAIK23/MilkTeaHouse-MVC.git
cd MilkTeaHouse-MVC
```

**5.2. Tạo Web.config riêng cho máy đang chạy**

Trong Developer PowerShell, tại thư mục gốc có `MilkTeaShopMVC.sln`:

```powershell
Copy-Item '.\MilkTeaShopMVC\Web.config.example' '.\MilkTeaShopMVC\Web.config'
```

Chỉ chạy lệnh trên khi chưa có file `MilkTeaShopMVC/Web.config`; nếu đã cấu hình kết nối riêng thì **không ghi đè**. Mở file mới tạo và chỉnh `Data Source` phù hợp với instance SQL Server ở máy.

**5.3. Mở solution**

```powershell
Start-Process '.\MilkTeaShopMVC.sln'
```

Trong Visual Studio: **Restore NuGet Packages**, chọn project `MilkTeaShopMVC` làm **Startup Project**, sau đó **Build/Rebuild Solution**. Gói NuGet được khai báo trong `MilkTeaShopMVC/packages.config`.

## 6. Kết nối SQL Server — phần quan trọng

### 6.1. Mở SQL Server

Mở SSMS → **Connect → Database Engine**. Chọn:

- **Server name:** `localhost` nếu SQL Server được cài làm instance mặc định; với SQL Express có thể là `.\\SQLEXPRESS` hoặc `TENMAY\\SQLEXPRESS` (tùy máy).
- **Authentication:** `Windows Authentication` theo cấu hình mẫu.

Trong SSMS chạy câu lệnh kiểm tra (chỉ đọc):

```sql
SELECT @@SERVERNAME AS ServerName, DB_NAME() AS CurrentDatabase;
SELECT name FROM sys.databases WHERE name = N'MilkTeaShopMVC';
```

Nếu không thấy `MilkTeaShopMVC`, bạn cần **khôi phục hoặc tạo đầy đủ cơ sở dữ liệu** trước khi chạy website; chỉ tạo database trống là chưa đủ.

### 6.2. Cấu hình chuỗi kết nối

Mở `MilkTeaShopMVC/Web.config` **tại thư mục gốc của project**, tìm:

```xml
<connectionStrings>
  <add name="MilkTeaDb"
       connectionString="Data Source=localhost;Initial Catalog=MilkTeaShopMVC;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

Giải thích:

| Thuộc tính | Ý nghĩa |
| --- | --- |
| `Data Source=localhost` | Instance SQL Server mà website kết nối đến |
| `Initial Catalog=MilkTeaShopMVC` | Database của website |
| `Integrated Security=True` | Xác thực bằng tài khoản Windows đang chạy ứng dụng |
| `MultipleActiveResultSets=True` | Cho phép nhiều tập kết quả hoạt động |
| `TrustServerCertificate=True` | Chấp nhận chứng chỉ SQL Server trong môi trường phát triển; cần cấu hình TLS đúng khi triển khai thực tế |

**Phân biệt:** `MilkTeaShopMVC/Views/Web.config` chỉ phục vụ Razor, không phải nơi thay đổi chuỗi kết nối SQL.

### 6.3. Liên kết code C# với bảng SQL

Trong `MilkTeaShopMVC/Models/MilkTeaDb.cs`, lớp `MilkTeaDb` kế thừa `DbContext` và gọi `base("MilkTeaDb")`, tức là lấy connection string cùng tên trong `Web.config`.

Các `DbSet` ánh xạ entity tới dữ liệu SQL, ví dụ `Products`, `Orders`, `CustomerAccounts`. Trong Controller/Service, các lệnh điển hình:

```csharp
using (var db = new MilkTeaDb())
{
    var products = db.Products.ToList();       // Đọc sản phẩm
    // db.Products.Add(product);                // Thêm sản phẩm (ví dụ)
    // db.SaveChanges();                        // Ghi thay đổi xuống SQL
}
```

Sửa giá/tên sản phẩm trong SQL Server sẽ phản ánh khi trang web truy vấn lại database; lưu ý có thể có giỏ hàng lưu trong Session hoặc dữ liệu đã được chốt theo giá tại lúc đặt hàng.

### 6.4. Khôi phục database trên máy khác

**Tại thời điểm tạo README này, repository chưa được xác nhận có một SQL script đầy đủ để tái tạo toàn bộ database.** Các chức năng được bổ sung theo từng đợt nên không thể coi riêng một file migration/add-on là đủ tạo database sạch.

Người bàn giao dự án cần cung cấp một trong hai cách:

- **Ưu tiên:** `database/MilkTeaShopMVC_schema.sql` tạo **toàn bộ bảng, khóa, chỉ mục, ràng buộc** của phiên bản đang chạy. Có thể kèm `database/MilkTeaShopMVC_demo_data.sql` chỉ chứa **dữ liệu thử nghiệm đã ẩn danh**.
- **Hoặc:** Bản sao lưu `MilkTeaShopMVC.bak` được chuyển **riêng tư** cho giảng viên sau khi xóa dữ liệu cá nhân/mật khẩu/khóa không cần thiết. Không commit backup thực tế lên GitHub.

Trên máy giảng viên, mở SSMS và chạy SQL script tương ứng theo thứ tự schema rồi dữ liệu mẫu; nếu dùng `.bak` thì chọn **Databases → Restore Database**. Xem hướng dẫn xuất chi tiết tại [`database/README.md`](database/README.md).

**Quan trọng:** File `Global.asax.cs` của phiên bản source được kiểm tra đang đặt `Database.SetInitializer<MilkTeaDb>(null)`, nghĩa là EF **không tự tạo hoặc cập nhật bảng SQL** khi khởi động. Không bỏ qua bước chuẩn bị database.

### 6.5. Kiểm tra database sau khi khôi phục

Trong SSMS:

```sql
USE MilkTeaShopMVC;
GO
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
GO
SELECT TOP (10) Id, Name, Price, IsAvailable
FROM dbo.Products
ORDER BY Id;
```

Nếu lỗi `Invalid object name`, `Cannot open database` hoặc `Login failed`, cần xác nhận instance SQL Server, quyền tài khoản Windows, tên database và schema tương thích với source.

## 7. Build và chạy website

Từ thư mục chứa `MilkTeaShopMVC.sln`, dùng Developer PowerShell:

```powershell
msbuild '.\MilkTeaShopMVC\MilkTeaShopMVC.csproj' /t:Rebuild /p:Configuration=Debug /v:minimal /nologo
```

Sau khi Build thành công, trong Visual Studio nhấn **Ctrl + F5** để mở website bằng IIS Express. **Cổng localhost do Visual Studio cấp có thể thay đổi**, ví dụ `http://localhost:xxxx/`; không nhầm với `Data Source=localhost` của SQL Server.

Một số đường dẫn chức năng (tùy bản code):

| Đường dẫn | Ý nghĩa |
| --- | --- |
| `/` | Trang chủ |
| `/Menu` | Thực đơn |
| `/Cart` | Giỏ hàng |
| `/Order/Checkout` | Đặt hàng, chọn cách thanh toán |
| `/Account/Login` | Đăng nhập khách hàng |
| `/Promotions` | Mã khuyến mãi |
| `/Loyalty` | Điểm thưởng |
| `/Payment` | Ví và thẻ mô phỏng (nếu đã cài đặt) |
| `/Admin/Login` | Đăng nhập quản trị |
| `/PaymentAdmin` | Quản lý giao dịch demo (nếu đã cài đặt) |

## 8. Kiểm thử cơ bản

1. Kiểm tra trang chủ/thực đơn lấy được dữ liệu từ `dbo.Products`.
2. Sửa tên hoặc giá một sản phẩm **thử nghiệm** trong SSMS, tải lại thực đơn và xác nhận giao diện cập nhật (sau đó trả dữ liệu về giá trị ban đầu).
3. Đăng ký/đăng nhập, thêm món, chọn size/topping, đặt hàng thử.
4. Kiểm tra `dbo.Orders`, `dbo.OrderLines` trong SSMS và trạng thái đơn trong Admin.
5. Thử áp mã khuyến mãi, đổi điểm và xác nhận tổng tiền.
6. Với COD, Admin chỉ xác nhận thanh toán khi đã thu tiền; với tài khoản demo, kiểm tra số dư và lịch sử trừ/hoàn tiền trong SQL. Không sử dụng tài khoản ngân hàng thật.

## 9. Bảo mật và giới hạn

- Không đưa `MilkTeaShopMVC/Web.config` thật, `.env`, mật khẩu, API key, khóa cá nhân, `.bak`, `.mdf`, `.ldf` hay dữ liệu khách hàng thật vào GitHub.
- `Web.config.example` chỉ là mẫu dùng ở môi trường học tập. **Không công bố website với `debug=true`, `customErrors=Off`, `requireSSL=false`** như bản phát triển.
- Không dùng tài khoản/mật khẩu demo làm tài khoản thật.
- MoMo và Thẻ Demo là **mô phỏng nghiệp vụ**, không có giao dịch tiền thật, không kết nối với hệ thống ngân hàng.
- Đây là đồ án môn học, chưa phải hệ thống production.

## 10. Tác giả và repository

- Trường Đại học Nguyễn Tất Thành — Khoa Công nghệ thông tin.
- Môn học: Lập trình Web.
- Repository: https://github.com/GeneralAIK23/MilkTeaHouse-MVC
- Thông tin sinh viên, giảng viên và nhóm: bổ sung theo hồ sơ nộp bài.
