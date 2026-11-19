using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ProjectManagement.DTO;

namespace ProjectManagement.DAL
{
    public class DuAnDAL
    {
        public List<DuAnDTO> GetAll()
        {
            var list = new List<DuAnDTO>();
            string sql = "SELECT MaDuAn, TenDuAn, NgayBatDau, NgayKetThuc, TrangThai FROM DuAn ORDER BY MaDuAn DESC";
            DataTable dt = DatabaseHelper.ExecuteQuery(sql);

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new DuAnDTO
                {
                    MaDuAn = Convert.ToInt32(row["MaDuAn"]),
                    TenDuAn = row["TenDuAn"].ToString() ?? "",
                    NgayBatDau = row["NgayBatDau"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["NgayBatDau"]),
                    NgayKetThuc = row["NgayKetThuc"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["NgayKetThuc"]),
                    TrangThai = row["TrangThai"].ToString() ?? "Đang chạy"
                });
            }
            return list;
        }

        public bool Insert(DuAnDTO da)
        {
            string sql = @"INSERT INTO DuAn (TenDuAn, NgayBatDau, NgayKetThuc, TrangThai) 
                           VALUES (@TenDuAn, @NgayBatDau, @NgayKetThuc, @TrangThai)";
            SqlParameter[] parameters = {
                new SqlParameter("@TenDuAn", SqlDbType.NVarChar, 150) { Value = da.TenDuAn },
                new SqlParameter("@NgayBatDau", SqlDbType.DateTime) { Value = (object?)da.NgayBatDau ?? DBNull.Value },
                new SqlParameter("@NgayKetThuc", SqlDbType.DateTime) { Value = (object?)da.NgayKetThuc ?? DBNull.Value },
                new SqlParameter("@TrangThai", SqlDbType.NVarChar, 50) { Value = da.TrangThai }
            };
            return DatabaseHelper.ExecuteNonQuery(sql, parameters) > 0;
        }

        public bool Update(DuAnDTO da)
        {
            string sql = @"UPDATE DuAn 
                           SET TenDuAn = @TenDuAn, NgayBatDau = @NgayBatDau, NgayKetThuc = @NgayKetThuc, TrangThai = @TrangThai 
                           WHERE MaDuAn = @MaDuAn";
            SqlParameter[] parameters = {
                new SqlParameter("@MaDuAn", SqlDbType.Int) { Value = da.MaDuAn },
                new SqlParameter("@TenDuAn", SqlDbType.NVarChar, 150) { Value = da.TenDuAn },
                new SqlParameter("@NgayBatDau", SqlDbType.DateTime) { Value = (object?)da.NgayBatDau ?? DBNull.Value },
                new SqlParameter("@NgayKetThuc", SqlDbType.DateTime) { Value = (object?)da.NgayKetThuc ?? DBNull.Value },
                new SqlParameter("@TrangThai", SqlDbType.NVarChar, 50) { Value = da.TrangThai }
            };
            return DatabaseHelper.ExecuteNonQuery(sql, parameters) > 0;
        }

        public bool Delete(int maDuAn)
        {
            string sql = "DELETE FROM DuAn WHERE MaDuAn = @MaDuAn";
            SqlParameter[] parameters = {
                new SqlParameter("@MaDuAn", SqlDbType.Int) { Value = maDuAn }
            };
            return DatabaseHelper.ExecuteNonQuery(sql, parameters) > 0;
        }
    }

    public class ThanhVienDAL
    {
        public List<ThanhVienDTO> GetAll()
        {
            var list = new List<ThanhVienDTO>();
            string sql = "SELECT MaThanhVien, HoTen, Email, ChuyenMon FROM ThanhVien ORDER BY HoTen ASC";
            DataTable dt = DatabaseHelper.ExecuteQuery(sql);

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ThanhVienDTO
                {
                    MaThanhVien = Convert.ToInt32(row["MaThanhVien"]),
                    HoTen = row["HoTen"].ToString() ?? "",
                    Email = row["Email"].ToString() ?? "",
                    ChuyenMon = row["ChuyenMon"]?.ToString() ?? ""
                });
            }
            return list;
        }
    }
}
