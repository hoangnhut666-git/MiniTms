# TÀI LIỆU ĐẶC TẢ HỆ THỐNG MINI-TMS (TRANSPORTATION MANAGEMENT SYSTEM)
## PHIÊN BẢN CẬP NHẬT CÔNG NGHỆ (.NET 10, EF CORE, SQL SERVER, XUNIT, PLAYWRIGHT)

---

## 1. TỔNG QUAN HỆ THỐNG

### 1.1. Giới thiệu chung
Hệ thống **Mini-TMS** là một phiên bản thu nhỏ của Hệ thống Quản lý Vận tải (Transportation Management System), được thiết kế nhằm mục đích tối ưu hóa quy trình phân tuyến giao hàng tự động (Vehicle Routing Problem - VRP). Hệ thống tiếp nhận danh sách đơn hàng, thông tin đội xe và cấu hình bảng giá, từ đó tự động tính toán phương án gom đơn, chia chọn tuyến đường tối ưu nhất đáp ứng các ràng buộc về tải trọng, thời gian giao hàng và khu vực địa lý.

### 1.2. Mục tiêu tài liệu
Tài liệu này đặc tả chi tiết kiến trúc, mô hình dữ liệu, logic nghiệp vụ cốt lõi và chiến lược kiểm thử của hệ thống Mini-TMS sau khi thực hiện chuyển đổi công nghệ. Hệ thống được cấu trúc lại hoàn toàn dựa trên các nền tảng công nghệ hiện đại, hướng tới khả năng quản lý dữ liệu linh hoạt, logic chặt chẽ và quy trình kiểm thử tự động toàn diện.

---

## 2. NGĂN XẾP CÔNG NGHỆ (TECH STACK)

Hệ thống được phát triển dựa trên kiến trúc phân lớp (API, Core, Data) sử dụng các công nghệ tiêu chuẩn doanh nghiệp sau:

| Tầng (Layer) | Công nghệ tích hợp | Vai trò / Nhiệm vụ |
| :--- | :--- | :--- |
| **Giao tiếp (API)** | ASP.NET Core Web API (.NET 10) | Cung cấp các RESTful API Endpoints dưới dạng  Controller để tiếp nhận yêu cầu, điều phối dịch vụ và trả kết quả JSON cho Client. Tích hợp Swagger/OpenAPI để quản lý tài liệu API công khai. |
| **Truy cập dữ liệu (Data)** | **Entity Framework Core** | Đóng vai trò Object-Relational Mapper (ORM), quản lý kết nối, ánh xạ thực thể (Entities), xử lý các mối quan hệ phức tạp qua Navigation Properties và thực thi truy vấn thông qua LINQ mạnh mẽ. |
| **Cơ sở dữ liệu (DB)** | Microsoft SQL Server 2022 | Cơ sở dữ liệu quan hệ lưu trữ dữ liệu nền (Khách hàng, Xe, Bảng giá cước) và dữ liệu vận hành (Đơn hàng, Kế hoạch chuyến đi đã lưu vết). |
| **Lõi thuật toán (Core)** | Google.OrTools (NuGet Package) | Thư viện giải thuật tối ưu hóa nâng cao, chịu trách nhiệm giải bài toán phân tuyến phương tiện (VRP) đa ràng buộc trong thời gian thực. |
| **Kiểm thử (Testing)** | **xUnit + Playwright** | **xUnit**: Xây dựng Unit Tests cho core logic độc lập (Region Split, Cost Calculator, Capacity Constraint). <br>**Playwright**: Thực hiện Automation API Testing và End-to-End (E2E) Testing, giả lập luồng gọi API thực tế của người dùng. |

---

## 3. MÔ HÌNH DỮ LIỆU & CÁC THỰC THỂ (EF CORE ENTITIES)

Sự chuyển đổi từ Dapper sang EF Core cho phép hệ thống định nghĩa các thực thể có mối quan hệ ràng buộc chặt chẽ thông qua các Navigation Properties. Dưới đây là cấu trúc chi tiết của các lớp Thực thể:

