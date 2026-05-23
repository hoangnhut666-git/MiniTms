-- ============================================================
-- Mini-TMS Seed Database
-- Bài thực hành onboarding cho intern migrate sang C# / .NET 8
-- ============================================================
--
-- Cách dùng:
--   1. Tạo database MiniTms trước (xem file 01-tao-database.md)
--   2. Chạy file này — schema + dữ liệu mẫu sẽ được tạo
--   3. Có thể chạy lại nhiều lần — script tự DROP rồi tạo lại
--
-- Lưu ý: Dự án dùng EF Core migrations + HasData (Data/Seed/TmsDataSeeder.cs)
-- là nguồn schema/seed chính. File SQL này giữ để tham chiếu / reset thủ công.
--
-- ============================================================

USE MiniTms;
GO

-- ============================================================
-- 1. CLEANUP: Drop tables nếu đã tồn tại (theo thứ tự FK)
-- ============================================================
IF OBJECT_ID('TripOrders', 'U') IS NOT NULL DROP TABLE TripOrders;
IF OBJECT_ID('Trips', 'U')      IS NOT NULL DROP TABLE Trips;
IF OBJECT_ID('Orders', 'U')     IS NOT NULL DROP TABLE Orders;
IF OBJECT_ID('RateCard', 'U')   IS NOT NULL DROP TABLE RateCard;
IF OBJECT_ID('Vehicles', 'U')   IS NOT NULL DROP TABLE Vehicles;
IF OBJECT_ID('Customers', 'U')  IS NOT NULL DROP TABLE Customers;
GO

-- ============================================================
-- 2. SCHEMA: Tạo 6 bảng
-- ============================================================

-- 2.1 Khách hàng (đã geocode sẵn lat/lng)
-- Tương ứng prod: TWB_M_CUSTOMER
CREATE TABLE Customers (
    CustomerId   INT          NOT NULL PRIMARY KEY,
    Name         NVARCHAR(100) NOT NULL,
    District     NVARCHAR(50)  NOT NULL,   -- Cầu Giấy, Đống Đa, Long Biên...
    Latitude     FLOAT         NOT NULL,
    Longitude    FLOAT         NOT NULL,
    IsVip        BIT           NOT NULL DEFAULT 0  -- DD-10: VIP có ưu tiên cao hơn
);

-- 2.2 Đơn hàng
-- Tương ứng prod: TWO_M_ORDER + TWO_M_SHIPMENT
CREATE TABLE Orders (
    OrderId       INT          NOT NULL PRIMARY KEY,
    CustomerId    INT          NOT NULL FOREIGN KEY REFERENCES Customers(CustomerId),
    WeightKg      FLOAT        NOT NULL,
    OrderDate     DATE         NOT NULL,
    DeliveryStart TIME         NOT NULL,    -- Time window start (vd: 07:00)
    DeliveryEnd   TIME         NOT NULL,    -- Time window end   (vd: 09:30)
    Status        NVARCHAR(20) NOT NULL DEFAULT 'NEW'  -- NEW / PLANNED / REJECTED
);

-- 2.3 Đội xe
-- Tương ứng prod: TWO_M_VEHICLE
CREATE TABLE Vehicles (
    VehicleId    INT          NOT NULL PRIMARY KEY,
    Plate        NVARCHAR(20) NOT NULL,
    VendorCode   NVARCHAR(10) NOT NULL,    -- 'V1' / 'V2' — mỗi vendor có rate card riêng
    VehicleType  NVARCHAR(10) NOT NULL,    -- '1.5T' / '2.5T' / '5T'
    CapacityKg   FLOAT        NOT NULL,
    IsActive     BIT          NOT NULL DEFAULT 1  -- DD-63: code C# phải filter IsActive=1
);

