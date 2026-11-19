using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using KaraokeManagement.BUS;
using KaraokeManagement.DTO;

namespace KaraokeManagement.GUI
{
    public class FormKaraokeMain : Form
    {
        private readonly PhongHatBUS _phongBUS = new PhongHatBUS();
        private readonly DichVuBUS _dichVuBUS = new DichVuBUS();
        private readonly HoaDonBUS _hoaDonBUS = new HoaDonBUS();
        private readonly ChiTietDichVuBUS _chiTietBUS = new ChiTietDichVuBUS();
        private readonly ThanhToanBUS _thanhToanBUS = new ThanhToanBUS();

        // 1. Sơ đồ phòng hát (Visual Room Grid)
        private FlowLayoutPanel flpDanhSachPhong = null!;
        private PhongHatDTO? _selectedPhong = null;
        private HoaDonDTO? _currentHoaDon = null;

        // Header & Selected Room Info
        private Label lblSelectedPhongInfo = null!;
        private Label lblThoiGianHat = null!;
        private Button btnMoPhong = null!;
        private System.Windows.Forms.Timer timerClock = null!;

        // 2. Gọi dịch vụ (Disconnected Model: SqlDataAdapter + DataTable)
        private DataGridView dgvDichVuDaDat = null!;
        private DataTable? _dtDichVuDaDat = null;
        private ComboBox cboDichVu = null!;
        private NumericUpDown nudSoLuongDV = null!;
        private Button btnThemDichVu = null!;

        // 3. Thanh toán (SqlTransaction)
        private Label lblTongTienDichVu = null!;
        private Label lblTongTienPhong = null!;
        private Label lblTongThanhToan = null!;
        private Button btnThanhToan = null!;

        public FormKaraokeMain()
        {
            InitializeComponent();
            LoadVisualRoomGrid();
            LoadDichVuComboBox();
            InitTimer();
        }

        private void InitializeComponent()
        {
            this.Text = "Hệ thống Quản lý Phòng hát & Dịch vụ Karaoke";
            this.Size = new Size(1250, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

            // Header Top
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 65,
                BackColor = Color.FromArgb(44, 62, 80),
                Padding = new Padding(15)
            };
            var lblTitle = new Label
            {
                Text = "QUẢN LÝ PHÒNG HÁT & THANH TOÁN KARAOKE",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                Dock = DockStyle.Left,
                AutoSize = true
            };
            var lblGhiChu = new Label
            {
                Text = "🟢 Xanh: Phòng Trống   |   🔴 Đỏ: Phòng Đang Hát",
                ForeColor = Color.Yellow,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Dock = DockStyle.Right,
                AutoSize = true
            };
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblGhiChu);

            // Left Side: Visual Room Grid Layout (FlowLayoutPanel)
            var pnlLeftRooms = new GroupBox
            {
                Text = "SƠ ĐỒ PHÒNG HÁT",
                Dock = DockStyle.Left,
                Width = 520,
                Padding = new Padding(10)
            };

            flpDanhSachPhong = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(240, 243, 244)
            };
            pnlLeftRooms.Controls.Add(flpDanhSachPhong);

            // Right Side: Room Management, Service Ordering & Checkout
            var pnlRight = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            // Box 1: Thông tin phòng chọn & Mở phòng
            var gbPhongInfo = new GroupBox
            {
                Text = "1. Trạng thái phòng & Mở phòng",
                Dock = DockStyle.Top,
                Height = 120,
                Padding = new Padding(10)
            };

            lblSelectedPhongInfo = new Label
            {
                Text = "Vui lòng bấm chọn một phòng từ sơ đồ bên trái",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                Location = new Point(15, 25),
                Size = new Size(420, 28)
            };

            lblThoiGianHat = new Label
            {
                Text = "Giờ vào: --:--   |   Thời gian: 0 phút",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                Location = new Point(15, 60),
                Size = new Size(420, 25)
            };

            btnMoPhong = new Button
            {
                Text = "Mở phòng",
                Location = new Point(460, 30),
                Size = new Size(180, 48),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Enabled = false
            };
            btnMoPhong.Click += BtnMoPhong_Click;

            gbPhongInfo.Controls.AddRange(new Control[] { lblSelectedPhongInfo, lblThoiGianHat, btnMoPhong });