### 3.1. Customer (Khách hàng)
Lưu trữ thông tin chi tiết về các điểm giao hàng/đối tác nhận hàng.
* `CustomerId` (int, PK, Identity)
* `Name` (string, Required, MaxLength: 250): Tên khách hàng hoặc tên cửa hàng.
* `District` (string, Required, MaxLength: 100): Quận/Huyện phục vụ cho việc tính cước.
* `Latitude` (double, Required): Vĩ độ phục vụ thuật toán khoảng cách và chia vùng.
* `Longitude` (double, Required): Kinh độ phục vụ thuật toán khoảng cách và chia vùng.
* `IsVip` (bool, Default: false): Cờ đánh dấu khách hàng ưu tiên.
* **Navigation Properties:** `ICollection<Order> Orders` (Mối quan hệ 1 - Nhiều với Đơn hàng).

### 3.2. Order (Đơn hàng)
Thông tin các đơn hàng cần giao trong ngày.
* `OrderId` (int, PK, Identity)
* `CustomerId` (int, FK): Mã định danh khách hàng nhận đơn.
* `WeightKg` (double, Required): Khối lượng của đơn hàng (đơn vị: kg).
* `OrderDate` (DateOnly, Required): Ngày cần giao đơn hàng.
* `DeliveryStart` (TimeOnly, Required): Thời gian bắt đầu khung giờ nhận hàng (Time Window Start).
* `DeliveryEnd` (TimeOnly, Required): Thời gian kết thúc khung giờ nhận hàng (Time Window End).
* `Status` (string, MaxLength: 50, Default: 'NEW'): Trạng thái đơn hàng (NEW, PLANNED, REJECTED, REJECTED_TIMEWINDOW).
* **Navigation Properties:** * `Customer Customer`: Tham chiếu ngược về thông tin khách hàng.
    * `TripOrder? TripOrder`: Tham chiếu tới thông tin phân tuyến (nếu đơn đã được xếp vào chuyến).

### 3.3. Vehicle (Phương tiện vận tải)
Đội xe khả dụng tham gia vào quá trình vận chuyển.
* `VehicleId` (int, PK, Identity)
* `Plate` (string, Required, MaxLength: 20): Biển kiểm soát xe.
* `VendorCode` (string, Required, MaxLength: 50): Mã nhà xe / đối tác vận tải (Ví dụ: V1, V2).
* `VehicleType` (string, Required, MaxLength: 20): Phân loại tải trọng xe (1.5T, 2.5T, 5T).
* `CapacityKg` (double, Required): Sức chứa tải trọng tối đa của xe (đơn vị: kg).
* `IsActive` (bool, Default: true): Trạng thái hoạt động của xe (xe inactive sẽ bị loại khỏi solver).
* **Navigation Properties:** `ICollection<Trip> Trips` (Mối quan hệ 1 - Nhiều với Chuyến xe).

### 3.4. RateCard (Bảng giá cước đối tác)
Cơ sở để tính toán chi phí vận chuyển dựa trên nhà xe và khu vực đích.
* `RateCardId` (int, PK, Identity)
* `VendorCode` (string, Required, MaxLength: 50): Mã nhà xe áp dụng giá.
* `ToDistrict` (string, Required, MaxLength: 100): Quận/Huyện đích đến.
* `VehicleType` (string, Required, MaxLength: 20): Loại xe áp dụng mức cước này.
* `BaseCost` (decimal, Required): Chi phí nền cho điểm dừng đầu tiên.
* `DropFee` (decimal, Required): Phụ phí áp dụng cho mỗi điểm dừng tăng thêm từ điểm thứ 2.
* `IsActive` (bool, Default: true): Trạng thái hiệu lực của dòng giá.