-- 2.4 Rate card — bảng giá theo vendor + tuyến + loại xe
-- Tương ứng prod: RATE_CARD (đơn giản hóa — bỏ Area level)
CREATE TABLE RateCard (
    RateCardId   INT          IDENTITY(1,1) PRIMARY KEY,
    VendorCode   NVARCHAR(10) NOT NULL,
    ToDistrict   NVARCHAR(50) NOT NULL,
    VehicleType  NVARCHAR(10) NOT NULL,
    BaseCost     DECIMAL(12,0) NOT NULL,   -- VND, cost cố định cho 1 chuyến
    DropFee      DECIMAL(12,0) NOT NULL,   -- VND, phí mỗi điểm dừng phụ (stop thứ 2 trở đi)
    IsActive     BIT          NOT NULL DEFAULT 1
);

-- 2.5 Trips — kết quả solver sinh ra
-- Tương ứng prod: TWO_T_VEHICLE_TRIP
CREATE TABLE Trips (
    TripId       INT          IDENTITY(1,1) PRIMARY KEY,
    PlanCode     NVARCHAR(50) NOT NULL,    -- Vd: 'COST-20260523-001-THE_BEST'
    VehicleId    INT          NOT NULL FOREIGN KEY REFERENCES Vehicles(VehicleId),
    TotalKm      FLOAT        NOT NULL,
    TotalCost    DECIMAL(12,0) NOT NULL,
    Strategy     NVARCHAR(20) NOT NULL,    -- 'COST' / 'TRIPS' / 'DISTANCE'
    CreatedAt    DATETIME2    NOT NULL DEFAULT SYSUTCDATETIME()
);

-- 2.6 TripOrders — chi tiết stops trong mỗi trip
-- Tương ứng prod: TWO_T_VEHICLE_TRIP_ORDER
CREATE TABLE TripOrders (
    TripId       INT      NOT NULL FOREIGN KEY REFERENCES Trips(TripId),
    OrderId      INT      NOT NULL FOREIGN KEY REFERENCES Orders(OrderId),
    StopIndex    INT      NOT NULL,    -- DD-35: 1, 2, 3... thứ tự dừng
    EtaTime      TIME     NOT NULL,    -- ETA dự kiến giao
    PRIMARY KEY (TripId, OrderId)
);
GO

