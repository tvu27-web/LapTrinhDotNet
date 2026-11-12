namespace Bai52_TinhTien
{
    public partial class Form1 : Form
    {
        // ---------------- DỮ LIỆU ----------------
        // Danh mục dịch vụ theo từng loại (cboCategory hiển thị các khóa này)
        private static readonly string[] Categories = { "Khám bệnh", "Xét nghiệm", "Chụp X-Quang", "Vắc-xin" };

        private readonly Dictionary<string, List<Service>> _catalog = new()
        {
            ["Khám bệnh"] = new()
            {
                new Service("Khám bệnh", "Khám tổng quát", 150_000),
                new Service("Khám bệnh", "Khám nội", 200_000),
                new Service("Khám bệnh", "Khám nhi", 180_000),
                new Service("Khám bệnh", "Khám tai mũi họng", 220_000),
                new Service("Khám bệnh", "Khám da liễu", 250_000),
            },
            ["Xét nghiệm"] = new()
            {
                new Service("Xét nghiệm", "Xét nghiệm máu tổng quát", 250_000),
                new Service("Xét nghiệm", "Xét nghiệm nước tiểu", 120_000),
                new Service("Xét nghiệm", "Xét nghiệm đường huyết", 80_000),
                new Service("Xét nghiệm", "Xét nghiệm mỡ máu", 180_000),
                new Service("Xét nghiệm", "Xét nghiệm chức năng gan", 300_000),
            },
            ["Chụp X-Quang"] = new()
            {
                new Service("Chụp X-Quang", "X-Quang phổi", 200_000),
                new Service("Chụp X-Quang", "X-Quang cột sống", 280_000),
                new Service("Chụp X-Quang", "X-Quang khớp gối", 250_000),
                new Service("Chụp X-Quang", "X-Quang răng", 100_000),
            },
            ["Vắc-xin"] = new()
            {
                new Service("Vắc-xin", "Vắc-xin cúm", 350_000),
                new Service("Vắc-xin", "Vắc-xin viêm gan B", 280_000),
                new Service("Vắc-xin", "Vắc-xin uốn ván", 150_000),
                new Service("Vắc-xin", "Vắc-xin phế cầu", 900_000),
                new Service("Vắc-xin", "Vắc-xin HPV", 1_800_000),
            },
        };

        // Mã giảm giá -> tỷ lệ chiết khấu (%)
        private static readonly Dictionary<string, int> PromoCodes = new()
        {
            ["GIAM10"] = 10,
            ["GIAM20"] = 20,
            ["VIP30"] = 30,
        };

        // Các dịch vụ người dùng đã chọn (theo thứ tự chọn)
        private readonly List<Service> _selected = new();
        private int _appliedRate = -1;   // tỷ lệ của mã giảm giá đã áp dụng (-1 = chưa có)

        public Form1()
        {
            InitializeComponent();

            // ListBox "đã chọn": hiển thị kèm loại dịch vụ cho dễ phân biệt
            lstSelectedServices.Format += (s, e) =>
            {
                if (e.ListItem is Service sv)
                    e.Value = $"[{sv.Category}] {sv.Name}  -  {sv.Price:N0} đ";
            };

            // ----- Gắn sự kiện -----
            cboCategory.SelectedIndexChanged += cboCategory_SelectedIndexChanged;
            btnSelect.Click += btnSelect_Click;
            btnRemove.Click += btnRemove_Click;
            btnClearAll.Click += btnClearAll_Click;
            lstAvailableServices.MouseDoubleClick += lstAvailableServices_MouseDoubleClick;
            lstSelectedServices.MouseDoubleClick += lstSelectedServices_MouseDoubleClick;
            nudDiscount.ValueChanged += nudDiscount_ValueChanged;
            btnApplyPromo.Click += btnApplyPromo_Click;
            txtPromo.TextChanged += (s, e) => epPromo.SetError(txtPromo, "");
            txtPromo.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    btnApplyPromo_Click(s, EventArgs.Empty);
                }
            };

            // ----- Nạp dữ liệu ban đầu -----
            cboCategory.Items.AddRange(Categories);
            cboCategory.SelectedIndex = 0;     // kích hoạt sự kiện -> nạp dịch vụ của loại đầu tiên
            LoadSelectedServices();            // hiển thị 0 đ ban đầu
        }

        // ============================================================
        // YÊU CẦU 1: đổi loại dịch vụ -> nạp danh sách tương ứng
        // ============================================================
        private void cboCategory_SelectedIndexChanged(object? sender, EventArgs e)
        {
            LoadAvailableServices();
        }

        // Dịch vụ có sẵn = dịch vụ của loại đang chọn, TRỪ những dịch vụ đã chọn rồi
        private void LoadAvailableServices()
        {
            lstAvailableServices.BeginUpdate();
            lstAvailableServices.Items.Clear();

            if (cboCategory.SelectedItem is string cat && _catalog.TryGetValue(cat, out var list))
            {
                foreach (var s in list)
                    if (!_selected.Contains(s))
                        lstAvailableServices.Items.Add(s);
            }
            lstAvailableServices.EndUpdate();
        }

        // Vẽ lại danh sách đã chọn rồi TÍNH LẠI TIỀN (yêu cầu 3)
        private void LoadSelectedServices()
        {
            lstSelectedServices.BeginUpdate();
            lstSelectedServices.Items.Clear();
            foreach (var s in _selected)
                lstSelectedServices.Items.Add(s);
            lstSelectedServices.EndUpdate();

            RecalculateTotal();
        }

        // ============================================================
        // YÊU CẦU 2: nút ">" hoặc double-click -> chuyển sang danh sách đã chọn
        // ============================================================
        private void btnSelect_Click(object? sender, EventArgs e)
        {
            var items = lstAvailableServices.SelectedItems.Cast<Service>().ToList();
            if (items.Count == 0)
            {
                MessageBox.Show("Hãy chọn ít nhất một dịch vụ ở danh sách bên trái.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            MoveToSelected(items);
        }

        private void lstAvailableServices_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            int idx = lstAvailableServices.IndexFromPoint(e.Location);
            if (idx == ListBox.NoMatches) return;
            if (lstAvailableServices.Items[idx] is Service s)
                MoveToSelected(new List<Service> { s });
        }

        private void MoveToSelected(List<Service> items)
        {
            int keep = lstAvailableServices.SelectedIndex;

            _selected.AddRange(items);
            LoadAvailableServices();      // dịch vụ vừa chuyển biến mất khỏi danh sách có sẵn
            LoadSelectedServices();       // xuất hiện ở danh sách đã chọn + tính lại tiền

            KeepSelection(lstAvailableServices, keep);
        }

        // ============================================================
        // Nút "<" và "<<" (và double-click ở danh sách đã chọn)
        // ============================================================
        private void btnRemove_Click(object? sender, EventArgs e)
        {
            var items = lstSelectedServices.SelectedItems.Cast<Service>().ToList();
            if (items.Count == 0)
            {
                MessageBox.Show("Hãy chọn ít nhất một dịch vụ ở danh sách bên phải để bỏ chọn.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            RemoveFromSelected(items);
        }

        private void lstSelectedServices_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            int idx = lstSelectedServices.IndexFromPoint(e.Location);
            if (idx == ListBox.NoMatches) return;
            if (lstSelectedServices.Items[idx] is Service s)
                RemoveFromSelected(new List<Service> { s });
        }

        private void RemoveFromSelected(List<Service> items)
        {
            int keep = lstSelectedServices.SelectedIndex;

            foreach (var s in items) _selected.Remove(s);
            LoadAvailableServices();      // dịch vụ trả về danh sách có sẵn (nếu đúng loại đang xem)
            LoadSelectedServices();

            KeepSelection(lstSelectedServices, keep);
        }

        private void btnClearAll_Click(object? sender, EventArgs e)
        {
            if (_selected.Count == 0) return;
            _selected.Clear();
            LoadAvailableServices();
            LoadSelectedServices();
        }

        // Sau khi chuyển, chọn lại dòng gần vị trí cũ để bấm liên tục cho tiện
        private static void KeepSelection(ListBox lb, int oldIndex)
        {
            if (lb.Items.Count == 0 || oldIndex < 0) return;
            lb.SelectedIndex = Math.Min(oldIndex, lb.Items.Count - 1);
        }

        // ============================================================
        // YÊU CẦU 3: tính lại tổng chi phí mỗi khi danh sách thay đổi
        // ============================================================
        private void RecalculateTotal()
        {
            decimal subtotal = _selected.Sum(s => s.Price);                 // tổng chưa giảm
            decimal rate = nudDiscount.Value;                               // % chiết khấu
            decimal discount = Math.Round(subtotal * rate / 100m, 0, MidpointRounding.AwayFromZero);
            decimal payable = subtotal - discount;                          // thành tiền

            txtSubtotal.Text = subtotal.ToString("N0") + " đ";
            txtDiscountAmount.Text = discount.ToString("N0") + " đ";
            txtPayable.Text = payable.ToString("N0") + " đ";
            lblCount.Text = $"Số dịch vụ đã chọn: {_selected.Count}";
        }

        // ============================================================
        // CHIẾT KHẤU / MÃ GIẢM GIÁ
        // ============================================================
        private void nudDiscount_ValueChanged(object? sender, EventArgs e)
        {
            // Người dùng tự đổi tỷ lệ khác với mã đã áp dụng -> xóa thông báo mã cũ
            if (_appliedRate >= 0 && nudDiscount.Value != _appliedRate)
            {
                lblPromoInfo.Text = "Mã mẫu: GIAM10, GIAM20, VIP30";
                lblPromoInfo.ForeColor = Color.Gray;
                _appliedRate = -1;
            }
            RecalculateTotal();
        }

        private void btnApplyPromo_Click(object? sender, EventArgs e)
        {
            epPromo.SetError(txtPromo, "");
            string code = txtPromo.Text.Trim().ToUpperInvariant();

            if (code.Length == 0)
            {
                epPromo.SetError(txtPromo, "Vui lòng nhập mã giảm giá.");
                return;
            }
            if (!PromoCodes.TryGetValue(code, out int rate))
            {
                epPromo.SetError(txtPromo, "Mã giảm giá không hợp lệ.");
                return;
            }

            _appliedRate = rate;
            nudDiscount.Value = rate;      // kích hoạt ValueChanged -> tính lại tiền
            _appliedRate = rate;           // (đặt lại vì ValueChanged có thể đã reset)
            lblPromoInfo.ForeColor = Color.SeaGreen;
            lblPromoInfo.Text = $"Đã áp dụng mã {code}: giảm {rate}%.";
        }
    }
}