### 3.5. Trip (Chuyến xe được lập kế hoạch)
Kết quả đầu ra tổng quan của bộ giải tối ưu hóa (Optimizer).
* `TripId` (int, PK, Identity)
* `PlanCode` (string, Required, MaxLength: 100): Mã kế hoạch chạy (Ví dụ: `COST-20260523-143000`).
* `VehicleId` (int, Required, FK): Xe được gán cho chuyến này.
* `TotalKm` (double, Required): Tổng quãng đường di chuyển thực tế theo chuỗi stop đường tròn (Depot -> Các Stops -> Depot).
* `TotalCost` (decimal, Required): Tổng chi phí thực của chuyến xe sau khi tính cước và fallback.
* `Strategy` (string, Required, MaxLength: 20): Chiến lược tối ưu của chuyến (COST, TRIPS, DISTANCE).
* **Navigation Properties:**
    * `Vehicle Vehicle`: Thông tin chi tiết phương tiện thực hiện.
    * `ICollection<TripOrder> TripOrders`: Danh sách các điểm dừng chi tiết xếp theo thứ tự (Thứ tự thực hiện tuyến).

### 3.6. TripOrder (Chi tiết điểm dừng/Đơn hàng trong chuyến)
Bảng trung gian cấu trúc chuỗi điểm dừng (Route Stops) của từng chuyến xe cụ thể.
* `TripOrderId` (int, PK, Identity)
* `TripId` (int, Required, FK): Thuộc chuyến xe nào.
* `OrderId` (int, Required, FK): Đơn hàng nào được giao tại điểm này.
* `StopIndex` (int, Required): Thứ tự giao hàng trong chuyến (1, 2, 3...).
* `EtaTime` (TimeOnly, Required): Thời gian dự kiến xe đến điểm giao (Estimated Time of Arrival).
* **Navigation Properties:**
    * `Trip Trip`: Đường dẫn tham chiếu về Chuyến tổng thể.
    * `Order Order`: Đường dẫn tham chiếu trực tiếp tới thông tin đơn hàng được giao.

---

## 4. QUY TRÌNH NGHIỆP VỤ CỐT LÕI & LOGIC TOÁN HỌC

Hệ thống giải quyết bài toán vận tải thông qua một chuỗi xử lý tuần tự kết hợp các ràng buộc nghiệp vụ Logistics thực tế:

```
[Tiếp nhận danh sách Đơn hàng] 
       │
       ▼
[Phân rã Địa lý (Region Split)] ---> Tách đơn Bắc Sông Hồng / Nam Sông Hồng
       │
       ▼
[Xử lý Đa chiến lược song song] ---> Chạy COST, TRIPS, DISTANCE (Task.WhenAll)
       │
       ▼
[Google.OrTools Solver] ----------> Tính toán VRP đáp ứng Capacity, TimeWindow, MaxStops
       │
       ▼
[Tính cước & Vehicle Fallback] ----> Lookup bảng giá, tự động nhảy bậc xe nếu thiếu rate
       │
       ▼
[Lưu trữ EF Core DB Transaction] -> Lưu đồng thời vào bảng Trips và TripOrders
```

### 4.1. Phân rã địa lý dựa trên Vĩ độ (Region Split)
Để tối ưu hóa thời gian xử lý của bộ giải OrTools và phản ánh thực tế vận hành (hạn chế xe đi qua cầu lớn gây ách tắc giao thông), hệ thống áp dụng cơ chế cắt đơn hàng:
* **Vĩ độ ranh giới cố định:** `Latitude = 21.03` (Ranh giới địa lý mô phỏng Sông Hồng tại Hà Nội).
* **Quy tắc phân chia:**
    * Nếu `Order.Latitude >= 21.03` $
ightarrow$ Xếp đơn vào khu vực **North (Phía Bắc)**.
    * Nếu `Order.Latitude < 21.03` $
ightarrow$ Xếp đơn vào khu vực **South (Phía Nam)**.
* Mỗi tập đơn hàng khu vực sau đó sẽ được đưa vào solver xử lý hoàn toàn độc lập.