            // Box 2: Gọi dịch vụ / Đồ uống
            var gbDichVu = new GroupBox
            {
                Text = "2. Gọi dịch vụ & Đồ uống",
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            var pnlAddDV = new Panel { Dock = DockStyle.Top, Height = 55, BackColor = Color.FromArgb(245, 247, 250) };
            var lblChonDV = new Label { Text = "Mặt hàng:", Location = new Point(10, 15), AutoSize = true, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            cboDichVu = new ComboBox { Location = new Point(85, 12), Width = 230, DropDownStyle = ComboBoxStyle.DropDownList };

            var lblSL = new Label { Text = "SL:", Location = new Point(325, 15), AutoSize = true };
            nudSoLuongDV = new NumericUpDown { Location = new Point(355, 12), Width = 60, Minimum = 1, Maximum = 100, Value = 1 };

            btnThemDichVu = new Button
            {
                Text = "Thêm dịch vụ",
                Location = new Point(430, 9),
                Size = new Size(140, 32),
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Enabled = false
            };
            btnThemDichVu.Click += BtnThemDichVu_Click;

            pnlAddDV.Controls.AddRange(new Control[] { lblChonDV, cboDichVu, lblSL, nudSoLuongDV, btnThemDichVu });

            dgvDichVuDaDat = new DataGridView
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

            gbDichVu.Controls.Add(dgvDichVuDaDat);
            gbDichVu.Controls.Add(pnlAddDV);

            // Box 3: Thanh toán
            var gbThanhToan = new GroupBox
            {
                Text = "3. Tính tiền & Thanh toán",
                Dock = DockStyle.Bottom,
                Height = 150,
                Padding = new Padding(15)
            };

            lblTongTienPhong = new Label { Text = "Tiền phòng: 0 VNĐ", Location = new Point(15, 25), Size = new Size(250, 25), Font = new Font("Segoe UI", 10.5F, FontStyle.Bold) };
            lblTongTienDichVu = new Label { Text = "Tiền dịch vụ: 0 VNĐ", Location = new Point(15, 55), Size = new Size(250, 25), Font = new Font("Segoe UI", 10.5F, FontStyle.Bold) };
            lblTongThanhToan = new Label { Text = "TỔNG CỘNG: 0 VNĐ", Location = new Point(15, 90), Size = new Size(350, 35), Font = new Font("Segoe UI", 14F, FontStyle.Bold), ForeColor = Color.FromArgb(192, 57, 43) };

            btnThanhToan = new Button
            {
                Text = "THANH TOÁN",
                Location = new Point(380, 40),
                Size = new Size(220, 60),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Enabled = false
            };
            btnThanhToan.Click += BtnThanhToan_Click;

            gbThanhToan.Controls.AddRange(new Control[] { lblTongTienPhong, lblTongTienDichVu, lblTongThanhToan, btnThanhToan });

            pnlRight.Controls.Add(gbDichVu);
            pnlRight.Controls.Add(gbPhongInfo);
            pnlRight.Controls.Add(gbThanhToan);

            this.Controls.Add(pnlRight);
            this.Controls.Add(pnlLeftRooms);
            this.Controls.Add(pnlHeader);
        }

        private void InitTimer()
        {
            timerClock = new System.Windows.Forms.Timer { Interval = 30000 }; // 30s cập nhật thời gian
            timerClock.Tick += (s, e) => UpdateLiveDuration();
            timerClock.Start();
        }

        // 1. Sơ đồ phòng hát & Nút động (Visual Room Grid)
        public void LoadVisualRoomGrid()
        {
            flpDanhSachPhong.Controls.Clear();
            var phongList = _phongBUS.GetAll();

            foreach (var p in phongList)
            {
                var btn = new Button
                {
                    Size = new Size(150, 110),
                    Margin = new Padding(8),
                    FlatStyle = FlatStyle.Flat,
                    Tag = p,
                    Cursor = Cursors.Hand
                };

                // Trạng thái: "Trống" màu xanh, "Đang hát" màu đỏ (theo đúng yêu cầu PDF)
                bool isTrong = (p.TrangThai == "Trống");
                btn.BackColor = isTrong ? Color.FromArgb(39, 174, 96) : Color.FromArgb(231, 76, 60);
                btn.ForeColor = Color.White;

                string icon = isTrong ? "🟢" : "🔴";
                btn.Text = $"{icon} {p.TenPhong}\n({p.LoaiPhong})\n{p.GiaTheoGio:N0} đ/h\n[{p.TrangThai}]";
                btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

                btn.Click += BtnRoom_Click;
                flpDanhSachPhong.Controls.Add(btn);
            }
        }

        private void LoadDichVuComboBox()
        {
            try
            {
                var listDV = _dichVuBUS.GetAll();
                cboDichVu.DataSource = listDV;
                cboDichVu.DisplayMember = "TenDV";
                cboDichVu.ValueMember = "MaDV";
            }
            catch { }
        }

        private void BtnRoom_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is PhongHatDTO p)
            {
                SelectRoom(p);
            }
        }

        private void SelectRoom(PhongHatDTO p)
        {
            _selectedPhong = p;
            lblSelectedPhongInfo.Text = $"{p.TenPhong} ({p.LoaiPhong}) - Giá: {p.GiaTheoGio:N0} đ/giờ - Trạng thái: {p.TrangThai}";

            if (p.TrangThai == "Trống")
            {
                _currentHoaDon = null;
                btnMoPhong.Enabled = true;
                btnThemDichVu.Enabled = false;
                btnThanhToan.Enabled = false;
                lblThoiGianHat.Text = "Phòng hiện đang TRỐNG. Nhấn 'MỞ PHÒNG' để nhận khách!";
                dgvDichVuDaDat.DataSource = null;
                lblTongTienPhong.Text = "Tiền phòng: 0 VNĐ";
                lblTongTienDichVu.Text = "Tiền dịch vụ: 0 VNĐ";
                lblTongThanhToan.Text = "TỔNG CỘNG: 0 VNĐ";
            }
            else // "Đang hát"
            {
                btnMoPhong.Enabled = false;
                btnThemDichVu.Enabled = true;
                btnThanhToan.Enabled = true;

                // Nạp hóa đơn chưa thanh toán
                _currentHoaDon = _hoaDonBUS.GetHoaDonChuaThanhToan(p.MaPhong);
                if (_currentHoaDon != null)
                {
                    UpdateLiveDuration();
                    LoadDichVuDaDatDisconnected(_currentHoaDon.MaHD);
                }
            }
        }

        private void UpdateLiveDuration()
        {
            if (_selectedPhong != null && _selectedPhong.TrangThai == "Đang hát" && _currentHoaDon != null)
            {
                TimeSpan span = DateTime.Now - _currentHoaDon.GioVao;
                int hours = (int)span.TotalHours;
                int minutes = span.Minutes;
                lblThoiGianHat.Text = $"Giờ vào: {_currentHoaDon.GioVao:HH:mm:ss}   |   Đã hát: {hours} giờ {minutes} phút";

                // Tạm tính tiền phòng
                double soGio = Math.Round(span.TotalHours, 2);
                if (soGio < 0.1) soGio = 0.1;
                decimal tienPhong = Math.Round((decimal)soGio * _currentHoaDon.GiaTheoGio, 0);
                lblTongTienPhong.Text = $"Tiền phòng: {tienPhong:N0} VNĐ ({soGio:N2}h)";

                CalculateTotal();
            }
        }

        // 2. Gọi dịch vụ / Đồ uống (Disconnected Model)
        private void LoadDichVuDaDatDisconnected(int maHD)
        {
            try
            {
                _dtDichVuDaDat = _chiTietBUS.GetDichVuDaDat(maHD);
                dgvDichVuDaDat.DataSource = _dtDichVuDaDat;

                if (dgvDichVuDaDat.Columns["TenDV"] != null) { dgvDichVuDaDat.Columns["TenDV"].HeaderText = "Tên Dịch Vụ / Đồ Uống"; dgvDichVuDaDat.Columns["TenDV"].Width = 200; }
                if (dgvDichVuDaDat.Columns["SoLuong"] != null) { dgvDichVuDaDat.Columns["SoLuong"].HeaderText = "Số Lượng"; dgvDichVuDaDat.Columns["SoLuong"].Width = 90; }
                if (dgvDichVuDaDat.Columns["DonGia"] != null) { dgvDichVuDaDat.Columns["DonGia"].HeaderText = "Đơn Giá (đ)"; dgvDichVuDaDat.Columns["DonGia"].DefaultCellStyle.Format = "N0"; }
                if (dgvDichVuDaDat.Columns["ThanhTien"] != null) { dgvDichVuDaDat.Columns["ThanhTien"].HeaderText = "Thành Tiền (đ)"; dgvDichVuDaDat.Columns["ThanhTien"].DefaultCellStyle.Format = "N0"; }
                if (dgvDichVuDaDat.Columns["MaHD"] != null) dgvDichVuDaDat.Columns["MaHD"].Visible = false;
                if (dgvDichVuDaDat.Columns["MaDV"] != null) dgvDichVuDaDat.Columns["MaDV"].Visible = false;

                CalculateTotal();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nạp dịch vụ ngắt kết nối: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CalculateTotal()
        {
            decimal tienDV = 0;
            if (_dtDichVuDaDat != null)
            {
                foreach (DataRow row in _dtDichVuDaDat.Rows)
                {
                    if (row["ThanhTien"] != DBNull.Value)
                    {
                        tienDV += Convert.ToDecimal(row["ThanhTien"]);
                    }
                }
            }
            lblTongTienDichVu.Text = $"Tiền dịch vụ: {tienDV:N0} VNĐ";

            // Tổng
            decimal tienPhong = 0;
            if (_selectedPhong != null && _currentHoaDon != null)
            {
                TimeSpan span = DateTime.Now - _currentHoaDon.GioVao;
                double soGio = Math.Round(span.TotalHours, 2);
                if (soGio < 0.1) soGio = 0.1;
                tienPhong = Math.Round((decimal)soGio * _currentHoaDon.GiaTheoGio, 0);
            }

            decimal tong = tienPhong + tienDV;
            lblTongThanhToan.Text = $"TỔNG CỘNG: {tong:N0} VNĐ";
        }

        // Bấm "Mở phòng" -> Tạo HoaDon mới ghi nhận GioVao = GetDate(), đổi TrangThai sang "Đang hát"
        private void BtnMoPhong_Click(object? sender, EventArgs e)
        {
            if (_selectedPhong == null) return;

            try
            {
                int newMaHD = _hoaDonBUS.MoPhong(_selectedPhong.MaPhong);
                MessageBox.Show($"Mở phòng '{_selectedPhong.TenPhong}' thành công!\nMã hóa đơn #{newMaHD} đã được tạo với GioVao = {DateTime.Now:HH:mm:ss}.", "Mở phòng thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadVisualRoomGrid();
                _selectedPhong.TrangThai = "Đang hát";
                SelectRoom(_selectedPhong);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi mở phòng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnThemDichVu_Click(object? sender, EventArgs e)
        {
            if (_currentHoaDon == null)
            {
                MessageBox.Show("Phòng chưa có hóa đơn đang hoạt động!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!(cboDichVu.SelectedItem is DichVuDTO dv))
            {
                MessageBox.Show("Vui lòng chọn dịch vụ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int soLuong = (int)nudSoLuongDV.Value;
            string err = _chiTietBUS.ThemDichVu(_currentHoaDon.MaHD, dv.MaDV, soLuong, dv.DonGia);
            if (!string.IsNullOrEmpty(err))
            {
                MessageBox.Show(err, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Tải lại DataTable ngắt kết nối
            LoadDichVuDaDatDisconnected(_currentHoaDon.MaHD);
        }

        // 3. Tính tiền & Thanh toán (SqlTransaction)
        private void BtnThanhToan_Click(object? sender, EventArgs e)
        {
            if (_selectedPhong == null || _currentHoaDon == null)
            {
                MessageBox.Show("Không có thông tin phòng hoặc hóa đơn cần thanh toán!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal tienDV = 0;
            if (_dtDichVuDaDat != null)
            {
                foreach (DataRow row in _dtDichVuDaDat.Rows)
                {
                    if (row["ThanhTien"] != DBNull.Value)
                    {
                        tienDV += Convert.ToDecimal(row["ThanhTien"]);
                    }
                }
            }

            if (MessageBox.Show($"Xác nhận thanh toán cho '{_selectedPhong.TenPhong}'?\nHệ thống sẽ mở SqlTransaction để cập nhật GioRa, trừ tồn kho dịch vụ và trả phòng về 'Trống'.", "Xác nhận thanh toán", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            var (ok, msg, tienPhong, tongDV, tongCong, soGio) = _thanhToanBUS.TinhTienVaThanhToan(_currentHoaDon, tienDV);

            if (ok)
            {
                string billText = 
                    $"================================================\n" +
                    $"           HÓA ĐƠN THANH TOÁN KARAOKE           \n" +
                    $"================================================\n" +
                    $"Phòng hát:       {_selectedPhong.TenPhong} ({_selectedPhong.LoaiPhong})\n" +
                    $"Đơn giá giờ:     {_selectedPhong.GiaTheoGio:N0} VNĐ/h\n" +
                    $"Giờ vào:         {_currentHoaDon.GioVao:dd/MM/yyyy HH:mm:ss}\n" +
                    $"Giờ ra:          {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n" +
                    $"Thời gian tính:  {soGio:N2} giờ\n" +
                    $"------------------------------------------------\n" +
                    $"Tiền phòng:      {tienPhong:N0} VNĐ\n" +
                    $"Tiền dịch vụ:    {tongDV:N0} VNĐ\n" +
                    $"------------------------------------------------\n" +
                    $"TỔNG THANH TOÁN: {tongCong:N0} VNĐ\n" +
                    $"================================================\n" +
                    $"Trạng thái: ĐÃ THANH TOÁN (SqlTransaction Commit)";

                MessageBox.Show(billText, "GIAO DỊCH HOÀN TẤT THÀNH CÔNG", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Cập nhật lại sơ đồ phòng: phòng chuyển về Xanh "Trống"
                LoadVisualRoomGrid();
                _selectedPhong.TrangThai = "Trống";
                SelectRoom(_selectedPhong);
            }
            else
            {
                MessageBox.Show(msg, "Lỗi thanh toán Transaction", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
