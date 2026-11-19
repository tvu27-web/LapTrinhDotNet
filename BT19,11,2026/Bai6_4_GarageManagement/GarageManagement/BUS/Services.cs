using System;
using System.Collections.Generic;
using GarageManagement.DAL;
using GarageManagement.DTO;

namespace GarageManagement.BUS
{
    public class KhachHangBUS
    {
        private readonly KhachHangDAL _dal = new KhachHangDAL();

        public List<KhachHangDTO> GetAll() => _dal.GetAll();

        public (string Error, int NewId) Add(KhachHangDTO kh)
        {
            if (string.IsNullOrWhiteSpace(kh.TenKH))
                return ("Tên khách hàng không được để trống!", 0);

            if (string.IsNullOrWhiteSpace(kh.DienThoai))
                return ("Số điện thoại không được để trống!", 0);

            int id = _dal.Insert(kh);
            return id > 0 ? ("", id) : ("Thêm khách hàng thất bại!", 0);
        }
    }

    public class XeBUS
    {
        private readonly XeDAL _dal = new XeDAL();

        public XeDTO? FindByBienSo(string bienSo)
        {
            if (string.IsNullOrWhiteSpace(bienSo))
                return null;
            return _dal.FindByBienSo(bienSo);
        }

        public List<XeDTO> GetAll() => _dal.GetAll();

        public string Add(XeDTO xe)
        {
            if (string.IsNullOrWhiteSpace(xe.BienSo))
                return "Biển số xe không được để trống!";

            if (xe.MaKH <= 0)
                return "Khách hàng không hợp lệ!";

            if (string.IsNullOrWhiteSpace(xe.HieuXe))
                return "Hiệu xe không được để trống!";

            bool ok = _dal.Insert(xe);
            return ok ? "" : "Thêm xe thất bại!";
        }
    }

    public class PhuTungBUS
    {
        private readonly PhuTungDAL _dal = new PhuTungDAL();

        public List<PhuTungDTO> GetAll() => _dal.GetAll();

        public string Add(PhuTungDTO pt)
        {
            if (string.IsNullOrWhiteSpace(pt.TenPhuTung))
                return "Tên phụ tùng không được để trống!";

            if (pt.DonGia < 0)
                return "Đơn giá không được âm!";

            if (pt.SoLuongTon < 0)
                return "Số lượng tồn không được âm!";

            return _dal.Insert(pt) ? "" : "Thêm phụ tùng thất bại!";
        }

        public string Update(PhuTungDTO pt)
        {
            if (pt.MaPhuTung <= 0)
                return "Mã phụ tùng không hợp lệ!";

            if (string.IsNullOrWhiteSpace(pt.TenPhuTung))
                return "Tên phụ tùng không được để trống!";

            if (pt.DonGia < 0)
                return "Đơn giá không được âm!";

            if (pt.SoLuongTon < 0)
                return "Số lượng tồn không được âm!";

            return _dal.Update(pt) ? "" : "Cập nhật phụ tùng thất bại!";
        }

        public string Delete(int maPhuTung)
        {
            if (maPhuTung <= 0)
                return "Mã phụ tùng không hợp lệ!";
            return _dal.Delete(maPhuTung) ? "" : "Xóa phụ tùng thất bại!";
        }
    }

    public class PhieuSuaChuaBUS
    {
        private readonly PhieuSuaChuaDAL _dal = new PhieuSuaChuaDAL();

        public List<PhieuSuaChuaDTO> GetAll() => _dal.GetAll();

        public List<ChiTietSuaChuaDTO> GetDetails(int maPhieu) => _dal.GetDetails(maPhieu);

        public (bool Success, string Message, int MaPhieu) LapPhieuSuaChua(PhieuSuaChuaDTO phieu, List<ChiTietSuaChuaDTO> chiTietList)
        {
            if (string.IsNullOrWhiteSpace(phieu.BienSo))
                return (false, "Vui lòng chọn hoặc nhập biển số xe!", 0);

            if (chiTietList == null || chiTietList.Count == 0)
                return (false, "Phiếu sửa chữa phải có ít nhất 1 hạng mục phụ tùng / dịch vụ!", 0);

            // Tính tổng tiền dự kiến
            decimal tong = 0;
            foreach (var ct in chiTietList)
            {
                tong += (ct.SoLuong * ct.DonGia) + ct.TienCong;
            }
            phieu.TongTien = tong;

            return _dal.TaoPhieuSuaChuaTransaction(phieu, chiTietList);
        }

        public string ThanhToan(int maPhieu, decimal tongChiPhi)
        {
            if (maPhieu <= 0)
                return "Mã phiếu không hợp lệ!";

            bool ok = _dal.ThanhToanPhieu(maPhieu, tongChiPhi);
            return ok ? "" : "Thanh toán phiếu thất bại!";
        }
    }
}
