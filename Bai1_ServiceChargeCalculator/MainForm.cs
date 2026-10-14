using System.Globalization;

namespace ServiceChargeCalculator;

public class MainForm : Form
{
    // ===== Controls =====
    private readonly Label lblTieuDe = new();
    private readonly Label lblDonGia = new();
    private readonly Label lblSoLuong = new();
    private readonly Label lblGiamGia = new();
    private readonly Label lblTongText = new();
    private readonly Label lblTongTien = new();

    private readonly TextBox txtDonGia = new();
    private readonly TextBox txtSoLuong = new();
    private readonly TextBox txtGiamGia = new();

    private readonly Button btnTinh = new();
    private readonly Button btnLamMoi = new();

    private static readonly CultureInfo ViCulture = new("vi-VN");

    public MainForm()
    {
        KhoiTaoGiaoDien();
    }

    private void KhoiTaoGiaoDien()
    {
        // ----- Form -----
        Text = "Máy tính cước dịch vụ & Giảm giá";
        ClientSize = new Size(440, 330);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        Font = new Font("Segoe UI", 10F);

        // ----- Tiêu đề -----
        lblTieuDe.Text = "TÍNH CƯỚC DỊCH VỤ";
        lblTieuDe.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
        lblTieuDe.ForeColor = Color.DarkSlateBlue;
        lblTieuDe.TextAlign = ContentAlignment.MiddleCenter;
        lblTieuDe.SetBounds(0, 15, 440, 35);

        // ----- Hàng 1: Đơn giá -----
        lblDonGia.Text = "Đơn giá dịch vụ (VNĐ):";
        lblDonGia.SetBounds(30, 75, 190, 25);
        txtDonGia.SetBounds(230, 72, 170, 28);
        txtDonGia.TabIndex = 0;

        // ----- Hàng 2: Số lượng khách -----
        lblSoLuong.Text = "Số lượng khách:";
        lblSoLuong.SetBounds(30, 120, 190, 25);
        txtSoLuong.SetBounds(230, 117, 170, 28);
        txtSoLuong.TabIndex = 1;

        // ----- Hàng 3: Mã giảm giá -----
        lblGiamGia.Text = "Mã giảm giá (% giảm):";
        lblGiamGia.SetBounds(30, 165, 190, 25);
        txtGiamGia.SetBounds(230, 162, 170, 28);
        txtGiamGia.TabIndex = 2;

        // ----- Tổng tiền -----
        lblTongText.Text = "Tổng tiền thanh toán:";
        lblTongText.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblTongText.SetBounds(30, 215, 190, 25);

        lblTongTien.Text = "0 VNĐ";
        lblTongTien.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        lblTongTien.ForeColor = Color.Firebrick;
        lblTongTien.TextAlign = ContentAlignment.MiddleRight;
        lblTongTien.BorderStyle = BorderStyle.FixedSingle;
        lblTongTien.SetBounds(230, 210, 170, 35);
        lblTongTien.TabStop = false;

        // ----- Buttons -----
        btnTinh.Text = "Tính tiền";
        btnTinh.SetBounds(90, 270, 120, 36);
        btnTinh.TabIndex = 3;
        btnTinh.Click += BtnTinh_Click;

        btnLamMoi.Text = "Làm mới";
        btnLamMoi.SetBounds(230, 270, 120, 36);
        btnLamMoi.TabIndex = 4;
        btnLamMoi.Click += BtnLamMoi_Click;

        // Enter = Tính tiền, Esc = Làm mới
        AcceptButton = btnTinh;
        CancelButton = btnLamMoi;

        Controls.AddRange(new Control[]
        {
            lblTieuDe,
            lblDonGia, txtDonGia,
            lblSoLuong, txtSoLuong,
            lblGiamGia, txtGiamGia,
            lblTongText, lblTongTien,
            btnTinh, btnLamMoi
        });
    }

    // ===== Sự kiện nút "Tính tiền" =====
    private void BtnTinh_Click(object? sender, EventArgs e)
    {
        if (!KiemTraDuLieu(out decimal donGia, out int soLuong, out decimal giamGia))
            return;

        // Tổng tiền = (Đơn giá × Số lượng) × (100 - %Giảm) / 100
        decimal tong = donGia * soLuong * (100 - giamGia) / 100;
        lblTongTien.Text = string.Format(ViCulture, "{0:N0} VNĐ", tong);
    }

    // ===== Sự kiện nút "Làm mới" =====
    private void BtnLamMoi_Click(object? sender, EventArgs e)
    {
        txtDonGia.Clear();
        txtSoLuong.Clear();
        txtGiamGia.Clear();
        lblTongTien.Text = "0 VNĐ";
        txtDonGia.Focus();
    }

    // ===== Validate dữ liệu nhập =====
    private bool KiemTraDuLieu(out decimal donGia, out int soLuong, out decimal giamGia)
    {
        donGia = 0;
        soLuong = 0;
        giamGia = 0;

        // Đơn giá
        if (!decimal.TryParse(txtDonGia.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out donGia)
            || donGia < 0)
        {
            BaoLoi("Đơn giá phải là số và không được để trống hoặc âm!", txtDonGia);
            return false;
        }

        // Số lượng khách
        if (!int.TryParse(txtSoLuong.Text.Trim(), NumberStyles.Integer, CultureInfo.CurrentCulture, out soLuong)
            || soLuong <= 0)
        {
            BaoLoi("Số lượng khách phải là số nguyên lớn hơn 0!", txtSoLuong);
            return false;
        }

        // % giảm giá
        if (!decimal.TryParse(txtGiamGia.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out giamGia)
            || giamGia < 0 || giamGia > 100)
        {
            BaoLoi("% giảm giá phải là số từ 0 đến 100!", txtGiamGia);
            return false;
        }

        return true;
    }

    private static void BaoLoi(string message, TextBox txt)
    {
        MessageBox.Show(message, "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        txt.Focus();
        txt.SelectAll();
    }
}
