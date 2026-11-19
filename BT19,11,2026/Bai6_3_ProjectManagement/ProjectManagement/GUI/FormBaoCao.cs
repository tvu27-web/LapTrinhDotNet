using System;
using System.Drawing;
using System.Windows.Forms;
using ProjectManagement.BUS;
using ProjectManagement.DTO;

namespace ProjectManagement.GUI
{
    public class FormBaoCao : Form
    {
        private readonly CongViecBUS _congViecBUS = new CongViecBUS();
        private readonly BaoCaoBUS _baoCaoBUS = new BaoCaoBUS();
        private readonly DuAnBUS _duAnBUS = new DuAnBUS();

        private TabControl tabControl = null!;
        private TabPage tabOverdue = null!;
        private TabPage tabThongKe = null!;

        // Tab Overdue controls
        private DataGridView dgvOverdue = null!;
        private Label lblOverdueCount = null!;
        private Button btnRefreshOverdue = null!;

        // Tab Stats controls
        private ComboBox cboDuAnStats = null!;
        private DataGridView dgvStats = null!;
        private Label lblTotalTasks = null!;
        private Label lblCompletedTasks = null!;
        private Label lblCompletionRate = null!;

        public FormBaoCao()
        {
            InitializeComponent();
            LoadOverdueTasks();
            LoadStatsProjects();
            LoadStatistics();
        }

        private void InitializeComponent()
        {
            this.Text = "Báo cáo & Thống kê Tiến độ Dự án";
            this.Size = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

            tabControl = new TabControl { Dock = DockStyle.Fill };
            tabOverdue = new TabPage { Text = "Cảnh báo trễ hạn", Padding = new Padding(10) };
            tabThongKe = new TabPage { Text = "Thống kê hoàn thành", Padding = new Padding(10) };

            tabControl.TabPages.Add(tabOverdue);
            tabControl.TabPages.Add(tabThongKe);

            // ================= TAB 1: OVERDUE =================
            var pnlOverdueTop = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(254, 237, 237) };
            lblOverdueCount = new Label
            {
                Text = "Đang tải danh sách công việc trễ hạn...",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(192, 57, 43),
                Location = new Point(15, 18),
                AutoSize = true
            };
            btnRefreshOverdue = new Button
            {
                Text = "Làm mới",
                Location = new Point(830, 14),
                Size = new Size(110, 32),
                BackColor = Color.FromArgb(192, 57, 43),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRefreshOverdue.Click += (s, e) => LoadOverdueTasks();
            pnlOverdueTop.Controls.Add(lblOverdueCount);
            pnlOverdueTop.Controls.Add(btnRefreshOverdue);

            dgvOverdue = new DataGridView
            {
                Dock = DockStyle.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                RowTemplate = { Height = 28 },
                ColumnHeadersHeight = 32
            };
            dgvOverdue.CellFormatting += DgvOverdue_CellFormatting;

            tabOverdue.Controls.Add(dgvOverdue);
            tabOverdue.Controls.Add(pnlOverdueTop);

            // ================= TAB 2: THONG KE =================
            var pnlStatsTop = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = Color.FromArgb(240, 244, 248) };
            var lblChonDA = new Label { Text = "Chọn dự án thống kê:", Location = new Point(15, 15), AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            cboDuAnStats = new ComboBox { Location = new Point(175, 12), Width = 280, DropDownStyle = ComboBoxStyle.DropDownList };
            cboDuAnStats.SelectedIndexChanged += (s, e) => LoadStatistics();

            // Summary cards
            lblTotalTasks = new Label { Text = "Tổng task: 0", Location = new Point(15, 55), Size = new Size(180, 30), Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.FromArgb(41, 128, 185) };
            lblCompletedTasks = new Label { Text = "Hoàn thành: 0", Location = new Point(210, 55), Size = new Size(180, 30), Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.FromArgb(39, 174, 96) };
            lblCompletionRate = new Label { Text = "Tỷ lệ chung: 0%", Location = new Point(410, 55), Size = new Size(200, 30), Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.FromArgb(142, 68, 173) };

            pnlStatsTop.Controls.AddRange(new Control[] { lblChonDA, cboDuAnStats, lblTotalTasks, lblCompletedTasks, lblCompletionRate });

            dgvStats = new DataGridView
            {
                Dock = DockStyle.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                RowTemplate = { Height = 28 },
                ColumnHeadersHeight = 32
            };

            tabThongKe.Controls.Add(dgvStats);
            tabThongKe.Controls.Add(pnlStatsTop);

            this.Controls.Add(tabControl);
        }

