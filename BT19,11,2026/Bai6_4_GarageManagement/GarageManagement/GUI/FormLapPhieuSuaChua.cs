using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GarageManagement.BUS;
using GarageManagement.DTO;

namespace GarageManagement.GUI
{
    public class FormLapPhieuSuaChua : Form
    {
        private readonly PhieuSuaChuaBUS _phieuBUS = new PhieuSuaChuaBUS();
        private readonly XeBUS _xeBUS = new XeBUS();
        private readonly PhuTungBUS _phuTungBUS = new PhuTungBUS();

        private ComboBox cboXe = null!;
        private TextBox txtTenChuXe = null!;
        private TextBox txtNoiDungSua = null!;

        // Pick items
        private ComboBox cboPhuTung = null!;
        private Label lblTonKho = null!;
        private NumericUpDown nudSoLuong = null!;
        private NumericUpDown nudTienCong = null!;
        private Button btnThemHangMuc = null!;
        private Button btnXoaHangMuc = null!;

        // Temporary grid
        private DataGridView dgvChiTiet = null!;
        private List<ChiTietSuaChuaDTO> _tempChiTietList = new List<ChiTietSuaChuaDTO>();

        private Label lblTongTien = null!;
        private Button btnHoanTatTransaction = null!;

        // Test Rollback button
        private Button btnTestRollback = null!;

        public FormLapPhieuSuaChua()
        {
            InitializeComponent();
            LoadXeAndPhuTung();
        }

