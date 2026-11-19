using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ProjectManagement.BUS;
using ProjectManagement.DTO;

namespace ProjectManagement.GUI
{
    public class FormCongViec : Form
    {
        private readonly CongViecBUS _congViecBUS = new CongViecBUS();
        private readonly DuAnBUS _duAnBUS = new DuAnBUS();
        private readonly ThanhVienBUS _thanhVienBUS = new ThanhVienBUS();
        private readonly PhanCongBUS _phanCongBUS = new PhanCongBUS();

        private ComboBox cboLocDuAn = null!;
        private ComboBox cboLocTrangThai = null!;
        private Button btnLamMoiLoc = null!;

        private DataGridView dgvCongViec = null!;
        private TextBox txtMaTask = null!;
        private ComboBox cboDuAn = null!;
        private TextBox txtTenTask = null!;
        private TextBox txtMoTa = null!;
        private DateTimePicker dtpHanNop = null!;
        private ComboBox cboDoUuTien = null!;
        private ComboBox cboTrangThai = null!;
        private CheckedListBox clbThanhVien = null!;

        private Button btnThem = null!;
        private Button btnSua = null!;
        private Button btnXoa = null!;
        private Button btnLamMoi = null!;

        private Button btnChuyenToDo = null!;
        private Button btnChuyenInProgress = null!;
        private Button btnChuyenDone = null!;

        public FormCongViec()
        {
            InitializeComponent();
            LoadComboBoxes();
            LoadTasks();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản lý Công việc & Tiến độ Dự án";
            this.Size = new Size(1180, 720);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

            // Filter Top Panel
            var pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 65,
                BackColor = Color.FromArgb(236, 240, 241),
                Padding = new Padding(15, 12, 15, 12)
            };

