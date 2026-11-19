-- ========================================================
-- KHOA HỆ THỐNG THÔNG TIN / CÔNG NGHỆ THÔNG TIN
-- MÔN HỌC: LẬP TRÌNH .NET (C# WINFORMS 3-TIER)
-- BÀI TẬP 6.3: HỆ THỐNG QUẢN LÝ CÔNG VIỆC & TIẾN ĐỘ DỰ ÁN
-- CSDL: ProjectTaskDB
-- ========================================================

IF DB_ID('ProjectTaskDB') IS NOT NULL
BEGIN
    ALTER DATABASE ProjectTaskDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ProjectTaskDB;
END
GO

CREATE DATABASE ProjectTaskDB;
GO

USE ProjectTaskDB;
GO

-- 1. Bảng DuAn (Dự án)
CREATE TABLE DuAn (
    MaDuAn INT IDENTITY(1,1) PRIMARY KEY,
    TenDuAn NVARCHAR(150) NOT NULL,
    NgayBatDau DATETIME DEFAULT GETDATE(),
    NgayKetThuc DATETIME,
    TrangThai NVARCHAR(50) DEFAULT N'Đang chạy' -- 'Đang chạy', 'Hoàn thành', 'Tạm dừng'
);
GO

-- 2. Bảng ThanhVien (Thành viên nhóm)
CREATE TABLE ThanhVien (
    MaThanhVien INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    ChuyenMon NVARCHAR(100)
);
GO

-- 3. Bảng CongViec (Công việc / Task)
CREATE TABLE CongViec (
    MaTask INT IDENTITY(1,1) PRIMARY KEY,
    MaDuAn INT NOT NULL FOREIGN KEY REFERENCES DuAn(MaDuAn) ON DELETE CASCADE,
    TenTask NVARCHAR(200) NOT NULL,
    MoTa NVARCHAR(MAX),
    HanNop DATETIME,
    DoUuTien NVARCHAR(20) DEFAULT N'Trung bình', -- 'Cao', 'Trung bình', 'Thấp'
    TrangThai NVARCHAR(50) DEFAULT N'To Do'       -- 'To Do', 'In Progress', 'Review', 'Done'
);
GO

-- 4. Bảng PhanCong (Phân công công việc)
CREATE TABLE PhanCong (
    MaTask INT NOT NULL FOREIGN KEY REFERENCES CongViec(MaTask) ON DELETE CASCADE,
    MaThanhVien INT NOT NULL FOREIGN KEY REFERENCES ThanhVien(MaThanhVien) ON DELETE CASCADE,
    NgayGiao DATETIME DEFAULT GETDATE(),
    CONSTRAINT PK_PhanCong PRIMARY KEY (MaTask, MaThanhVien)
);
GO

-- ========================================================
-- CHÈN DỮ LIỆU MẪU (SAMPLE DATA)
-- ========================================================

-- Dữ liệu Dự án
INSERT INTO DuAn (TenDuAn, NgayBatDau, NgayKetThuc, TrangThai) VALUES
(N'Xây dựng App Thương Mại Điện Tử', '2026-10-01', '2026-12-31', N'Đang chạy'),
(N'Hệ thống ERP Doanh Nghiệp', '2026-08-15', '2026-11-30', N'Đang chạy'),
(N'Nâng cấp Hạ tầng Cloud AWS', '2026-05-01', '2026-09-30', N'Hoàn thành');

-- Dữ liệu Thành viên
INSERT INTO ThanhVien (HoTen, Email, ChuyenMon) VALUES
(N'Nguyễn Văn An', 'an.nguyen@company.com', N'Backend C#/.NET'),
(N'Trần Thị Bình', 'binh.tran@company.com', N'Frontend React/Vue'),
(N'Lê Hoàng Cường', 'cuong.le@company.com', N'DevOps & Database'),
(N'Phạm Minh Dung', 'dung.pham@company.com', N'UI/UX & QA Tester');

-- Dữ liệu Công việc (Task)
-- Dự án 1:
INSERT INTO CongViec (MaDuAn, TenTask, MoTa, HanNop, DoUuTien, TrangThai) VALUES
(1, N'Thiết kế CSDL SQL Server', N'Xây dựng schema và bảng cho giỏ hàng, đơn hàng', '2026-10-15', N'Cao', N'Done'),
(1, N'Viết RESTful API Giỏ hàng', N'Triển khai API giỏ hàng, thanh toán JWT', '2026-11-10', N'Cao', N'Done'),
(1, N'Thiết kế Giao diện Giỏ hàng', N'Thiết kế responsive theo Figma', '2026-11-15', N'Trung bình', N'In Progress'),
(1, N'Tích hợp Cổng thanh toán VNPay', N'Kết nối sandbox VNPay API', '2026-11-17', N'Cao', N'In Progress'), -- Quá hạn giả lập
(1, N'Viết Unit Test & Kiểm thử tải', N'Sử dụng NUnit và JMeter kiểm tra hiệu năng', '2026-11-14', N'Thấp', N'To Do'); -- Quá hạn giả lập

-- Dự án 2:
INSERT INTO CongViec (MaDuAn, TenTask, MoTa, HanNop, DoUuTien, TrangThai) VALUES
(2, N'Module Quản lý Nhân sự HRM', N'CRUD nhân viên, tính công', '2026-11-25', N'Cao', N'In Progress'),
(2, N'Module Kế toán Thu Chi', N'Báo cáo dòng tiền, xuất Excel', '2026-11-28', N'Trung bình', N'To Do');

-- Dữ liệu Phân công
INSERT INTO PhanCong (MaTask, MaThanhVien, NgayGiao) VALUES
(1, 1, '2026-10-02'),
(1, 3, '2026-10-02'),
(2, 1, '2026-10-16'),
(3, 2, '2026-11-01'),
(4, 1, '2026-11-05'),
(4, 3, '2026-11-05'),
(5, 4, '2026-11-08'),
(6, 1, '2026-11-10'),
(6, 2, '2026-11-10'),
(7, 4, '2026-11-12');
GO

PRINT N'Khởi tạo CSDL ProjectTaskDB thành công!';
