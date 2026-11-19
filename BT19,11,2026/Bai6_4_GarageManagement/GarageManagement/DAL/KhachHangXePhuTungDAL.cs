using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using GarageManagement.DTO;

namespace GarageManagement.DAL
{
    public class KhachHangDAL
    {
        public List<KhachHangDTO> GetAll()
        {
            var list = new List<KhachHangDTO>();
            string sql = "SELECT MaKH, TenKH, DienThoai, DiaChi FROM KhachHang ORDER BY MaKH DESC";
            DataTable dt = DatabaseHelper.ExecuteQuery(sql);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new KhachHangDTO
                {
                    MaKH = Convert.ToInt32(row["MaKH"]),
                    TenKH = row["TenKH"].ToString() ?? "",
                    DienThoai = row["DienThoai"].ToString() ?? "",
                    DiaChi = row["DiaChi"]?.ToString() ?? ""
                });
            }
            return list;
        }

        public int Insert(KhachHangDTO kh)
        {
            string sql = @"INSERT INTO KhachHang (TenKH, DienThoai, DiaChi)
                           OUTPUT INSERTED.MaKH
                           VALUES (@TenKH, @DienThoai, @DiaChi)";
            SqlParameter[] parameters = {
                new SqlParameter("@TenKH", SqlDbType.NVarChar, 100) { Value = kh.TenKH },
                new SqlParameter("@DienThoai", SqlDbType.VarChar, 15) { Value = kh.DienThoai },
                new SqlParameter("@DiaChi", SqlDbType.NVarChar, 200) { Value = (object?)kh.DiaChi ?? DBNull.Value }
            };
            object? result = DatabaseHelper.ExecuteScalar(sql, parameters);
            return result != null ? Convert.ToInt32(result) : 0;
        }
    }

    public class XeDAL
    {
        // 1. Tra cứu biển số xe bằng SqlParameter
        public XeDTO? FindByBienSo(string bienSo)
        {
            string sql = @"
                SELECT x.BienSo, x.MaKH, x.HieuXe, x.NamSanXuat, kh.TenKH, kh.DienThoai
                FROM Xe x
                INNER JOIN KhachHang kh ON x.MaKH = kh.MaKH
                WHERE UPPER(RTRIM(LTRIM(x.BienSo))) = UPPER(RTRIM(LTRIM(@BienSo)))";

            SqlParameter[] parameters = {
                new SqlParameter("@BienSo", SqlDbType.VarChar, 20) { Value = bienSo }
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(sql, parameters);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new XeDTO
                {
                    BienSo = row["BienSo"].ToString() ?? "",
                    MaKH = Convert.ToInt32(row["MaKH"]),
                    HieuXe = row["HieuXe"].ToString() ?? "",
                    NamSanXuat = Convert.ToInt32(row["NamSanXuat"]),
                    TenKH = row["TenKH"].ToString() ?? "",
                    DienThoai = row["DienThoai"].ToString() ?? ""
                };
            }
            return null;
        }

        public List<XeDTO> GetAll()
        {
            var list = new List<XeDTO>();
            string sql = @"
                SELECT x.BienSo, x.MaKH, x.HieuXe, x.NamSanXuat, kh.TenKH, kh.DienThoai
                FROM Xe x
                INNER JOIN KhachHang kh ON x.MaKH = kh.MaKH
                ORDER BY x.BienSo ASC";
            DataTable dt = DatabaseHelper.ExecuteQuery(sql);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new XeDTO
                {
                    BienSo = row["BienSo"].ToString() ?? "",
                    MaKH = Convert.ToInt32(row["MaKH"]),
                    HieuXe = row["HieuXe"].ToString() ?? "",
                    NamSanXuat = Convert.ToInt32(row["NamSanXuat"]),
                    TenKH = row["TenKH"].ToString() ?? "",
                    DienThoai = row["DienThoai"].ToString() ?? ""
                });
            }
            return list;
        }

        public bool Insert(XeDTO xe)
        {
            string sql = "INSERT INTO Xe (BienSo, MaKH, HieuXe, NamSanXuat) VALUES (@BienSo, @MaKH, @HieuXe, @NamSanXuat)";
            SqlParameter[] parameters = {
                new SqlParameter("@BienSo", SqlDbType.VarChar, 20) { Value = xe.BienSo },
                new SqlParameter("@MaKH", SqlDbType.Int) { Value = xe.MaKH },
                new SqlParameter("@HieuXe", SqlDbType.NVarChar, 50) { Value = xe.HieuXe },
                new SqlParameter("@NamSanXuat", SqlDbType.Int) { Value = xe.NamSanXuat }
            };
            return DatabaseHelper.ExecuteNonQuery(sql, parameters) > 0;
        }
    }

    public class PhuTungDAL
    {
        public List<PhuTungDTO> GetAll()
        {
            var list = new List<PhuTungDTO>();
            string sql = "SELECT MaPhuTung, TenPhuTung, DonGia, SoLuongTon FROM PhuTung ORDER BY MaPhuTung ASC";
            DataTable dt = DatabaseHelper.ExecuteQuery(sql);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new PhuTungDTO
                {
                    MaPhuTung = Convert.ToInt32(row["MaPhuTung"]),
                    TenPhuTung = row["TenPhuTung"].ToString() ?? "",
                    DonGia = Convert.ToDecimal(row["DonGia"]),
                    SoLuongTon = Convert.ToInt32(row["SoLuongTon"])
                });
            }
            return list;
        }

        public bool Insert(PhuTungDTO pt)
        {
            string sql = "INSERT INTO PhuTung (TenPhuTung, DonGia, SoLuongTon) VALUES (@TenPhuTung, @DonGia, @SoLuongTon)";
            SqlParameter[] parameters = {
                new SqlParameter("@TenPhuTung", SqlDbType.NVarChar, 150) { Value = pt.TenPhuTung },
                new SqlParameter("@DonGia", SqlDbType.Decimal) { Value = pt.DonGia },
                new SqlParameter("@SoLuongTon", SqlDbType.Int) { Value = pt.SoLuongTon }
            };
            return DatabaseHelper.ExecuteNonQuery(sql, parameters) > 0;
        }

        public bool Update(PhuTungDTO pt)
        {
            string sql = "UPDATE PhuTung SET TenPhuTung = @TenPhuTung, DonGia = @DonGia, SoLuongTon = @SoLuongTon WHERE MaPhuTung = @MaPhuTung";
            SqlParameter[] parameters = {
                new SqlParameter("@MaPhuTung", SqlDbType.Int) { Value = pt.MaPhuTung },
                new SqlParameter("@TenPhuTung", SqlDbType.NVarChar, 150) { Value = pt.TenPhuTung },
                new SqlParameter("@DonGia", SqlDbType.Decimal) { Value = pt.DonGia },
                new SqlParameter("@SoLuongTon", SqlDbType.Int) { Value = pt.SoLuongTon }
            };
            return DatabaseHelper.ExecuteNonQuery(sql, parameters) > 0;
        }

        public bool Delete(int maPhuTung)
        {
            string sql = "DELETE FROM PhuTung WHERE MaPhuTung = @MaPhuTung";
            SqlParameter[] parameters = {
                new SqlParameter("@MaPhuTung", SqlDbType.Int) { Value = maPhuTung }
            };
            return DatabaseHelper.ExecuteNonQuery(sql, parameters) > 0;
        }
    }
}
