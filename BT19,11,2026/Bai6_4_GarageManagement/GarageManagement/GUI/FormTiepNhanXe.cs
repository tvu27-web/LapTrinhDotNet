using System;
using System.Drawing;
using System.Windows.Forms;
using GarageManagement.BUS;
using GarageManagement.DTO;

namespace GarageManagement.GUI
{
    public class FormTiepNhanXe : Form
    {
        private readonly XeBUS _xeBUS = new XeBUS();
        private readonly KhachHangBUS _khachHangBUS = new KhachHangBUS();

        private TextBox txtTraCuuBienSo = null!;
        private Button btnTraCuu = null!;
        private Label lblKetQua = null!;

        // Khach Hang controls
        private TextBox txtTenKH = null!;
        private TextBox txtDienThoai = null!;
        private TextBox txtDiaChi = null!;

        // Xe controls
        private TextBox txtBienSo = null!;
        private TextBox txtHieuXe = null!;
        private NumericUpDown nudNamSX = null!;

        private Button btnLuuXeMoi = null!;
        private Button btnLamMoi = null!;
        private DataGridView dgvDanhSachXe = null!;

        public FormTiepNhanXe()
        {
            InitializeComponent();
            LoadDanhSachXe();
        }

        private void InitializeComponent()
        {
            this.Text = "Tiếp nhận Xe & Quản lý Chủ xe";
            this.Size = new Size(1050, 650);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

            // Header Tra cứu
            var pnlSearch = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(41, 128, 185),
                Padding = new Padding(20, 15, 20, 15)
            };

