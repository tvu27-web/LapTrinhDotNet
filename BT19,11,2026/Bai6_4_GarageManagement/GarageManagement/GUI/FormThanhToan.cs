using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GarageManagement.BUS;
using GarageManagement.DTO;

namespace GarageManagement.GUI
{
    public class FormThanhToan : Form
    {
        private readonly PhieuSuaChuaBUS _phieuBUS = new PhieuSuaChuaBUS();

        private DataGridView dgvPhieu = null!;
        private DataGridView dgvChiTiet = null!;
        private TextBox txtHoaDonPreview = null!;

        private Label lblSelectedPhieu = null!;
        private Label lblTongTien = null!;
        private Button btnThanhToan = null!;
        private Button btnLamMoi = null!;

        private PhieuSuaChuaDTO? _selectedPhieu = null;

        public FormThanhToan()
        {
            InitializeComponent();
            LoadDanhSachPhieu();
        }

        private void InitializeComponent()
        {
            this.Text = "Thanh toán & Hóa đơn Dịch vụ";
            this.Size = new Size(1150, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

            // Header
            var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(39, 174, 96) };
            var lblTitle = new Label
            {
                Text = "THANH TOÁN & XUẤT HÓA ĐƠN SỬA CHỮA",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlHeader.Controls.Add(lblTitle);

            // Split Container
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 620,
                Padding = new Padding(10)
            };

            // Left side: Danh sách phiếu & Chi tiết
            var pnlLeftTop = new GroupBox { Text = "1. Danh sách phiếu sửa chữa", Dock = DockStyle.Top, Height = 280, Padding = new Padding(10) };
            dgvPhieu = new DataGridView
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
            dgvPhieu.CellClick += DgvPhieu_CellClick;
            dgvPhieu.CellFormatting += DgvPhieu_CellFormatting;
            pnlLeftTop.Controls.Add(dgvPhieu);

            var pnlLeftBottom = new GroupBox { Text = "2. Chi tiết Phụ tùng & Tiền công của phiếu đang chọn", Dock = DockStyle.Fill, Padding = new Padding(10) };
            dgvChiTiet = new DataGridView
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
            pnlLeftBottom.Controls.Add(dgvChiTiet);

            split.Panel1.Controls.Add(pnlLeftBottom);
            split.Panel1.Controls.Add(pnlLeftTop);

            // Right side: Action & Invoice Preview
            var pnlRight = new GroupBox { Text = "3. Hóa đơn thanh toán", Dock = DockStyle.Fill, Padding = new Padding(15) };

            lblSelectedPhieu = new Label
            {
                Text = "Chưa chọn phiếu sửa chữa",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                Dock = DockStyle.Top,
                Height = 30
            };

            lblTongTien = new Label
            {
                Text = "TỔNG THANH TOÁN: 0 VNĐ",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(192, 57, 43),
                Dock = DockStyle.Top,
                Height = 40
            };

            var pnlBtn = new Panel { Dock = DockStyle.Top, Height = 55 };
            btnThanhToan = new Button
            {
                Text = "Xác nhận thanh toán",
                Location = new Point(0, 5),
                Size = new Size(200, 42),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold)
            };
            btnThanhToan.Click += BtnThanhToan_Click;

            btnLamMoi = new Button
            {
                Text = "Làm mới",
                Location = new Point(215, 5),
                Size = new Size(110, 42),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLamMoi.Click += (s, e) => LoadDanhSachPhieu();

            pnlBtn.Controls.AddRange(new Control[] { btnThanhToan, btnLamMoi });

            txtHoaDonPreview = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                BackColor = Color.FromArgb(250, 250, 250),
                Font = new Font("Consolas", 10F, FontStyle.Regular, GraphicsUnit.Point),
                ScrollBars = ScrollBars.Vertical
            };

