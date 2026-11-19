using System;

namespace KaraokeManagement.DTO
{
    public class PhongHatDTO
    {
        public int MaPhong { get; set; }
        public string TenPhong { get; set; } = string.Empty;
        public string LoaiPhong { get; set; } = "Thường";
        public decimal GiaTheoGio { get; set; } = 150000;
        public string TrangThai { get; set; } = "Trống"; // "Trống" hoặc "Đang hát"
    }

    public class DichVuDTO
    {
        public int MaDV { get; set; }
        public string TenDV { get; set; } = string.Empty;
        public decimal DonGia { get; set; }
        public int TonKho { get; set; }
    }

    public class HoaDonDTO
    {
        public int MaHD { get; set; }
        public int MaPhong { get; set; }
        public string TenPhong { get; set; } = string.Empty;
        public decimal GiaTheoGio { get; set; }
        public DateTime GioVao { get; set; } = DateTime.Now;
        public DateTime? GioRa { get; set; }
        public decimal TongTienPhong { get; set; }
        public decimal TongTienDichVu { get; set; }
        public bool TrangThai { get; set; } = false; // false: Chưa thanh toán, true: Đã thanh toán
    }

    public class ChiTietDichVuDTO
    {
        public int MaHD { get; set; }
        public int MaDV { get; set; }
        public string TenDV { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
    }
}
