using System;
using System.Collections.Generic;
using ProjectManagement.DAL;
using ProjectManagement.DTO;

namespace ProjectManagement.BUS
{
    public class DuAnBUS
    {
        private readonly DuAnDAL _dal = new DuAnDAL();

        public List<DuAnDTO> GetAll()
        {
            return _dal.GetAll();
        }

        public string Add(DuAnDTO da)
        {
            if (string.IsNullOrWhiteSpace(da.TenDuAn))
                return "Tên dự án không được để trống!";

            if (da.NgayBatDau.HasValue && da.NgayKetThuc.HasValue && da.NgayKetThuc < da.NgayBatDau)
                return "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu!";

            bool ok = _dal.Insert(da);
            return ok ? "" : "Thêm dự án thất bại!";
        }

        public string Update(DuAnDTO da)
        {
            if (da.MaDuAn <= 0)
                return "Mã dự án không hợp lệ!";

            if (string.IsNullOrWhiteSpace(da.TenDuAn))
                return "Tên dự án không được để trống!";

            if (da.NgayBatDau.HasValue && da.NgayKetThuc.HasValue && da.NgayKetThuc < da.NgayBatDau)
                return "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu!";

            bool ok = _dal.Update(da);
            return ok ? "" : "Cập nhật dự án thất bại!";
        }

        public string Delete(int maDuAn)
        {
            if (maDuAn <= 0)
                return "Mã dự án không hợp lệ!";

            bool ok = _dal.Delete(maDuAn);
            return ok ? "" : "Xóa dự án thất bại!";
        }
    }

    public class ThanhVienBUS
    {
        private readonly ThanhVienDAL _dal = new ThanhVienDAL();

        public List<ThanhVienDTO> GetAll()
        {
            return _dal.GetAll();
        }
    }

    public class CongViecBUS
    {
        private readonly CongViecDAL _dal = new CongViecDAL();

        public List<CongViecDTO> GetTasks(int? maDuAn = null, string? trangThai = null)
        {
            return _dal.GetTasks(maDuAn, trangThai);
        }

        public (string Error, int NewId) Add(CongViecDTO task)
        {
            if (task.MaDuAn <= 0)
                return ("Vui lòng chọn dự án hợp lệ!", 0);

            if (string.IsNullOrWhiteSpace(task.TenTask))
                return ("Tên công việc không được để trống!", 0);

            int id = _dal.Insert(task);
            return id > 0 ? ("", id) : ("Thêm công việc thất bại!", 0);
        }

        public string Update(CongViecDTO task)
        {
            if (task.MaTask <= 0)
                return "Mã công việc không hợp lệ!";

            if (string.IsNullOrWhiteSpace(task.TenTask))
                return "Tên công việc không được để trống!";

            bool ok = _dal.Update(task);
            return ok ? "" : "Cập nhật công việc thất bại!";
        }

        public string UpdateStatus(int maTask, string newStatus)
        {
            if (maTask <= 0)
                return "Mã công việc không hợp lệ!";

            if (string.IsNullOrWhiteSpace(newStatus))
                return "Trạng thái không hợp lệ!";

            bool ok = _dal.UpdateStatus(maTask, newStatus);
            return ok ? "" : "Cập nhật trạng thái thất bại!";
        }

        public string Delete(int maTask)
        {
            if (maTask <= 0)
                return "Mã công việc không hợp lệ!";

            bool ok = _dal.Delete(maTask);
            return ok ? "" : "Xóa công việc thất bại!";
        }

        public List<CongViecDTO> GetOverdueTasks()
        {
            return _dal.GetOverdueTasksUsingDataReader();
        }
    }

    public class PhanCongBUS
    {
        private readonly PhanCongDAL _dal = new PhanCongDAL();

        public bool Assign(int maTask, int maThanhVien)
        {
            return _dal.Assign(maTask, maThanhVien);
        }

        public bool Remove(int maTask, int maThanhVien)
        {
            return _dal.Remove(maTask, maThanhVien);
        }

        public List<int> GetAssignedMemberIds(int maTask)
        {
            return _dal.GetAssignedMemberIds(maTask);
        }
    }

    public class BaoCaoBUS
    {
        private readonly BaoCaoDAL _dal = new BaoCaoDAL();

        public List<ThongKeThanhVienDTO> GetMemberCompletionStats(int? maDuAn = null)
        {
            return _dal.GetMemberCompletionStats(maDuAn);
        }

        public (int Total, int Completed, double Rate) GetOverallStats(int? maDuAn = null)
        {
            int total = _dal.GetTotalTasksCount(maDuAn);
            int completed = _dal.GetCompletedTasksCount(maDuAn);
            double rate = total > 0 ? Math.Round((double)completed / total * 100, 2) : 0;
            return (total, completed, rate);
        }
    }
}