            pnlRight.Controls.Add(txtHoaDonPreview);
            pnlRight.Controls.Add(pnlBtn);
            pnlRight.Controls.Add(lblTongTien);
            pnlRight.Controls.Add(lblSelectedPhieu);

            split.Panel2.Controls.Add(pnlRight);

            this.Controls.Add(split);
            this.Controls.Add(pnlHeader);
        }

        private void LoadDanhSachPhieu()
        {
            try
            {
                var list = _phieuBUS.GetAll();
                dgvPhieu.DataSource = list;

                if (dgvPhieu.Columns["MaPhieu"] != null) { dgvPhieu.Columns["MaPhieu"].HeaderText = "Mã Phiếu"; dgvPhieu.Columns["MaPhieu"].Width = 70; }
                if (dgvPhieu.Columns["BienSo"] != null) { dgvPhieu.Columns["BienSo"].HeaderText = "Biển Số"; dgvPhieu.Columns["BienSo"].Width = 90; }
                if (dgvPhieu.Columns["TenKH"] != null) { dgvPhieu.Columns["TenKH"].HeaderText = "Chủ Xe"; dgvPhieu.Columns["TenKH"].Width = 120; }
                if (dgvPhieu.Columns["TongTien"] != null) { dgvPhieu.Columns["TongTien"].HeaderText = "Tổng Tiền (đ)"; dgvPhieu.Columns["TongTien"].DefaultCellStyle.Format = "N0"; }
                if (dgvPhieu.Columns["TrangThai"] != null) { dgvPhieu.Columns["TrangThai"].HeaderText = "Trạng Thái"; dgvPhieu.Columns["TrangThai"].Width = 100; }
                if (dgvPhieu.Columns["HieuXe"] != null) dgvPhieu.Columns["HieuXe"].Visible = false;
                if (dgvPhieu.Columns["NgayNhan"] != null) dgvPhieu.Columns["NgayNhan"].Visible = false;
                if (dgvPhieu.Columns["NoiDungSua"] != null) dgvPhieu.Columns["NoiDungSua"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách phiếu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvPhieu_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvPhieu.Columns[e.ColumnIndex].Name == "TrangThai" && e.Value != null)
            {
                string st = e.Value.ToString()!;
                if (st == "Đã thanh toán")
                {
                    e.CellStyle!.ForeColor = Color.Green;
                    e.CellStyle.Font = new Font(dgvPhieu.Font, FontStyle.Bold);
                }
                else
                {
                    e.CellStyle!.ForeColor = Color.Red;
                }
            }
        }

        private void DgvPhieu_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvPhieu.Rows[e.RowIndex].DataBoundItem is PhieuSuaChuaDTO phieu)
            {
                _selectedPhieu = phieu;
                lblSelectedPhieu.Text = $"Phiếu #{phieu.MaPhieu} - Xe {phieu.BienSo} ({phieu.TenKH})";

                var details = _phieuBUS.GetDetails(phieu.MaPhieu);
                dgvChiTiet.DataSource = details;

                if (dgvChiTiet.Columns["TenPhuTung"] != null) dgvChiTiet.Columns["TenPhuTung"].HeaderText = "Phụ Tùng";
                if (dgvChiTiet.Columns["SoLuong"] != null) dgvChiTiet.Columns["SoLuong"].HeaderText = "Số Lượng";
                if (dgvChiTiet.Columns["DonGia"] != null) { dgvChiTiet.Columns["DonGia"].HeaderText = "Đơn Giá (đ)"; dgvChiTiet.Columns["DonGia"].DefaultCellStyle.Format = "N0"; }
                if (dgvChiTiet.Columns["TienCong"] != null) { dgvChiTiet.Columns["TienCong"].HeaderText = "Tiền Công (đ)"; dgvChiTiet.Columns["TienCong"].DefaultCellStyle.Format = "N0"; }
                if (dgvChiTiet.Columns["ThanhTien"] != null) { dgvChiTiet.Columns["ThanhTien"].HeaderText = "Thành Tiền (đ)"; dgvChiTiet.Columns["ThanhTien"].DefaultCellStyle.Format = "N0"; }
                if (dgvChiTiet.Columns["MaPhieu"] != null) dgvChiTiet.Columns["MaPhieu"].Visible = false;
                if (dgvChiTiet.Columns["MaPhuTung"] != null) dgvChiTiet.Columns["MaPhuTung"].Visible = false;

                // Tính tổng chi phí = Tổng (Số lượng phụ tùng × Đơn giá) + Tổng tiền công
                decimal total = details.Sum(x => x.ThanhTien);
                lblTongTien.Text = $"TỔNG THANH TOÁN: {total:N0} VNĐ";

                btnThanhToan.Enabled = phieu.TrangThai != "Đã thanh toán";
                btnThanhToan.Text = phieu.TrangThai == "Đã thanh toán" ? "✔ ĐÃ THANH TOÁN" : "💳 XÁC NHẬN THANH TOÁN";

                // Generate text preview of the invoice
                RenderInvoicePreview(phieu, details, total);
            }
        }

        private void RenderInvoicePreview(PhieuSuaChuaDTO phieu, System.Collections.Generic.List<ChiTietSuaChuaDTO> details, decimal total)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("==================================================");
            sb.AppendLine("             GARAGE Ô TÔ CHUYÊN NGHIỆP            ");
            sb.AppendLine("                 HÓA ĐƠN DỊCH VỤ                  ");
            sb.AppendLine("==================================================");
            sb.AppendLine($"Mã phiếu sửa chữa: #{phieu.MaPhieu}");
            sb.AppendLine($"Biển số xe:       {phieu.BienSo} ({phieu.HieuXe})");
            sb.AppendLine($"Chủ xe:           {phieu.TenKH}");
            sb.AppendLine($"Ngày tiếp nhận:   {phieu.NgayNhan:dd/MM/yyyy HH:mm}");
            sb.AppendLine($"Nội dung:         {phieu.NoiDungSua}");
            sb.AppendLine($"Trạng thái:       {phieu.TrangThai}");
            sb.AppendLine("--------------------------------------------------");
            sb.AppendLine(string.Format("{0,-20} {1,4} {2,10} {3,10}", "Hạng mục", "SL", "Tiền công", "Thành tiền"));
            sb.AppendLine("--------------------------------------------------");
            foreach (var d in details)
            {
                string name = d.TenPhuTung.Length > 19 ? d.TenPhuTung.Substring(0, 16) + "..." : d.TenPhuTung;
                sb.AppendLine(string.Format("{0,-20} {1,4} {2,10:N0} {3,10:N0}", name, d.SoLuong, d.TienCong, d.ThanhTien));
            }
            sb.AppendLine("--------------------------------------------------");
            sb.AppendLine($"TỔNG CỘNG THANH TOÁN: {total:N0} VNĐ");
            sb.AppendLine("==================================================");
            sb.AppendLine("      Cảm ơn quý khách và hẹn gặp lại!          ");
            txtHoaDonPreview.Text = sb.ToString();
        }

        private void BtnThanhToan_Click(object? sender, EventArgs e)
        {
            if (_selectedPhieu == null)
            {
                MessageBox.Show("Vui lòng chọn phiếu sửa chữa cần thanh toán!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var details = _phieuBUS.GetDetails(_selectedPhieu.MaPhieu);
            decimal total = details.Sum(x => x.ThanhTien);

            string err = _phieuBUS.ThanhToan(_selectedPhieu.MaPhieu, total);
            if (!string.IsNullOrEmpty(err))
            {
                MessageBox.Show(err, "Lỗi thanh toán", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show($"Thanh toán thành công phiếu #{_selectedPhieu.MaPhieu}!\nTổng tiền đã thanh toán: {total:N0} VNĐ\nTrạng thái đã chuyển sang 'Đã thanh toán'.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadDanhSachPhieu();
        }
    }
}
