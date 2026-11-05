using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace LabEquipmentCheckout;

public class MainForm : Form
{
    // ===================== DỮ LIỆU =====================
    private static readonly string[] Equipments =
    {
        "Máy hiện sóng Oscilloscope", "Bộ kit Arduino", "Máy hàn thiếc", "Đồng hồ vạn năng",
        "Laptop thực hành", "Kính hiển vi", "Bộ Raspberry Pi"
    };

    private readonly List<Loan> _all = new();                  // toàn bộ phiếu mượn
    private readonly BindingList<Loan> _view = new();          // danh sách sau khi lọc (gắn vào lưới)
    private readonly BindingSource _bs = new();

    private bool _loading;
    private bool _suspendFilter;
    private DateTime _today = DateTime.Today;
    private readonly System.Windows.Forms.Timer _timer = new();

    // ===================== CONTROLS =====================
    private readonly MenuStrip menuStrip = new();
    private readonly StatusStrip statusStrip = new();
    private readonly ToolStripStatusLabel lblTotal = new();
    private readonly ToolStripStatusLabel lblOverdue = new();
    private readonly ToolStripStatusLabel lblShown = new();
    private readonly ToolStripStatusLabel lblMessage = new();

    private readonly TextBox txtId = new();
    private readonly TextBox txtStudentId = new();
    private readonly TextBox txtStudentName = new();
    private readonly ComboBox cboEquipment = new();
    private readonly NumericUpDown nudQuantity = new();
    private readonly DateTimePicker dtpBorrow = new();
    private readonly DateTimePicker dtpDue = new();
    private readonly DateTimePicker dtpReturn = new();
    private readonly Label lblStatusValue = new();

    private readonly Button btnAdd = new();
    private readonly Button btnUpdate = new();
    private readonly Button btnDelete = new();
    private readonly Button btnReturn = new();
    private readonly Button btnNew = new();

    private readonly TextBox txtSearch = new();
    private readonly ComboBox cboFilterStatus = new();
    private readonly Button btnClearFilter = new();

    private readonly DataGridView dgv = new();
    private readonly ErrorProvider errorProvider = new();

    public MainForm()
    {
        _suspendFilter = true;
        BuildUi();
        SeedData();
        _suspendFilter = false;

        _bs.DataSource = _view;
        dgv.DataSource = _bs;
        _bs.CurrentChanged += (s, e) => { if (!_loading) LoadCurrentToForm(); };

        ApplyFilter();
        if (_bs.Current != null) LoadCurrentToForm(); else NewForm();
        SetMessage("Sẵn sàng.");

        // Qua nửa đêm thì ngày hiện tại đổi -> phiếu có thể chuyển sang "Quá hạn"
        _timer.Interval = 30_000;
        _timer.Tick += (s, e) =>
        {
            if (DateTime.Today != _today)
            {
                _today = DateTime.Today;
                ApplyFilter();
                UpdateStatusPreview();
                SetMessage("Sang ngày mới, đã cập nhật lại tình trạng các phiếu.");
            }
        };
        _timer.Start();
    }

    private Loan? Current => _bs.Current as Loan;

    // =====================================================================
    // 1. GIAO DIỆN (Dock + TableLayoutPanel => co giãn theo form)
    // =====================================================================
    private void BuildUi()
    {
        Text = "Lab Equipment Checkout - Quản lý mượn trả thiết bị";
        Font = new Font("Segoe UI", 10f);
        ClientSize = new Size(1120, 720);
        MinimumSize = new Size(940, 620);
        StartPosition = FormStartPosition.CenterScreen;

        errorProvider.ContainerControl = this;
        errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;

        BuildMenu();
        BuildStatusStrip();

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(8)
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        root.Controls.Add(BuildInputGroup(), 0, 0);
        root.Controls.Add(BuildFilterPanel(), 0, 1);
        BuildGrid();
        root.Controls.Add(dgv, 0, 2);

        Controls.Add(root);          // control Fill add trước
        Controls.Add(statusStrip);
        Controls.Add(menuStrip);
        MainMenuStrip = menuStrip;
    }

