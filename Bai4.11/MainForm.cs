using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace RepairServiceTracker;

public class MainForm : Form
{
    // ===================== DỮ LIỆU =====================
    private static readonly string[] DeviceTypes =
    {
        "Điện thoại", "Laptop", "Máy tính bảng", "Tivi", "Máy in", "Máy giặt", "Tủ lạnh", "Khác"
    };

    private readonly List<RepairOrder> _all = new();               // toàn bộ đơn
    private readonly BindingList<RepairOrder> _view = new();       // danh sách sau khi lọc (gắn vào lưới)
    private readonly BindingSource _bs = new();

    private bool _loading;
    private bool _suspendFilter;
    private string _imagePath = "";    // ảnh đang hiển thị ở khu nhập (được lưu vào đơn khi Thêm/Cập nhật)

    // ===================== CONTROLS =====================
    private readonly MenuStrip menuStrip = new();
    private readonly StatusStrip statusStrip = new();
    private readonly ToolStripStatusLabel lblShown = new();
    private readonly ToolStripStatusLabel lblCost = new();
    private readonly ToolStripStatusLabel lblMessage = new();

    private readonly TextBox txtId = new();
    private readonly TextBox txtName = new();
    private readonly TextBox txtPhone = new();
    private readonly ComboBox cboDevice = new();
    private readonly ComboBox cboStatus = new();
    private readonly DateTimePicker dtpReceived = new();
    private readonly TextBox txtCost = new();
    private readonly TextBox txtDescription = new();

    private readonly PictureBox picDevice = new();
    private readonly Label lblImageInfo = new();
    private readonly Button btnChooseImage = new();
    private readonly Button btnClearImage = new();
    private readonly ToolTip toolTip = new();

