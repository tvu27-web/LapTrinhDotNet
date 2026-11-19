using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using KaraokeManagement.DTO;

namespace KaraokeManagement.DAL
{
    public class PhongHatDAL
    {
        public List<PhongHatDTO> GetAll()
        {
            var list = new List<PhongHatDTO>();
            string sql = "SELECT MaPhong, TenPhong, LoaiPhong, GiaTheoGio, TrangThai FROM PhongHat ORDER BY MaPhong ASC";
            DataTable dt = DatabaseHelper.ExecuteQuery(sql);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new PhongHatDTO
                {
                    MaPhong = Convert.ToInt32(row["MaPhong"]),
                    TenPhong = row["TenPhong"].ToString() ?? "",
                    LoaiPhong = row["LoaiPhong"].ToString() ?? "Thường",
                    GiaTheoGio = Convert.ToDecimal(row["GiaTheoGio"]),
                    TrangThai = row["TrangThai"].ToString() ?? "Trống"
                });
            }
            return list;
        }

        public bool UpdateTrangThai(int maPhong, string trangThai)
        {
            string sql = "UPDATE PhongHat SET TrangThai = @TrangThai WHERE MaPhong = @MaPhong";
            SqlParameter[] parameters = {
                new SqlParameter("@TrangThai", SqlDbType.NVarChar, 20) { Value = trangThai },
                new SqlParameter("@MaPhong", SqlDbType.Int) { Value = maPhong }
            };
            return DatabaseHelper.ExecuteNonQuery(sql, parameters) > 0;
        }
    }

    public class DichVuDAL
    {
        public List<DichVuDTO> GetAll()
        {
            var list = new List<DichVuDTO>();
            string sql = "SELECT MaDV, TenDV, DonGia, TonKho FROM DichVu ORDER BY MaDV ASC";
            DataTable dt = DatabaseHelper.ExecuteQuery(sql);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new DichVuDTO
                {
                    MaDV = Convert.ToInt32(row["MaDV"]),
                    TenDV = row["TenDV"].ToString() ?? "",
                    DonGia = Convert.ToDecimal(row["DonGia"]),
                    TonKho = Convert.ToInt32(row["TonKho"])
                });
            }
            return list;
        }
    }

    public class HoaDonDAL
    {
        // Lấy hóa đơn chưa thanh toán gần nhất của phòng
        public HoaDonDTO? GetHoaDonChuaThanhToan(int maPhong)
        {
            string sql = @"
                SELECT TOP 1 h.MaHD, h.MaPhong, p.TenPhong, p.GiaTheoGio, h.GioVao, h.GioRa, 
                             h.TongTienPhong, h.TongTienDichVu, h.TrangThai
                FROM HoaDon h
                INNER JOIN PhongHat p ON h.MaPhong = p.MaPhong
                WHERE h.MaPhong = @MaPhong AND h.TrangThai = 0
                ORDER BY h.MaHD DESC";

            SqlParameter[] parameters = {
                new SqlParameter("@MaPhong", SqlDbType.Int) { Value = maPhong }
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(sql, parameters);
            if (dt.Rows.Count > 0)
            {
                DataRow r = dt.Rows[0];
                return new HoaDonDTO
                {
                    MaHD = Convert.ToInt32(r["MaHD"]),
                    MaPhong = Convert.ToInt32(r["MaPhong"]),
                    TenPhong = r["TenPhong"].ToString() ?? "",
                    GiaTheoGio = Convert.ToDecimal(r["GiaTheoGio"]),
                    GioVao = Convert.ToDateTime(r["GioVao"]),
                    GioRa = r["GioRa"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(r["GioRa"]),
                    TongTienPhong = Convert.ToDecimal(r["TongTienPhong"]),
                    TongTienDichVu = Convert.ToDecimal(r["TongTienDichVu"]),
                    TrangThai = Convert.ToBoolean(r["TrangThai"])
                };
            }
            return null;
        }

        // Mở phòng: Tạo HoaDon mới ghi nhận GioVao = GetDate(), đổi TrangThai phòng sang "Đang hát"
        public int MoPhongTaoHoaDon(int maPhong)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Tạo hóa đơn mới
                        string sqlHD = @"
                            INSERT INTO HoaDon (MaPhong, GioVao, GioRa, TongTienPhong, TongTienDichVu, TrangThai)
                            OUTPUT INSERTED.MaHD
                            VALUES (@MaPhong, GETDATE(), NULL, 0, 0, 0)";
                        SqlCommand cmdHD = new SqlCommand(sqlHD, conn, trans);
                        cmdHD.Parameters.Add(new SqlParameter("@MaPhong", SqlDbType.Int) { Value = maPhong });
                        int newMaHD = Convert.ToInt32(cmdHD.ExecuteScalar());

                        // 2. Cập nhật trạng thái phòng sang "Đang hát"
                        string sqlP = "UPDATE PhongHat SET TrangThai = N'Đang hát' WHERE MaPhong = @MaPhong";
                        SqlCommand cmdP = new SqlCommand(sqlP, conn, trans);
                        cmdP.Parameters.Add(new SqlParameter("@MaPhong", SqlDbType.Int) { Value = maPhong });
                        cmdP.ExecuteNonQuery();

                        trans.Commit();
                        return newMaHD;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
    }

    public class ChiTietDichVuDAL
    {
        // 2. Gọi Dịch vụ / Đồ uống (Disconnected Model):
        // Hiển thị danh sách dịch vụ đã đặt bằng SqlDataAdapter + DataTable trên DataGridView
        public DataTable GetDichVuDaDatDisconnected(int maHD)
        {
            string sql = @"
                SELECT ct.MaHD, ct.MaDV, dv.TenDV, ct.SoLuong, ct.DonGia, 
                       (ct.SoLuong * ct.DonGia) AS ThanhTien
                FROM ChiTietDichVu ct
                INNER JOIN DichVu dv ON ct.MaDV = dv.MaDV
                WHERE ct.MaHD = @MaHD";

            using (var conn = DatabaseHelper.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@MaHD", SqlDbType.Int) { Value = maHD });
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt); // Mô hình ngắt kết nối (Disconnected Model)
                        return dt;
                    }
                }
            }
        }

        // Thêm đồ uống/dịch vụ vào phòng (nếu đã có thì cộng dồn số lượng)
        public bool ThemDichVuVaoHoaDon(int maHD, int maDV, int soLuong, decimal donGia)
        {
            string sql = @"
                IF EXISTS (SELECT 1 FROM ChiTietDichVu WHERE MaHD = @MaHD AND MaDV = @MaDV)
                BEGIN
                    UPDATE ChiTietDichVu 
                    SET SoLuong = SoLuong + @SoLuong 
                    WHERE MaHD = @MaHD AND MaDV = @MaDV
                END
                ELSE
                BEGIN
                    INSERT INTO ChiTietDichVu (MaHD, MaDV, SoLuong, DonGia)
                    VALUES (@MaHD, @MaDV, @SoLuong, @DonGia)
                END";

            SqlParameter[] parameters = {
                new SqlParameter("@MaHD", SqlDbType.Int) { Value = maHD },
                new SqlParameter("@MaDV", SqlDbType.Int) { Value = maDV },
                new SqlParameter("@SoLuong", SqlDbType.Int) { Value = soLuong },
                new SqlParameter("@DonGia", SqlDbType.Decimal) { Value = donGia }
            };

            return DatabaseHelper.ExecuteNonQuery(sql, parameters) > 0;
        }
    }

    public class ThanhToanDAL
    {
        // ========================================================
        // 3. Tính Tiền & Thanh Toán (SqlTransaction):
        // 1. Cập nhật GioRa, TongTienPhong, TongTienDichVu, TrangThai = 1 cho HoaDon.
        // 2. Trừ TonKho tương ứng trong bảng DichVu.
        // 3. Cập nhật trạng thái PhongHat về "Trống".
        // 4. Gọi trans.Commit().
        // ========================================================
        public (bool Success, string Message) ThanhToanHoaDonTransaction(
            int maHD, 
            int maPhong, 
            DateTime gioRa, 
            decimal tongTienPhong, 
            decimal tongTienDichVu)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Cập nhật GioRa, TongTienPhong, TongTienDichVu, TrangThai = 1 cho HoaDon
                        string sqlHD = @"
                            UPDATE HoaDon 
                            SET GioRa = @GioRa, 
                                TongTienPhong = @TongTienPhong, 
                                TongTienDichVu = @TongTienDichVu, 
                                TrangThai = 1 
                            WHERE MaHD = @MaHD";

                        SqlCommand cmdHD = new SqlCommand(sqlHD, conn, trans);
                        cmdHD.Parameters.Add(new SqlParameter("@GioRa", SqlDbType.DateTime) { Value = gioRa });
                        cmdHD.Parameters.Add(new SqlParameter("@TongTienPhong", SqlDbType.Decimal) { Value = tongTienPhong });
                        cmdHD.Parameters.Add(new SqlParameter("@TongTienDichVu", SqlDbType.Decimal) { Value = tongTienDichVu });
                        cmdHD.Parameters.Add(new SqlParameter("@MaHD", SqlDbType.Int) { Value = maHD });
                        cmdHD.ExecuteNonQuery();

                        // 2. Trừ TonKho tương ứng trong bảng DichVu
                        // Lấy danh sách dịch vụ đã dùng trong hóa đơn
                        string sqlCT = "SELECT MaDV, SoLuong FROM ChiTietDichVu WHERE MaHD = @MaHD";
                        SqlCommand cmdCT = new SqlCommand(sqlCT, conn, trans);
                        cmdCT.Parameters.Add(new SqlParameter("@MaHD", SqlDbType.Int) { Value = maHD });

                        var listDung = new List<(int MaDV, int SoLuong)>();
                        using (var reader = cmdCT.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listDung.Add((reader.GetInt32(0), reader.GetInt32(1)));
                            }
                        }

                        // Trừ tồn kho cho từng món
                        foreach (var item in listDung)
                        {
                            string sqlTruKho = @"
                                UPDATE DichVu 
                                SET TonKho = TonKho - @SoLuong 
                                WHERE MaDV = @MaDV";
                            SqlCommand cmdTru = new SqlCommand(sqlTruKho, conn, trans);
                            cmdTru.Parameters.Add(new SqlParameter("@SoLuong", SqlDbType.Int) { Value = item.SoLuong });
                            cmdTru.Parameters.Add(new SqlParameter("@MaDV", SqlDbType.Int) { Value = item.MaDV });
                            cmdTru.ExecuteNonQuery();
                        }

                        // 3. Cập nhật trạng thái PhongHat về "Trống"
                        string sqlPhong = "UPDATE PhongHat SET TrangThai = N'Trống' WHERE MaPhong = @MaPhong";
                        SqlCommand cmdPhong = new SqlCommand(sqlPhong, conn, trans);
                        cmdPhong.Parameters.Add(new SqlParameter("@MaPhong", SqlDbType.Int) { Value = maPhong });
                        cmdPhong.ExecuteNonQuery();

                        // 4. Gọi trans.Commit()
                        trans.Commit();
                        return (true, "Thanh toán thành công! Phòng đã được trả về trạng thái 'Trống' và kho dịch vụ đã được khấu trừ.");
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            trans.Rollback();
                        }
                        catch { }
                        return (false, $"[TRANSACTION ROLLBACK] Lỗi thanh toán: {ex.Message}");
                    }
                }
            }
        }
    }
}
