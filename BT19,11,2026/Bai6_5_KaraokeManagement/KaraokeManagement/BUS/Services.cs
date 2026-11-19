using System;
using System.Collections.Generic;
using System.Data;
using KaraokeManagement.DAL;
using KaraokeManagement.DTO;

namespace KaraokeManagement.BUS
{
    public class PhongHatBUS
    {
        private readonly PhongHatDAL _dal = new PhongHatDAL();
        public List<PhongHatDTO> GetAll() => _dal.GetAll();
    }

    public class DichVuBUS
    {
        private readonly DichVuDAL _dal = new DichVuDAL();
        public List<DichVuDTO> GetAll() => _dal.GetAll();
    }

    public class HoaDonBUS
    {
        private readonly HoaDonDAL _dal = new HoaDonDAL();

        public HoaDonDTO? GetHoaDonChuaThanhToan(int maPhong) => _dal.GetHoaDonChuaThanhToan(maPhong);

        public int MoPhong(int maPhong)
        {
            if (maPhong <= 0)
                throw new ArgumentException("Mã phòng không hợp lệ!");
            return _dal.MoPhongTaoHoaDon(maPhong);
        }
    }

    public class ChiTietDichVuBUS
    {
        private readonly ChiTietDichVuDAL _dal = new ChiTietDichVuDAL();

        // Disconnected Model via SqlDataAdapter + DataTable
        public DataTable GetDichVuDaDat(int maHD) => _dal.GetDichVuDaDatDisconnected(maHD);

        public string ThemDichVu(int maHD, int maDV, int soLuong, decimal donGia)
        {
            if (maHD <= 0) return "Vui lòng chọn phòng đang hát có hóa đơn!";
            if (maDV <= 0) return "Vui lòng chọn dịch vụ hợp lệ!";
            if (soLuong <= 0) return "Số lượng dịch vụ phải lớn hơn 0!";

            bool ok = _dal.ThemDichVuVaoHoaDon(maHD, maDV, soLuong, donGia);
            return ok ? "" : "Thêm dịch vụ thất bại!";
        }
    }

    public class ThanhToanBUS
    {
        private readonly ThanhToanDAL _dal = new ThanhToanDAL();

        public (bool Success, string Message, decimal TongTienPhong, decimal TongTienDichVu, decimal TongCong, double SoGioHat)
            TinhTienVaThanhToan(HoaDonDTO hoaDon, decimal tongTienDichVu)
        {
            DateTime gioRa = DateTime.Now;
            TimeSpan duration = gioRa - hoaDon.GioVao;

            // Tính số giờ hát (làm tròn lên theo bước 15 phút hoặc số giờ thập phân, tối thiểu 0.5 giờ)
            double soGioHat = Math.Round(duration.TotalHours, 2);
            if (soGioHat < 0.1) soGioHat = 0.1; // Tối thiểu

            decimal tongTienPhong = Math.Round((decimal)soGioHat * hoaDon.GiaTheoGio, 0);
            decimal tongCong = tongTienPhong + tongTienDichVu;

            var (ok, msg) = _dal.ThanhToanHoaDonTransaction(hoaDon.MaHD, hoaDon.MaPhong, gioRa, tongTienPhong, tongTienDichVu);
            return (ok, msg, tongTienPhong, tongTienDichVu, tongCong, soGioHat);
        }
    }
}