            var lblLocDuAn = new Label { Text = "Dự án:", Location = new Point(15, 18), AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            cboLocDuAn = new ComboBox { Location = new Point(70, 15), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            cboLocDuAn.SelectedIndexChanged += (s, e) => LoadTasks();

            var lblLocTrangThai = new Label { Text = "Trạng thái:", Location = new Point(340, 18), AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            cboLocTrangThai = new ComboBox { Location = new Point(430, 15), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cboLocTrangThai.Items.AddRange(new object[] { "Tất cả", "To Do", "In Progress", "Review", "Done" });
            cboLocTrangThai.SelectedIndex = 0;
            cboLocTrangThai.SelectedIndexChanged += (s, e) => LoadTasks();

            btnLamMoiLoc = new Button { Text = "Tải lại", Location = new Point(600, 13), Size = new Size(90, 32), BackColor = Color.FromArgb(52, 73, 94), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnLamMoiLoc.Click += (s, e) => LoadTasks();

            pnlTop.Controls.AddRange(new Control[] { lblLocDuAn, cboLocDuAn, lblLocTrangThai, cboLocTrangThai, btnLamMoiLoc });

            // Left Input / Form Panel
            var pnlLeft = new GroupBox
            {
                Text = "Thông tin công việc",
                Dock = DockStyle.Left,
                Width = 380,
                Padding = new Padding(12)
            };

            int y = 25;
            var lblMa = new Label { Text = "Mã Task:", Location = new Point(12, y), AutoSize = true };
            txtMaTask = new TextBox { Location = new Point(12, y + 22), Width = 340, ReadOnly = true, BackColor = Color.LightGray };
            y += 55;

            var lblDA = new Label { Text = "Dự án (*):", Location = new Point(12, y), AutoSize = true };
            cboDuAn = new ComboBox { Location = new Point(12, y + 22), Width = 340, DropDownStyle = ComboBoxStyle.DropDownList };
            y += 55;

            var lblTen = new Label { Text = "Tên công việc (*):", Location = new Point(12, y), AutoSize = true };
            txtTenTask = new TextBox { Location = new Point(12, y + 22), Width = 340 };
            y += 55;

            var lblMoTa = new Label { Text = "Mô tả chi tiết:", Location = new Point(12, y), AutoSize = true };
            txtMoTa = new TextBox { Location = new Point(12, y + 22), Width = 340, Height = 45, Multiline = true };
            y += 75;

            var lblHan = new Label { Text = "Hạn nộp:", Location = new Point(12, y), AutoSize = true };
            dtpHanNop = new DateTimePicker { Location = new Point(12, y + 22), Width = 340, Format = DateTimePickerFormat.Short };
            y += 55;

            var lblUuTien = new Label { Text = "Độ ưu tiên:", Location = new Point(12, y), AutoSize = true };
            cboDoUuTien = new ComboBox { Location = new Point(12, y + 22), Width = 160, DropDownStyle = ComboBoxStyle.DropDownList };
            cboDoUuTien.Items.AddRange(new object[] { "Cao", "Trung bình", "Thấp" });
            cboDoUuTien.SelectedIndex = 1;

            var lblTrangThai = new Label { Text = "Trạng thái:", Location = new Point(190, y), AutoSize = true };
            cboTrangThai = new ComboBox { Location = new Point(190, y + 22), Width = 160, DropDownStyle = ComboBoxStyle.DropDownList };
            cboTrangThai.Items.AddRange(new object[] { "To Do", "In Progress", "Review", "Done" });
            cboTrangThai.SelectedIndex = 0;
            y += 60;

            var lblPhanCong = new Label { Text = "Thành viên phụ trách:", Location = new Point(12, y), AutoSize = true, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            clbThanhVien = new CheckedListBox { Location = new Point(12, y + 22), Width = 340, Height = 95, CheckOnClick = true };
            y += 125;

            // Action Buttons
            btnThem = new Button { Text = "Thêm", Location = new Point(12, y), Size = new Size(80, 32), BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSua = new Button { Text = "Sửa", Location = new Point(98, y), Size = new Size(80, 32), BackColor = Color.FromArgb(41, 128, 185), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnXoa = new Button { Text = "Xóa", Location = new Point(184, y), Size = new Size(75, 32), BackColor = Color.FromArgb(192, 57, 43), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnLamMoi = new Button { Text = "Làm mới", Location = new Point(265, y), Size = new Size(85, 32), BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            btnThem.Click += BtnThem_Click;
            btnSua.Click += BtnSua_Click;
            btnXoa.Click += BtnXoa_Click;
            btnLamMoi.Click += (s, e) => ResetFields();

            pnlLeft.Controls.AddRange(new Control[] {
                lblMa, txtMaTask,
                lblDA, cboDuAn,
                lblTen, txtTenTask,
                lblMoTa, txtMoTa,
                lblHan, dtpHanNop,
                lblUuTien, cboDoUuTien,
                lblTrangThai, cboTrangThai,
                lblPhanCong, clbThanhVien,
                btnThem, btnSua, btnXoa, btnLamMoi
            });

            // Center / Right Panel for Grid & Quick Status Update
            var pnlCenter = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            // Quick Status Transition Bar
            var pnlQuickAction = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(245, 247, 250) };
            var lblQuick = new Label { Text = "Chuyển nhanh trạng thái:", Location = new Point(10, 14), AutoSize = true, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            btnChuyenToDo = new Button { Text = "To Do", Location = new Point(185, 8), Size = new Size(80, 32), BackColor = Color.FromArgb(149, 165, 166), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnChuyenInProgress = new Button { Text = "In Progress", Location = new Point(275, 8), Size = new Size(100, 32), BackColor = Color.FromArgb(243, 156, 18), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnChuyenDone = new Button { Text = "Done", Location = new Point(385, 8), Size = new Size(80, 32), BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            btnChuyenToDo.Click += (s, e) => ChangeSelectedTaskStatus("To Do");
            btnChuyenInProgress.Click += (s, e) => ChangeSelectedTaskStatus("In Progress");
            btnChuyenDone.Click += (s, e) => ChangeSelectedTaskStatus("Done");

            pnlQuickAction.Controls.AddRange(new Control[] { lblQuick, btnChuyenToDo, btnChuyenInProgress, btnChuyenDone });

            dgvCongViec = new DataGridView
            {
                Dock = DockStyle.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                RowTemplate = { Height = 28 },
                ColumnHeadersHeight = 32
            };
            dgvCongViec.CellClick += DgvCongViec_CellClick;
            dgvCongViec.CellFormatting += DgvCongViec_CellFormatting;

            pnlCenter.Controls.Add(dgvCongViec);
            pnlCenter.Controls.Add(pnlQuickAction);

            this.Controls.Add(pnlCenter);
            this.Controls.Add(pnlLeft);
            this.Controls.Add(pnlTop);
        }

        private void LoadComboBoxes()
        {
            try
            {
                var projects = _duAnBUS.GetAll();
                cboDuAn.DisplayMember = "TenDuAn";
                cboDuAn.ValueMember = "MaDuAn";
                cboDuAn.DataSource = new List<DuAnDTO>(projects);

                var filterProjects = new List<DuAnDTO> { new DuAnDTO { MaDuAn = 0, TenDuAn = "-- Tất cả dự án --" } };
                filterProjects.AddRange(projects);
                cboLocDuAn.DisplayMember = "TenDuAn";
                cboLocDuAn.ValueMember = "MaDuAn";
                cboLocDuAn.DataSource = filterProjects;
                cboLocDuAn.SelectedIndex = 0;

                var members = _thanhVienBUS.GetAll();
                clbThanhVien.Items.Clear();
                foreach (var m in members)
                {
                    clbThanhVien.Items.Add(m, false);
                }
                clbThanhVien.DisplayMember = "HoTen";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi nạp danh mục: " + ex.Message);
            }
        }

        private void LoadTasks()
        {
            try
            {
                int? maDuAn = null;
                if (cboLocDuAn != null)
                {
                    if (cboLocDuAn.SelectedValue is int selectedId && selectedId > 0)
                    {
                        maDuAn = selectedId;
                    }
                    else if (cboLocDuAn.SelectedItem is DuAnDTO dto && dto.MaDuAn > 0)
                    {
                        maDuAn = dto.MaDuAn;
                    }
                }

                string trangThai = cboLocTrangThai?.SelectedItem?.ToString() ?? "Tất cả";

                var list = _congViecBUS.GetTasks(maDuAn, trangThai);
                if (dgvCongViec != null)
                {
                    dgvCongViec.DataSource = null;
                    dgvCongViec.DataSource = list;

                    if (dgvCongViec.Columns["MaTask"] != null) { dgvCongViec.Columns["MaTask"].HeaderText = "Mã"; dgvCongViec.Columns["MaTask"].Width = 60; }
                    if (dgvCongViec.Columns["TenTask"] != null) { dgvCongViec.Columns["TenTask"].HeaderText = "Tên Công Việc"; dgvCongViec.Columns["TenTask"].Width = 220; }
                    if (dgvCongViec.Columns["TenDuAn"] != null) { dgvCongViec.Columns["TenDuAn"].HeaderText = "Dự Án"; dgvCongViec.Columns["TenDuAn"].Width = 160; }
                    if (dgvCongViec.Columns["HanNop"] != null) { dgvCongViec.Columns["HanNop"].HeaderText = "Hạn Nộp"; dgvCongViec.Columns["HanNop"].Width = 110; dgvCongViec.Columns["HanNop"].DefaultCellStyle.Format = "dd/MM/yyyy"; }
                    if (dgvCongViec.Columns["DoUuTien"] != null) { dgvCongViec.Columns["DoUuTien"].HeaderText = "Độ Ưu Tiên"; dgvCongViec.Columns["DoUuTien"].Width = 100; }
                    if (dgvCongViec.Columns["TrangThai"] != null) { dgvCongViec.Columns["TrangThai"].HeaderText = "Trạng Thái"; dgvCongViec.Columns["TrangThai"].Width = 105; }
                    if (dgvCongViec.Columns["NguoiThucHien"] != null) { dgvCongViec.Columns["NguoiThucHien"].HeaderText = "Người Phụ Trách"; dgvCongViec.Columns["NguoiThucHien"].Width = 160; }
                    if (dgvCongViec.Columns["MoTa"] != null) dgvCongViec.Columns["MoTa"].Visible = false;
                    if (dgvCongViec.Columns["MaDuAn"] != null) dgvCongViec.Columns["MaDuAn"].Visible = false;

                    SelectRow(0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi tải danh sách công việc FormCongViec: " + ex.ToString());
            }
        }

        public void SelectRow(int index)
        {
            if (dgvCongViec.Rows.Count > index && index >= 0)
            {
                dgvCongViec.ClearSelection();
                dgvCongViec.Rows[index].Selected = true;
                if (dgvCongViec.Rows[index].DataBoundItem is CongViecDTO task)
                {
                    txtMaTask.Text = task.MaTask.ToString();
                    cboDuAn.SelectedValue = task.MaDuAn;
                    txtTenTask.Text = task.TenTask;
                    txtMoTa.Text = task.MoTa;
                    if (task.HanNop.HasValue) dtpHanNop.Value = task.HanNop.Value;
                    cboDoUuTien.SelectedItem = task.DoUuTien;
                    cboTrangThai.SelectedItem = task.TrangThai;

                    var assignedIds = _phanCongBUS.GetAssignedMemberIds(task.MaTask);
                    for (int i = 0; i < clbThanhVien.Items.Count; i++)
                    {
                        if (clbThanhVien.Items[i] is ThanhVienDTO member)
                        {
                            clbThanhVien.SetItemChecked(i, assignedIds.Contains(member.MaThanhVien));
                        }
                    }
                }
            }
        }

        private void DgvCongViec_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvCongViec.Columns[e.ColumnIndex].Name == "TrangThai" && e.Value != null)
            {
                string st = e.Value.ToString()!;
                if (st == "Done")
                {
                    e.CellStyle!.ForeColor = Color.Green;
                    e.CellStyle.Font = new Font(dgvCongViec.Font, FontStyle.Bold);
                }
                else if (st == "In Progress")
                {
                    e.CellStyle!.ForeColor = Color.OrangeRed;
                    e.CellStyle.Font = new Font(dgvCongViec.Font, FontStyle.Bold);
                }
            }
        }

        private void DgvCongViec_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvCongViec.Rows[e.RowIndex].DataBoundItem is CongViecDTO task)
            {
                txtMaTask.Text = task.MaTask.ToString();
                cboDuAn.SelectedValue = task.MaDuAn;
                txtTenTask.Text = task.TenTask;
                txtMoTa.Text = task.MoTa;
                if (task.HanNop.HasValue) dtpHanNop.Value = task.HanNop.Value;
                cboDoUuTien.SelectedItem = task.DoUuTien;
                cboTrangThai.SelectedItem = task.TrangThai;

                // Load checked members
                var assignedIds = _phanCongBUS.GetAssignedMemberIds(task.MaTask);
                for (int i = 0; i < clbThanhVien.Items.Count; i++)
                {
                    if (clbThanhVien.Items[i] is ThanhVienDTO member)
                    {
                        clbThanhVien.SetItemChecked(i, assignedIds.Contains(member.MaThanhVien));
                    }
                }
            }
        }

        private void BtnThem_Click(object? sender, EventArgs e)
        {
            if (cboDuAn.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn Dự án!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var task = new CongViecDTO
            {
                MaDuAn = Convert.ToInt32(cboDuAn.SelectedValue),
                TenTask = txtTenTask.Text.Trim(),
                MoTa = txtMoTa.Text.Trim(),
                HanNop = dtpHanNop.Value,
                DoUuTien = cboDoUuTien.SelectedItem?.ToString() ?? "Trung bình",
                TrangThai = cboTrangThai.SelectedItem?.ToString() ?? "To Do"
            };

            var (error, newId) = _congViecBUS.Add(task);
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Giao việc cho các thành viên được tích chọn
            SaveMemberAssignments(newId);

            MessageBox.Show("Thêm công việc và phân công thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadTasks();
            ResetFields();
        }

        private void BtnSua_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtMaTask.Text, out int maTask))
            {
                MessageBox.Show("Vui lòng chọn công việc cần sửa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var task = new CongViecDTO
            {
                MaTask = maTask,
                MaDuAn = Convert.ToInt32(cboDuAn.SelectedValue),
                TenTask = txtTenTask.Text.Trim(),
                MoTa = txtMoTa.Text.Trim(),
                HanNop = dtpHanNop.Value,
                DoUuTien = cboDoUuTien.SelectedItem?.ToString() ?? "Trung bình",
                TrangThai = cboTrangThai.SelectedItem?.ToString() ?? "To Do"
            };

            string error = _congViecBUS.Update(task);
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Cập nhật phân công
            SaveMemberAssignments(maTask);

            MessageBox.Show("Cập nhật công việc thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadTasks();
        }

        private void SaveMemberAssignments(int maTask)
        {
            var currentlyAssigned = _phanCongBUS.GetAssignedMemberIds(maTask);
            for (int i = 0; i < clbThanhVien.Items.Count; i++)
            {
                if (clbThanhVien.Items[i] is ThanhVienDTO member)
                {
                    bool isChecked = clbThanhVien.GetItemChecked(i);
                    if (isChecked && !currentlyAssigned.Contains(member.MaThanhVien))
                    {
                        _phanCongBUS.Assign(maTask, member.MaThanhVien);
                    }
                    else if (!isChecked && currentlyAssigned.Contains(member.MaThanhVien))
                    {
                        _phanCongBUS.Remove(maTask, member.MaThanhVien);
                    }
                }
            }
        }

        private void BtnXoa_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtMaTask.Text, out int maTask))
            {
                MessageBox.Show("Vui lòng chọn công việc cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa công việc này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string error = _congViecBUS.Delete(maTask);
                if (!string.IsNullOrEmpty(error))
                {
                    MessageBox.Show(error, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                MessageBox.Show("Xóa công việc thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadTasks();
                ResetFields();
            }
        }

        private void ChangeSelectedTaskStatus(string newStatus)
        {
            if (dgvCongViec.SelectedRows.Count == 0 || !(dgvCongViec.SelectedRows[0].DataBoundItem is CongViecDTO task))
            {
                MessageBox.Show("Vui lòng chọn một công việc từ bảng bên dưới để chuyển trạng thái!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string error = _congViecBUS.UpdateStatus(task.MaTask, newStatus);
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show($"Đã chuyển công việc '{task.TenTask}' sang trạng thái '{newStatus}' thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadTasks();
        }

        private void ResetFields()
        {
            txtMaTask.Clear();
            txtTenTask.Clear();
            txtMoTa.Clear();
            dtpHanNop.Value = DateTime.Now.AddDays(7);
            cboDoUuTien.SelectedIndex = 1;
            cboTrangThai.SelectedIndex = 0;
            for (int i = 0; i < clbThanhVien.Items.Count; i++)
            {
                clbThanhVien.SetItemChecked(i, false);
            }
        }
    }
}
