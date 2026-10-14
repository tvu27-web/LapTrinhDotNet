using System.Text;

namespace ITSupportTicket;

public class MainForm : Form
{
    // ===== GroupBox =====
    private readonly GroupBox grpThongTin = new();
    private readonly GroupBox grpPhanLoai = new();

    // ===== Thông tin phiếu =====
    private readonly Label lblMaPhieu = new();
    private readonly Label lblNguoiYeuCau = new();
    private readonly Label lblNgayGhiNhan = new();
    private readonly Label lblUuTien = new();
    private readonly TextBox txtMaPhieu = new();
    private readonly TextBox txtNguoiYeuCau = new();
    private readonly DateTimePicker dtpNgayGhiNhan = new();
    private readonly RadioButton rdoThap = new();
    private readonly RadioButton rdoTrungBinh = new();
    private readonly RadioButton rdoKhanCap = new();

    // ===== Phân loại & chi tiết =====
    private readonly Label lblLoaiSuCo = new();
    private readonly Label lblThietBi = new();
    private readonly Label lblAnhLoi = new();
    private readonly ComboBox cboLoaiSuCo = new();
    private readonly CheckBox chkMayTinhBan = new();
    private readonly CheckBox chkLaptop = new();
    private readonly CheckBox chkMayIn = new();
    private readonly CheckBox chkDienThoai = new();
    private readonly PictureBox picAnhLoi = new();
    private readonly Button btnTaiAnh = new();

    // ===== Nút chức năng =====
    private readonly Button btnGuiYeuCau = new();
    private readonly Button btnNhapLai = new();

    // Lưu tên file ảnh đã chọn để hiển thị trong bảng tóm tắt
    private string tenFileAnh = "";

    public MainForm()
    {
        KhoiTaoGiaoDien();
    }