        private void LoadOverdueTasks()
        {
            try
            {
                var list = _congViecBUS.GetOverdueTasks();
                dgvOverdue.DataSource = null;
                dgvOverdue.DataSource = list;

                lblOverdueCount.Text = $"Phát hiện {list.Count} công việc quá hạn chưa hoàn thành:";

                if (dgvOverdue.Columns["MaTask"] != null) { dgvOverdue.Columns["MaTask"].HeaderText = "Mã"; dgvOverdue.Columns["MaTask"].Width = 55; }
                if (dgvOverdue.Columns["TenTask"] != null) { dgvOverdue.Columns["TenTask"].HeaderText = "Tên Task Quá Hạn"; dgvOverdue.Columns["TenTask"].Width = 220; }
                if (dgvOverdue.Columns["TenDuAn"] != null) { dgvOverdue.Columns["TenDuAn"].HeaderText = "Dự Án"; dgvOverdue.Columns["TenDuAn"].Width = 180; }
                if (dgvOverdue.Columns["HanNop"] != null) { dgvOverdue.Columns["HanNop"].HeaderText = "Hạn Nộp Đã Qua"; dgvOverdue.Columns["HanNop"].Width = 130; dgvOverdue.Columns["HanNop"].DefaultCellStyle.Format = "dd/MM/yyyy"; }
                if (dgvOverdue.Columns["DoUuTien"] != null) { dgvOverdue.Columns["DoUuTien"].HeaderText = "Ưu Tiên"; dgvOverdue.Columns["DoUuTien"].Width = 95; }
                if (dgvOverdue.Columns["TrangThai"] != null) { dgvOverdue.Columns["TrangThai"].HeaderText = "Trạng Thái Hiện Tại"; dgvOverdue.Columns["TrangThai"].Width = 150; }
                if (dgvOverdue.Columns["MoTa"] != null) dgvOverdue.Columns["MoTa"].Visible = false;
                if (dgvOverdue.Columns["MaDuAn"] != null) dgvOverdue.Columns["MaDuAn"].Visible = false;
                if (dgvOverdue.Columns["NguoiThucHien"] != null) dgvOverdue.Columns["NguoiThucHien"].Visible = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi lấy danh sách trễ hạn FormBaoCao: " + ex.ToString());
            }
        }

        private void DgvOverdue_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvOverdue.Columns[e.ColumnIndex].Name == "HanNop" && e.Value != null)
            {
                e.CellStyle!.ForeColor = Color.Red;
                e.CellStyle.Font = new Font(dgvOverdue.Font, FontStyle.Bold);
            }
        }

        private void LoadStatsProjects()
        {
            try
            {
                var projects = _duAnBUS.GetAll();
                var list = new System.Collections.Generic.List<DuAnDTO>
                {
                    new DuAnDTO { MaDuAn = 0, TenDuAn = "-- Toàn bộ các dự án --" }
                };
                list.AddRange(projects);
                cboDuAnStats.DisplayMember = "TenDuAn";
                cboDuAnStats.ValueMember = "MaDuAn";
                cboDuAnStats.DataSource = list;
                cboDuAnStats.SelectedIndex = 0;
            }
            catch { }
        }

        private void LoadStatistics()
        {
            try
            {
                int? maDuAn = null;
                if (cboDuAnStats.SelectedValue is int id && id > 0)
                {
                    maDuAn = id;
                }

                // ExecuteScalar-based overall count
                var overall = _baoCaoBUS.GetOverallStats(maDuAn);
                lblTotalTasks.Text = $"Tổng số task: {overall.Total}";
                lblCompletedTasks.Text = $"Hoàn thành: {overall.Completed}";
                lblCompletionRate.Text = $"Tỷ lệ hoàn thành: {overall.Rate}%";

                // Member breakdown with GROUP BY
                var memberStats = _baoCaoBUS.GetMemberCompletionStats(maDuAn);
                dgvStats.DataSource = memberStats;

                if (dgvStats.Columns["MaThanhVien"] != null) { dgvStats.Columns["MaThanhVien"].HeaderText = "Mã TV"; dgvStats.Columns["MaThanhVien"].Width = 60; }
                if (dgvStats.Columns["HoTen"] != null) { dgvStats.Columns["HoTen"].HeaderText = "Họ và Tên"; dgvStats.Columns["HoTen"].Width = 180; }
                if (dgvStats.Columns["ChuyenMon"] != null) { dgvStats.Columns["ChuyenMon"].HeaderText = "Chuyên Môn"; dgvStats.Columns["ChuyenMon"].Width = 150; }
                if (dgvStats.Columns["TongSoTask"] != null) { dgvStats.Columns["TongSoTask"].HeaderText = "Tổng Task Giao"; dgvStats.Columns["TongSoTask"].Width = 120; }
                if (dgvStats.Columns["SoTaskHoanThanh"] != null) { dgvStats.Columns["SoTaskHoanThanh"].HeaderText = "Task Đã Xong"; dgvStats.Columns["SoTaskHoanThanh"].Width = 120; }
                if (dgvStats.Columns["TyLeHoanThanh"] != null) { dgvStats.Columns["TyLeHoanThanh"].HeaderText = "Tỷ Lệ Hoàn Thành (%)"; dgvStats.Columns["TyLeHoanThanh"].Width = 160; }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy số liệu thống kê: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
