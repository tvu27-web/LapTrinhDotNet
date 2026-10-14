using System.Globalization;

namespace ItemListManager;

public class MainForm : Form
{
    // ===== GroupBox =====
    private readonly GroupBox grpNhapLieu = new();
    private readonly GroupBox grpDanhSach = new();

    // ===== Khung nhập liệu (bên trái) =====
    private readonly Label lblMaVT = new();
    private readonly Label lblTenVT = new();
    private readonly Label lblDonViTinh = new();
    private readonly Label lblDonGia = new();
    private readonly TextBox txtMaVT = new();
    private readonly TextBox txtTenVT = new();
    private readonly ComboBox cboDonViTinh = new();
    private readonly TextBox txtDonGia = new();

    // ===== Nút chức năng =====
    private readonly Button btnThemMoi = new();
    private readonly Button btnCapNhat = new();
    private readonly Button btnXoaDong = new();
    private readonly Button btnXoaToanBo = new();

    // ===== Khung danh sách (bên phải) =====
    private readonly ListView lvVatTu = new();

    public MainForm()
    {
        KhoiTaoGiaoDien();
    }

    private void KhoiTaoGiaoDien()
    {
        // ----- Form -----
        Text = "Quản lý danh mục Vật tư / Linh kiện";
        ClientSize = new Size(900, 380);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        // ===== Group Left: Nhập liệu =====
        grpNhapLieu.Text = "Thông tin vật tư";
        grpNhapLieu.SetBounds(15, 15, 340, 350);
        grpNhapLieu.TabIndex = 0;

        lblMaVT.Text = "Mã vật tư:";
        lblMaVT.SetBounds(20, 43, 100, 25);
        txtMaVT.SetBounds(130, 40, 190, 28);
        txtMaVT.TabIndex = 0;

        lblTenVT.Text = "Tên vật tư:";
        lblTenVT.SetBounds(20, 93, 100, 25);
        txtTenVT.SetBounds(130, 90, 190, 28);
        txtTenVT.TabIndex = 1;

        lblDonViTinh.Text = "Đơn vị tính:";
        lblDonViTinh.SetBounds(20, 143, 100, 25);
        cboDonViTinh.DropDownStyle = ComboBoxStyle.DropDownList;
        cboDonViTinh.Items.AddRange(new object[] { "Cái", "Bộ", "Kg", "Mét" });
        cboDonViTinh.SetBounds(130, 140, 190, 28);
        cboDonViTinh.TabIndex = 2;

        lblDonGia.Text = "Đơn giá nhập:";
        lblDonGia.SetBounds(20, 193, 100, 25);
        txtDonGia.SetBounds(130, 190, 190, 28);
        txtDonGia.TabIndex = 3;

        btnThemMoi.Text = "Thêm mới";
        btnThemMoi.SetBounds(20, 250, 140, 36);
        btnThemMoi.TabIndex = 4;
        btnThemMoi.Click += BtnThemMoi_Click;

        btnCapNhat.Text = "Cập nhật";
        btnCapNhat.SetBounds(180, 250, 140, 36);
        btnCapNhat.TabIndex = 5;
        btnCapNhat.Click += BtnCapNhat_Click;

        btnXoaDong.Text = "Xóa dòng";
        btnXoaDong.SetBounds(20, 297, 140, 36);
        btnXoaDong.TabIndex = 6;
        btnXoaDong.Click += BtnXoaDong_Click;

        btnXoaToanBo.Text = "Xóa toàn bộ";
        btnXoaToanBo.SetBounds(180, 297, 140, 36);
        btnXoaToanBo.TabIndex = 7;
        btnXoaToanBo.Click += BtnXoaToanBo_Click;

        grpNhapLieu.Controls.AddRange(new Control[]
        {
            lblMaVT, txtMaVT,
            lblTenVT, txtTenVT,
            lblDonViTinh, cboDonViTinh,
            lblDonGia, txtDonGia,
            btnThemMoi, btnCapNhat, btnXoaDong, btnXoaToanBo
        });

        // ===== Group Right: Danh sách =====
        grpDanhSach.Text = "Danh sách vật tư";
        grpDanhSach.SetBounds(370, 15, 515, 350);
        grpDanhSach.TabIndex = 1;

        lvVatTu.View = View.Details;
        lvVatTu.FullRowSelect = true;
        lvVatTu.GridLines = true;
        lvVatTu.MultiSelect = false;
        lvVatTu.HideSelection = false;
        lvVatTu.SetBounds(15, 25, 485, 310);
        lvVatTu.Columns.Add("Mã VT", 90);
        lvVatTu.Columns.Add("Tên VT", 190);
        lvVatTu.Columns.Add("Đơn vị tính", 90);
        lvVatTu.Columns.Add("Đơn giá", 105, HorizontalAlignment.Right);
        lvVatTu.SelectedIndexChanged += LvVatTu_SelectedIndexChanged;

        grpDanhSach.Controls.Add(lvVatTu);

        Controls.AddRange(new Control[] { grpNhapLieu, grpDanhSach });
    }

    // ===== Chọn 1 dòng -> đẩy dữ liệu ngược lại ô nhập =====
    private void LvVatTu_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (lvVatTu.SelectedItems.Count == 0)
            return;

        ListViewItem dong = lvVatTu.SelectedItems[0];
        txtMaVT.Text = dong.SubItems[0].Text;
        txtTenVT.Text = dong.SubItems[1].Text;
        cboDonViTinh.SelectedItem = dong.SubItems[2].Text;