    private readonly Button btnAdd = new();
    private readonly Button btnUpdate = new();
    private readonly Button btnDelete = new();
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
    }

    private RepairOrder? Current => _bs.Current as RepairOrder;

    // =====================================================================
    // 1. GIAO DIỆN RESPONSIVE (TableLayoutPanel + Dock)
    // =====================================================================
    private void BuildUi()
    {
        Text = "Repair Service Tracker - Quản lý đơn sửa chữa";
        Font = new Font("Segoe UI", 10f);
        ClientSize = new Size(1150, 740);
        MinimumSize = new Size(960, 640);
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

        // Hàng trên: [thông tin đơn (giãn) | ảnh thiết bị (cố định 250px)]
        var top = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            ColumnCount = 2,
            RowCount = 1
        };
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250));
        top.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        top.Controls.Add(BuildInputGroup(), 0, 0);
        top.Controls.Add(BuildPicturePanel(), 1, 0);

        root.Controls.Add(top, 0, 0);
        root.Controls.Add(BuildFilterPanel(), 0, 1);
        BuildGrid();
        root.Controls.Add(dgv, 0, 2);

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
        mnuFile.DropDownItems.Add(new ToolStripMenuItem("Thoát", null,
            (s, e) => Close()) { ShortcutKeys = Keys.Control | Keys.Q });

        var mnuOrder = new ToolStripMenuItem("&Đơn sửa chữa");
        mnuOrder.DropDownItems.Add(new ToolStripMenuItem("Tạo đơn mới (xóa trắng form)", null,
            (s, e) => NewForm()) { ShortcutKeys = Keys.Control | Keys.N });
        mnuOrder.DropDownItems.Add(new ToolStripMenuItem("Xóa bộ lọc", null,
            (s, e) => ClearFilters()) { ShortcutKeys = Keys.Control | Keys.R });

        menuStrip.Items.AddRange(new ToolStripItem[] { mnuFile, mnuOrder });
    }

    private void BuildStatusStrip()
    {
        lblShown.BorderSides = ToolStripStatusLabelBorderSides.Right;
        lblShown.Font = new Font(Font, FontStyle.Bold);
        lblCost.BorderSides = ToolStripStatusLabelBorderSides.Right;
        lblCost.Font = new Font(Font, FontStyle.Bold);
        lblMessage.Spring = true;
        lblMessage.TextAlign = ContentAlignment.MiddleLeft;
        statusStrip.Items.AddRange(new ToolStripItem[] { lblShown, lblCost, lblMessage });
    }

    private Control BuildInputGroup()
    {
        var grp = new GroupBox
        {
            Text = "Thông tin đơn sửa chữa",
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
            RowCount = 5
        };
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        for (int i = 0; i < 5; i++) tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        cboDevice.DropDownStyle = ComboBoxStyle.DropDownList;
        cboDevice.Items.AddRange(DeviceTypes);
        cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cboStatus.Items.AddRange(OrderStatus.All);

        dtpReceived.Format = DateTimePickerFormat.Custom;
        dtpReceived.CustomFormat = "dd/MM/yyyy";

        txtCost.TextAlign = HorizontalAlignment.Right;

        txtDescription.Multiline = true;
        txtDescription.ScrollBars = ScrollBars.Vertical;
        txtDescription.AcceptsReturn = true;
        txtDescription.MinimumSize = new Size(0, 64);

        AddField(tlp, "Mã đơn:", txtId, 0, 0);
        AddField(tlp, "Ngày tiếp nhận:", dtpReceived, 0, 2);
        AddField(tlp, "Tên khách hàng:", txtName, 1, 0);
        AddField(tlp, "Loại thiết bị:", cboDevice, 1, 2);
        AddField(tlp, "Số điện thoại:", txtPhone, 2, 0);
        AddField(tlp, "Trạng thái:", cboStatus, 2, 2);
        AddField(tlp, "Mô tả lỗi:", txtDescription, 3, 0);
        tlp.SetColumnSpan(txtDescription, 3);

        // Chi phí đặt ở hàng riêng cho gọn
        // (hàng 3 là Mô tả lỗi nên Chi phí nằm hàng 4 cùng nút bấm bên dưới)
        var costFlow = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            ColumnCount = 4,
            RowCount = 1,
            Margin = new Padding(0)
        };
        costFlow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        costFlow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
        costFlow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        costFlow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        costFlow.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        costFlow.Controls.Add(new Label
        {
            Text = "Chi phí dự kiến:",
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(3, 6, 8, 6)
        }, 0, 0);
        txtCost.Dock = DockStyle.Fill;
        txtCost.Margin = new Padding(3, 4, 26, 4);
        errorProvider.SetIconAlignment(txtCost, ErrorIconAlignment.MiddleRight);
        errorProvider.SetIconPadding(txtCost, 2);
        costFlow.Controls.Add(txtCost, 1, 0);
        costFlow.Controls.Add(new Label
        {
            Text = "VNĐ",
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(0, 6, 3, 6)
        }, 2, 0);

        tlp.Controls.Add(costFlow, 0, 4);
        tlp.SetColumnSpan(costFlow, 2);

        // Nút chức năng (cùng hàng với chi phí, bên phải)
        btnAdd.Text = "Thêm";
        btnUpdate.Text = "Cập nhật";
        btnDelete.Text = "Xóa";
        btnNew.Text = "Tạo mới";
        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Margin = new Padding(0, 2, 0, 0)
        };
        foreach (var b in new[] { btnAdd, btnUpdate, btnDelete, btnNew })
        {
            b.Size = new Size(100, 34);
            flow.Controls.Add(b);
        }
        tlp.Controls.Add(flow, 2, 4);
        tlp.SetColumnSpan(flow, 2);

        btnAdd.Click += BtnAdd_Click;
        btnUpdate.Click += BtnUpdate_Click;
        btnDelete.Click += BtnDelete_Click;
        btnNew.Click += (s, e) => NewForm();

        foreach (Control c in new Control[] { txtId, txtName, txtPhone, txtCost, txtDescription, cboDevice, cboStatus })
        {
            var ctl = c;
            ctl.TextChanged += (s, e) => errorProvider.SetError(ctl, "");
        }

        grp.Controls.Add(tlp);
        return grp;
    }

    private void AddField(TableLayoutPanel tlp, string label, Control ctl, int row, int col)
    {
        var lbl = new Label
        {
            Text = label,
            AutoSize = true,
            Anchor = col == 0 && ctl == txtDescription ? AnchorStyles.Top | AnchorStyles.Left : AnchorStyles.Left,
            Margin = new Padding(3, 6, 8, 6)
        };
        ctl.Dock = DockStyle.Fill;
        ctl.Margin = new Padding(3, 4, 26, 4);
        tlp.Controls.Add(lbl, col, row);
        tlp.Controls.Add(ctl, col + 1, row);
        errorProvider.SetIconAlignment(ctl, ErrorIconAlignment.MiddleRight);
        errorProvider.SetIconPadding(ctl, 2);
    }

    // Khu vực ảnh thiết bị: PictureBox (Zoom) + tên file + nút chọn/bỏ ảnh
    private Control BuildPicturePanel()
    {
        var grp = new GroupBox
        {
            Text = "Ảnh thiết bị",
            Dock = DockStyle.Fill,
            Padding = new Padding(8, 4, 8, 8),
            MinimumSize = new Size(0, 230)
        };

        var tlp = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3
        };
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        picDevice.Dock = DockStyle.Fill;
        picDevice.SizeMode = PictureBoxSizeMode.Zoom;       // giữ đúng tỷ lệ, không méo
        picDevice.BorderStyle = BorderStyle.FixedSingle;
        picDevice.BackColor = Color.WhiteSmoke;
        picDevice.MinimumSize = new Size(120, 120);

        lblImageInfo.Dock = DockStyle.Fill;
        lblImageInfo.AutoSize = false;
        lblImageInfo.Height = 24;
        lblImageInfo.TextAlign = ContentAlignment.MiddleCenter;
        lblImageInfo.AutoEllipsis = true;
        lblImageInfo.ForeColor = Color.DimGray;

        btnChooseImage.Text = "Chọn ảnh...";
        btnChooseImage.Size = new Size(105, 32);
        btnChooseImage.Click += BtnChooseImage_Click;
        btnClearImage.Text = "Bỏ ảnh";
        btnClearImage.Size = new Size(85, 32);
        btnClearImage.Click += (s, e) =>
        {
            _imagePath = "";
            ShowImage("");
            SetMessage("Đã bỏ ảnh. Bấm Cập nhật để lưu thay đổi.");
        };

        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0)
        };
        flow.Controls.Add(btnChooseImage);
        flow.Controls.Add(btnClearImage);

        tlp.Controls.Add(picDevice, 0, 0);
        tlp.Controls.Add(lblImageInfo, 0, 1);
        tlp.Controls.Add(flow, 0, 2);
        grp.Controls.Add(tlp);
        return grp;
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

        txtSearch.Width = 320;
        txtSearch.PlaceholderText = "Tìm theo mã đơn, tên khách hàng hoặc SĐT...";
        txtSearch.Margin = new Padding(3, 4, 18, 3);

        cboFilterStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cboFilterStatus.Width = 160;
        cboFilterStatus.Items.Add("Tất cả");
        cboFilterStatus.Items.AddRange(OrderStatus.All);
        cboFilterStatus.SelectedIndex = 0;
        cboFilterStatus.Margin = new Padding(3, 4, 18, 3);

        btnClearFilter.Text = "Xóa bộ lọc";
        btnClearFilter.Size = new Size(110, 30);

        flow.Controls.Add(new Label { Text = "Tìm kiếm:", AutoSize = true, Margin = new Padding(3, 8, 3, 3) });
        flow.Controls.Add(txtSearch);
        flow.Controls.Add(new Label { Text = "Trạng thái:", AutoSize = true, Margin = new Padding(3, 8, 3, 3) });
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
        dgv.AutoGenerateColumns = false;
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgv.MultiSelect = false;
        dgv.ReadOnly = true;
        dgv.AllowUserToAddRows = false;
        dgv.AllowUserToDeleteRows = false;
        dgv.RowHeadersVisible = false;
        dgv.BackgroundColor = SystemColors.Window;
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        dgv.Columns.Add(new DataGridViewTextBoxColumn
        { DataPropertyName = nameof(RepairOrder.OrderId), HeaderText = "Mã đơn", FillWeight = 9 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        { DataPropertyName = nameof(RepairOrder.CustomerName), HeaderText = "Khách hàng", FillWeight = 16 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        { DataPropertyName = nameof(RepairOrder.Phone), HeaderText = "Số điện thoại", FillWeight = 12 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        { DataPropertyName = nameof(RepairOrder.DeviceType), HeaderText = "Loại thiết bị", FillWeight = 12 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        { DataPropertyName = nameof(RepairOrder.Description), HeaderText = "Mô tả lỗi", FillWeight = 22 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(RepairOrder.ReceivedDate),
            HeaderText = "Ngày tiếp nhận",
            FillWeight = 12,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
        });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(RepairOrder.EstimatedCost),
            HeaderText = "Chi phí dự kiến",
            FillWeight = 13,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Format = "N0",                                   // định dạng chi phí N0
                Alignment = DataGridViewContentAlignment.MiddleRight
            }
        });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        { DataPropertyName = nameof(RepairOrder.Status), HeaderText = "Trạng thái", FillWeight = 12 });

        dgv.DataError += (s, e) => e.ThrowException = false;
        dgv.CellFormatting += Dgv_CellFormatting;
        dgv.CellClick += (s, e) => { if (e.RowIndex >= 0) LoadCurrentToForm(); };
    }

    private void Dgv_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0) return;
        if (dgv.Columns[e.ColumnIndex].DataPropertyName != nameof(RepairOrder.Status)) return;

        e.CellStyle!.ForeColor = (e.Value as string) switch
        {
            OrderStatus.Done => Color.SeaGreen,
            OrderStatus.Cancelled => Color.Gray,
            OrderStatus.WaitingParts => Color.DarkOrange,
            OrderStatus.Repairing => Color.RoyalBlue,
            _ => Color.DarkMagenta
        };
    }

    // =====================================================================
    // 2. DỮ LIỆU MẪU
    // =====================================================================
    private void SeedData()
    {
        DateTime t = DateTime.Today;
        _all.AddRange(new[]
        {
            Make("DH001", "Nguyễn Văn An",  "0912345678", "Điện thoại",    "Vỡ màn hình",            t.AddDays(-5),  1_200_000, OrderStatus.Repairing),
            Make("DH002", "Trần Thị Bình",  "0987654321", "Laptop",        "Không lên nguồn",        t.AddDays(-4),  2_500_000, OrderStatus.WaitingParts),
            Make("DH003", "Lê Hoàng Cường", "0369852147", "Máy giặt",      "Không vắt được",         t.AddDays(-3),    800_000, OrderStatus.New),
            Make("DH004", "Phạm Thu Dung",  "0903123456", "Tivi",          "Mất hình, còn tiếng",    t.AddDays(-10), 1_800_000, OrderStatus.Done),
            Make("DH005", "Hoàng Minh Đức", "0775123987", "Máy in",        "Kẹt giấy liên tục",      t.AddDays(-2),    350_000, OrderStatus.Repairing),
            Make("DH006", "Vũ Ngọc Hà",     "0584123456", "Tủ lạnh",       "Không đủ lạnh",          t.AddDays(-7),    900_000, OrderStatus.Cancelled),
            Make("DH007", "Đặng Quốc Huy",  "0898765432", "Máy tính bảng", "Pin chai, sạc chậm",     t.AddDays(-1),    600_000, OrderStatus.New),
        });
    }

    private static RepairOrder Make(string id, string name, string phone, string device, string desc,
                                    DateTime date, decimal cost, string status) => new()
    {
        OrderId = id, CustomerName = name, Phone = phone, DeviceType = device, Description = desc,
        ReceivedDate = date, EstimatedCost = cost, Status = status
    };

    // =====================================================================
    // 3. LỌC / TÌM KIẾM
    // =====================================================================
    private void ApplyFilter(string? selectId = null)
    {
        if (_suspendFilter) return;

        var prev = _bs.Current;
        string? keepId = selectId ?? (prev as RepairOrder)?.OrderId;

        string kw = txtSearch.Text.Trim();
        string kwName = RemoveDiacritics(kw).ToLowerInvariant();
        string kwPhone = Regex.Replace(kw, @"[\s.\-()]", "");
        string? status = cboFilterStatus.SelectedIndex > 0 ? cboFilterStatus.SelectedItem as string : null;

        var result = _all.Where(o =>
        {
            if (kw.Length > 0)
            {
                bool idOk = o.OrderId.Contains(kw, StringComparison.OrdinalIgnoreCase);
                bool nameOk = RemoveDiacritics(o.CustomerName).ToLowerInvariant().Contains(kwName);
                bool phoneOk = kwPhone.Length > 0 &&
                               Regex.Replace(o.Phone, @"[\s.\-()]", "").Contains(kwPhone);
                if (!idOk && !nameOk && !phoneOk) return false;
            }
            if (status != null && o.Status != status) return false;
            return true;
        }).OrderBy(o => o.OrderId).ToList();

        _loading = true;
        try
        {
            _view.RaiseListChangedEvents = false;
            _view.Clear();
            foreach (var o in result) _view.Add(o);
            _view.RaiseListChangedEvents = true;
            _view.ResetBindings();

            if (keepId != null)
            {
                int idx = result.FindIndex(o => o.OrderId == keepId);
                if (idx >= 0) _bs.Position = idx;
            }
        }
        finally { _loading = false; }

        if (_bs.Current != null && !ReferenceEquals(prev, _bs.Current)) LoadCurrentToForm();

        btnUpdate.Enabled = btnDelete.Enabled = _bs.Current != null;
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
        decimal openAll = _all.Where(o => o.IsOpen).Sum(o => o.EstimatedCost);
        decimal openView = _view.Where(o => o.IsOpen).Sum(o => o.EstimatedCost);
        int openCount = _all.Count(o => o.IsOpen);

        lblShown.Text = $"Số đơn theo bộ lọc: {_view.Count} / {_all.Count}";
        lblCost.Text = $"Tổng chi phí dự kiến đơn chưa hoàn tất ({openCount} đơn): {openAll:N0} đ" +
                       $"   |   trong bộ lọc: {openView:N0} đ";
    }

    private void SetMessage(string msg) =>
        lblMessage.Text = DateTime.Now.ToString("HH:mm:ss") + " - " + msg;

    // =====================================================================
    // 4. THÊM / CẬP NHẬT / XÓA
    // =====================================================================
    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        if (!ValidateForm(null)) return;

        if (MessageBox.Show($"Thêm đơn sửa chữa mới của khách hàng \"{txtName.Text.Trim()}\"?", "Xác nhận thêm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        var o = new RepairOrder();
        FillFromForm(o);
        _all.Add(o);
        ApplyFilter(o.OrderId);

        SetMessage(_view.Contains(o)
            ? $"Đã thêm đơn {o.OrderId}."
            : $"Đã thêm đơn {o.OrderId} (không nằm trong bộ lọc hiện tại nên chưa hiển thị).");
    }

    private void BtnUpdate_Click(object? sender, EventArgs e)
    {
        var cur = Current;
        if (cur == null)
        {
            MessageBox.Show("Hãy chọn một đơn trong danh sách để cập nhật.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (!ValidateForm(cur)) return;

        if (MessageBox.Show($"Cập nhật đơn {cur.OrderId} của \"{cur.CustomerName}\"?", "Xác nhận cập nhật",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        FillFromForm(cur);
        ApplyFilter(cur.OrderId);
        SetMessage(_view.Contains(cur)
            ? $"Đã cập nhật đơn {cur.OrderId}."
            : $"Đã cập nhật đơn {cur.OrderId} (đơn không còn nằm trong bộ lọc hiện tại).");
    }

    private void BtnDelete_Click(object? sender, EventArgs e)
    {
        var cur = Current;
        if (cur == null)
        {
            MessageBox.Show("Hãy chọn một đơn trong danh sách để xóa.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (MessageBox.Show($"Xóa đơn {cur.OrderId} của \"{cur.CustomerName}\"?\nThao tác này không thể hoàn tác.",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

        string id = cur.OrderId;
        _all.Remove(cur);
        ApplyFilter();
        SetMessage($"Đã xóa đơn {id}.");
    }

    // =====================================================================
    // 5. CHỌN ẢNH (OpenFileDialog) + HIỂN THỊ (PictureBox Zoom)
    // =====================================================================
    private void BtnChooseImage_Click(object? sender, EventArgs e)
    {
        using var ofd = new OpenFileDialog
        {
            Title = "Chọn ảnh thiết bị",
            Filter = "Ảnh (*.jpg;*.jpeg;*.png;*.bmp;*.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif"
        };
        if (ofd.ShowDialog(this) != DialogResult.OK) return;

        _imagePath = ofd.FileName;
        ShowImage(_imagePath);
        SetMessage("Đã chọn ảnh thiết bị. Bấm Thêm hoặc Cập nhật để lưu vào đơn.");
    }

    private void ShowImage(string path)
    {
        var old = picDevice.Image;
        picDevice.Image = null;
        old?.Dispose();
        toolTip.SetToolTip(lblImageInfo, path);

        if (string.IsNullOrWhiteSpace(path)) { lblImageInfo.Text = "Chưa có ảnh"; return; }
        if (!File.Exists(path)) { lblImageInfo.Text = "Không tìm thấy file ảnh"; return; }

        try
        {
            // Đọc qua stream rồi nhân bản để không khóa file ảnh
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            using var img = Image.FromStream(fs);
            picDevice.Image = new Bitmap(img);
            lblImageInfo.Text = Path.GetFileName(path);
        }
        catch
        {
            lblImageInfo.Text = "File không phải ảnh hợp lệ";
        }
    }

    // =====================================================================
    // 6. KIỂM TRA DỮ LIỆU (ErrorProvider)
    // =====================================================================
    private bool ValidateForm(RepairOrder? editing)
    {
        ClearErrors();
        Control? first = null;

        void Fail(Control c, string msg)
        {
            errorProvider.SetError(c, msg);
            first ??= c;
        }

        // Mã đơn: bắt buộc + không trùng
        string id = txtId.Text.Trim();
        if (id.Length == 0)
            Fail(txtId, "Mã đơn không được để trống.");
        else if (_all.Any(o => !ReferenceEquals(o, editing) &&
                               string.Equals(o.OrderId, id, StringComparison.OrdinalIgnoreCase)))
            Fail(txtId, $"Mã đơn \"{id}\" đã tồn tại.");

        if (txtName.Text.Trim().Length == 0)
            Fail(txtName, "Tên khách hàng không được để trống.");

        if (txtPhone.Text.Trim().Length == 0)
            Fail(txtPhone, "Số điện thoại không được để trống.");

        if (cboDevice.SelectedIndex < 0)
            Fail(cboDevice, "Vui lòng chọn loại thiết bị.");

        if (txtDescription.Text.Trim().Length == 0)
            Fail(txtDescription, "Vui lòng nhập mô tả lỗi.");

        if (!TryParseCost(txtCost.Text, out _, out string costError))
            Fail(txtCost, costError);

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

    // Chi phí: số nguyên VNĐ, cho phép dấu phân cách hàng nghìn (1.500.000 hoặc 1,500,000)
    private static bool TryParseCost(string input, out decimal value, out string error)
    {
        value = 0;
        error = "";

        string s = Regex.Replace(input ?? "", @"(?i)vnđ|vnd|đ|₫|\s", "");
        if (s.Length == 0)
        {
            error = "Vui lòng nhập chi phí dự kiến (nhập 0 nếu chưa có).";
            return false;
        }
        if (!Regex.IsMatch(s, @"^-?\d+(?:[.,]\d{3})*$"))
        {
            error = "Chi phí phải là một số (VD: 1500000 hoặc 1.500.000).";
            return false;
        }

        bool negative = s.StartsWith("-");
        string digits = s.Replace("-", "").Replace(".", "").Replace(",", "");
        if (!decimal.TryParse(digits, NumberStyles.None, CultureInfo.InvariantCulture, out value))
        {
            error = "Chi phí quá lớn.";
            return false;
        }
        if (negative && value != 0)
        {
            value = 0;
            error = "Chi phí dự kiến không được là số âm.";
            return false;
        }
        return true;
    }

    private void ClearErrors()
    {
        foreach (Control c in new Control[]
                 { txtId, txtName, txtPhone, cboDevice, cboStatus, dtpReceived, txtCost, txtDescription })
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
    // 7. FORM <-> ĐỐI TƯỢNG
    // =====================================================================
    private void FillFromForm(RepairOrder o)
    {
        TryParseCost(txtCost.Text, out decimal cost, out _);

        o.OrderId = txtId.Text.Trim();
        o.CustomerName = txtName.Text.Trim();
        o.Phone = txtPhone.Text.Trim();
        o.DeviceType = cboDevice.SelectedItem as string ?? "";
        o.Description = txtDescription.Text.Trim();
        o.ReceivedDate = dtpReceived.Value.Date;
        o.EstimatedCost = cost;
        o.Status = cboStatus.SelectedItem as string ?? OrderStatus.New;
        o.ImagePath = _imagePath;
    }

    private void LoadCurrentToForm()
    {
        if (Current is not RepairOrder o) return;

        txtId.Text = o.OrderId;
        txtName.Text = o.CustomerName;
        txtPhone.Text = o.Phone;
        cboDevice.SelectedItem = o.DeviceType;
        txtDescription.Text = o.Description;
        dtpReceived.Value = o.ReceivedDate;
        txtCost.Text = o.EstimatedCost.ToString("N0");
        cboStatus.SelectedItem = o.Status;

        _imagePath = o.ImagePath;
        ShowImage(_imagePath);
        ClearErrors();
    }

    private void NewForm()
    {
        ClearErrors();
        txtId.Text = NextId();
        txtName.Clear();
        txtPhone.Clear();
        txtDescription.Clear();
        txtCost.Text = "0";
        dtpReceived.Value = DateTime.Today;
        cboDevice.SelectedIndex = 0;
        cboStatus.SelectedItem = OrderStatus.New;
        _imagePath = "";
        ShowImage("");
        txtName.Focus();
    }

    private string NextId()
    {
        int max = 0;
        foreach (var o in _all)
        {
            var m = Regex.Match(o.OrderId, @"(\d+)$");
            if (m.Success && int.TryParse(m.Groups[1].Value, out int n) && n > max) max = n;
        }
        return "DH" + (max + 1).ToString("D3");
    }

    // =====================================================================
    // 8. XUẤT CSV (danh sách đang hiển thị)
    // =====================================================================
    private void ExportCsv()
    {
        if (_view.Count == 0)
        {
            MessageBox.Show("Không có đơn nào đang hiển thị để xuất.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var sfd = new SaveFileDialog
        {
            Title = "Xuất danh sách đơn sửa chữa",
            Filter = "CSV (*.csv)|*.csv",
            FileName = $"DonSuaChua_{DateTime.Now:yyyyMMdd_HHmm}.csv"
        };
        if (sfd.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            using var sw = new StreamWriter(sfd.FileName, false, new UTF8Encoding(true));   // UTF-8 BOM cho Excel
            sw.WriteLine("Mã đơn,Tên khách hàng,Số điện thoại,Loại thiết bị,Mô tả lỗi,Ngày tiếp nhận,Chi phí dự kiến,Trạng thái,Đường dẫn ảnh");
            foreach (var o in _view)
            {
                sw.WriteLine(string.Join(",",
                    Csv(o.OrderId), Csv(o.CustomerName), Csv(o.Phone), Csv(o.DeviceType), Csv(o.Description),
                    Csv(o.ReceivedDate.ToString("dd/MM/yyyy")),
                    o.EstimatedCost.ToString("0", CultureInfo.InvariantCulture),
                    Csv(o.Status), Csv(o.ImagePath)));
            }
            SetMessage($"Đã xuất {_view.Count} đơn ra file CSV.");
            MessageBox.Show($"Xuất thành công {_view.Count} đơn.", "Thông báo",
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
        picDevice.Image?.Dispose();
        base.OnFormClosed(e);
    }
}