        private void InitializeComponent()
        {
            this.Text = "Lập Phiếu Sửa Chữa";
            this.Size = new Size(1100, 680);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

            // Header
            var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(142, 68, 173) };
            var lblTitle = new Label
            {
                Text = "LẬP PHIẾU TIẾP NHẬN SỬA CHỮA",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlHeader.Controls.Add(lblTitle);

            // Top Information Panel
            var pnlTopInfo = new GroupBox
            {
                Text = "1. Thông tin tiếp nhận xe",
                Dock = DockStyle.Top,
                Height = 110,
                Padding = new Padding(15, 10, 15, 10)
            };

            var lblXe = new Label { Text = "Chọn xe:", Location = new Point(15, 25), AutoSize = true, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            cboXe = new ComboBox { Location = new Point(15, 48), Width = 280, DropDownStyle = ComboBoxStyle.DropDownList };
            cboXe.SelectedIndexChanged += CboXe_SelectedIndexChanged;

            var lblChu = new Label { Text = "Chủ xe:", Location = new Point(315, 25), AutoSize = true };
            txtTenChuXe = new TextBox { Location = new Point(315, 48), Width = 220, ReadOnly = true, BackColor = Color.LightGray };

            var lblND = new Label { Text = "Nội dung sửa chữa:", Location = new Point(555, 25), AutoSize = true };
            txtNoiDungSua = new TextBox { Location = new Point(555, 48), Width = 500 };

            pnlTopInfo.Controls.AddRange(new Control[] { lblXe, cboXe, lblChu, txtTenChuXe, lblND, txtNoiDungSua });

            // Middle Add Item Panel
            var pnlAddItem = new GroupBox
            {
                Text = "2. Chọn phụ tùng & tiền công",
                Dock = DockStyle.Top,
                Height = 100,
                Padding = new Padding(15, 10, 15, 10)
            };

            var lblPT = new Label { Text = "Phụ tùng / Vật tư:", Location = new Point(15, 25), AutoSize = true, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            cboPhuTung = new ComboBox { Location = new Point(15, 48), Width = 280, DropDownStyle = ComboBoxStyle.DropDownList };
            cboPhuTung.SelectedIndexChanged += CboPhuTung_SelectedIndexChanged;

            lblTonKho = new Label { Text = "Tồn kho: 0", Location = new Point(310, 52), AutoSize = true, ForeColor = Color.DarkBlue, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };

            var lblSL = new Label { Text = "Số lượng:", Location = new Point(410, 25), AutoSize = true };
            nudSoLuong = new NumericUpDown { Location = new Point(410, 48), Width = 80, Minimum = 1, Maximum = 1000, Value = 1 };

            var lblTC = new Label { Text = "Tiền công (VNĐ):", Location = new Point(510, 25), AutoSize = true };
            nudTienCong = new NumericUpDown { Location = new Point(510, 48), Width = 150, Maximum = 100000000, Increment = 50000, Value = 100000, ThousandsSeparator = true };

            btnThemHangMuc = new Button
            {
                Text = "➕ Thêm vào phiếu",
                Location = new Point(680, 42),
                Size = new Size(160, 34),
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            btnThemHangMuc.Click += BtnThemHangMuc_Click;

            btnXoaHangMuc = new Button
            {
                Text = "✖ Xóa dòng chọn",
                Location = new Point(850, 42),
                Size = new Size(140, 34),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnXoaHangMuc.Click += BtnXoaHangMuc_Click;

            pnlAddItem.Controls.AddRange(new Control[] { lblPT, cboPhuTung, lblTonKho, lblSL, nudSoLuong, lblTC, nudTienCong, btnThemHangMuc, btnXoaHangMuc });

            // Bottom Action Panel
            var pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 85,
                BackColor = Color.FromArgb(245, 247, 250),
                Padding = new Padding(20, 15, 20, 15)
            };

            lblTongTien = new Label
            {
                Text = "TỔNG TIỀN: 0 VNĐ",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(192, 57, 43),
                Location = new Point(20, 25),
                AutoSize = true
            };

            btnHoanTatTransaction = new Button
            {
                Text = "Xác nhận lập phiếu",
                Location = new Point(480, 18),
                Size = new Size(200, 45),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold)
            };
            btnHoanTatTransaction.Click += BtnHoanTatTransaction_Click;

            btnTestRollback = new Button
            {
                Text = "Thử nghiệm Rollback",
                Location = new Point(695, 18),
                Size = new Size(180, 45),
                BackColor = Color.FromArgb(230, 126, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            btnTestRollback.Click += BtnTestRollback_Click;

            pnlBottom.Controls.AddRange(new Control[] { lblTongTien, btnHoanTatTransaction, btnTestRollback });

            // Center Grid
            var pnlGrid = new GroupBox
            {
                Text = "3. Danh sách các phụ tùng trong phiếu sửa chữa",
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

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
            pnlGrid.Controls.Add(dgvChiTiet);

            this.Controls.Add(pnlGrid);
            this.Controls.Add(pnlAddItem);
            this.Controls.Add(pnlTopInfo);
            this.Controls.Add(pnlBottom);
            this.Controls.Add(pnlHeader);
        }

        private void LoadXeAndPhuTung()
        {
            try
            {
                var xeList = _xeBUS.GetAll();
                cboXe.DataSource = xeList;
                cboXe.DisplayMember = "BienSo";
                cboXe.ValueMember = "BienSo";

                LoadPhuTungList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải thông tin: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPhuTungList()
        {
            var ptList = _phuTungBUS.GetAll();
            cboPhuTung.DataSource = ptList;
            cboPhuTung.DisplayMember = "TenPhuTung";
            cboPhuTung.ValueMember = "MaPhuTung";
            UpdateTonKhoLabel();
        }

        private void CboXe_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cboXe.SelectedItem is XeDTO xe)
            {
                txtTenChuXe.Text = $"{xe.TenKH} ({xe.DienThoai})";
            }
        }

        private void CboPhuTung_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateTonKhoLabel();
        }

        private void UpdateTonKhoLabel()
        {
            if (cboPhuTung.SelectedItem is PhuTungDTO pt)
            {
                lblTonKho.Text = $"Tồn kho: {pt.SoLuongTon} | Đơn giá: {pt.DonGia:N0} đ";
            }
        }

        private void BtnThemHangMuc_Click(object? sender, EventArgs e)
        {
            if (!(cboPhuTung.SelectedItem is PhuTungDTO pt))
            {
                MessageBox.Show("Vui lòng chọn một phụ tùng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int soLuong = (int)nudSoLuong.Value;
            decimal tienCong = nudTienCong.Value;

            // Kiểm tra xem đã có trong danh sách tạm chưa
            var existing = _tempChiTietList.FirstOrDefault(x => x.MaPhuTung == pt.MaPhuTung);
            if (existing != null)
            {
                existing.SoLuong += soLuong;
                existing.TienCong += tienCong;
            }
            else
            {
                _tempChiTietList.Add(new ChiTietSuaChuaDTO
                {
                    MaPhuTung = pt.MaPhuTung,
                    TenPhuTung = pt.TenPhuTung,
                    DonGia = pt.DonGia,
                    SoLuong = soLuong,
                    TienCong = tienCong
                });
            }

            RefreshGridAndTotal();
        }

        private void BtnXoaHangMuc_Click(object? sender, EventArgs e)
        {
            if (dgvChiTiet.SelectedRows.Count > 0 && dgvChiTiet.SelectedRows[0].DataBoundItem is ChiTietSuaChuaDTO item)
            {
                _tempChiTietList.Remove(item);
                RefreshGridAndTotal();
            }
        }

        private void RefreshGridAndTotal()
        {
            dgvChiTiet.DataSource = null;
            dgvChiTiet.DataSource = _tempChiTietList.ToList();

            if (dgvChiTiet.Columns["MaPhuTung"] != null) { dgvChiTiet.Columns["MaPhuTung"].HeaderText = "Mã"; dgvChiTiet.Columns["MaPhuTung"].Width = 50; }
            if (dgvChiTiet.Columns["TenPhuTung"] != null) { dgvChiTiet.Columns["TenPhuTung"].HeaderText = "Tên Phụ Tùng"; dgvChiTiet.Columns["TenPhuTung"].Width = 240; }
            if (dgvChiTiet.Columns["DonGia"] != null) { dgvChiTiet.Columns["DonGia"].HeaderText = "Đơn Giá (đ)"; dgvChiTiet.Columns["DonGia"].DefaultCellStyle.Format = "N0"; }
            if (dgvChiTiet.Columns["SoLuong"] != null) { dgvChiTiet.Columns["SoLuong"].HeaderText = "Số Lượng"; dgvChiTiet.Columns["SoLuong"].Width = 90; }
            if (dgvChiTiet.Columns["TienCong"] != null) { dgvChiTiet.Columns["TienCong"].HeaderText = "Tiền Công (đ)"; dgvChiTiet.Columns["TienCong"].DefaultCellStyle.Format = "N0"; }
            if (dgvChiTiet.Columns["ThanhTien"] != null) { dgvChiTiet.Columns["ThanhTien"].HeaderText = "Thành Tiền (đ)"; dgvChiTiet.Columns["ThanhTien"].DefaultCellStyle.Format = "N0"; }
            if (dgvChiTiet.Columns["MaPhieu"] != null) dgvChiTiet.Columns["MaPhieu"].Visible = false;

            decimal total = _tempChiTietList.Sum(x => x.ThanhTien);
            lblTongTien.Text = $"TỔNG TIỀN: {total:N0} VNĐ";
        }

        private void BtnHoanTatTransaction_Click(object? sender, EventArgs e)
        {
            if (cboXe.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn xe cần sửa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_tempChiTietList.Count == 0)
            {
                MessageBox.Show("Vui lòng thêm ít nhất một phụ tùng hoặc dịch vụ vào phiếu!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var phieu = new PhieuSuaChuaDTO
            {
                BienSo = cboXe.SelectedValue?.ToString() ?? "",
                NgayNhan = DateTime.Now,
                NoiDungSua = txtNoiDungSua.Text.Trim(),
                TrangThai = "Đang sửa"
            };

            // Thực thi Transaction: 1. Tạo Phiếu -> 2. Tạo Chi tiết -> 3. Trừ tồn kho -> 4. Rollback nếu thiếu hàng
            var (success, message, newId) = _phieuBUS.LapPhieuSuaChua(phieu, _tempChiTietList);

            if (success)
            {
                MessageBox.Show(message, "GIAO DỊCH THÀNH CÔNG (SqlTransaction COMMIT)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _tempChiTietList.Clear();
                RefreshGridAndTotal();
                txtNoiDungSua.Clear();
                LoadPhuTungList();
            }
            else
            {
                MessageBox.Show(message, "GIAO DỊCH BỊ HỦY BỎ (SqlTransaction ROLLBACK)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnTestRollback_Click(object? sender, EventArgs e)
        {
            if (!(cboPhuTung.SelectedItem is PhuTungDTO pt))
            {
                MessageBox.Show("Vui lòng chọn phụ tùng để test rollback!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Cố tình thêm số lượng vượt quá tồn kho để kiểm thử cơ chế Rollback an toàn dữ liệu
            int soLuongVuot = pt.SoLuongTon + 50;
            _tempChiTietList.Add(new ChiTietSuaChuaDTO
            {
                MaPhuTung = pt.MaPhuTung,
                TenPhuTung = pt.TenPhuTung,
                DonGia = pt.DonGia,
                SoLuong = soLuongVuot,
                TienCong = 100000
            });
            RefreshGridAndTotal();

            MessageBox.Show($"Đã thêm hạng mục '{pt.TenPhuTung}' với số lượng yêu cầu là {soLuongVuot} (trong khi tồn kho chỉ còn {pt.SoLuongTon}).\n\nBây giờ hãy nhấn 'XÁC NHẬN LẬP PHIẾU' để quan sát SqlTransaction tự động ROLLBACK!", "Mô phỏng Rollback", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
