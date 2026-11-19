using System;
using System.Drawing;
using System.Windows.Forms;
using GarageManagement.BUS;
using GarageManagement.DTO;

namespace GarageManagement.GUI
{
    public class FormPhuTung : Form
    {
        private readonly PhuTungBUS _phuTungBUS = new PhuTungBUS();

        private TextBox txtMaPT = null!;
        private TextBox txtTenPT = null!;
        private NumericUpDown nudDonGia = null!;
        private NumericUpDown nudSoLuongTon = null!;

        private Button btnThem = null!;
        private Button btnSua = null!;
        private Button btnXoa = null!;
        private Button btnLamMoi = null!;

        private DataGridView dgvPhuTung = null!;

        public FormPhuTung()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản lý Phụ tùng & Tồn kho";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

            // Header
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(52, 73, 94)
            };
            var lblTitle = new Label
            {
                Text = "QUẢN LÝ PHỤ TÙNG & TỒN KHO",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlHeader.Controls.Add(lblTitle);

            // Left Input
            var pnlInput = new GroupBox
            {
                Text = "Thông tin phụ tùng",
                Dock = DockStyle.Left,
                Width = 380,
                Padding = new Padding(15)
            };

            int y = 30;
            var lblMa = new Label { Text = "Mã phụ tùng:", Location = new Point(15, y), AutoSize = true };
            txtMaPT = new TextBox { Location = new Point(15, y + 25), Width = 340, ReadOnly = true, BackColor = Color.LightGray };
            y += 65;

            var lblTen = new Label { Text = "Tên phụ tùng (*):", Location = new Point(15, y), AutoSize = true };
            txtTenPT = new TextBox { Location = new Point(15, y + 25), Width = 340 };
            y += 65;

            var lblGia = new Label { Text = "Đơn giá (VNĐ):", Location = new Point(15, y), AutoSize = true };
            nudDonGia = new NumericUpDown
            {
                Location = new Point(15, y + 25),
                Width = 340,
                Maximum = 1000000000,
                Increment = 10000,
                ThousandsSeparator = true
            };
            y += 65;

            var lblTon = new Label { Text = "Số lượng tồn kho:", Location = new Point(15, y), AutoSize = true };
            nudSoLuongTon = new NumericUpDown
            {
                Location = new Point(15, y + 25),
                Width = 340,
                Maximum = 100000,
                Value = 0
            };
            y += 75;

            btnThem = new Button { Text = "Thêm", Location = new Point(15, y), Size = new Size(80, 32), BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            btnSua = new Button { Text = "Sửa", Location = new Point(102, y), Size = new Size(80, 32), BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            btnXoa = new Button { Text = "Xóa", Location = new Point(189, y), Size = new Size(75, 32), BackColor = Color.FromArgb(231, 76, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            btnLamMoi = new Button { Text = "Làm mới", Location = new Point(271, y), Size = new Size(85, 32), BackColor = Color.FromArgb(108, 117, 125), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };

            btnThem.Click += BtnThem_Click;
            btnSua.Click += BtnSua_Click;
            btnXoa.Click += BtnXoa_Click;
            btnLamMoi.Click += (s, e) => ResetFields();

            pnlInput.Controls.AddRange(new Control[] {
                lblMa, txtMaPT,
                lblTen, txtTenPT,
                lblGia, nudDonGia,
                lblTon, nudSoLuongTon,
                btnThem, btnSua, btnXoa, btnLamMoi
            });

            // Right Grid
            var pnlGrid = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            dgvPhuTung = new DataGridView
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
            dgvPhuTung.CellClick += DgvPhuTung_CellClick;
            dgvPhuTung.CellFormatting += DgvPhuTung_CellFormatting;
            pnlGrid.Controls.Add(dgvPhuTung);

            this.Controls.Add(pnlGrid);
            this.Controls.Add(pnlInput);
            this.Controls.Add(pnlHeader);
        }

        public void SelectRow(int index)
        {
            if (dgvPhuTung.Rows.Count > index && index >= 0)
            {
                dgvPhuTung.ClearSelection();
                dgvPhuTung.Rows[index].Selected = true;
                if (dgvPhuTung.Rows[index].DataBoundItem is PhuTungDTO pt)
                {
                    txtMaPT.Text = pt.MaPhuTung.ToString();
                    txtTenPT.Text = pt.TenPhuTung;
                    nudDonGia.Value = pt.DonGia;
                    nudSoLuongTon.Value = pt.SoLuongTon;
                }
            }
        }

        private void LoadData()
        {
            try
            {
                dgvPhuTung.DataSource = null;
                dgvPhuTung.DataSource = _phuTungBUS.GetAll();
                if (dgvPhuTung.Columns["MaPhuTung"] != null) { dgvPhuTung.Columns["MaPhuTung"].HeaderText = "Mã PT"; dgvPhuTung.Columns["MaPhuTung"].Width = 60; }
                if (dgvPhuTung.Columns["TenPhuTung"] != null) { dgvPhuTung.Columns["TenPhuTung"].HeaderText = "Tên Phụ Tùng / Dịch Vụ"; dgvPhuTung.Columns["TenPhuTung"].Width = 240; }
                if (dgvPhuTung.Columns["DonGia"] != null) { dgvPhuTung.Columns["DonGia"].HeaderText = "Đơn Giá (VNĐ)"; dgvPhuTung.Columns["DonGia"].DefaultCellStyle.Format = "N0"; }
                if (dgvPhuTung.Columns["SoLuongTon"] != null) { dgvPhuTung.Columns["SoLuongTon"].HeaderText = "Số Lượng Tồn"; }

                SelectRow(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải phụ tùng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvPhuTung_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvPhuTung.Columns[e.ColumnIndex].Name == "SoLuongTon" && e.Value != null)
            {
                int ton = Convert.ToInt32(e.Value);
                if (ton <= 3)
                {
                    e.CellStyle!.ForeColor = Color.Red;
                    e.CellStyle.Font = new Font(dgvPhuTung.Font, FontStyle.Bold);
                }
            }
        }

        private void DgvPhuTung_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvPhuTung.Rows[e.RowIndex].DataBoundItem is PhuTungDTO pt)
            {
                txtMaPT.Text = pt.MaPhuTung.ToString();
                txtTenPT.Text = pt.TenPhuTung;
                nudDonGia.Value = pt.DonGia;
                nudSoLuongTon.Value = pt.SoLuongTon;
            }
        }

        private void BtnThem_Click(object? sender, EventArgs e)
        {
            var pt = new PhuTungDTO
            {
                TenPhuTung = txtTenPT.Text.Trim(),
                DonGia = nudDonGia.Value,
                SoLuongTon = (int)nudSoLuongTon.Value
            };

            string error = _phuTungBUS.Add(pt);
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Thêm phụ tùng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadData();
            ResetFields();
        }

        private void BtnSua_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtMaPT.Text, out int ma))
            {
                MessageBox.Show("Vui lòng chọn phụ tùng cần sửa từ danh sách!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var pt = new PhuTungDTO
            {
                MaPhuTung = ma,
                TenPhuTung = txtTenPT.Text.Trim(),
                DonGia = nudDonGia.Value,
                SoLuongTon = (int)nudSoLuongTon.Value
            };

            string error = _phuTungBUS.Update(pt);
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Cập nhật phụ tùng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadData();
            ResetFields();
        }

        private void BtnXoa_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtMaPT.Text, out int ma))
            {
                MessageBox.Show("Vui lòng chọn phụ tùng cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa phụ tùng này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string error = _phuTungBUS.Delete(ma);
                if (!string.IsNullOrEmpty(error))
                {
                    MessageBox.Show(error, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                MessageBox.Show("Xóa phụ tùng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ResetFields();
            }
        }

        private void ResetFields()
        {
            txtMaPT.Clear();
            txtTenPT.Clear();
            nudDonGia.Value = 0;
            nudSoLuongTon.Value = 0;
        }
    }
}