    private void KhoiTaoGiaoDien()
    {
        // ----- Form -----
        Text = "Tiếp nhận & Phân loại sự cố IT";
        ClientSize = new Size(560, 600);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        // ===== GroupBox 1: Thông tin phiếu =====
        grpThongTin.Text = "Thông tin phiếu";
        grpThongTin.SetBounds(15, 15, 530, 200);
        grpThongTin.TabIndex = 0;

        lblMaPhieu.Text = "Mã phiếu:";
        lblMaPhieu.SetBounds(20, 35, 130, 25);
        txtMaPhieu.SetBounds(160, 32, 330, 28);
        txtMaPhieu.TabIndex = 0;

        lblNguoiYeuCau.Text = "Người yêu cầu:";
        lblNguoiYeuCau.SetBounds(20, 75, 130, 25);
        txtNguoiYeuCau.SetBounds(160, 72, 330, 28);
        txtNguoiYeuCau.TabIndex = 1;

        lblNgayGhiNhan.Text = "Ngày ghi nhận:";
        lblNgayGhiNhan.SetBounds(20, 115, 130, 25);
        dtpNgayGhiNhan.Format = DateTimePickerFormat.Short;
        dtpNgayGhiNhan.SetBounds(160, 112, 330, 28);
        dtpNgayGhiNhan.TabIndex = 2;

        lblUuTien.Text = "Mức độ ưu tiên:";
        lblUuTien.SetBounds(20, 157, 130, 25);

        rdoThap.Text = "Thấp";
        rdoThap.SetBounds(160, 155, 80, 28);
        rdoThap.TabIndex = 3;

        rdoTrungBinh.Text = "Trung bình";
        rdoTrungBinh.SetBounds(250, 155, 120, 28);
        rdoTrungBinh.TabIndex = 4;
        rdoTrungBinh.Checked = true; // Mặc định

        rdoKhanCap.Text = "Khẩn cấp";
        rdoKhanCap.SetBounds(380, 155, 110, 28);
        rdoKhanCap.TabIndex = 5;

        grpThongTin.Controls.AddRange(new Control[]
        {
            lblMaPhieu, txtMaPhieu,
            lblNguoiYeuCau, txtNguoiYeuCau,
            lblNgayGhiNhan, dtpNgayGhiNhan,
            lblUuTien, rdoThap, rdoTrungBinh, rdoKhanCap
        });

        // ===== GroupBox 2: Phân loại & chi tiết =====
        grpPhanLoai.Text = "Phân loại && Chi tiết";
        grpPhanLoai.SetBounds(15, 225, 530, 300);
        grpPhanLoai.TabIndex = 1;

        lblLoaiSuCo.Text = "Loại sự cố:";
        lblLoaiSuCo.SetBounds(20, 35, 130, 25);
        cboLoaiSuCo.DropDownStyle = ComboBoxStyle.DropDownList;
        cboLoaiSuCo.Items.AddRange(new object[] { "Phần cứng", "Phần mềm", "Mạng", "Tài khoản" });
        cboLoaiSuCo.SetBounds(160, 32, 330, 28);
        cboLoaiSuCo.TabIndex = 0;

        lblThietBi.Text = "Thiết bị ảnh hưởng:";
        lblThietBi.SetBounds(20, 80, 135, 25);

        chkMayTinhBan.Text = "Máy tính bàn";
        chkMayTinhBan.SetBounds(160, 77, 150, 28);
        chkMayTinhBan.TabIndex = 1;

        chkLaptop.Text = "Laptop";
        chkLaptop.SetBounds(330, 77, 150, 28);
        chkLaptop.TabIndex = 2;

        chkMayIn.Text = "Máy in";
        chkMayIn.SetBounds(160, 109, 150, 28);
        chkMayIn.TabIndex = 3;

        chkDienThoai.Text = "Điện thoại";
        chkDienThoai.SetBounds(330, 109, 150, 28);
        chkDienThoai.TabIndex = 4;

        lblAnhLoi.Text = "Ảnh chụp lỗi:";
        lblAnhLoi.SetBounds(20, 160, 130, 25);

        picAnhLoi.SetBounds(160, 155, 200, 125);
        picAnhLoi.BorderStyle = BorderStyle.FixedSingle;
        picAnhLoi.SizeMode = PictureBoxSizeMode.StretchImage; // Theo đề bài
        picAnhLoi.TabStop = false;

        btnTaiAnh.Text = "Tải ảnh lỗi";
        btnTaiAnh.SetBounds(375, 155, 115, 34);
        btnTaiAnh.TabIndex = 5;
        btnTaiAnh.Click += BtnTaiAnh_Click;

        grpPhanLoai.Controls.AddRange(new Control[]
        {
            lblLoaiSuCo, cboLoaiSuCo,
            lblThietBi, chkMayTinhBan, chkLaptop, chkMayIn, chkDienThoai,
            lblAnhLoi, picAnhLoi, btnTaiAnh
        });

        // ===== Nút chức năng =====
        btnGuiYeuCau.Text = "Gửi yêu cầu";
        btnGuiYeuCau.SetBounds(140, 543, 130, 38);
        btnGuiYeuCau.TabIndex = 2;
        btnGuiYeuCau.Click += BtnGuiYeuCau_Click;

        btnNhapLai.Text = "Nhập lại";
        btnNhapLai.SetBounds(290, 543, 130, 38);
        btnNhapLai.TabIndex = 3;
        btnNhapLai.Click += BtnNhapLai_Click;

        Controls.AddRange(new Control[]
        {
            grpThongTin, grpPhanLoai, btnGuiYeuCau, btnNhapLai
        });
    }

