-- ========================================================
-- KHOA HỆ THỐNG THÔNG TIN / CÔNG NGHỆ THÔNG TIN
-- MÔN HỌC: LẬP TRÌNH .NET (C# WINFORMS 3-TIER)
-- BÀI TẬP 6.4: HỆ THỐNG QUẢN LÝ GARAGE VÀ BẢO BƯỠNG XE Ô TÔ
-- CSDL: GarageDB
-- ========================================================

IF DB_ID('GarageDB') IS NOT NULL
BEGIN
    ALTER DATABASE GarageDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE GarageDB;
END
GO

CREATE DATABASE GarageDB;
GO

USE GarageDB;
GO

-- 1. Bảng KhachHang (Chủ xe)
CREATE TABLE KhachHang (
    MaKH INT IDENTITY(1,1) PRIMARY KEY,
    TenKH NVARCHAR(100) NOT NULL,
    DienThoai VARCHAR(15) NOT NULL UNIQUE,
    DiaChi NVARCHAR(200)
);
GO

-- 2. Bảng Xe (Thông tin ô tô)
CREATE TABLE Xe (
    BienSo VARCHAR(20) PRIMARY KEY,
    MaKH INT NOT NULL FOREIGN KEY REFERENCES KhachHang(MaKH) ON DELETE CASCADE,
    HieuXe NVARCHAR(50) NOT NULL, -- "Toyota", "Honda", "VinFast", "Mercedes", "Mazda"
    NamSanXuat INT
);
GO

-- 3. Bảng PhuTung (Phụ tùng / Vật tư)
CREATE TABLE PhuTung (
    MaPhuTung INT IDENTITY(1,1) PRIMARY KEY,
    TenPhuTung NVARCHAR(150) NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL DEFAULT 0,
    SoLuongTon INT NOT NULL DEFAULT 0,
    CONSTRAINT CK_PhuTung_Ton CHECK (SoLuongTon >= 0)
);
GO

-- 4. Bảng PhieuSuaChua (Phiếu sửa chữa)
CREATE TABLE PhieuSuaChua (
    MaPhieu INT IDENTITY(1,1) PRIMARY KEY,
    BienSo VARCHAR(20) NOT NULL FOREIGN KEY REFERENCES Xe(BienSo),
    NgayNhan DATETIME DEFAULT GETDATE(),
    NoiDungSua NVARCHAR(MAX),
    TongTien DECIMAL(18,2) DEFAULT 0,
    TrangThai NVARCHAR(30) DEFAULT N'Đang sửa' -- "Đang sửa", "Đã hoàn thành", "Đã thanh toán"
);
GO

-- 5. Bảng ChiTietSuaChua (Chi tiết phụ tùng & tiền công)
CREATE TABLE ChiTietSuaChua (
    MaPhieu INT NOT NULL FOREIGN KEY REFERENCES PhieuSuaChua(MaPhieu) ON DELETE CASCADE,
    MaPhuTung INT NOT NULL FOREIGN KEY REFERENCES PhuTung(MaPhuTung),
    SoLuong INT NOT NULL,
    TienCong DECIMAL(18,2) NOT NULL DEFAULT 0,
    CONSTRAINT PK_ChiTietSuaChua PRIMARY KEY (MaPhieu, MaPhuTung),
    CONSTRAINT CK_ChiTiet_SoLuong CHECK (SoLuong > 0)
);
GO

-- ========================================================
-- CHÈN DỮ LIỆU MẪU (SAMPLE DATA)
-- ========================================================

-- Khách hàng mẫu
INSERT INTO KhachHang (TenKH, DienThoai, DiaChi) VALUES
(N'Trần Minh Hoàng', '0912345678', N'123 Cầu Giấy, Hà Nội'),
(N'Lê Hải Yến', '0987654321', N'45 Nguyễn Huệ, Quận 1, TP.HCM'),
(N'Nguyễn Quốc Anh', '0905123456', N'78 Lê Duẩn, Đà Nẵng');

-- Xe mẫu
INSERT INTO Xe (BienSo, MaKH, HieuXe, NamSanXuat) VALUES
('30A-999.88', 1, N'Toyota Camry', 2021),
('51F-123.45', 2, N'Honda CR-V', 2022),
('43A-678.90', 3, N'VinFast VF8', 2023);

-- Phụ tùng mẫu
INSERT INTO PhuTung (TenPhuTung, DonGia, SoLuongTon) VALUES
(N'Dầu động cơ Castrol Magnatec 4L', 450000, 20),
(N'Lọc nhớt chính hãng Denso', 150000, 15),
(N'Bộ Má phanh trước Brembo', 850000, 5),
(N'Gạt mưa Silicon Bosch', 250000, 10),
(N'Bugi Iridium NGK', 180000, 2), -- Số lượng tồn ít để test rollback khi thiếu hàng
(N'Lọc gió điều hòa than hoạt tính', 220000, 8);

-- Phiếu sửa chữa mẫu
INSERT INTO PhieuSuaChua (BienSo, NgayNhan, NoiDungSua, TongTien, TrangThai) VALUES
('30A-999.88', '2026-11-18 08:30:00', N'Bảo dưỡng định kỳ 40.000km, thay dầu và má phanh', 1600000, N'Đang sửa'),
('51F-123.45', '2026-11-17 14:00:00', N'Thay gạt mưa và lọc gió điều hòa', 670000, N'Đã hoàn thành');

-- Chi tiết sửa chữa mẫu
INSERT INTO ChiTietSuaChua (MaPhieu, MaPhuTung, SoLuong, TienCong) VALUES
(1, 1, 1, 100000), -- Dầu nhớt + công
(1, 3, 1, 200000), -- Má phanh + công
(2, 4, 1, 50000),  -- Gạt mưa + công
(2, 6, 1, 150000); -- Lọc gió + công
GO

PRINT N'Khởi tạo CSDL GarageDB thành công!';
