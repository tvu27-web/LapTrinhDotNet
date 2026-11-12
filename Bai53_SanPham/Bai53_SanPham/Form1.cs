using System.Globalization;
using System.Text;

namespace Bai53_SanPham
{
    public partial class Form1 : Form
    {
        private static readonly string[] Categories =
            { "Điện tử", "Gia dụng", "Thời trang", "Thực phẩm", "Văn phòng phẩm", "Khác" };

        // Dữ liệu gốc nằm trong bộ nhớ: List<Product>
        private readonly List<Product> _products = new();

        private string _keyword = "";     // từ khóa tìm kiếm đang áp dụng (rỗng = hiện tất cả)
        private bool _loading;            // chặn sự kiện khi đang nạp lại lưới

        public Form1()
        {
            InitializeComponent();

            BuildColumns();
            cboCategory.Items.AddRange(Categories);
            cboCategory.SelectedIndex = 0;

            // ----- Gắn sự kiện -----
            btnAdd.Click += btnAdd_Click;
            btnEdit.Click += btnEdit_Click;
            btnDelete.Click += btnDelete_Click;
            btnSearch.Click += btnSearch_Click;
            btnShowAll.Click += btnShowAll_Click;
            txtSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    btnSearch_Click(s, EventArgs.Empty);
                }
            };

            // Yêu cầu 3: click vào dòng -> đẩy dữ liệu lên các ô nhập
            dgvProducts.CellClick += dgvProducts_CellClick;
            bsProducts.CurrentChanged += (s, e) => { if (!_loading) LoadToInputs(); };   // đổi dòng bằng phím mũi tên

            // Sửa lại ô nào thì xóa biểu tượng lỗi của ô đó
            txtId.TextChanged += (s, e) => epCheck.SetError(txtId, "");
            txtName.TextChanged += (s, e) => epCheck.SetError(txtName, "");

            dgvProducts.DataSource = bsProducts;     // BindingSource làm trung gian