    // ===== Bấm "Tải ảnh lỗi" =====
    private void BtnTaiAnh_Click(object? sender, EventArgs e)
    {
        using OpenFileDialog dlg = new();
        dlg.Title = "Chọn ảnh chụp lỗi";
        dlg.Filter = "Tệp hình ảnh (*.jpg;*.png)|*.jpg;*.png";

        if (dlg.ShowDialog() == DialogResult.OK)
        {
            try
            {
                // Giải phóng ảnh cũ (nếu có)
                picAnhLoi.Image?.Dispose();

                // Nạp ảnh qua Bitmap để không bị khóa file gốc
                using Image anhGoc = Image.FromFile(dlg.FileName);
                picAnhLoi.Image = new Bitmap(anhGoc);

                tenFileAnh = Path.GetFileName(dlg.FileName);
            }
            catch (Exception)
            {
                MessageBox.Show("Không thể mở tệp ảnh này!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // ===== Bấm "Gửi yêu cầu" =====
    private void BtnGuiYeuCau_Click(object? sender, EventArgs e)
    {
        // Kiểm tra các thông tin bắt buộc
        if (txtMaPhieu.Text.Trim() == "")
        {
            MessageBox.Show("Vui lòng nhập mã phiếu!", "Thiếu thông tin",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtMaPhieu.Focus();
            return;
        }

        if (txtNguoiYeuCau.Text.Trim() == "")
        {
            MessageBox.Show("Vui lòng nhập tên người yêu cầu!", "Thiếu thông tin",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtNguoiYeuCau.Focus();
            return;
        }

        if (cboLoaiSuCo.SelectedIndex == -1)
        {
            MessageBox.Show("Vui lòng chọn loại sự cố!", "Thiếu thông tin",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            cboLoaiSuCo.Focus();
            return;
        }

        // Mức độ ưu tiên
        string uuTien = "Trung bình";
        if (rdoThap.Checked) uuTien = "Thấp";
        else if (rdoKhanCap.Checked) uuTien = "Khẩn cấp";

        // Thiết bị ảnh hưởng
        List<string> dsThietBi = new();
        if (chkMayTinhBan.Checked) dsThietBi.Add(chkMayTinhBan.Text);
        if (chkLaptop.Checked) dsThietBi.Add(chkLaptop.Text);
        if (chkMayIn.Checked) dsThietBi.Add(chkMayIn.Text);
        if (chkDienThoai.Checked) dsThietBi.Add(chkDienThoai.Text);
        string thietBi = dsThietBi.Count > 0 ? string.Join(", ", dsThietBi) : "(Không chọn)";

        // Ảnh lỗi
        string anhLoi = picAnhLoi.Image != null ? tenFileAnh : "(Chưa tải ảnh)";

        // Gom thông tin vào bảng tóm tắt
        StringBuilder sb = new();
        sb.AppendLine("Mã phiếu: " + txtMaPhieu.Text.Trim());
        sb.AppendLine("Người yêu cầu: " + txtNguoiYeuCau.Text.Trim());
        sb.AppendLine("Ngày ghi nhận: " + dtpNgayGhiNhan.Value.ToString("dd/MM/yyyy"));
        sb.AppendLine("Mức độ ưu tiên: " + uuTien);
        sb.AppendLine("Loại sự cố: " + cboLoaiSuCo.SelectedItem);
        sb.AppendLine("Thiết bị ảnh hưởng: " + thietBi);
        sb.AppendLine("Ảnh chụp lỗi: " + anhLoi);

        MessageBox.Show(sb.ToString(), "Tóm tắt phiếu yêu cầu",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    // ===== Bấm "Nhập lại" =====
    private void BtnNhapLai_Click(object? sender, EventArgs e)
    {
        txtMaPhieu.Clear();
        txtNguoiYeuCau.Clear();
        dtpNgayGhiNhan.Value = DateTime.Now;
        rdoTrungBinh.Checked = true;
        cboLoaiSuCo.SelectedIndex = -1;
        chkMayTinhBan.Checked = false;
        chkLaptop.Checked = false;
        chkMayIn.Checked = false;
        chkDienThoai.Checked = false;

        picAnhLoi.Image?.Dispose();
        picAnhLoi.Image = null;
        tenFileAnh = "";

        txtMaPhieu.Focus();
    }
}