    private void BuildMenu()
    {
        var mnuFile = new ToolStripMenuItem("&Tệp");
        mnuFile.DropDownItems.Add(new ToolStripMenuItem("Xuất các phiếu đang lọc ra CSV...", null,
            (s, e) => ExportCsv()) { ShortcutKeys = Keys.Control | Keys.E });
        mnuFile.DropDownItems.Add(new ToolStripSeparator());
        mnuFile.DropDownItems.Add(new ToolStripMenuItem("Thoát", null, (s, e) => Close()));

        var mnuLoan = new ToolStripMenuItem("&Phiếu mượn");
        mnuLoan.DropDownItems.Add(new ToolStripMenuItem("Tạo phiếu mới (xóa trắng form)", null,
            (s, e) => NewForm()) { ShortcutKeys = Keys.Control | Keys.N });
        mnuLoan.DropDownItems.Add(new ToolStripMenuItem("Đánh dấu đã trả", null,
            (s, e) => MarkReturned()) { ShortcutKeys = Keys.Control | Keys.T });
        mnuLoan.DropDownItems.Add(new ToolStripSeparator());
        mnuLoan.DropDownItems.Add(new ToolStripMenuItem("Xóa bộ lọc", null,
            (s, e) => ClearFilters()) { ShortcutKeys = Keys.Control | Keys.R });

        menuStrip.Items.AddRange(new ToolStripItem[] { mnuFile, mnuLoan });
    }

    private void BuildStatusStrip()
    {
        lblTotal.BorderSides = ToolStripStatusLabelBorderSides.Right;
        lblTotal.Font = new Font(Font, FontStyle.Bold);
        lblOverdue.BorderSides = ToolStripStatusLabelBorderSides.Right;
        lblOverdue.Font = new Font(Font, FontStyle.Bold);
        lblShown.BorderSides = ToolStripStatusLabelBorderSides.Right;
        lblMessage.Spring = true;
        lblMessage.TextAlign = ContentAlignment.MiddleLeft;
        statusStrip.Items.AddRange(new ToolStripItem[] { lblTotal, lblOverdue, lblShown, lblMessage });
    }

    private Control BuildInputGroup()
    {
        var grp = new GroupBox
        {
            Text = "Phiếu mượn thiết bị",
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(8, 4, 8, 8)
        };

        var tlp = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 4,
            RowCount = 6
        };
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        for (int i = 0; i < 6; i++) tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        cboEquipment.DropDownStyle = ComboBoxStyle.DropDownList;
        cboEquipment.Items.AddRange(Equipments);

        nudQuantity.Minimum = 0;          // cho phép chọn 0 để kiểm tra lỗi "số lượng > 0"
        nudQuantity.Maximum = 1000;
        nudQuantity.Value = 1;

        foreach (var d in new[] { dtpBorrow, dtpDue, dtpReturn })
        {
            d.Format = DateTimePickerFormat.Custom;
            d.CustomFormat = "dd/MM/yyyy";
        }
        dtpReturn.ShowCheckBox = true;    // bỏ tick = chưa trả (ngày trả thực tế = rỗng)
        dtpReturn.Checked = false;

        lblStatusValue.AutoSize = true;
        lblStatusValue.Font = new Font(Font, FontStyle.Bold);
        lblStatusValue.Anchor = AnchorStyles.Left;
        lblStatusValue.Margin = new Padding(3, 6, 3, 6);

        AddField(tlp, "Mã phiếu:", txtId, 0, 0);
        AddField(tlp, "Thiết bị:", cboEquipment, 0, 2);
        AddField(tlp, "Mã sinh viên:", txtStudentId, 1, 0);
        AddField(tlp, "Số lượng:", nudQuantity, 1, 2);
        AddField(tlp, "Họ tên sinh viên:", txtStudentName, 2, 0);
        AddField(tlp, "Ngày mượn:", dtpBorrow, 2, 2);
        AddField(tlp, "Hạn trả:", dtpDue, 3, 0);
        AddField(tlp, "Ngày trả thực tế:", dtpReturn, 3, 2);

        // Dòng tình trạng (tự tính, chỉ hiển thị)
        tlp.Controls.Add(new Label
        {
            Text = "Tình trạng:",
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(3, 6, 8, 6)
        }, 0, 4);
        tlp.Controls.Add(lblStatusValue, 1, 4);

