using System;

namespace ProjectManagement.DTO
{
    public class DuAnDTO
    {
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string TrangThai { get; set; } = "Đang chạy";
    }

    public class ThanhVienDTO
    {
        public int MaThanhVien { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ChuyenMon { get; set; } = string.Empty;
    }

    public class CongViecDTO
    {
        public int MaTask { get; set; }
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
        public string TenTask { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public DateTime? HanNop { get; set; }
        public string DoUuTien { get; set; } = "Trung bình";
        public string TrangThai { get; set; } = "To Do";
        public string NguoiThucHien { get; set; } = string.Empty;
    }

    public class PhanCongDTO
    {
        public int MaTask { get; set; }
        public int MaThanhVien { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public DateTime NgayGiao { get; set; } = DateTime.Now;
    }

    public class ThongKeThanhVienDTO
    {
        public int MaThanhVien { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string ChuyenMon { get; set; } = string.Empty;
        public int TongSoTask { get; set; }
        public int SoTaskHoanThanh { get; set; }
        public double TyLeHoanThanh { get; set; }
    }
}