            SeedData();
            RefreshGrid();
        }

        // ------------------------------------------------------------
        // Cột của DataGridView (hiển thị các thuộc tính của Product)
        // ------------------------------------------------------------
        private void BuildColumns()
        {
            dgvProducts.AutoGenerateColumns = false;
            dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            { DataPropertyName = nameof(Product.ProductId), HeaderText = "Mã SP", FillWeight = 14 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            { DataPropertyName = nameof(Product.ProductName), HeaderText = "Tên sản phẩm", FillWeight = 36 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Product.UnitPrice),
                HeaderText = "Đơn giá",
                FillWeight = 18,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N0",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Product.Quantity),
                HeaderText = "Số lượng",
                FillWeight = 12,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            { DataPropertyName = nameof(Product.Category), HeaderText = "Danh mục", FillWeight = 20 });
        }

        private void SeedData()
        {
            _products.Add(new Product { ProductId = "SP001", ProductName = "Bàn phím cơ",      UnitPrice = 850_000,   Quantity = 15,  Category = "Điện tử" });
            _products.Add(new Product { ProductId = "SP002", ProductName = "Chuột không dây",  UnitPrice = 320_000,   Quantity = 40,  Category = "Điện tử" });
            _products.Add(new Product { ProductId = "SP003", ProductName = "Nồi cơm điện",     UnitPrice = 1_200_000, Quantity = 12,  Category = "Gia dụng" });
            _products.Add(new Product { ProductId = "SP004", ProductName = "Áo thun nam",      UnitPrice = 180_000,   Quantity = 60,  Category = "Thời trang" });
            _products.Add(new Product { ProductId = "SP005", ProductName = "Bánh quy bơ",      UnitPrice = 45_000,    Quantity = 200, Category = "Thực phẩm" });
            _products.Add(new Product { ProductId = "SP006", ProductName = "Sổ tay A5",        UnitPrice = 25_000,    Quantity = 150, Category = "Văn phòng phẩm" });
        }

        // ------------------------------------------------------------
        // Làm mới DataGridView từ List<Product> (có áp dụng từ khóa tìm kiếm)
        // ------------------------------------------------------------
        private void RefreshGrid(string? selectId = null, bool loadInputs = true)
        {
            string? keep = selectId ?? (bsProducts.Current as Product)?.ProductId;

            IEnumerable<Product> query = _products;
            if (_keyword.Length > 0)
            {
                string k = RemoveDiacritics(_keyword).ToLowerInvariant();
                query = query.Where(p => RemoveDiacritics(p.ProductName).ToLowerInvariant().Contains(k));
            }
            var shown = query.ToList();

            _loading = true;
            try
            {
                bsProducts.DataSource = shown;          // List<Product> là nguồn dữ liệu của BindingSource
                bsProducts.ResetBindings(false);
                if (keep != null)
                {
                    int idx = shown.FindIndex(p => p.ProductId == keep);
                    if (idx >= 0) bsProducts.Position = idx;
                }
            }
            finally { _loading = false; }

            if (loadInputs && bsProducts.Current != null) LoadToInputs();

            btnEdit.Enabled = btnDelete.Enabled = shown.Count > 0;
            lblCount.Text = $"Đang hiển thị: {shown.Count} / {_products.Count} sản phẩm";
        }

        // ------------------------------------------------------------
        // Yêu cầu 3: đẩy dữ liệu dòng đang chọn lên các ô nhập
        // ------------------------------------------------------------
        private void dgvProducts_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) LoadToInputs();
        }

        private void LoadToInputs()
        {
            if (bsProducts.Current is not Product p) return;

            txtId.Text = p.ProductId;
            txtName.Text = p.ProductName;
            nudUnitPrice.Value = Math.Clamp(p.UnitPrice, nudUnitPrice.Minimum, nudUnitPrice.Maximum);
            nudQuantity.Value = Math.Clamp(p.Quantity, (int)nudQuantity.Minimum, (int)nudQuantity.Maximum);
            cboCategory.SelectedItem = p.Category;
            epCheck.Clear();
        }

        private void FillFromInputs(Product p)
        {
            p.ProductId = txtId.Text.Trim();
            p.ProductName = txtName.Text.Trim();
            p.UnitPrice = nudUnitPrice.Value;
            p.Quantity = (int)nudQuantity.Value;
            p.Category = cboCategory.SelectedItem as string ?? "";
        }

        private void ClearInputs()
        {
            txtId.Text = NextId();
            txtName.Clear();
            nudUnitPrice.Value = 0;
            nudQuantity.Value = 0;
            cboCategory.SelectedIndex = 0;
            epCheck.Clear();
            txtName.Focus();
        }

        // ------------------------------------------------------------
        // Yêu cầu 2: nút THÊM
        // ------------------------------------------------------------
        private void btnAdd_Click(object? sender, EventArgs e)
        {
            if (!ValidateInputs(null)) return;

            var p = new Product();       // khởi tạo đối tượng từ các ô nhập liệu
            FillFromInputs(p);
            _products.Add(p);            // thêm vào List<Product>

            // Hiện lại toàn bộ danh sách để thấy ngay sản phẩm mới
            txtSearch.Clear();
            _keyword = "";
            RefreshGrid(p.ProductId, loadInputs: false);   // làm mới DataGridView
            ClearInputs();               // chuẩn bị nhập sản phẩm tiếp theo

            MessageBox.Show($"Đã thêm sản phẩm \"{p.ProductName}\".", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ------------------------------------------------------------
        // Nút SỬA: cập nhật dòng đang chọn bằng dữ liệu trên các ô nhập
        // ------------------------------------------------------------
        private void btnEdit_Click(object? sender, EventArgs e)
        {
            if (bsProducts.Current is not Product cur)
            {
                MessageBox.Show("Hãy chọn một sản phẩm trong danh sách để sửa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!ValidateInputs(cur)) return;

            FillFromInputs(cur);
            RefreshGrid(cur.ProductId);

            MessageBox.Show($"Đã cập nhật sản phẩm \"{cur.ProductName}\".", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ------------------------------------------------------------
        // Yêu cầu 4: nút XÓA (có xác nhận bằng MessageBox)
        // ------------------------------------------------------------
        private void btnDelete_Click(object? sender, EventArgs e)
        {
            if (bsProducts.Current is not Product cur)
            {
                MessageBox.Show("Hãy chọn một sản phẩm trong danh sách để xóa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var rs = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa sản phẩm \"{cur.ProductName}\" (mã {cur.ProductId})?",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (rs != DialogResult.Yes) return;

            _products.Remove(cur);       // xóa khỏi danh sách gốc
            RefreshGrid();               // làm mới lưới, dòng kế tiếp được chọn
        }

        // ------------------------------------------------------------
        // Nút TÌM KIẾM (theo Tên SP) và HIỆN TẤT CẢ
        // ------------------------------------------------------------
        private void btnSearch_Click(object? sender, EventArgs e)
        {
            _keyword = txtSearch.Text.Trim();
            RefreshGrid();

            if (_keyword.Length > 0 && bsProducts.Count == 0)
                MessageBox.Show($"Không tìm thấy sản phẩm nào có tên chứa \"{_keyword}\".", "Kết quả tìm kiếm",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnShowAll_Click(object? sender, EventArgs e)
        {
            txtSearch.Clear();
            _keyword = "";
            RefreshGrid();
        }

        // ------------------------------------------------------------
        // Kiểm tra dữ liệu nhập (ErrorProvider)
        // ------------------------------------------------------------
        private bool ValidateInputs(Product? editing)
        {
            epCheck.Clear();
            Control? first = null;

            void Fail(Control c, string msg)
            {
                epCheck.SetError(c, msg);
                first ??= c;
            }

            string id = txtId.Text.Trim();
            if (id.Length == 0)
                Fail(txtId, "Mã SP không được để trống.");
            else if (_products.Any(p => !ReferenceEquals(p, editing) &&
                                        string.Equals(p.ProductId, id, StringComparison.OrdinalIgnoreCase)))
                Fail(txtId, $"Mã SP \"{id}\" đã tồn tại.");

            if (txtName.Text.Trim().Length == 0)
                Fail(txtName, "Tên SP không được để trống.");

            if (cboCategory.SelectedIndex < 0)
                Fail(cboCategory, "Vui lòng chọn danh mục.");

            if (first != null)
            {
                first.Focus();
                return false;
            }
            return true;
        }

        private string NextId()
        {
            int max = 0;
            foreach (var p in _products)
            {
                string digits = new string(p.ProductId.Reverse().TakeWhile(c => char.IsDigit(c)).Reverse().ToArray());
                if (int.TryParse(digits, out int n) && n > max) max = n;
            }
            return "SP" + (max + 1).ToString("D3");
        }

        // Bỏ dấu tiếng Việt để tìm "ban phim" vẫn ra "Bàn phím"
        private static string RemoveDiacritics(string text)
        {
            string s = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(s.Length);
            foreach (char c in s)
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            return sb.ToString().Replace('đ', 'd').Replace('Đ', 'D').Normalize(NormalizationForm.FormC);
        }
    }
}
