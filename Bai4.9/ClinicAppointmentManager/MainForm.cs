using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ClinicAppointmentManager;

public class MainForm : Form
{
    // ===================== DỮ LIỆU =====================
    private static readonly string[] Doctors =
    {
        "BS. Nguyễn Văn Hùng", "BS. Trần Thị Mai", "BS. Lê Quốc Khánh", "BS. Phạm Ngọc Anh"
    };

    // SĐT di động VN sau khi chuẩn hóa: 0 + (3|5|7|8|9) + 8 chữ số
    private static readonly Regex PhoneRegex = new(@"^0[35789]\d{8}$");

    private readonly List<Appointment> _all = new();              // toàn bộ lịch hẹn
    private readonly BindingList<Appointment> _view = new();      // danh sách sau khi lọc (gắn vào lưới)
    private readonly BindingSource _bs = new();

    private bool _loading;          // chặn sự kiện khi đang nạp lại danh sách
    private bool _suspendFilter;    // chặn lọc lặp khi đặt lại nhiều bộ lọc cùng lúc

    // ===================== CONTROLS =====================
    private readonly MenuStrip menuStrip = new();
    private readonly StatusStrip statusStrip = new();
    private readonly ToolStripStatusLabel lblCount = new();
    private readonly ToolStripStatusLabel lblBreakdown = new();
    private readonly ToolStripStatusLabel lblMessage = new();

    private readonly TextBox txtId = new();
    private readonly TextBox txtName = new();
    private readonly TextBox txtPhone = new();
    private readonly DateTimePicker dtpTime = new();
    private readonly ComboBox cboDoctor = new();
    private readonly ComboBox cboStatus = new();
    private readonly TextBox txtReason = new();

    private readonly Button btnAdd = new();
    private readonly Button btnUpdate = new();
    private readonly Button btnDelete = new();
    private readonly Button btnNew = new();

