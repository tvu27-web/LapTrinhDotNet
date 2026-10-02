using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace TechMartProductManager;

public class MainForm : Form
{
    private readonly BindingList<Product> products = new();
    private readonly BindingSource bindingSource = new();
    private readonly ErrorProvider errorProvider = new();
    private Product? selectedProduct;
    private string selectedImagePath = "";

    private TextBox txtProductId = null!;
    private TextBox txtProductName = null!;
    private TextBox txtUnitPrice = null!;
    private TextBox txtQuantity = null!;
    private ComboBox cboCategory = null!;
    private TextBox txtSearch = null!;
    private PictureBox picAvatar = null!;
    private DataGridView dgvProducts = null!;
    private Button btnChooseImage = null!;
    private Button btnAdd = null!;
    private Button btnUpdate = null!;
    private Button btnDelete = null!;
    private ToolStripStatusLabel statusLabel = null!;

    public MainForm()
    {
        Text = "TechMart Product Manager";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1000, 650);
        Size = new Size(1200, 750);
        Font = new Font("Segoe UI", 10F);

        BuildInterface();
        InitializeData();
        ConfigureGrid();
        RefreshGrid();
        ClearInput();
    }

    private void BuildInterface()
    {
        var menuStrip = new MenuStrip();
        var fileMenu = new ToolStripMenuItem("File");
        var exportMenu = new ToolStripMenuItem("Export CSV", null, ExportCsv_Click)
        {
            ShortcutKeys = Keys.Control | Keys.E
        };
        var exitMenu = new ToolStripMenuItem("Exit", null, (_, _) => Close())
        {
            ShortcutKeys = Keys.Control | Keys.X
        };
        fileMenu.DropDownItems.Add(exportMenu);
        fileMenu.DropDownItems.Add(new ToolStripSeparator());
        fileMenu.DropDownItems.Add(exitMenu);
        menuStrip.Items.Add(fileMenu);
        MainMenuStrip = menuStrip;
        Controls.Add(menuStrip);

        var statusStrip = new StatusStrip();
        statusLabel = new ToolStripStatusLabel("Tổng số sản phẩm: 0");
        statusStrip.Items.Add(statusLabel);
        Controls.Add(statusStrip);

        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(10),
            AutoSize = false
        };
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        mainLayout.Dock = DockStyle.Fill;
        Controls.Add(mainLayout);
        mainLayout.BringToFront();
        menuStrip.BringToFront();
        statusStrip.BringToFront();

        var left = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 9,
            AutoScroll = true,
            Padding = new Padding(5)
        };
        left.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
        left.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
        for (int i = 0; i < 9; i++)
            left.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        AddLabelAndControl(left, 0, "Mã SP", out txtProductId);
        AddLabelAndControl(left, 1, "Tên SP", out txtProductName);
        AddLabelAndControl(left, 2, "Đơn giá", out txtUnitPrice);
        AddLabelAndControl(left, 3, "Số lượng", out txtQuantity);

        var categoryLabel = new Label { Text = "Danh mục", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(3, 9, 3, 3) };
        left.Controls.Add(categoryLabel, 0, 4);
        cboCategory = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill, Margin = new Padding(3) };
        left.Controls.Add(cboCategory, 1, 4);

        var imageLabel = new Label { Text = "Ảnh", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(3, 9, 3, 3) };
        left.Controls.Add(imageLabel, 0, 5);
        picAvatar = new PictureBox
        {
            Dock = DockStyle.Fill,
            Height = 180,
            SizeMode = PictureBoxSizeMode.Zoom,
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = SystemColors.ControlLight,
            Margin = new Padding(3)
        };
        left.Controls.Add(picAvatar, 1, 5);

        btnChooseImage = new Button { Text = "Chọn ảnh", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(3) };
        btnChooseImage.Click += BtnChooseImage_Click;
        left.Controls.Add(new Label(), 0, 6);
        left.Controls.Add(btnChooseImage, 1, 6);

        var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true, Margin = new Padding(3) };
        btnAdd = new Button { Text = "Thêm", AutoSize = true, Margin = new Padding(3) };
        btnUpdate = new Button { Text = "Cập nhật", AutoSize = true, Margin = new Padding(3) };
        btnDelete = new Button { Text = "Xóa", AutoSize = true, Margin = new Padding(3) };
        btnAdd.Click += BtnAdd_Click;
        btnUpdate.Click += BtnUpdate_Click;
        btnDelete.Click += BtnDelete_Click;
        buttonPanel.Controls.Add(btnAdd);
        buttonPanel.Controls.Add(btnUpdate);
        buttonPanel.Controls.Add(btnDelete);
        left.Controls.Add(new Label(), 0, 7);
        left.Controls.Add(buttonPanel, 1, 7);
        left.SetColumnSpan(buttonPanel, 1);

        var note = new Label
        {
            Text = "Giá nhập được phép dùng dấu . hoặc , để phân cách hàng nghìn.",
            AutoSize = true,
            MaximumSize = new Size(320, 0),
            ForeColor = SystemColors.GrayText,
            Margin = new Padding(3, 10, 3, 3)
        };
        left.Controls.Add(note, 0, 8);
        left.SetColumnSpan(note, 2);

        var right = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(5)
        };
        right.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        right.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        txtSearch = new TextBox { Dock = DockStyle.Fill, PlaceholderText = "Tìm kiếm theo tên sản phẩm...", Margin = new Padding(3) };
        txtSearch.TextChanged += TxtSearch_TextChanged;
        right.Controls.Add(txtSearch, 0, 0);

        dgvProducts = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible = false,
            Margin = new Padding(3)
        };
        dgvProducts.SelectionChanged += DgvProducts_SelectionChanged;
        right.Controls.Add(dgvProducts, 0, 1);

        mainLayout.Controls.Add(left, 0, 0);
        mainLayout.Controls.Add(right, 1, 0);

        errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
    }

    private static void AddLabelAndControl(TableLayoutPanel layout, int row, string labelText, out TextBox textBox)
    {
        var label = new Label { Text = labelText, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(3, 9, 3, 3) };
        layout.Controls.Add(label, 0, row);
        textBox = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(3) };
        layout.Controls.Add(textBox, 1, row);
    }

    private void InitializeData()
    {
        var categories = new List<CategoryItem>
        {
            new("Điện thoại", "PHONE"),
            new("Laptop", "LAPTOP"),
            new("Phụ kiện", "ACCESSORY")
        };
        cboCategory.DataSource = categories;
        cboCategory.DisplayMember = nameof(CategoryItem.Name);
        cboCategory.ValueMember = nameof(CategoryItem.Value);
        cboCategory.SelectedIndex = 0;
        bindingSource.DataSource = products;
    }

    private void ConfigureGrid()
    {
        dgvProducts.Columns.Clear();
        dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mã SP", DataPropertyName = nameof(Product.ProductId), FillWeight = 18 });
        dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tên SP", DataPropertyName = nameof(Product.ProductName), FillWeight = 30 });
        dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Danh Mục", DataPropertyName = nameof(Product.Category), FillWeight = 22 });
        dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Đơn Giá", DataPropertyName = nameof(Product.UnitPrice), DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" }, FillWeight = 22 });
        dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Số Lượng", DataPropertyName = nameof(Product.Quantity), FillWeight = 15 });
    }

    private void RefreshGrid()
    {
        string keyword = txtSearch?.Text.Trim() ?? "";
        IEnumerable<Product> result = products;
        if (keyword.Length > 0)
            result = products.Where(p => p.ProductName.Contains(keyword, StringComparison.OrdinalIgnoreCase));

        bindingSource.DataSource = new BindingList<Product>(result.ToList());
        dgvProducts.DataSource = bindingSource;
        UpdateStatus();
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        if (!ValidateInput())
            return;

        var product = ReadInput();
        products.Add(product);
        RefreshGrid();
        ClearInput();
    }

    private void BtnUpdate_Click(object? sender, EventArgs e)
    {
        if (selectedProduct == null)
        {
            MessageBox.Show("Hãy chọn sản phẩm cần cập nhật.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (!ValidateInput())
            return;

        var product = ReadInput();
        selectedProduct.ProductId = product.ProductId;
        selectedProduct.ProductName = product.ProductName;
        selectedProduct.Category = product.Category;
        selectedProduct.CategoryValue = product.CategoryValue;
        selectedProduct.UnitPrice = product.UnitPrice;
        selectedProduct.Quantity = product.Quantity;
        selectedProduct.ImagePath = product.ImagePath;
        RefreshGrid();
        ClearInput();
    }

    private void BtnDelete_Click(object? sender, EventArgs e)
    {
        if (selectedProduct == null)
        {
            MessageBox.Show("Hãy chọn sản phẩm cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        DialogResult result = MessageBox.Show(
            $"Bạn có chắc chắn muốn xóa sản phẩm '{selectedProduct.ProductName}' không?",
            "Xác nhận xóa",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result != DialogResult.Yes)
            return;

        products.Remove(selectedProduct);
        RefreshGrid();
        ClearInput();
    }

    private void BtnChooseImage_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Chọn ảnh sản phẩm",
            Filter = "Image files (*.jpg;*.jpeg;*.png;*.bmp;*.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All files (*.*)|*.*"
        };

        if (dialog.ShowDialog() != DialogResult.OK)
            return;

        selectedImagePath = dialog.FileName;
        LoadImage(selectedImagePath);
    }

    private void TxtSearch_TextChanged(object? sender, EventArgs e)
    {
        RefreshGrid();
    }

    private void DgvProducts_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgvProducts.CurrentRow?.DataBoundItem is not Product product)
            return;

        selectedProduct = product;
        txtProductId.Text = product.ProductId;
        txtProductName.Text = product.ProductName;
        txtUnitPrice.Text = product.UnitPrice.ToString("N0", CultureInfo.InvariantCulture);
        txtQuantity.Text = product.Quantity.ToString();
        cboCategory.SelectedValue = product.CategoryValue;
        selectedImagePath = product.ImagePath;
        LoadImage(selectedImagePath);
        ClearErrors();
    }

    private bool ValidateInput()
    {
        ClearErrors();
        bool valid = true;

        if (string.IsNullOrWhiteSpace(txtProductName.Text))
        {
            errorProvider.SetError(txtProductName, "Tên sản phẩm không được để trống.");
            valid = false;
        }

        if (!TryParsePrice(txtUnitPrice.Text, out decimal price) || price <= 0)
        {
            errorProvider.SetError(txtUnitPrice, "Đơn giá phải lớn hơn 0.");
            valid = false;
        }

        if (!int.TryParse(txtQuantity.Text.Trim(), out int quantity) || quantity < 0)
        {
            errorProvider.SetError(txtQuantity, "Số lượng phải là số nguyên >= 0.");
            valid = false;
        }

        return valid;
    }

    private Product ReadInput()
    {
        TryParsePrice(txtUnitPrice.Text, out decimal price);
        int.TryParse(txtQuantity.Text.Trim(), out int quantity);
        var category = cboCategory.SelectedItem as CategoryItem;

        return new Product
        {
            ProductId = txtProductId.Text.Trim(),
            ProductName = txtProductName.Text.Trim(),
            Category = category?.Name ?? "",
            CategoryValue = category?.Value ?? "",
            UnitPrice = price,
            Quantity = quantity,
            ImagePath = selectedImagePath
        };
    }

    private static bool TryParsePrice(string text, out decimal price)
    {
        string cleaned = text.Trim().Replace(",", "").Replace(".", "");
        return decimal.TryParse(cleaned, NumberStyles.Number, CultureInfo.InvariantCulture, out price);
    }

    private void ClearInput()
    {
        selectedProduct = null;
        selectedImagePath = "";
        txtProductId.Clear();
        txtProductName.Clear();
        txtUnitPrice.Clear();
        txtQuantity.Clear();
        if (cboCategory.Items.Count > 0)
            cboCategory.SelectedIndex = 0;
        ClearErrors();
        ClearImage();
        dgvProducts.ClearSelection();
    }

    private void ClearErrors()
    {
        errorProvider.SetError(txtProductName, "");
        errorProvider.SetError(txtUnitPrice, "");
        errorProvider.SetError(txtQuantity, "");
    }

    private void LoadImage(string path)
    {
        ClearImage();
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            return;

        try
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            using var temp = Image.FromStream(stream);
            picAvatar.Image = new Bitmap(temp);
        }
        catch
        {
            picAvatar.Image = null;
        }
    }

    private void ClearImage()
    {
        Image? old = picAvatar.Image;
        picAvatar.Image = null;
        old?.Dispose();
    }

    private void UpdateStatus()
    {
        statusLabel.Text = $"Tổng số sản phẩm: {products.Count}";
    }

    private void ExportCsv_Click(object? sender, EventArgs e)
    {
        using var dialog = new SaveFileDialog
        {
            Title = "Xuất danh sách sản phẩm",
            Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
            FileName = "products.csv"
        };

        if (dialog.ShowDialog() != DialogResult.OK)
            return;

        var lines = new List<string>
        {
            "Mã SP,Tên SP,Danh Mục,Đơn Giá,Số Lượng"
        };

        foreach (var p in products)
        {
            lines.Add(string.Join(",",
                EscapeCsv(p.ProductId),
                EscapeCsv(p.ProductName),
                EscapeCsv(p.Category),
                p.UnitPrice.ToString("0", CultureInfo.InvariantCulture),
                p.Quantity.ToString(CultureInfo.InvariantCulture)));
        }

        File.WriteAllLines(dialog.FileName, lines, new UTF8Encoding(true));
        MessageBox.Show("Đã xuất file CSV thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private static string EscapeCsv(string value)
    {
        if (value.Contains('"'))
            value = value.Replace("\"", "\"\"");

        if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            return $"\"{value}\"";

        return value;
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        ClearImage();
        errorProvider.Dispose();
        base.OnFormClosed(e);
    }

    private sealed record CategoryItem(string Name, string Value)
    {
        public override string ToString() => Name;
    }
}