        // Đơn giá gốc được lưu trong Tag để không bị lệch định dạng
        if (dong.Tag is decimal gia)
            txtDonGia.Text = gia.ToString(CultureInfo.CurrentCulture);
    }

    // ===== Thêm mới =====
    private void BtnThemMoi_Click(object? sender, EventArgs e)
    {
        if (!KiemTraDuLieu(out decimal donGia))
            return;

        string ma = txtMaVT.Text.Trim();

        // Không cho thêm nếu Mã VT đã tồn tại
        if (MaDaTonTai(ma, null))
        {
            MessageBox.Show("Mã vật tư \"" + ma + "\" đã tồn tại trong danh sách!",
                "Trùng mã", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtMaVT.Focus();
            txtMaVT.SelectAll();
            return;
        }

        ListViewItem dong = new(ma);
        dong.SubItems.Add(txtTenVT.Text.Trim());
        dong.SubItems.Add(cboDonViTinh.SelectedItem!.ToString());
        dong.SubItems.Add(DinhDangGia(donGia));
        dong.Tag = donGia;
        lvVatTu.Items.Add(dong);

        XoaOnhap();
    }

    // ===== Cập nhật =====
    private void BtnCapNhat_Click(object? sender, EventArgs e)
    {
        if (lvVatTu.SelectedItems.Count == 0)
        {
            MessageBox.Show("Vui lòng chọn một dòng trong danh sách để cập nhật!",
                "Chưa chọn dòng", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (!KiemTraDuLieu(out decimal donGia))
            return;

        ListViewItem dong = lvVatTu.SelectedItems[0];
        string ma = txtMaVT.Text.Trim();

        // Nếu đổi mã thì mã mới không được trùng với dòng khác
        if (MaDaTonTai(ma, dong))
        {
            MessageBox.Show("Mã vật tư \"" + ma + "\" đã tồn tại ở dòng khác!",
                "Trùng mã", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtMaVT.Focus();
            txtMaVT.SelectAll();
            return;
        }

        dong.SubItems[0].Text = ma;
        dong.SubItems[1].Text = txtTenVT.Text.Trim();
        dong.SubItems[2].Text = cboDonViTinh.SelectedItem!.ToString();
        dong.SubItems[3].Text = DinhDangGia(donGia);
        dong.Tag = donGia;

        XoaOnhap();
    }

    // ===== Xóa dòng (có xác nhận Yes/No) =====
    private void BtnXoaDong_Click(object? sender, EventArgs e)
    {
        if (lvVatTu.SelectedItems.Count == 0)
        {
            MessageBox.Show("Vui lòng chọn một dòng để xóa!",
                "Chưa chọn dòng", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        ListViewItem dong = lvVatTu.SelectedItems[0];
        DialogResult kq = MessageBox.Show(
            "Bạn có chắc muốn xóa vật tư \"" + dong.SubItems[1].Text + "\" không?",
            "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (kq == DialogResult.Yes)
        {
            lvVatTu.Items.Remove(dong);
            XoaOnhap();
        }
    }

    // ===== Xóa toàn bộ =====
    private void BtnXoaToanBo_Click(object? sender, EventArgs e)
    {
        if (lvVatTu.Items.Count == 0)
        {
            MessageBox.Show("Danh sách đang trống!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        DialogResult kq = MessageBox.Show(
            "Bạn có chắc muốn xóa toàn bộ danh sách không?",
            "Xác nhận xóa toàn bộ", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (kq == DialogResult.Yes)
        {
            lvVatTu.Items.Clear();
            XoaOnhap();
        }
    }

    // ===== Các hàm hỗ trợ =====

    // Kiểm tra mã đã có trong ListView chưa (boQua: dòng được bỏ qua khi so sánh)
    private bool MaDaTonTai(string ma, ListViewItem? boQua)
    {
        foreach (ListViewItem dong in lvVatTu.Items)
        {
            if (dong == boQua)
                continue;

            if (string.Equals(dong.SubItems[0].Text, ma, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    // Validate các ô nhập
    private bool KiemTraDuLieu(out decimal donGia)
    {
        donGia = 0;

        if (txtMaVT.Text.Trim() == "")
        {
            BaoLoi("Vui lòng nhập mã vật tư!", txtMaVT);
            return false;
        }

        if (txtTenVT.Text.Trim() == "")
        {
            BaoLoi("Vui lòng nhập tên vật tư!", txtTenVT);
            return false;
        }

        if (cboDonViTinh.SelectedIndex == -1)
        {
            MessageBox.Show("Vui lòng chọn đơn vị tính!", "Thiếu thông tin",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            cboDonViTinh.Focus();
            return false;
        }

        if (!decimal.TryParse(txtDonGia.Text.Trim(), NumberStyles.Number,
                CultureInfo.CurrentCulture, out donGia) || donGia < 0)
        {
            BaoLoi("Đơn giá phải là số và không được âm!", txtDonGia);
            return false;
        }

        return true;
    }

    private static void BaoLoi(string noiDung, TextBox txt)
    {
        MessageBox.Show(noiDung, "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        txt.Focus();
        txt.SelectAll();
    }

    private static string DinhDangGia(decimal gia)
    {
        return gia.ToString("#,##0.##", CultureInfo.CurrentCulture);
    }

    private void XoaOnhap()
    {
        txtMaVT.Clear();
        txtTenVT.Clear();
        txtDonGia.Clear();
        cboDonViTinh.SelectedIndex = -1;
        lvVatTu.SelectedItems.Clear();
        txtMaVT.Focus();
    }
}