    private readonly TextBox txtSearch = new();
    private readonly CheckBox chkDate = new();
    private readonly DateTimePicker dtpFilter = new();
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
    }

    private Appointment? Current => _bs.Current as Appointment;

    // =====================================================================
    // 1. DỰNG GIAO DIỆN (TableLayoutPanel co giãn theo kích thước form)
    // =====================================================================
    private void BuildUi()
    {
        Text = "Clinic Appointment Manager - Quản lý lịch hẹn phòng khám";
        Font = new Font("Segoe UI", 10f);
        ClientSize = new Size(1080, 700);
        MinimumSize = new Size(920, 600);
        StartPosition = FormStartPosition.CenterScreen;

        errorProvider.ContainerControl = this;
        errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;

        BuildMenu();
        BuildStatusStrip();

        // ---- Bố cục gốc: 1 cột, 3 hàng (nhập liệu / bộ lọc / lưới) ----
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

        // Thứ tự: control Fill trước, rồi các thanh Dock cạnh
        Controls.Add(root);
        Controls.Add(statusStrip);
        Controls.Add(menuStrip);
        MainMenuStrip = menuStrip;
    }

    private void BuildMenu()
    {
        var mnuFile = new ToolStripMenuItem("&Tệp");
        mnuFile.DropDownItems.Add(new ToolStripMenuItem("Xuất danh sách đang hiển thị ra CSV...", null,
            (s, e) => ExportCsv()) { ShortcutKeys = Keys.Control | Keys.E });
        mnuFile.DropDownItems.Add(new ToolStripSeparator());
        mnuFile.DropDownItems.Add(new ToolStripMenuItem("Thoát", null, (s, e) => Close()));

        var mnuAppt = new ToolStripMenuItem("&Lịch hẹn");
        mnuAppt.DropDownItems.Add(new ToolStripMenuItem("Tạo lịch hẹn mới (xóa trắng form)", null,
            (s, e) => NewForm()) { ShortcutKeys = Keys.Control | Keys.N });
        mnuAppt.DropDownItems.Add(new ToolStripMenuItem("Xóa bộ lọc", null,
            (s, e) => ClearFilters()) { ShortcutKeys = Keys.Control | Keys.R });

        menuStrip.Items.AddRange(new ToolStripItem[] { mnuFile, mnuAppt });
    }

    private void BuildStatusStrip()
    {
        lblCount.BorderSides = ToolStripStatusLabelBorderSides.Right;
        lblCount.Font = new Font(Font, FontStyle.Bold);
        lblBreakdown.BorderSides = ToolStripStatusLabelBorderSides.Right;
        lblMessage.Spring = true;
        lblMessage.TextAlign = ContentAlignment.MiddleLeft;
        statusStrip.Items.AddRange(new ToolStripItem[] { lblCount, lblBreakdown, lblMessage });
    }

    private Control BuildInputGroup()
    {
        var grp = new GroupBox
        {
            Text = "Thông tin lịch hẹn",
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(8, 4, 8, 8)
        };

        // Lưới nhập liệu 4 cột: [nhãn | ô nhập | nhãn | ô nhập]
        var tlp = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 4,
            RowCount = 5
        };
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        for (int i = 0; i < 5; i++) tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        dtpTime.Format = DateTimePickerFormat.Custom;
        dtpTime.CustomFormat = "dd/MM/yyyy HH:mm";

        cboDoctor.DropDownStyle = ComboBoxStyle.DropDownList;
        cboDoctor.Items.AddRange(Doctors);
        cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cboStatus.Items.AddRange(AppointmentStatus.All);
        txtPhone.MaxLength = 16;

        AddField(tlp, "Mã lịch hẹn:", txtId, 0, 0);
        AddField(tlp, "Ngày giờ hẹn:", dtpTime, 0, 2);
        AddField(tlp, "Họ tên bệnh nhân:", txtName, 1, 0);
        AddField(tlp, "Bác sĩ:", cboDoctor, 1, 2);
        AddField(tlp, "Số điện thoại:", txtPhone, 2, 0);
        AddField(tlp, "Trạng thái:", cboStatus, 2, 2);
        AddField(tlp, "Lý do khám:", txtReason, 3, 0);
        tlp.SetColumnSpan(txtReason, 3);

        // Nút chức năng
        btnAdd.Text = "Thêm";
        btnUpdate.Text = "Cập nhật";
        btnDelete.Text = "Xóa";
        btnNew.Text = "Tạo mới";
        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0, 6, 0, 0)
        };
        foreach (var b in new[] { btnAdd, btnUpdate, btnDelete, btnNew })
        {
            b.Size = new Size(110, 34);
            flow.Controls.Add(b);
        }
        tlp.Controls.Add(flow, 0, 4);
        tlp.SetColumnSpan(flow, 4);

        btnAdd.Click += BtnAdd_Click;
        btnUpdate.Click += BtnUpdate_Click;
        btnDelete.Click += BtnDelete_Click;
        btnNew.Click += (s, e) => NewForm();

        // Gõ/chọn lại thì xóa biểu tượng lỗi của ô đó
        foreach (Control c in new Control[] { txtId, txtName, txtPhone, txtReason, cboDoctor, cboStatus })
        {
            var ctl = c;
            ctl.TextChanged += (s, e) => errorProvider.SetError(ctl, "");
        }
        dtpTime.ValueChanged += (s, e) => errorProvider.SetError(dtpTime, "");

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
        ctl.Margin = new Padding(3, 4, 26, 4);    // chừa chỗ bên phải cho biểu tượng lỗi
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

        txtSearch.Width = 260;
        txtSearch.PlaceholderText = "Tìm theo tên hoặc số điện thoại...";
        txtSearch.Margin = new Padding(3, 4, 18, 3);

        chkDate.Text = "Lọc theo ngày hẹn:";
        chkDate.AutoSize = true;
        chkDate.Margin = new Padding(3, 7, 3, 3);

        dtpFilter.Format = DateTimePickerFormat.Short;
        dtpFilter.Width = 130;
        dtpFilter.Enabled = false;
        dtpFilter.Value = DateTime.Today;
        dtpFilter.Margin = new Padding(3, 4, 18, 3);

        var lblStatus = new Label { Text = "Trạng thái:", AutoSize = true, Margin = new Padding(3, 8, 3, 3) };
        cboFilterStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cboFilterStatus.Width = 140;
        cboFilterStatus.Items.Add("Tất cả");
        cboFilterStatus.Items.AddRange(AppointmentStatus.All);
        cboFilterStatus.SelectedIndex = 0;
        cboFilterStatus.Margin = new Padding(3, 4, 18, 3);

        btnClearFilter.Text = "Xóa bộ lọc";
        btnClearFilter.Size = new Size(110, 30);

        flow.Controls.Add(new Label { Text = "Tìm kiếm:", AutoSize = true, Margin = new Padding(3, 8, 3, 3) });
        flow.Controls.Add(txtSearch);
        flow.Controls.Add(chkDate);
        flow.Controls.Add(dtpFilter);
        flow.Controls.Add(lblStatus);
        flow.Controls.Add(cboFilterStatus);
        flow.Controls.Add(btnClearFilter);

        // Lọc/tìm kiếm tức thời
        txtSearch.TextChanged += (s, e) => ApplyFilter();
        chkDate.CheckedChanged += (s, e) => { dtpFilter.Enabled = chkDate.Checked; ApplyFilter(); };
        dtpFilter.ValueChanged += (s, e) => ApplyFilter();
        cboFilterStatus.SelectedIndexChanged += (s, e) => ApplyFilter();
        btnClearFilter.Click += (s, e) => ClearFilters();

        return flow;
    }

    private void BuildGrid()
    {
        dgv.Dock = DockStyle.Fill;
        dgv.AutoGenerateColumns = false;                       // cột tự định nghĩa
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgv.MultiSelect = false;
        dgv.ReadOnly = true;
        dgv.AllowUserToAddRows = false;
        dgv.AllowUserToDeleteRows = false;
        dgv.RowHeadersVisible = false;
        dgv.BackgroundColor = SystemColors.Window;
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        dgv.Columns.Add(new DataGridViewTextBoxColumn
        { DataPropertyName = nameof(Appointment.AppointmentId), HeaderText = "Mã LH", FillWeight = 9 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        { DataPropertyName = nameof(Appointment.PatientName), HeaderText = "Bệnh nhân", FillWeight = 20 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        { DataPropertyName = nameof(Appointment.Phone), HeaderText = "Số điện thoại", FillWeight = 13 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Appointment.AppointmentTime),
            HeaderText = "Ngày giờ hẹn",
            FillWeight = 15,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" }
        });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        { DataPropertyName = nameof(Appointment.Doctor), HeaderText = "Bác sĩ", FillWeight = 17 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        { DataPropertyName = nameof(Appointment.Reason), HeaderText = "Lý do khám", FillWeight = 25 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        { DataPropertyName = nameof(Appointment.Status), HeaderText = "Trạng thái", FillWeight = 11 });

        dgv.DataError += (s, e) => e.ThrowException = false;
        dgv.CellFormatting += Dgv_CellFormatting;
        // Bấm vào dòng (kể cả dòng đang chọn) thì nạp lại dữ liệu lên khu vực nhập
        dgv.CellClick += (s, e) => { if (e.RowIndex >= 0) LoadCurrentToForm(); };
    }

    private void Dgv_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0) return;
        if (dgv.Columns[e.ColumnIndex].DataPropertyName != nameof(Appointment.Status)) return;

        e.CellStyle!.ForeColor = (e.Value as string) switch
        {
            AppointmentStatus.Done => Color.SeaGreen,
            AppointmentStatus.InProgress => Color.DarkOrange,
            AppointmentStatus.Cancelled => Color.Gray,
            _ => Color.RoyalBlue
        };
    }

    // =====================================================================
    // 2. DỮ LIỆU MẪU
    // =====================================================================
    private void SeedData()
    {
        DateTime d = DateTime.Today;
        _all.AddRange(new[]
        {
            Make("LH001", "Nguyễn Văn An",  "0912345678", d.AddHours(8),               Doctors[0], "Khám tổng quát",      AppointmentStatus.Done),
            Make("LH002", "Trần Thị Bình",  "0987654321", d.AddHours(9).AddMinutes(30), Doctors[1], "Đau đầu, chóng mặt",  AppointmentStatus.Done),
            Make("LH003", "Lê Hoàng Cường", "0369852147", d.AddHours(10).AddMinutes(30),Doctors[2], "Đau răng",            AppointmentStatus.InProgress),
            Make("LH008", "Bùi Thị Lan",    "0933445566", d.AddHours(11),              Doctors[3], "Đau bụng",            AppointmentStatus.Cancelled),
            Make("LH004", "Phạm Thu Dung",  "0903123456", d.AddHours(14),              Doctors[3], "Tái khám sau mổ",     AppointmentStatus.Waiting),
            Make("LH005", "Hoàng Minh Đức", "0775123987", d.AddHours(15).AddMinutes(30),Doctors[0], "Khám da liễu",        AppointmentStatus.Waiting),
            Make("LH006", "Vũ Ngọc Hà",     "0584123456", d.AddHours(16).AddMinutes(30),Doctors[1], "Tiêm vắc xin",        AppointmentStatus.Waiting),
            Make("LH007", "Đặng Quốc Huy",  "0898765432", d.AddDays(1).AddHours(9),    Doctors[2], "Nhổ răng khôn",       AppointmentStatus.Waiting),
        });
    }

    private static Appointment Make(string id, string name, string phone, DateTime time,
                                    string doctor, string reason, string status) => new()
    {
        AppointmentId = id, PatientName = name, Phone = phone, AppointmentTime = time,
        Doctor = doctor, Reason = reason, Status = status
    };

    // =====================================================================
    // 3. LỌC / TÌM KIẾM  (dựng lại _view từ _all)
    // =====================================================================
    private void ApplyFilter(string? selectId = null)
    {
        if (_suspendFilter) return;

        var prev = _bs.Current;
        string? keepId = selectId ?? (prev as Appointment)?.AppointmentId;

        string kw = txtSearch.Text.Trim();
        string kwName = RemoveDiacritics(kw).ToLowerInvariant();
        string kwPhone = NormalizePhone(kw);
        string? status = cboFilterStatus.SelectedIndex > 0 ? cboFilterStatus.SelectedItem as string : null;
        bool byDate = chkDate.Checked;
        DateTime date = dtpFilter.Value.Date;

        var result = _all.Where(a =>
        {
            if (kw.Length > 0)
            {
                bool nameOk = RemoveDiacritics(a.PatientName).ToLowerInvariant().Contains(kwName);
                bool phoneOk = kwPhone.Length > 0 && a.Phone.Contains(kwPhone);
                if (!nameOk && !phoneOk) return false;
            }
            if (byDate && a.AppointmentTime.Date != date) return false;
            if (status != null && a.Status != status) return false;
            return true;
        }).OrderBy(a => a.AppointmentTime).ToList();

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
                int idx = result.FindIndex(a => a.AppointmentId == keepId);
                if (idx >= 0) _bs.Position = idx;
            }
        }
        finally { _loading = false; }

        // Nếu dòng đang chọn đã đổi (do lọc/thêm/xóa) thì nạp lại lên form
        if (_bs.Current != null && !ReferenceEquals(prev, _bs.Current)) LoadCurrentToForm();

        btnUpdate.Enabled = btnDelete.Enabled = _bs.Current != null;
        UpdateStatusBar();
    }

    private void ClearFilters()
    {
        _suspendFilter = true;
        txtSearch.Clear();
        chkDate.Checked = false;
        dtpFilter.Value = DateTime.Today;
        cboFilterStatus.SelectedIndex = 0;
        _suspendFilter = false;
        ApplyFilter();
        SetMessage("Đã xóa bộ lọc.");
    }

    private void UpdateStatusBar()
    {
        lblCount.Text = $"Hiển thị: {_view.Count} / {_all.Count} lịch hẹn";
        lblBreakdown.Text = string.Join("  |  ",
            AppointmentStatus.All.Select(s => $"{s}: {_view.Count(a => a.Status == s)}"));
    }

    private void SetMessage(string msg) =>
        lblMessage.Text = DateTime.Now.ToString("HH:mm:ss") + " - " + msg;

    // =====================================================================
    // 4. THÊM / CẬP NHẬT / XÓA
    // =====================================================================
    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        if (!ValidateForm(null)) return;

        if (MessageBox.Show($"Thêm lịch hẹn mới cho bệnh nhân \"{txtName.Text.Trim()}\"?", "Xác nhận thêm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        var a = new Appointment();
        FillFromForm(a);
        _all.Add(a);
        ApplyFilter(a.AppointmentId);

        bool visible = _view.Contains(a);
        SetMessage(visible
            ? $"Đã thêm lịch hẹn {a.AppointmentId}."
            : $"Đã thêm lịch hẹn {a.AppointmentId} (không nằm trong bộ lọc hiện tại nên chưa hiển thị).");
    }

    private void BtnUpdate_Click(object? sender, EventArgs e)
    {
        var cur = Current;
        if (cur == null)
        {
            MessageBox.Show("Hãy chọn một lịch hẹn trong danh sách để cập nhật.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (!ValidateForm(cur)) return;

        if (MessageBox.Show($"Cập nhật lịch hẹn {cur.AppointmentId} của \"{cur.PatientName}\"?", "Xác nhận cập nhật",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        FillFromForm(cur);
        ApplyFilter(cur.AppointmentId);
        SetMessage($"Đã cập nhật lịch hẹn {cur.AppointmentId}.");
    }

    private void BtnDelete_Click(object? sender, EventArgs e)
    {
        var cur = Current;
        if (cur == null)
        {
            MessageBox.Show("Hãy chọn một lịch hẹn trong danh sách để xóa.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (MessageBox.Show($"Xóa lịch hẹn {cur.AppointmentId} của \"{cur.PatientName}\"?\nThao tác này không thể hoàn tác.",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

        string id = cur.AppointmentId;
        _all.Remove(cur);
        ApplyFilter();
        SetMessage($"Đã xóa lịch hẹn {id}.");
    }

    // =====================================================================
    // 5. KIỂM TRA DỮ LIỆU (ErrorProvider)
    // =====================================================================
    // editing = null  -> đang THÊM mới;  editing != null -> đang CẬP NHẬT bản ghi đó
    private bool ValidateForm(Appointment? editing)
    {
        ClearErrors();
        Control? first = null;

        void Fail(Control c, string msg)
        {
            errorProvider.SetError(c, msg);
            first ??= c;
        }

        // Mã lịch hẹn: bắt buộc + duy nhất
        string id = txtId.Text.Trim();
        if (id.Length == 0)
            Fail(txtId, "Mã lịch hẹn không được để trống.");
        else if (_all.Any(a => !ReferenceEquals(a, editing) &&
                               string.Equals(a.AppointmentId, id, StringComparison.OrdinalIgnoreCase)))
            Fail(txtId, $"Mã lịch hẹn \"{id}\" đã tồn tại.");

        // Họ tên
        if (txtName.Text.Trim().Length == 0)
            Fail(txtName, "Vui lòng nhập họ tên bệnh nhân.");

        // Số điện thoại
        if (txtPhone.Text.Trim().Length == 0)
            Fail(txtPhone, "Vui lòng nhập số điện thoại.");
        else if (!PhoneRegex.IsMatch(NormalizePhone(txtPhone.Text)))
            Fail(txtPhone, "Số điện thoại không hợp lệ (VD: 0912345678 hoặc +84912345678).");

        // Ngày giờ hẹn không nằm trong quá khứ.
        // Khi CẬP NHẬT mà không đổi giờ hẹn (VD: chỉ đổi trạng thái thành "Hoàn tất")
        // thì không kiểm tra lại, vì lịch đó vốn đã tới giờ khám.
        DateTime time = ReadTime();
        bool timeChanged = editing == null || time != editing.AppointmentTime;
        if (timeChanged && time < TruncateToMinute(DateTime.Now))
            Fail(dtpTime, "Ngày giờ hẹn không được nằm trong quá khứ.");

        if (cboDoctor.SelectedIndex < 0)
            Fail(cboDoctor, "Vui lòng chọn bác sĩ.");

        if (txtReason.Text.Trim().Length == 0)
            Fail(txtReason, "Vui lòng nhập lý do khám.");

        if (cboStatus.SelectedIndex < 0)
            Fail(cboStatus, "Vui lòng chọn trạng thái.");

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
        foreach (Control c in new Control[] { txtId, txtName, txtPhone, dtpTime, cboDoctor, cboStatus, txtReason })
            errorProvider.SetError(c, "");
    }

    private static string NormalizePhone(string input)
    {
        string p = Regex.Replace(input ?? "", @"[\s.\-()]", "");
        if (p.StartsWith("+84")) p = "0" + p.Substring(3);
        return p;
    }

    private static DateTime TruncateToMinute(DateTime d) =>
        new(d.Year, d.Month, d.Day, d.Hour, d.Minute, 0);

    private DateTime ReadTime() => TruncateToMinute(dtpTime.Value);

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
    private void FillFromForm(Appointment a)
    {
        a.AppointmentId = txtId.Text.Trim();
        a.PatientName = txtName.Text.Trim();
        a.Phone = NormalizePhone(txtPhone.Text);
        a.AppointmentTime = ReadTime();
        a.Doctor = cboDoctor.SelectedItem as string ?? "";
        a.Reason = txtReason.Text.Trim();
        a.Status = cboStatus.SelectedItem as string ?? AppointmentStatus.Waiting;
    }

    private void LoadCurrentToForm()
    {
        if (Current is not Appointment a) return;

        txtId.Text = a.AppointmentId;
        txtName.Text = a.PatientName;
        txtPhone.Text = a.Phone;
        dtpTime.Value = a.AppointmentTime;
        cboDoctor.SelectedItem = a.Doctor;
        cboStatus.SelectedItem = a.Status;
        txtReason.Text = a.Reason;
        ClearErrors();
    }

    private void NewForm()
    {
        ClearErrors();
        txtId.Text = NextId();
        txtName.Clear();
        txtPhone.Clear();
        txtReason.Clear();

        // Mặc định: đầu giờ kế tiếp
        DateTime next = TruncateToMinute(DateTime.Now);
        dtpTime.Value = new DateTime(next.Year, next.Month, next.Day, next.Hour, 0, 0).AddHours(1);

        cboDoctor.SelectedIndex = 0;
        cboStatus.SelectedItem = AppointmentStatus.Waiting;
        txtName.Focus();
    }

    private string NextId()
    {
        int max = 0;
        foreach (var a in _all)
        {
            var m = Regex.Match(a.AppointmentId, @"(\d+)$");
            if (m.Success && int.TryParse(m.Groups[1].Value, out int n) && n > max) max = n;
        }
        return "LH" + (max + 1).ToString("D3");
    }

    // =====================================================================
    // 7. XUẤT CSV (danh sách đang hiển thị)
    // =====================================================================
    private void ExportCsv()
    {
        if (_view.Count == 0)
        {
            MessageBox.Show("Không có lịch hẹn nào đang hiển thị để xuất.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var sfd = new SaveFileDialog
        {
            Title = "Xuất danh sách lịch hẹn",
            Filter = "CSV (*.csv)|*.csv",
            FileName = $"LichHen_{DateTime.Now:yyyyMMdd_HHmm}.csv"
        };
        if (sfd.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            // UTF-8 có BOM để Excel đọc đúng tiếng Việt
            using var sw = new StreamWriter(sfd.FileName, false, new UTF8Encoding(true));
            sw.WriteLine("Mã lịch hẹn,Họ tên bệnh nhân,Số điện thoại,Ngày giờ hẹn,Bác sĩ,Lý do khám,Trạng thái");
            foreach (var a in _view)
            {
                sw.WriteLine(string.Join(",",
                    Csv(a.AppointmentId), Csv(a.PatientName), Csv(a.Phone),
                    Csv(a.AppointmentTime.ToString("dd/MM/yyyy HH:mm")),
                    Csv(a.Doctor), Csv(a.Reason), Csv(a.Status)));
            }
            SetMessage($"Đã xuất {_view.Count} lịch hẹn ra file CSV.");
            MessageBox.Show($"Xuất thành công {_view.Count} lịch hẹn.", "Thông báo",
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
}