### 4.2. Bộ giải thuật đa ràng buộc (VRP Solver với Google.OrTools)
Thuật toán giải toán phân tuyến vận tải thực thi các ràng buộc cứng:
1.  **Tọa độ Kho trung tâm (Depot):** Cố định tại Cầu Giấy (`Latitude = 21.0285`, `Longitude = 105.7822`).
2.  **Ma trận khoảng cách (Distance Callback):** Khoảng cách giữa các Node (Depot và các điểm giao) được tính toán theo công thức Haversine (đơn vị: mét).
3.  **Ràng buộc tải trọng (Capacity Dimension):** Tổng khối lượng hàng của các đơn trên cùng một xe phải nhỏ hơn hoặc bằng tải trọng của xe đó ($\sum WeightKg \le Vehicle.CapacityKg$). Nếu một đơn hàng đơn lẻ nặng vượt quá tải trọng xe lớn nhất, solver sẽ đẩy đơn đó vào danh sách `RejectedOrderIds`.
4.  **Ràng buộc thời gian (Time Window Dimension):** * Vận tốc di chuyển giả định nội thành: $40 km/h$. Thời gian di chuyển (phút) = $rac{Distance(km)}{40} 	imes 60$.
    * Thời gian xử lý tại mỗi điểm dừng (Service Time): Cố định $20$ phút cho mỗi đơn hàng.
    * Xe xuất phát từ kho lúc: $06:00$ sáng ($360$ phút tính từ nửa đêm).
    * Thời gian đến điểm dừng thực tế phải nằm trong khoảng: $[DeliveryStart, DeliveryEnd]$. Nếu không thể đáp ứng do quá xa hoặc xung đột thời gian, đơn hàng sẽ bị hủy bỏ (Drop/Reject) khỏi tuyến để bảo toàn tính khả thi của lộ trình.
5.  **Giới hạn số điểm dừng (Max Stops Dimension):** Số lượng đơn hàng tối đa gán cho một chuyến xe không được vượt quá tham số đầu vào `MaxStopsPerTrip` (Mặc định = 3).

### 4.3. Công thức tính cước phí và Cơ chế Fallback xe lớn
Sau khi solver đề xuất danh sách gom đơn, hệ thống tính toán chi phí tài chính thực tế:
* **Công thức tính cước cơ bản:**
    $$TotalCost = BaseCost + (StopCount - 1) 	imes DropFee$$
    *(Trong đó: $StopCount$ là số lượng đơn hàng thực tế nằm trong chuyến xe đó).*
* **Cơ chế Vehicle Fallback (Nhảy bậc giá tự động):**
    * Thứ tự bậc xe từ nhỏ đến lớn được định nghĩa: `1.5T` $
ightarrow$ `2.5T` $
ightarrow$ `5T`.
    * Khi xe thực hiện chuyến đi thuộc loại `1.5T`, hệ thống sẽ tìm kiếm dòng giá tương ứng trong bảng `RateCard` khớp với `VendorCode` và `ToDistrict`.
    * Nếu **không tồn tại** giá chính xác cho loại xe `1.5T` tại quận đó, hệ thống sẽ tự động tìm kiếm mức giá của loại xe lớn hơn tiếp theo là `2.5T` (cùng nhà xe, cùng quận) để áp dụng. Nếu vẫn không có, tiếp tục nhảy lên xe `5T`. 
    * Giá trị `UsedVehicleType` trả về sẽ phản ánh loại xe được tính tiền thực tế, giúp hệ thống không bao giờ bị lỗi tính cước khi đối tác thiếu cấu hình chi tiết cho xe nhỏ.

### 4.4. Đa chiến lược tối ưu chạy song song (Multi-Strategy)
Hệ thống cung cấp cơ chế so sánh kế hoạch thông qua 3 chiến lược:
* **COST:** Tối ưu hóa tổng chi phí tiền mặt trả cho đối tác vận tải (Ưu tiên gom đơn vào các xe có cước rẻ nhất).
* **TRIPS:** Tối ưu hóa tổng số lượng chuyến xe xuất phát (Khuyến khích gom tối đa số stop vào một xe để giảm thiểu đầu xe chạy, áp dụng cước ảo phạt $500$đ/chuyến xuất phát).
* **DISTANCE:** Tối ưu hóa tổng quãng đường di chuyển ($km$) của toàn đội xe (Không tính chi phí tiền mặt).
Hệ thống sử dụng kỹ thuật xử lý đa luồng bất đồng bộ `Task.WhenAll` để kích hoạt đồng thời bộ giải cho cả 3 chiến lược, tận dụng tối đa năng lực phần cứng CPU giúp thời gian phản hồi API tối ưu nhất.

