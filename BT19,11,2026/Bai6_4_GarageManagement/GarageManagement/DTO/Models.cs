using System;

namespace GarageManagement.DTO
{
    public class KhachHangDTO
    {
        public int MaKH { get; set; }
        public string TenKH { get; set; } = string.Empty;
        public string DienThoai { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;
    }

    public class XeDTO
    {
        public string BienSo { get; set; } = string.Empty;
        public int MaKH { get; set; }
        public string TenKH { get; set; } = string.Empty;
        public string DienThoai { get; set; } = string.Empty;
        public string HieuXe { get; set; } = string.Empty;
        public int NamSanXuat { get; set; } = DateTime.Now.Year;
    }

    public class PhuTungDTO
    {
        public int MaPhuTung { get; set; }
        public string TenPhuTung { get; set; } = string.Empty;
        public decimal DonGia { get; set; }
        public int SoLuongTon { get; set; }
    }

    public class PhieuSuaChuaDTO
    {
        public int MaPhieu { get; set; }
        public string BienSo { get; set; } = string.Empty;
        public string HieuXe { get; set; } = string.Empty;
        public string TenKH { get; set; } = string.Empty;
        public DateTime NgayNhan { get; set; } = DateTime.Now;
        public string NoiDungSua { get; set; } = string.Empty;
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; } = "Đang sửa";
    }

    public class ChiTietSuaChuaDTO
    {
        public int MaPhieu { get; set; }
        public int MaPhuTung { get; set; }
        public string TenPhuTung { get; set; } = string.Empty;
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        public decimal TienCong { get; set; }
        public decimal ThanhTien => (SoLuong * DonGia) + TienCong;
    }
}