        // Nút chức năng
        btnAdd.Text = "Thêm";
        btnUpdate.Text = "Cập nhật";
        btnDelete.Text = "Xóa";
        btnReturn.Text = "Đánh dấu đã trả";
        btnNew.Text = "Tạo mới";
        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0, 6, 0, 0)
        };
        foreach (var b in new[] { btnAdd, btnUpdate, btnDelete, btnReturn, btnNew })
        {
            b.Size = new Size(b == btnReturn ? 150 : 110, 34);
            flow.Controls.Add(b);
        }
        tlp.Controls.Add(flow, 0, 5);
        tlp.SetColumnSpan(flow, 4);

        btnAdd.Click += BtnAdd_Click;
        btnUpdate.Click += BtnUpdate_Click;
        btnDelete.Click += BtnDelete_Click;
        btnReturn.Click += (s, e) => MarkReturned();
        btnNew.Click += (s, e) => NewForm();

        // Sửa lại ô nào thì xóa biểu tượng lỗi của ô đó
        foreach (Control c in new Control[] { txtId, txtStudentId, txtStudentName, cboEquipment, nudQuantity })
        {
            var ctl = c;
            ctl.TextChanged += (s, e) => errorProvider.SetError(ctl, "");
        }
        dtpBorrow.ValueChanged += (s, e) =>
        {
            errorProvider.SetError(dtpBorrow, "");
            errorProvider.SetError(dtpDue, "");
            errorProvider.SetError(dtpReturn, "");
        };
        dtpDue.ValueChanged += (s, e) => { errorProvider.SetError(dtpDue, ""); UpdateStatusPreview(); };
        dtpReturn.ValueChanged += (s, e) => { errorProvider.SetError(dtpReturn, ""); UpdateStatusPreview(); };

        grp.Controls.Add(tlp);
        return grp;
    }

    private void AddField(TableLayoutPanel tlp, string label, Control ctl, int row, int col)
    {
        var lbl = new Label
        {
            Text = label,
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(3, 6, 8, 6)
        };
        ctl.Dock = DockStyle.Fill;
        ctl.Margin = new Padding(3, 4, 26, 4);       // chừa chỗ cho biểu tượng lỗi
        tlp.Controls.Add(lbl, col, row);
        tlp.Controls.Add(ctl, col + 1, row);
        errorProvider.SetIconAlignment(ctl, ErrorIconAlignment.MiddleRight);
        errorProvider.SetIconPadding(ctl, 2);
    }

    private Control BuildFilterPanel()
    {
        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            WrapContents = true,
            Padding = new Padding(0, 6, 0, 6)
        };

        txtSearch.Width = 300;
        txtSearch.PlaceholderText = "Tìm theo mã sinh viên hoặc tên thiết bị...";
        txtSearch.Margin = new Padding(3, 4, 18, 3);

        cboFilterStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cboFilterStatus.Width = 150;
        cboFilterStatus.Items.Add("Tất cả");
        cboFilterStatus.Items.AddRange(LoanStatus.All);
        cboFilterStatus.SelectedIndex = 0;
        cboFilterStatus.Margin = new Padding(3, 4, 18, 3);

        btnClearFilter.Text = "Xóa bộ lọc";
        btnClearFilter.Size = new Size(110, 30);

        flow.Controls.Add(new Label { Text = "Tìm kiếm:", AutoSize = true, Margin = new Padding(3, 8, 3, 3) });
        flow.Controls.Add(txtSearch);
        flow.Controls.Add(new Label { Text = "Tình trạng:", AutoSize = true, Margin = new Padding(3, 8, 3, 3) });
        flow.Controls.Add(cboFilterStatus);
        flow.Controls.Add(btnClearFilter);

        txtSearch.TextChanged += (s, e) => ApplyFilter();
        cboFilterStatus.SelectedIndexChanged += (s, e) => ApplyFilter();
        btnClearFilter.Click += (s, e) => ClearFilters();
        return flow;
    }

    private void BuildGrid()
    {
        dgv.Dock = DockStyle.Fill;
        dgv.AutoGenerateColumns = false;                 // cột cấu hình thủ công
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgv.MultiSelect = false;
        dgv.ReadOnly = true;
        dgv.AllowUserToAddRows = false;
        dgv.AllowUserToDeleteRows = false;
        dgv.RowHeadersVisible = false;
        dgv.BackgroundColor = SystemColors.Window;
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        var dateStyle = () => new DataGridViewCellStyle { Format = "dd/MM/yyyy", NullValue = "" };

        dgv.Columns.Add(new DataGridViewTextBoxColumn
        { DataPropertyName = nameof(Loan.LoanId), HeaderText = "Mã phiếu", FillWeight = 9 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        { DataPropertyName = nameof(Loan.StudentId), HeaderText = "Mã SV", FillWeight = 11 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        { DataPropertyName = nameof(Loan.StudentName), HeaderText = "Họ tên sinh viên", FillWeight = 17 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        { DataPropertyName = nameof(Loan.Equipment), HeaderText = "Thiết bị", FillWeight = 20 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        { DataPropertyName = nameof(Loan.Quantity), HeaderText = "SL", FillWeight = 5 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        { DataPropertyName = nameof(Loan.BorrowDate), HeaderText = "Ngày mượn", FillWeight = 11, DefaultCellStyle = dateStyle() });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        { DataPropertyName = nameof(Loan.DueDate), HeaderText = "Hạn trả", FillWeight = 11, DefaultCellStyle = dateStyle() });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        { DataPropertyName = nameof(Loan.ReturnDate), HeaderText = "Ngày trả thực tế", FillWeight = 13, DefaultCellStyle = dateStyle() });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        { DataPropertyName = nameof(Loan.Status), HeaderText = "Tình trạng", FillWeight = 11 });

        dgv.DataError += (s, e) => e.ThrowException = false;
        dgv.CellFormatting += Dgv_CellFormatting;
        dgv.CellClick += (s, e) => { if (e.RowIndex >= 0) LoadCurrentToForm(); };
    }

    // Tô màu cả dòng theo tình trạng
    private void Dgv_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= dgv.Rows.Count) return;
        if (dgv.Rows[e.RowIndex].DataBoundItem is not Loan loan) return;

        switch (loan.Status)
        {
            case LoanStatus.Overdue:
                e.CellStyle!.ForeColor = Color.DarkRed;
                e.CellStyle.BackColor = Color.MistyRose;
                break;
            case LoanStatus.Returned:
                e.CellStyle!.ForeColor = Color.SeaGreen;
                break;
        }
    }

    // =====================================================================
    // 2. DỮ LIỆU MẪU (tính theo ngày hôm nay)
    // =====================================================================
    private void SeedData()
    {
        DateTime t = DateTime.Today;
        _all.AddRange(new[]
        {
            Make("PM001", "SV2021001", "Nguyễn Văn An",  Equipments[0], 1, t.AddDays(-10), t.AddDays(-3), null),
            Make("PM002", "SV2021002", "Trần Thị Bình",  Equipments[1], 2, t.AddDays(-5),  t.AddDays(2),  null),
            Make("PM003", "SV2021003", "Lê Hoàng Cường", Equipments[2], 1, t.AddDays(-12), t.AddDays(-5), t.AddDays(-6)),
            Make("PM004", "SV2021004", "Phạm Thu Dung",  Equipments[3], 3, t.AddDays(-2),  t.AddDays(5),  null),
            Make("PM005", "SV2021005", "Hoàng Minh Đức", Equipments[4], 1, t.AddDays(-20), t.AddDays(-10), t.AddDays(-9)),
            Make("PM006", "SV2021006", "Vũ Ngọc Hà",     Equipments[6], 2, t.AddDays(-8),  t.AddDays(-1), null),
            Make("PM007", "SV2021007", "Đặng Quốc Huy",  Equipments[5], 1, t,              t.AddDays(7),  null),
        });
    }

    private static Loan Make(string id, string sid, string name, string equip, int qty,
                             DateTime borrow, DateTime due, DateTime? ret) => new()
    {
        LoanId = id, StudentId = sid, StudentName = name, Equipment = equip, Quantity = qty,
        BorrowDate = borrow, DueDate = due, ReturnDate = ret
    };

    // =====================================================================
    // 3. LỌC / TÌM KIẾM
    // =====================================================================
    private void ApplyFilter(string? selectId = null)
    {
        if (_suspendFilter) return;

        var prev = _bs.Current;
        string? keepId = selectId ?? (prev as Loan)?.LoanId;

        string kw = txtSearch.Text.Trim();
        string kwNorm = RemoveDiacritics(kw).ToLowerInvariant();
        string? status = cboFilterStatus.SelectedIndex > 0 ? cboFilterStatus.SelectedItem as string : null;

        var result = _all.Where(a =>
        {
            if (kw.Length > 0)
            {
                bool idOk = a.StudentId.Contains(kw, StringComparison.OrdinalIgnoreCase);
                bool equipOk = RemoveDiacritics(a.Equipment).ToLowerInvariant().Contains(kwNorm);
                if (!idOk && !equipOk) return false;
            }
            if (status != null && a.Status != status) return false;
            return true;
        }).OrderBy(a => a.LoanId).ToList();

        _loading = true;
        try
        {
            _view.RaiseListChangedEvents = false;
            _view.Clear();
            foreach (var a in result) _view.Add(a);
            _view.RaiseListChangedEvents = true;
            _view.ResetBindings();

            if (keepId != null)
            {
                int idx = result.FindIndex(a => a.LoanId == keepId);
                if (idx >= 0) _bs.Position = idx;
            }
        }
        finally { _loading = false; }

        if (_bs.Current != null && !ReferenceEquals(prev, _bs.Current)) LoadCurrentToForm();

        btnUpdate.Enabled = btnDelete.Enabled = btnReturn.Enabled = _bs.Current != null;
        UpdateStatusBar();
    }

    private void ClearFilters()
    {
        _suspendFilter = true;
        txtSearch.Clear();
        cboFilterStatus.SelectedIndex = 0;
        _suspendFilter = false;
        ApplyFilter();
        SetMessage("Đã xóa bộ lọc.");
    }

    private void UpdateStatusBar()
    {
        int overdue = _all.Count(a => a.IsOverdue);
        lblTotal.Text = $"Tổng số phiếu: {_all.Count}";
        lblOverdue.Text = $"Phiếu quá hạn: {overdue}";
        lblOverdue.ForeColor = overdue > 0 ? Color.Firebrick : SystemColors.ControlText;
        lblShown.Text = $"Đang hiển thị: {_view.Count}";
    }

    private void SetMessage(string msg) =>
        lblMessage.Text = DateTime.Now.ToString("HH:mm:ss") + " - " + msg;

    // =====================================================================
    // 4. THÊM / CẬP NHẬT / XÓA / ĐÁNH DẤU ĐÃ TRẢ
    // =====================================================================
    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        if (!ValidateForm(null)) return;

        if (MessageBox.Show($"Thêm phiếu mượn mới cho sinh viên \"{txtStudentId.Text.Trim()}\"?", "Xác nhận thêm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        var loan = new Loan();
        FillFromForm(loan);
        _all.Add(loan);
        ApplyFilter(loan.LoanId);

        SetMessage(_view.Contains(loan)
            ? $"Đã thêm phiếu {loan.LoanId} (tình trạng: {loan.Status})."
            : $"Đã thêm phiếu {loan.LoanId} (không nằm trong bộ lọc hiện tại nên chưa hiển thị).");
    }

    private void BtnUpdate_Click(object? sender, EventArgs e)
    {
        var cur = Current;
        if (cur == null)
        {
            MessageBox.Show("Hãy chọn một phiếu trong danh sách để cập nhật.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (!ValidateForm(cur)) return;

        if (MessageBox.Show($"Cập nhật phiếu {cur.LoanId}?", "Xác nhận cập nhật",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        FillFromForm(cur);
        ApplyFilter(cur.LoanId);
        SetMessage($"Đã cập nhật phiếu {cur.LoanId} (tình trạng: {cur.Status}).");
    }

    private void BtnDelete_Click(object? sender, EventArgs e)
    {
        var cur = Current;
        if (cur == null)
        {
            MessageBox.Show("Hãy chọn một phiếu trong danh sách để xóa.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (MessageBox.Show($"Xóa phiếu {cur.LoanId} ({cur.StudentId} - {cur.Equipment})?\nThao tác này không thể hoàn tác.",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

        string id = cur.LoanId;
        _all.Remove(cur);
        ApplyFilter();
        SetMessage($"Đã xóa phiếu {id}.");
    }

    // Đánh dấu đã trả: ngày trả thực tế = ngày hôm nay
    private void MarkReturned()
    {
        var cur = Current;
        if (cur == null)
        {
            MessageBox.Show("Hãy chọn một phiếu trong danh sách để đánh dấu đã trả.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (cur.ReturnDate.HasValue)
        {
            MessageBox.Show($"Phiếu {cur.LoanId} đã được trả vào ngày {cur.ReturnDate:dd/MM/yyyy}.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        DateTime today = DateTime.Today;
        if (today < cur.BorrowDate.Date)
        {
            errorProvider.SetError(dtpReturn, "Ngày trả thực tế không được trước ngày mượn.");
            MessageBox.Show("Ngày trả thực tế (hôm nay) đang trước ngày mượn của phiếu này nên không thể đánh dấu đã trả.",
                "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (MessageBox.Show($"Đánh dấu phiếu {cur.LoanId} đã trả vào ngày {today:dd/MM/yyyy}?", "Xác nhận trả thiết bị",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        cur.ReturnDate = today;
        ApplyFilter(cur.LoanId);
        LoadCurrentToForm();          // làm tươi khu nhập (ngày trả + tình trạng)

        SetMessage(_view.Contains(cur)
            ? $"Đã đánh dấu phiếu {cur.LoanId} đã trả ngày {today:dd/MM/yyyy}."
            : $"Đã đánh dấu phiếu {cur.LoanId} đã trả (phiếu không còn nằm trong bộ lọc hiện tại).");
    }

    // =====================================================================
    // 5. KIỂM TRA DỮ LIỆU (ErrorProvider)
    // =====================================================================
    // editing = null -> THÊM mới;  editing != null -> CẬP NHẬT bản ghi đó
    private bool ValidateForm(Loan? editing)
    {
        ClearErrors();
        Control? first = null;

        void Fail(Control c, string msg)
        {
            errorProvider.SetError(c, msg);
            first ??= c;
        }

        // Mã phiếu: bắt buộc + không trùng
        string id = txtId.Text.Trim();
        if (id.Length == 0)
            Fail(txtId, "Mã phiếu không được để trống.");
        else if (_all.Any(a => !ReferenceEquals(a, editing) &&
                               string.Equals(a.LoanId, id, StringComparison.OrdinalIgnoreCase)))
            Fail(txtId, $"Mã phiếu \"{id}\" đã tồn tại.");

        // Mã sinh viên, họ tên
        if (txtStudentId.Text.Trim().Length == 0)
            Fail(txtStudentId, "Mã sinh viên không được để trống.");
        if (txtStudentName.Text.Trim().Length == 0)
            Fail(txtStudentName, "Vui lòng nhập họ tên sinh viên.");

        // Thiết bị
        if (cboEquipment.SelectedIndex < 0)
            Fail(cboEquipment, "Vui lòng chọn thiết bị.");

        // Số lượng > 0
        if (nudQuantity.Value <= 0)
            Fail(nudQuantity, "Số lượng phải lớn hơn 0.");

        // Hạn trả không trước ngày mượn
        DateTime borrow = dtpBorrow.Value.Date;
        DateTime due = dtpDue.Value.Date;
        if (due < borrow)
            Fail(dtpDue, "Hạn trả không được trước ngày mượn.");

        // Ngày trả thực tế (nếu có tick) không trước ngày mượn
        if (dtpReturn.Checked && dtpReturn.Value.Date < borrow)
            Fail(dtpReturn, "Ngày trả thực tế không được trước ngày mượn.");

        if (first != null)
        {
            first.Focus();
            SetMessage("Dữ liệu chưa hợp lệ, vui lòng kiểm tra các ô báo lỗi.");
            return false;
        }
        return true;
    }

    private void ClearErrors()
    {
        foreach (Control c in new Control[]
                 { txtId, txtStudentId, txtStudentName, cboEquipment, nudQuantity, dtpBorrow, dtpDue, dtpReturn })
            errorProvider.SetError(c, "");
    }

    private static string RemoveDiacritics(string text)
    {
        string s = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(s.Length);
        foreach (char c in s)
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        return sb.ToString().Replace('đ', 'd').Replace('Đ', 'D').Normalize(NormalizationForm.FormC);
    }

    // =====================================================================
    // 6. FORM <-> ĐỐI TƯỢNG
    // =====================================================================
    private void FillFromForm(Loan a)
    {
        a.LoanId = txtId.Text.Trim();
        a.StudentId = txtStudentId.Text.Trim();
        a.StudentName = txtStudentName.Text.Trim();
        a.Equipment = cboEquipment.SelectedItem as string ?? "";
        a.Quantity = (int)nudQuantity.Value;
        a.BorrowDate = dtpBorrow.Value.Date;
        a.DueDate = dtpDue.Value.Date;
        a.ReturnDate = dtpReturn.Checked ? dtpReturn.Value.Date : null;
    }

    private void LoadCurrentToForm()
    {
        if (Current is not Loan a) return;

        txtId.Text = a.LoanId;
        txtStudentId.Text = a.StudentId;
        txtStudentName.Text = a.StudentName;
        cboEquipment.SelectedItem = a.Equipment;
        nudQuantity.Value = Math.Clamp(a.Quantity, (int)nudQuantity.Minimum, (int)nudQuantity.Maximum);
        dtpBorrow.Value = a.BorrowDate;
        dtpDue.Value = a.DueDate;
        dtpReturn.Value = a.ReturnDate ?? DateTime.Today;
        dtpReturn.Checked = a.ReturnDate.HasValue;

        ClearErrors();
        UpdateStatusPreview();
    }

    private void NewForm()
    {
        ClearErrors();
        txtId.Text = NextId();
        txtStudentId.Clear();
        txtStudentName.Clear();
        cboEquipment.SelectedIndex = -1;
        nudQuantity.Value = 1;
        dtpBorrow.Value = DateTime.Today;
        dtpDue.Value = DateTime.Today.AddDays(7);
        dtpReturn.Value = DateTime.Today;
        dtpReturn.Checked = false;
        UpdateStatusPreview();
        txtStudentId.Focus();
    }

    // Hiển thị tình trạng dự kiến theo giá trị đang nhập (tự tính, không cho sửa tay)
    private void UpdateStatusPreview()
    {
        DateTime? ret = dtpReturn.Checked ? dtpReturn.Value.Date : null;
        string st = Loan.ComputeStatus(dtpDue.Value.Date, ret);
        lblStatusValue.Text = st;
        lblStatusValue.ForeColor = st switch
        {
            LoanStatus.Overdue => Color.Firebrick,
            LoanStatus.Returned => Color.SeaGreen,
            _ => Color.RoyalBlue
        };
    }

    private string NextId()
    {
        int max = 0;
        foreach (var a in _all)
        {
            var m = Regex.Match(a.LoanId, @"(\d+)$");
            if (m.Success && int.TryParse(m.Groups[1].Value, out int n) && n > max) max = n;
        }
        return "PM" + (max + 1).ToString("D3");
    }

    // =====================================================================
    // 7. XUẤT CSV (các phiếu đang lọc)
    // =====================================================================
    private void ExportCsv()
    {
        if (_view.Count == 0)
        {
            MessageBox.Show("Không có phiếu nào đang hiển thị để xuất.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var sfd = new SaveFileDialog
        {
            Title = "Xuất danh sách phiếu mượn",
            Filter = "CSV (*.csv)|*.csv",
            FileName = $"PhieuMuon_{DateTime.Now:yyyyMMdd_HHmm}.csv"
        };
        if (sfd.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            using var sw = new StreamWriter(sfd.FileName, false, new UTF8Encoding(true));   // UTF-8 BOM cho Excel
            sw.WriteLine("Mã phiếu,Mã sinh viên,Họ tên sinh viên,Thiết bị,Số lượng,Ngày mượn,Hạn trả,Ngày trả thực tế,Tình trạng");
            foreach (var a in _view)
            {
                sw.WriteLine(string.Join(",",
                    Csv(a.LoanId), Csv(a.StudentId), Csv(a.StudentName), Csv(a.Equipment),
                    a.Quantity.ToString(CultureInfo.InvariantCulture),
                    Csv(a.BorrowDate.ToString("dd/MM/yyyy")),
                    Csv(a.DueDate.ToString("dd/MM/yyyy")),
                    Csv(a.ReturnDate?.ToString("dd/MM/yyyy")),
                    Csv(a.Status)));
            }
            SetMessage($"Đã xuất {_view.Count} phiếu ra file CSV.");
            MessageBox.Show($"Xuất thành công {_view.Count} phiếu.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi khi ghi file: " + ex.Message, "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static string Csv(string? v)
    {
        v ??= "";
        if (v.Contains(',') || v.Contains('"') || v.Contains('\n') || v.Contains('\r'))
            return "\"" + v.Replace("\"", "\"\"") + "\"";
        return v;
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _timer.Stop();
        _timer.Dispose();
        base.OnFormClosed(e);
    }
}