---

## 5. THIẾT KẾ ENDPOINTS API (ASP.NET CORE)

Hệ thống cung cấp các REST API Endpoints phục vụ quản trị dữ liệu vận tải:

### 5.1. GET /api/orders
Lấy danh sách toàn bộ đơn hàng cần xử lý theo ngày.
* **Query Parameters:** `date` (Kiểu dữ liệu: `DateOnly`, mặc định là Ngày hiện tại).
* **Response (OK - 200):** Trả về mảng JSON chứa thông tin đơn hàng cùng dữ liệu Khách hàng tương ứng (được nạp thông qua `.Include(o => o.Customer)` trong EF Core).

### 5.2. GET /api/vehicles
Lấy danh sách các phương tiện vận tải đang sẵn sàng hoạt động.
* **Response (OK - 200):** Danh sách xe có thuộc tính `IsActive == true`, sắp xếp tăng dần theo `CapacityKg`.

### 5.3. POST /api/optimize
Thực thi tối ưu hóa phân tuyến đơn lẻ cho một ngày cụ thể dựa trên một chiến lược được chỉ định.
* **Request Body (JSON):**
    ```json
    {
      "date": "2026-05-23",
      "strategy": "COST",
      "maxStopsPerTrip": 3
    }
    ```
* **Xử lý bên trong:** Kích hoạt `OrderRepository`, `VehicleRepository` thông qua EF Core, gọi `VrpSolver.Solve()`, thực hiện tính cước, sau đó mở một **Database Transaction** (`DbContext.Database.BeginTransactionAsync()`) để lưu dữ liệu đồng thời vào bảng `Trips` và `TripOrders`.
* **Response (OK - 200):** Trả về mã kế hoạch chạy (`planCode`), danh sách các `trips` kèm chi tiết điểm dừng (`stops`), thời gian `eta` cụ thể, và danh sách mã đơn bị từ chối (`rejectedOrderIds`).

### 5.4. POST /api/optimize/multi
Thực thi chạy giả lập đồng thời cả 3 chiến lược (COST, TRIPS, DISTANCE) để người điều hành logistics lựa chọn phương án tối ưu nhất.
* **Request Body (JSON):**
    ```json
    {
      "date": "2026-05-23",
      "maxStopsPerTrip": 3
    }
    ```
* **Response (OK - 200):** Mảng JSON gồm 3 phần tử tương ứng với kết quả phân tích của 3 chiến lược, hiển thị tổng chi phí (`totalCost`), tổng số chuyến (`totalTrips`), và tổng quãng đường (`totalKm`) của từng phương án để đối chiếu.

---

## 6. CHIẾN LƯỢC KIỂM THỬ TOÀN DIỆN (XUNIT & PLAYWRIGHT)

Hệ thống áp dụng mô hình kiểm thử phân cấp để đảm bảo tính ổn định tuyệt đối từ logic xử lý nghiệp vụ cho đến luồng dữ liệu API tích hợp đầu cuối.

### 6.1. Unit Testing với xUnit (Backend Core Logic)
Các bài kiểm thử đơn vị được thiết lập độc lập, loại bỏ phụ thuộc I/O vật lý bằng cách sử dụng dữ liệu giả lập (Mock/Stub) hoặc sử dụng **EF Core InMemory Database Provider** để thay thế kết nối SQL Server thực tế trong môi trường test.

