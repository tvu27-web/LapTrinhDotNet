-- ========================================================
-- KHOA HỆ THỐNG THÔNG TIN / CÔNG NGHỆ THÔNG TIN
-- MÔN HỌC: LẬP TRÌNH .NET (C# WINFORMS 3-TIER)
-- BÀI TẬP 6.5: HỆ THỐNG QUẢN LÝ ĐẶT PHÒNG & DỊCH VỤ KARAOKE
-- CSDL: KaraokeDB
-- ========================================================

IF DB_ID('KaraokeDB') IS NOT NULL
BEGIN
    ALTER DATABASE KaraokeDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE KaraokeDB;
END
GO

CREATE DATABASE KaraokeDB;
GO

USE KaraokeDB;
GO

-- 1. Bảng PhongHat (Danh sách phòng hát)
CREATE TABLE PhongHat (
    MaPhong INT IDENTITY(1,1) PRIMARY KEY,
    TenPhong NVARCHAR(50) NOT NULL,
    LoaiPhong NVARCHAR(30) NOT NULL, -- 'VIP', 'Thường'
    GiaTheoGio DECIMAL(18,2) NOT NULL DEFAULT 100000,
    TrangThai NVARCHAR(20) NOT NULL DEFAULT N'Trống' -- 'Trống', 'Đang hát'
);
GO

-- 2. Bảng DichVu (Mặt hàng dịch vụ / Đồ uống)
CREATE TABLE DichVu (
    MaDV INT IDENTITY(1,1) PRIMARY KEY,
    TenDV NVARCHAR(100) NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL DEFAULT 0,
    TonKho INT NOT NULL DEFAULT 0,
    CONSTRAINT CK_DichVu_TonKho CHECK (TonKho >= 0)
);
GO

-- 3. Bảng HoaDon (Hóa đơn sử dụng)
CREATE TABLE HoaDon (
    MaHD INT IDENTITY(1,1) PRIMARY KEY,
    MaPhong INT NOT NULL FOREIGN KEY REFERENCES PhongHat(MaPhong) ON DELETE CASCADE,
    GioVao DATETIME NOT NULL DEFAULT GETDATE(),
    GioRa DATETIME NULL,
    TongTienPhong DECIMAL(18,2) DEFAULT 0,
    TongTienDichVu DECIMAL(18,2) DEFAULT 0,
    TrangThai BIT NOT NULL DEFAULT 0 -- 0: Chưa thanh toán, 1: Đã thanh toán
);
GO

-- 4. Bảng ChiTietDichVu (Chi tiết dịch vụ gọi)
CREATE TABLE ChiTietDichVu (
    MaHD INT NOT NULL FOREIGN KEY REFERENCES HoaDon(MaHD) ON DELETE CASCADE,
    MaDV INT NOT NULL FOREIGN KEY REFERENCES DichVu(MaDV),
    SoLuong INT NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL DEFAULT 0,
    CONSTRAINT PK_ChiTietDichVu PRIMARY KEY (MaHD, MaDV),
    CONSTRAINT CK_ChiTietDichVu_SoLuong CHECK (SoLuong > 0)
);
GO

-- ========================================================
-- CHÈN DỮ LIỆU MẪU (SAMPLE DATA)
-- ========================================================

-- Danh sách phòng hát
INSERT INTO PhongHat (TenPhong, LoaiPhong, GiaTheoGio, TrangThai) VALUES
(N'Phòng VIP 01', N'VIP', 250000, N'Đang hát'),
(N'Phòng VIP 02', N'VIP', 250000, N'Trống'),
(N'Phòng Thường 01', N'Thường', 150000, N'Đang hát'),
(N'Phòng Thường 02', N'Thường', 150000, N'Trống'),
(N'Phòng Thường 03', N'Thường', 150000, N'Trống'),
(N'Phòng Thường 04', N'Thường', 150000, N'Trống'),
(N'Phòng VIP Hoàng Gia', N'VIP', 350000, N'Trống'),
(N'Phòng Mini Party', N'Thường', 180000, N'Trống');

-- Danh mục dịch vụ / Đồ uống
INSERT INTO DichVu (TenDV, DonGia, TonKho) VALUES
(N'Bia Heineken lon', 25000, 120),
(N'Bia Tiger bạc lon', 22000, 150),
(N'Nước khoáng Lavie', 12000, 80),
(N'Nước ngọt Coca-Cola', 15000, 95),
(N'Đĩa Trái cây thập cẩm đặc biệt', 120000, 20),
(N'Bò khô xé cay dĩa', 95000, 15),
(N'Mực nướng sa tế dĩa', 150000, 10),
(N'Đậu phộng rang muối tỏi', 20000, 50);

-- Giả lập Hóa đơn đang hát cho Phòng VIP 01 (vào từ 1 giờ trước)
INSERT INTO HoaDon (MaPhong, GioVao, GioRa, TongTienPhong, TongTienDichVu, TrangThai) VALUES
(1, DATEADD(MINUTE, -75, GETDATE()), NULL, 0, 195000, 0);

-- Dịch vụ đã gọi của Phòng VIP 01
INSERT INTO ChiTietDichVu (MaHD, MaDV, SoLuong, DonGia) VALUES
(1, 1, 4, 25000),  -- 4 bia Hein = 100k
(1, 6, 1, 95000);  -- 1 bò khô = 95k

-- Giả lập Hóa đơn đang hát cho Phòng Thường 01 (vào từ 40 phút trước)
INSERT INTO HoaDon (MaPhong, GioVao, GioRa, TongTienPhong, TongTienDichVu, TrangThai) VALUES
(3, DATEADD(MINUTE, -40, GETDATE()), NULL, 0, 149000, 0);

-- Dịch vụ đã gọi của Phòng Thường 01
INSERT INTO ChiTietDichVu (MaHD, MaDV, SoLuong, DonGia) VALUES
(2, 2, 3, 22000),  -- 3 tiger = 66k
(2, 5, 1, 120000); -- 1 trái cây = 120k (giả lập)
GO

PRINT N'Khởi tạo CSDL KaraokeDB thành công!';