-- ============================================================
-- 3. SEED DATA
-- ============================================================
-- Depot ngầm định: Kho Cầu Giấy — Latitude 21.0285, Longitude 105.7822
-- (em sẽ hard-code depot trong code C# ở bước 02)

-- 3.1 Customers (8 khách quanh Hà Nội)
INSERT INTO Customers (CustomerId, Name, District, Latitude, Longitude, IsVip) VALUES
(1, N'Cafe Highland Tây Hồ',     N'Tây Hồ',       21.0700, 105.8200, 1),
(2, N'Tiệm cơm Đống Đa',         N'Đống Đa',      21.0150, 105.8300, 0),
(3, N'Văn phòng Long Biên',      N'Long Biên',    21.0400, 105.9000, 0),
(4, N'Trường THCS Hà Đông',      N'Hà Đông',      20.9700, 105.7600, 0),
(5, N'Khách sạn Hoàn Kiếm',      N'Hoàn Kiếm',    21.0290, 105.8520, 1),
(6, N'Quán phở Ba Đình',         N'Ba Đình',      21.0350, 105.8100, 0),
(7, N'Trung tâm Thanh Xuân',     N'Thanh Xuân',   20.9950, 105.7950, 0),
(8, N'Siêu thị Hai Bà Trưng',    N'Hai Bà Trưng', 21.0050, 105.8650, 0);

-- 3.2 Orders (ngày 2026-05-23)
-- Mix nhẹ + nặng để test capacity. Đơn 1007 đặc biệt nặng (900kg) → chỉ xe 5T tải nổi
INSERT INTO Orders (OrderId, CustomerId, WeightKg, OrderDate, DeliveryStart, DeliveryEnd, Status) VALUES
(1001, 1, 120, '2026-05-23', '07:00', '09:00', 'NEW'),
(1002, 2,  80, '2026-05-23', '07:30', '10:00', 'NEW'),
(1003, 3, 450, '2026-05-23', '08:00', '11:00', 'NEW'),
(1004, 4, 300, '2026-05-23', '07:00', '10:00', 'NEW'),
(1005, 5,  60, '2026-05-23', '07:00', '08:30', 'NEW'),  -- VIP, window hẹp
(1006, 6, 200, '2026-05-23', '07:30', '10:30', 'NEW'),
(1007, 7, 900, '2026-05-23', '08:00', '11:00', 'NEW'),  -- nặng → ép xe 5T
(1008, 8, 150, '2026-05-23', '07:00', '09:30', 'NEW');

-- 3.3 Vehicles (4 xe, 1 inactive — intern phải filter ở M1)
INSERT INTO Vehicles (VehicleId, Plate, VendorCode, VehicleType, CapacityKg, IsActive) VALUES
(1, N'29C-111.11', 'V1', '1.5T', 1500, 1),
(2, N'29C-222.22', 'V1', '2.5T', 2500, 1),
(3, N'29C-333.33', 'V2', '5T',   5000, 1),
(4, N'29C-444.44', 'V2', '2.5T', 2500, 0);  -- INACTIVE: không bao giờ xuất hiện trong kết quả

-- 3.4 Rate card
-- Lưu ý:
--   - Long Biên: KHÔNG có rate cho 1.5T → ở M3 intern sẽ implement vehicle fallback (1.5T → 2.5T)
--   - V2 chỉ có rate cho 5T ở Tây Hồ và Long Biên → test cross-vendor matching
INSERT INTO RateCard (VendorCode, ToDistrict, VehicleType, BaseCost, DropFee) VALUES
-- V1 — vendor chính, phủ hầu hết các quận
('V1', N'Tây Hồ',       '1.5T', 250000, 30000),
('V1', N'Tây Hồ',       '2.5T', 320000, 30000),
('V1', N'Tây Hồ',       '5T',   480000, 30000),
('V1', N'Đống Đa',      '1.5T', 200000, 25000),
('V1', N'Đống Đa',      '2.5T', 270000, 25000),
('V1', N'Long Biên',    '2.5T', 350000, 35000),   -- 1.5T thiếu → fallback test
('V1', N'Long Biên',    '5T',   520000, 35000),
('V1', N'Hà Đông',      '2.5T', 380000, 40000),
('V1', N'Hà Đông',      '5T',   550000, 40000),
('V1', N'Hoàn Kiếm',    '1.5T', 220000, 25000),
('V1', N'Ba Đình',      '1.5T', 230000, 25000),
('V1', N'Thanh Xuân',   '2.5T', 290000, 30000),
('V1', N'Thanh Xuân',   '5T',   450000, 30000),
('V1', N'Hai Bà Trưng', '1.5T', 240000, 25000),
-- V2 — vendor phụ, chỉ có rate xe 5T
('V2', N'Tây Hồ',       '5T',   460000, 30000),
('V2', N'Long Biên',    '5T',   500000, 35000);
GO

-- ============================================================
-- 4. VERIFICATION QUERIES — chạy thử sau khi seed xong
-- ============================================================
PRINT N'=== Seed thành công. Kết quả verify: ===';

SELECT 'Customers' AS Bảng, COUNT(*) AS [Số dòng] FROM Customers
UNION ALL SELECT 'Orders',   COUNT(*) FROM Orders
UNION ALL SELECT 'Vehicles', COUNT(*) FROM Vehicles
UNION ALL SELECT 'RateCard', COUNT(*) FROM RateCard
UNION ALL SELECT 'Trips',    COUNT(*) FROM Trips
UNION ALL SELECT 'TripOrders', COUNT(*) FROM TripOrders;

PRINT N'';
PRINT N'Số dòng kỳ vọng: Customers=8, Orders=8, Vehicles=4, RateCard=16, Trips=0, TripOrders=0';
GO