Các kịch bản Unit Test bắt buộc phải bao phủ:
* **Kịch bản 1 - Kiểm thử Region Split:** Nạp vào danh sách các đơn hàng có tọa độ thuộc cả 2 phía Bắc/Nam Sông Hồng (Ví dụ: Tây Hồ vĩ độ 21.07, Hà Đông vĩ độ 20.97). Hàm `VrpSolver.SplitByRegion` phải phân loại chính xác số lượng đơn về từng vùng và không làm thất thoát đơn hàng. Kiểm tra trường hợp biên khi đơn nằm chính xác tại vĩ độ ranh giới `21.03`.
* **Kịch bản 2 - Kiểm thử Vehicle Fallback:** Tạo bảng giá giả lập trong bộ nhớ (`InMemoryDb`), trong đó quận Long Biên **chỉ có** cấu hình cước cho loại xe `2.5T` mà **không có** cấu hình cho xe `1.5T`. Khi chạy hàm `CostCalculator.Calculate` với tham số đầu vào là xe `1.5T` đến quận Long Biên, kết quả trả về phải chứng minh hệ thống tự động nhận giá của xe `2.5T` làm cước phí thực tế (`UsedVehicleType == "2.5T"`).
* **Kịch bản 3 - Kiểm thử Ràng buộc Tải trọng (Capacity Constraint):** Khởi tạo một đơn hàng có khối lượng cực lớn ($900 kg$). Khi đưa vào bộ giải tối ưu, kiểm tra xem solver có tự động bỏ qua các dòng xe tải nhỏ như `1.5T` (sức chứa nhỏ) để gán đơn hàng này một cách chính xác vào xe tải lớn `5T` hay không.
* **Kịch bản 4 - Kiểm thử Giới hạn Điểm dừng:** Nạp 5 đơn hàng vào khu vực có cùng một xe chạy. Đặt tham số `maxStopsPerTrip = 2`. Kiểm tra xem kết quả đầu ra có phân tách thành ít nhất 3 chuyến đi độc lập để đảm bảo không có bất kỳ chuyến đi nào chứa quá 2 điểm giao hay không.

### 6.2. End-to-End (E2E) & API Integration Testing với Playwright
Playwright được tích hợp chung vào runner của xUnit để thực hiện kiểm thử tự động hộp đen (Black-box testing) trực tiếp lên ứng dụng khi đang khởi chạy.

Quy trình thực thi kịch bản E2E bằng Playwright:
1.  **Khởi tạo Host ảo:** Bộ kiểm thử khởi động một bản sao chạy ngầm của API Web Server (định vị tại một cổng Localhost ngẫu nhiên).
2.  **Khởi tạo Client ngầm:** Playwright sử dụng `APIRequestContext` để gửi một HTTP `POST` thực tế đến endpoint `/api/optimize/multi` kèm payload JSON kiểm thử.
3.  **Kiểm tra tính toàn vẹn (Assertions):**
    * Xác thực mã trạng thái phản hồi HTTP trả về phải là `200 OK`.
    * Phân tích cú pháp chuỗi JSON trả về để đảm bảo có cấu trúc mảng đủ 3 chiến lược (`COST`, `TRIPS`, `DISTANCE`).
    * Kiểm tra tính logic tài chính: Tổng chi phí (`totalCost`) của chiến lược `COST` phải nhỏ hơn hoặc bằng tổng chi phí của chiến lược `DISTANCE`.
    * Kiểm tra tính chính xác của dữ liệu: Toàn bộ các mã chuyến đi (`tripId`) trả về trong JSON phải tồn tại thực tế trong cơ sở dữ liệu (sử dụng một DbContext kiểm tra chéo sang database SQL Server Test), đảm bảo luồng ghi dữ liệu và Transaction thành công, không có tình trạng mồ côi dữ liệu (orphan data).
4.  **Đo lường hiệu năng:** Kiểm tra tổng thời gian phản hồi của API đa chiến lược chạy song song, đảm bảo không vượt quá ngưỡng quy định (Ví dụ: < 8 giây) nhằm chứng minh kỹ thuật đa luồng bất đồng bộ hoạt động đạt yêu cầu hiệu năng cao.
