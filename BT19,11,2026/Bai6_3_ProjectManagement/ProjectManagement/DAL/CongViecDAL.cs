using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ProjectManagement.DTO;

namespace ProjectManagement.DAL
{
    public class CongViecDAL
    {
        // 1. Lọc danh sách Task theo từng Dự án và theo Trạng thái ("To Do", "In Progress", "Done")
        public List<CongViecDTO> GetTasks(int? maDuAn = null, string? trangThai = null)
        {
            var list = new List<CongViecDTO>();
            string sql = @"
                SELECT c.MaTask, c.MaDuAn, d.TenDuAn, c.TenTask, c.MoTa, c.HanNop, c.DoUuTien, c.TrangThai,
                       STRING_AGG(tv.HoTen, ', ') AS NguoiThucHien
                FROM CongViec c
                INNER JOIN DuAn d ON c.MaDuAn = d.MaDuAn
                LEFT JOIN PhanCong pc ON c.MaTask = pc.MaTask
                LEFT JOIN ThanhVien tv ON pc.MaThanhVien = tv.MaThanhVien
                WHERE (1 = 1) ";

            var parameters = new List<SqlParameter>();

            if (maDuAn.HasValue && maDuAn.Value > 0)
            {
                sql += " AND c.MaDuAn = @MaDuAn ";
                parameters.Add(new SqlParameter("@MaDuAn", SqlDbType.Int) { Value = maDuAn.Value });
            }

            if (!string.IsNullOrEmpty(trangThai) && trangThai != "Tất cả")
            {
                sql += " AND c.TrangThai = @TrangThai ";
                parameters.Add(new SqlParameter("@TrangThai", SqlDbType.NVarChar, 50) { Value = trangThai });
            }

            sql += " GROUP BY c.MaTask, c.MaDuAn, d.TenDuAn, c.TenTask, c.MoTa, c.HanNop, c.DoUuTien, c.TrangThai ORDER BY c.MaTask DESC";

            DataTable dt = DatabaseHelper.ExecuteQuery(sql, parameters.ToArray());
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new CongViecDTO
                {
                    MaTask = Convert.ToInt32(row["MaTask"]),
                    MaDuAn = Convert.ToInt32(row["MaDuAn"]),
                    TenDuAn = row["TenDuAn"].ToString() ?? "",
                    TenTask = row["TenTask"].ToString() ?? "",
                    MoTa = row["MoTa"]?.ToString() ?? "",
                    HanNop = row["HanNop"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["HanNop"]),
                    DoUuTien = row["DoUuTien"]?.ToString() ?? "Trung bình",
                    TrangThai = row["TrangThai"]?.ToString() ?? "To Do",
                    NguoiThucHien = row["NguoiThucHien"] == DBNull.Value ? "Chưa giao" : row["NguoiThucHien"].ToString()!
                });
            }
            return list;
        }

        public int Insert(CongViecDTO task)
        {
            string sql = @"INSERT INTO CongViec (MaDuAn, TenTask, MoTa, HanNop, DoUuTien, TrangThai)
                           OUTPUT INSERTED.MaTask
                           VALUES (@MaDuAn, @TenTask, @MoTa, @HanNop, @DoUuTien, @TrangThai)";
            SqlParameter[] parameters = {
                new SqlParameter("@MaDuAn", SqlDbType.Int) { Value = task.MaDuAn },
                new SqlParameter("@TenTask", SqlDbType.NVarChar, 200) { Value = task.TenTask },
                new SqlParameter("@MoTa", SqlDbType.NVarChar) { Value = (object?)task.MoTa ?? DBNull.Value },
                new SqlParameter("@HanNop", SqlDbType.DateTime) { Value = (object?)task.HanNop ?? DBNull.Value },
                new SqlParameter("@DoUuTien", SqlDbType.NVarChar, 20) { Value = task.DoUuTien },
                new SqlParameter("@TrangThai", SqlDbType.NVarChar, 50) { Value = task.TrangThai }
            };
            object? result = DatabaseHelper.ExecuteScalar(sql, parameters);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public bool Update(CongViecDTO task)
        {
            string sql = @"UPDATE CongViec
                           SET MaDuAn = @MaDuAn, TenTask = @TenTask, MoTa = @MoTa, HanNop = @HanNop, 
                               DoUuTien = @DoUuTien, TrangThai = @TrangThai
                           WHERE MaTask = @MaTask";
            SqlParameter[] parameters = {
                new SqlParameter("@MaTask", SqlDbType.Int) { Value = task.MaTask },
                new SqlParameter("@MaDuAn", SqlDbType.Int) { Value = task.MaDuAn },
                new SqlParameter("@TenTask", SqlDbType.NVarChar, 200) { Value = task.TenTask },
                new SqlParameter("@MoTa", SqlDbType.NVarChar) { Value = (object?)task.MoTa ?? DBNull.Value },
                new SqlParameter("@HanNop", SqlDbType.DateTime) { Value = (object?)task.HanNop ?? DBNull.Value },
                new SqlParameter("@DoUuTien", SqlDbType.NVarChar, 20) { Value = task.DoUuTien },
                new SqlParameter("@TrangThai", SqlDbType.NVarChar, 50) { Value = task.TrangThai }
            };
            return DatabaseHelper.ExecuteNonQuery(sql, parameters) > 0;
        }

        // 2. Chuyển trạng thái công việc qua lệnh Parameterized SQL Update
        public bool UpdateStatus(int maTask, string newStatus)
        {
            string sql = "UPDATE CongViec SET TrangThai = @TrangThai WHERE MaTask = @MaTask";
            SqlParameter[] parameters = {
                new SqlParameter("@TrangThai", SqlDbType.NVarChar, 50) { Value = newStatus },
                new SqlParameter("@MaTask", SqlDbType.Int) { Value = maTask }
            };
            return DatabaseHelper.ExecuteNonQuery(sql, parameters) > 0;
        }

        public bool Delete(int maTask)
        {
            string sql = "DELETE FROM CongViec WHERE MaTask = @MaTask";
            SqlParameter[] parameters = {
                new SqlParameter("@MaTask", SqlDbType.Int) { Value = maTask }
            };
            return DatabaseHelper.ExecuteNonQuery(sql, parameters) > 0;
        }

        // 3. Cảnh báo trễ hạn (Overdue): Dùng SqlDataReader hiển thị các công việc quá HanNop mà TrangThai != 'Done'
        public List<CongViecDTO> GetOverdueTasksUsingDataReader()
        {
            var list = new List<CongViecDTO>();
            string sql = @"
                SELECT c.MaTask, c.MaDuAn, d.TenDuAn, c.TenTask, c.MoTa, c.HanNop, c.DoUuTien, c.TrangThai
                FROM CongViec c
                INNER JOIN DuAn d ON c.MaDuAn = d.MaDuAn
                WHERE c.HanNop < GETDATE() AND c.TrangThai != N'Done'
                ORDER BY c.HanNop ASC";

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new CongViecDTO
                            {
                                MaTask = reader.GetInt32(reader.GetOrdinal("MaTask")),
                                MaDuAn = reader.GetInt32(reader.GetOrdinal("MaDuAn")),
                                TenDuAn = reader.GetString(reader.GetOrdinal("TenDuAn")),
                                TenTask = reader.GetString(reader.GetOrdinal("TenTask")),
                                MoTa = reader.IsDBNull(reader.GetOrdinal("MoTa")) ? "" : reader.GetString(reader.GetOrdinal("MoTa")),
                                HanNop = reader.IsDBNull(reader.GetOrdinal("HanNop")) ? null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("HanNop")),
                                DoUuTien = reader.IsDBNull(reader.GetOrdinal("DoUuTien")) ? "Trung bình" : reader.GetString(reader.GetOrdinal("DoUuTien")),
                                TrangThai = reader.GetString(reader.GetOrdinal("TrangThai"))
                            });
                        }
                    }
                }
            }
            return list;
        }
    }

    public class PhanCongDAL
    {
        public bool Assign(int maTask, int maThanhVien)
        {
            string sql = @"
                IF NOT EXISTS (SELECT 1 FROM PhanCong WHERE MaTask = @MaTask AND MaThanhVien = @MaThanhVien)
                BEGIN
                    INSERT INTO PhanCong (MaTask, MaThanhVien, NgayGiao) VALUES (@MaTask, @MaThanhVien, GETDATE())
                END";
            SqlParameter[] parameters = {
                new SqlParameter("@MaTask", SqlDbType.Int) { Value = maTask },
                new SqlParameter("@MaThanhVien", SqlDbType.Int) { Value = maThanhVien }
            };
            return DatabaseHelper.ExecuteNonQuery(sql, parameters) >= 0;
        }

        public bool Remove(int maTask, int maThanhVien)
        {
            string sql = "DELETE FROM PhanCong WHERE MaTask = @MaTask AND MaThanhVien = @MaThanhVien";
            SqlParameter[] parameters = {
                new SqlParameter("@MaTask", SqlDbType.Int) { Value = maTask },
                new SqlParameter("@MaThanhVien", SqlDbType.Int) { Value = maThanhVien }
            };
            return DatabaseHelper.ExecuteNonQuery(sql, parameters) > 0;
        }

        public List<int> GetAssignedMemberIds(int maTask)
        {
            var list = new List<int>();
            string sql = "SELECT MaThanhVien FROM PhanCong WHERE MaTask = @MaTask";
            SqlParameter[] parameters = {
                new SqlParameter("@MaTask", SqlDbType.Int) { Value = maTask }
            };
            DataTable dt = DatabaseHelper.ExecuteQuery(sql, parameters);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(Convert.ToInt32(row["MaThanhVien"]));
            }
            return list;
        }
    }

    public class BaoCaoDAL
    {
        // Thống kê tỷ lệ % hoàn thành công việc của từng thành viên trong dự án bằng GROUP BY và ExecuteScalar
        public List<ThongKeThanhVienDTO> GetMemberCompletionStats(int? maDuAn = null)
        {
            var list = new List<ThongKeThanhVienDTO>();
            // Lấy danh sách thành viên cùng số task phân công bằng GROUP BY
            string sql = @"
                SELECT tv.MaThanhVien, tv.HoTen, tv.ChuyenMon,
                       COUNT(c.MaTask) AS TongSoTask,
                       SUM(CASE WHEN c.TrangThai = N'Done' THEN 1 ELSE 0 END) AS SoTaskHoanThanh
                FROM ThanhVien tv
                LEFT JOIN PhanCong pc ON tv.MaThanhVien = pc.MaThanhVien
                LEFT JOIN CongViec c ON pc.MaTask = c.MaTask " +
                (maDuAn.HasValue && maDuAn.Value > 0 ? " AND c.MaDuAn = @MaDuAn " : "") +
                @"GROUP BY tv.MaThanhVien, tv.HoTen, tv.ChuyenMon
                ORDER BY HoTen ASC";

            SqlParameter[]? parameters = maDuAn.HasValue && maDuAn.Value > 0
                ? new[] { new SqlParameter("@MaDuAn", SqlDbType.Int) { Value = maDuAn.Value } }
                : null;

            DataTable dt = DatabaseHelper.ExecuteQuery(sql, parameters);
            foreach (DataRow row in dt.Rows)
            {
                int tong = Convert.ToInt32(row["TongSoTask"]);
                int xong = Convert.ToInt32(row["SoTaskHoanThanh"]);
                double tyLe = tong > 0 ? Math.Round((double)xong / tong * 100, 2) : 0;

                list.Add(new ThongKeThanhVienDTO
                {
                    MaThanhVien = Convert.ToInt32(row["MaThanhVien"]),
                    HoTen = row["HoTen"].ToString() ?? "",
                    ChuyenMon = row["ChuyenMon"]?.ToString() ?? "",
                    TongSoTask = tong,
                    SoTaskHoanThanh = xong,
                    TyLeHoanThanh = tyLe
                });
            }
            return list;
        }

        // Minh họa việc dùng ExecuteScalar để đếm tổng số task hoặc tính tỷ lệ theo yêu cầu
        public int GetTotalTasksCount(int? maDuAn = null)
        {
            string sql = "SELECT COUNT(*) FROM CongViec " + (maDuAn.HasValue && maDuAn.Value > 0 ? "WHERE MaDuAn = @MaDuAn" : "");
            SqlParameter[]? parameters = maDuAn.HasValue && maDuAn.Value > 0
                ? new[] { new SqlParameter("@MaDuAn", SqlDbType.Int) { Value = maDuAn.Value } }
                : null;
            object? res = DatabaseHelper.ExecuteScalar(sql, parameters);
            return res != null ? Convert.ToInt32(res) : 0;
        }

        public int GetCompletedTasksCount(int? maDuAn = null)
        {
            string sql = "SELECT COUNT(*) FROM CongViec WHERE TrangThai = N'Done' " + (maDuAn.HasValue && maDuAn.Value > 0 ? "AND MaDuAn = @MaDuAn" : "");
            SqlParameter[]? parameters = maDuAn.HasValue && maDuAn.Value > 0
                ? new[] { new SqlParameter("@MaDuAn", SqlDbType.Int) { Value = maDuAn.Value } }
                : null;
            object? res = DatabaseHelper.ExecuteScalar(sql, parameters);
            return res != null ? Convert.ToInt32(res) : 0;
        }
    }
}