            var lblSearch = new Label
            {
                Text = "Biển số xe cần tra cứu:",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            txtTraCuuBienSo = new TextBox { Location = new Point(210, 18), Width = 180, CharacterCasing = CharacterCasing.Upper };
            btnTraCuu = new Button
            {
                Text = "Tra cứu",
                Location = new Point(405, 15),
                Size = new Size(110, 32),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            btnTraCuu.Click += BtnTraCuu_Click;

            lblKetQua = new Label
            {
                Text = "Nhập biển số và bấm 'Tra cứu'",
                ForeColor = Color.Yellow,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Italic),
                Location = new Point(530, 22),
                AutoSize = true
            };

            pnlSearch.Controls.AddRange(new Control[] { lblSearch, txtTraCuuBienSo, btnTraCuu, lblKetQua });

            // Left Panel Input (Customer & Vehicle)
            var pnlInput = new GroupBox
            {
                Text = "Thông tin tiếp nhận xe",
                Dock = DockStyle.Left,
                Width = 380,
                Padding = new Padding(12)
            };

            int y = 25;
            var lblSection1 = new Label { Text = "1. THÔNG TIN CHỦ XE", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(41, 128, 185), Location = new Point(12, y), AutoSize = true };
            y += 25;

            var lblTen = new Label { Text = "Họ và tên chủ xe (*):", Location = new Point(12, y), AutoSize = true };
            txtTenKH = new TextBox { Location = new Point(12, y + 22), Width = 340 };
            y += 55;

            var lblDT = new Label { Text = "Số điện thoại (*):", Location = new Point(12, y), AutoSize = true };
            txtDienThoai = new TextBox { Location = new Point(12, y + 22), Width = 340 };
            y += 55;

            var lblDC = new Label { Text = "Địa chỉ liên hệ:", Location = new Point(12, y), AutoSize = true };
            txtDiaChi = new TextBox { Location = new Point(12, y + 22), Width = 340 };
            y += 65;

            var lblSection2 = new Label { Text = "2. THÔNG TIN Ô TÔ", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(41, 128, 185), Location = new Point(12, y), AutoSize = true };
            y += 25;

            var lblBS = new Label { Text = "Biển số xe (*):", Location = new Point(12, y), AutoSize = true };
            txtBienSo = new TextBox { Location = new Point(12, y + 22), Width = 340, CharacterCasing = CharacterCasing.Upper };
            y += 55;

            var lblHX = new Label { Text = "Hiệu xe (*):", Location = new Point(12, y), AutoSize = true };
            txtHieuXe = new TextBox { Location = new Point(12, y + 22), Width = 340 };
            y += 55;

            var lblNam = new Label { Text = "Năm sản xuất:", Location = new Point(12, y), AutoSize = true };
            nudNamSX = new NumericUpDown { Location = new Point(12, y + 22), Width = 340, Minimum = 1990, Maximum = 2030, Value = DateTime.Now.Year };
            y += 65;

            btnLuuXeMoi = new Button
            {
                Text = "Lưu tiếp nhận",
                Location = new Point(12, y),
                Size = new Size(160, 36),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            btnLuuXeMoi.Click += BtnLuuXeMoi_Click;

            btnLamMoi = new Button
            {
                Text = "Làm mới",
                Location = new Point(185, y),
                Size = new Size(120, 36),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLamMoi.Click += (s, e) => ResetFields();

            pnlInput.Controls.AddRange(new Control[] {
                lblSection1, lblTen, txtTenKH, lblDT, txtDienThoai, lblDC, txtDiaChi,
                lblSection2, lblBS, txtBienSo, lblHX, txtHieuXe, lblNam, nudNamSX,
                btnLuuXeMoi, btnLamMoi
            });

            // Center Panel Grid
            var pnlGrid = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            dgvDanhSachXe = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false
            };
            dgvDanhSachXe.CellClick += DgvDanhSachXe_CellClick;
            pnlGrid.Controls.Add(dgvDanhSachXe);

            this.Controls.Add(pnlGrid);
            this.Controls.Add(pnlInput);
            this.Controls.Add(pnlSearch);
        }

        public void SelectRow(int index)
        {
            if (dgvDanhSachXe.Rows.Count > index && index >= 0)
            {
                dgvDanhSachXe.ClearSelection();
                dgvDanhSachXe.Rows[index].Selected = true;
                if (dgvDanhSachXe.Rows[index].DataBoundItem is XeDTO xe)
                {
                    txtBienSo.Text = xe.BienSo;
                    txtHieuXe.Text = xe.HieuXe;
                    nudNamSX.Value = xe.NamSanXuat;
                    txtTenKH.Text = xe.TenKH;
                    txtDienThoai.Text = xe.DienThoai;
                    txtTraCuuBienSo.Text = xe.BienSo;
                }
            }
        }

        private void LoadDanhSachXe()
        {
            try
            {
                dgvDanhSachXe.DataSource = null;
                dgvDanhSachXe.DataSource = _xeBUS.GetAll();
                if (dgvDanhSachXe.Columns["BienSo"] != null) dgvDanhSachXe.Columns["BienSo"].HeaderText = "Biển Số";
                if (dgvDanhSachXe.Columns["HieuXe"] != null) dgvDanhSachXe.Columns["HieuXe"].HeaderText = "Hiệu Xe";
                if (dgvDanhSachXe.Columns["NamSanXuat"] != null) dgvDanhSachXe.Columns["NamSanXuat"].HeaderText = "Năm SX";
                if (dgvDanhSachXe.Columns["TenKH"] != null) dgvDanhSachXe.Columns["TenKH"].HeaderText = "Tên Chủ Xe";
                if (dgvDanhSachXe.Columns["DienThoai"] != null) dgvDanhSachXe.Columns["DienThoai"].HeaderText = "Điện Thoại";
                if (dgvDanhSachXe.Columns["MaKH"] != null) dgvDanhSachXe.Columns["MaKH"].Visible = false;

                SelectRow(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách xe: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnTraCuu_Click(object? sender, EventArgs e)
        {
            string bienSo = txtTraCuuBienSo.Text.Trim();
            if (string.IsNullOrEmpty(bienSo))
            {
                MessageBox.Show("Vui lòng nhập biển số xe để tra cứu!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var xe = _xeBUS.FindByBienSo(bienSo);
            if (xe != null)
            {
                lblKetQua.Text = $"✔ ĐÃ TÌM THẤY: Xe {xe.HieuXe} - Chủ xe: {xe.TenKH}";
                lblKetQua.ForeColor = Color.LightGreen;

                // Fill details
                txtBienSo.Text = xe.BienSo;
                txtHieuXe.Text = xe.HieuXe;
                nudNamSX.Value = xe.NamSanXuat;
                txtTenKH.Text = xe.TenKH;
                txtDienThoai.Text = xe.DienThoai;
                txtDiaChi.Text = "(Khách hàng cũ trong hệ thống)";

                MessageBox.Show($"Tìm thấy thông tin xe '{xe.BienSo}' của khách hàng '{xe.TenKH}'!", "Thông tin xe", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                lblKetQua.Text = "ℹ XE MỚI: Chưa có trong CSDL! Mời nhập thông tin.";
                lblKetQua.ForeColor = Color.Yellow;
                ResetFields();
                txtBienSo.Text = bienSo;
                txtTenKH.Focus();
                MessageBox.Show($"Biển số '{bienSo}' chưa có trong hệ thống.\nVui lòng nhập thông tin Chủ xe và Xe mới vào biểu mẫu bên trái!", "Xe mới", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DgvDanhSachXe_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvDanhSachXe.Rows[e.RowIndex].DataBoundItem is XeDTO xe)
            {
                txtBienSo.Text = xe.BienSo;
                txtHieuXe.Text = xe.HieuXe;
                nudNamSX.Value = xe.NamSanXuat;
                txtTenKH.Text = xe.TenKH;
                txtDienThoai.Text = xe.DienThoai;
                txtTraCuuBienSo.Text = xe.BienSo;
            }
        }

        private void BtnLuuXeMoi_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenKH.Text))
            {
                MessageBox.Show("Vui lòng nhập tên khách hàng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDienThoai.Text))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtBienSo.Text))
            {
                MessageBox.Show("Vui lòng nhập biển số xe!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtHieuXe.Text))
            {
                MessageBox.Show("Vui lòng nhập hiệu xe!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 1. Thêm khách hàng mới
            var kh = new KhachHangDTO
            {
                TenKH = txtTenKH.Text.Trim(),
                DienThoai = txtDienThoai.Text.Trim(),
                DiaChi = txtDiaChi.Text.Trim()
            };

            var (errKH, maKH) = _khachHangBUS.Add(kh);
            if (!string.IsNullOrEmpty(errKH))
            {
                MessageBox.Show(errKH, "Lỗi thêm khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Thêm xe mới gắn với MaKH
            var xe = new XeDTO
            {
                BienSo = txtBienSo.Text.Trim(),
                MaKH = maKH,
                HieuXe = txtHieuXe.Text.Trim(),
                NamSanXuat = (int)nudNamSX.Value
            };

            string errXe = _xeBUS.Add(xe);
            if (!string.IsNullOrEmpty(errXe))
            {
                MessageBox.Show(errXe, "Lỗi thêm xe", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show($"Tiếp nhận thành công xe '{xe.BienSo}' cho khách hàng '{kh.TenKH}'!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadDanhSachXe();
            ResetFields();
        }

        private void ResetFields()
        {
            txtTenKH.Clear();
            txtDienThoai.Clear();
            txtDiaChi.Clear();
            txtBienSo.Clear();
            txtHieuXe.Clear();
            nudNamSX.Value = DateTime.Now.Year;
        }
    }
}
