using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using GarageManagement.DTO;

namespace GarageManagement.DAL
{
    public class PhieuSuaChuaDAL
    {
        public List<PhieuSuaChuaDTO> GetAll()
        {
            var list = new List<PhieuSuaChuaDTO>();
            string sql = @"
                SELECT p.MaPhieu, p.BienSo, x.HieuXe, kh.TenKH, p.NgayNhan, p.NoiDungSua, p.TongTien, p.TrangThai
                FROM PhieuSuaChua p
                INNER JOIN Xe x ON p.BienSo = x.BienSo
                INNER JOIN KhachHang kh ON x.MaKH = kh.MaKH
                ORDER BY p.MaPhieu DESC";
            DataTable dt = DatabaseHelper.ExecuteQuery(sql);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new PhieuSuaChuaDTO
                {
                    MaPhieu = Convert.ToInt32(row["MaPhieu"]),
                    BienSo = row["BienSo"].ToString() ?? "",
                    HieuXe = row["HieuXe"].ToString() ?? "",
                    TenKH = row["TenKH"].ToString() ?? "",
                    NgayNhan = Convert.ToDateTime(row["NgayNhan"]),
                    NoiDungSua = row["NoiDungSua"]?.ToString() ?? "",
                    TongTien = Convert.ToDecimal(row["TongTien"]),
                    TrangThai = row["TrangThai"].ToString() ?? ""
                });
            }
            return list;
        }

        public List<ChiTietSuaChuaDTO> GetDetails(int maPhieu)
        {
            var list = new List<ChiTietSuaChuaDTO>();
            string sql = @"
                SELECT c.MaPhieu, c.MaPhuTung, pt.TenPhuTung, pt.DonGia, c.SoLuong, c.TienCong
                FROM ChiTietSuaChua c
                INNER JOIN PhuTung pt ON c.MaPhuTung = pt.MaPhuTung
                WHERE c.MaPhieu = @MaPhieu";
            SqlParameter[] parameters = {
                new SqlParameter("@MaPhieu", SqlDbType.Int) { Value = maPhieu }
            };
            DataTable dt = DatabaseHelper.ExecuteQuery(sql, parameters);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ChiTietSuaChuaDTO
                {
                    MaPhieu = Convert.ToInt32(row["MaPhieu"]),
                    MaPhuTung = Convert.ToInt32(row["MaPhuTung"]),
                    TenPhuTung = row["TenPhuTung"].ToString() ?? "",
                    DonGia = Convert.ToDecimal(row["DonGia"]),
                    SoLuong = Convert.ToInt32(row["SoLuong"]),
                    TienCong = Convert.ToDecimal(row["TienCong"])
                });
            }
            return list;
        }

        // ========================================================
        // 2. Lập Phiếu Sửa Chữa & Thay Thế Phụ Tùng Bằng SqlTransaction
        // ========================================================
        public (bool Success, string Message, int MaPhieu) TaoPhieuSuaChuaTransaction(
            PhieuSuaChuaDTO phieu, 
            List<ChiTietSuaChuaDTO> chiTietList)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();

                try
                {
                    // 1. Tạo bản ghi PhieuSuaChua
                    string sqlPhieu = @"
                        INSERT INTO PhieuSuaChua (BienSo, NgayNhan, NoiDungSua, TongTien, TrangThai)
                        OUTPUT INSERTED.MaPhieu
                        VALUES (@BienSo, @NgayNhan, @NoiDungSua, @TongTien, @TrangThai)";

                    SqlCommand cmdPhieu = new SqlCommand(sqlPhieu, conn, trans);
                    cmdPhieu.Parameters.Add(new SqlParameter("@BienSo", SqlDbType.VarChar, 20) { Value = phieu.BienSo });
                    cmdPhieu.Parameters.Add(new SqlParameter("@NgayNhan", SqlDbType.DateTime) { Value = phieu.NgayNhan });
                    cmdPhieu.Parameters.Add(new SqlParameter("@NoiDungSua", SqlDbType.NVarChar) { Value = (object?)phieu.NoiDungSua ?? DBNull.Value });
                    cmdPhieu.Parameters.Add(new SqlParameter("@TongTien", SqlDbType.Decimal) { Value = phieu.TongTien });
                    cmdPhieu.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.NVarChar, 30) { Value = phieu.TrangThai });

                    int maPhieuMoi = Convert.ToInt32(cmdPhieu.ExecuteScalar());

                    // 2. Duyệt qua từng chi tiết: kiểm tra tồn kho, tạo ChiTiet, trừ tồn kho
                    foreach (var item in chiTietList)
                    {
                        // Kiểm tra số lượng tồn kho hiện tại
                        string sqlCheckTon = "SELECT SoLuongTon, TenPhuTung FROM PhuTung WHERE MaPhuTung = @MaPhuTung";
                        SqlCommand cmdCheck = new SqlCommand(sqlCheckTon, conn, trans);
                        cmdCheck.Parameters.Add(new SqlParameter("@MaPhuTung", SqlDbType.Int) { Value = item.MaPhuTung });

                        using (SqlDataReader reader = cmdCheck.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int tonKho = reader.GetInt32(0);
                                string tenPT = reader.GetString(1);

                                // 4. Nếu phụ tùng không đủ số lượng trong kho -> trans.Rollback() và xuất thông báo
                                if (tonKho < item.SoLuong)
                                {
                                    reader.Close();
                                    trans.Rollback();
                                    return (false, $"[TRANSACTION ROLLBACK] Phụ tùng '{tenPT}' không đủ số lượng tồn kho (Hiện còn: {tonKho}, Yêu cầu: {item.SoLuong})!", 0);
                                }
                            }
                            else
                            {
                                reader.Close();
                                trans.Rollback();
                                return (false, $"[TRANSACTION ROLLBACK] Không tìm thấy phụ tùng mã {item.MaPhuTung}!", 0);
                            }
                        }

                        // 2. Tạo bản ghi ChiTietSuaChua
                        string sqlCT = @"
                            INSERT INTO ChiTietSuaChua (MaPhieu, MaPhuTung, SoLuong, TienCong)
                            VALUES (@MaPhieu, @MaPhuTung, @SoLuong, @TienCong)";
                        SqlCommand cmdCT = new SqlCommand(sqlCT, conn, trans);
                        cmdCT.Parameters.Add(new SqlParameter("@MaPhieu", SqlDbType.Int) { Value = maPhieuMoi });
                        cmdCT.Parameters.Add(new SqlParameter("@MaPhuTung", SqlDbType.Int) { Value = item.MaPhuTung });
                        cmdCT.Parameters.Add(new SqlParameter("@SoLuong", SqlDbType.Int) { Value = item.SoLuong });
                        cmdCT.Parameters.Add(new SqlParameter("@TienCong", SqlDbType.Decimal) { Value = item.TienCong });
                        cmdCT.ExecuteNonQuery();

                        // 3. Trừ số lượng phụ tùng tương ứng trong bảng PhuTung (SoLuongTon -= SoLuong)
                        string sqlTruKho = @"
                            UPDATE PhuTung 
                            SET SoLuongTon = SoLuongTon - @SoLuong 
                            WHERE MaPhuTung = @MaPhuTung";
                        SqlCommand cmdTruKho = new SqlCommand(sqlTruKho, conn, trans);
                        cmdTruKho.Parameters.Add(new SqlParameter("@SoLuong", SqlDbType.Int) { Value = item.SoLuong });
                        cmdTruKho.Parameters.Add(new SqlParameter("@MaPhuTung", SqlDbType.Int) { Value = item.MaPhuTung });
                        cmdTruKho.ExecuteNonQuery();
                    }

                    // Commit giao dịch khi mọi bước thành công
                    trans.Commit();
                    return (true, $"Lập phiếu sửa chữa #{maPhieuMoi} thành công! Kho phụ tùng đã được cập nhật.", maPhieuMoi);
                }
                catch (Exception ex)
                {
                    try
                    {
                        trans.Rollback();
                    }
                    catch { }
                    return (false, $"[TRANSACTION ROLLBACK] Lỗi hệ thống: {ex.Message}", 0);
                }
            }
        }

        // ========================================================
        // 3. Thanh Toán & Cập Nhật Trạng Thái
        // ========================================================
        public bool ThanhToanPhieu(int maPhieu, decimal tongChiPhi)
        {
            string sql = @"
                UPDATE PhieuSuaChua 
                SET TrangThai = N'Đã thanh toán', TongTien = @TongTien 
                WHERE MaPhieu = @MaPhieu";
            SqlParameter[] parameters = {
                new SqlParameter("@TongTien", SqlDbType.Decimal) { Value = tongChiPhi },
                new SqlParameter("@MaPhieu", SqlDbType.Int) { Value = maPhieu }
            };
            return DatabaseHelper.ExecuteNonQuery(sql, parameters) > 0;
        }
    }
}
