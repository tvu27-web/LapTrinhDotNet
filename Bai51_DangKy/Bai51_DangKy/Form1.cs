namespace Bai51_DangKy
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Không cho chọn ngày sinh ở tương lai
            dtpBirthDate.MaxDate = DateTime.Today;

            // Người dùng sửa lại ô nào thì xóa biểu tượng lỗi của ô đó
            txtUsername.TextChanged += (s, e) => epCheck.SetError(txtUsername, "");
            txtPassword.TextChanged += (s, e) => epCheck.SetError(txtPassword, "");
            txtConfirm.TextChanged += (s, e) => epCheck.SetError(txtConfirm, "");
            dtpBirthDate.ValueChanged += (s, e) => epCheck.SetError(dtpBirthDate, "");
            chkTerms.CheckedChanged += (s, e) => epCheck.SetError(chkTerms, "");
        }

        // ------------------------------------------------------------
        // Nút ĐĂNG KÝ
        // ------------------------------------------------------------
        private void btnRegister_Click(object? sender, EventArgs e)
        {
            if (!ValidateForm()) return;     // còn lỗi -> dừng, các ô sai đã có biểu tượng cảnh báo

            string gender = rdoMale.Checked ? "Nam" : "Nữ";
            MessageBox.Show(
                "Đăng ký tài khoản thành công!\n\n" +
                "Tên đăng nhập: " + txtUsername.Text.Trim() + "\n" +
                "Ngày sinh: " + dtpBirthDate.Value.ToString("dd/MM/yyyy") + "\n" +
                "Giới tính: " + gender,
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ResetForm();
        }

        // ------------------------------------------------------------
        // Nút LÀM MỚI
        // ------------------------------------------------------------
        private void btnReset_Click(object? sender, EventArgs e)
        {
            ResetForm();
        }

        // ------------------------------------------------------------
        // Kiểm tra dữ liệu bằng ErrorProvider (epCheck)
        // ------------------------------------------------------------
        private bool ValidateForm()
        {
            epCheck.Clear();                 // xóa hết lỗi cũ
            Control? firstError = null;      // ô lỗi đầu tiên (để đưa con trỏ về)

            void ShowError(Control c, string message)
            {
                epCheck.SetError(c, message);
                firstError ??= c;
            }

            // 1. Tên đăng nhập không được để trống
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
                ShowError(txtUsername, "Tên đăng nhập không được để trống.");

            // 2. Mật khẩu không được để trống
            if (string.IsNullOrEmpty(txtPassword.Text))
                ShowError(txtPassword, "Mật khẩu không được để trống.");

            // 3. Xác nhận mật khẩu phải khớp
            if (string.IsNullOrEmpty(txtConfirm.Text))
                ShowError(txtConfirm, "Vui lòng nhập lại mật khẩu để xác nhận.");
            else if (txtConfirm.Text != txtPassword.Text)
                ShowError(txtConfirm, "Mật khẩu xác nhận không khớp.");

            // 4. Tuổi tính từ ngày sinh phải >= 18
            int age = CalculateAge(dtpBirthDate.Value.Date, DateTime.Today);
            if (age < 18)
                ShowError(dtpBirthDate, $"Bạn phải từ 18 tuổi trở lên (hiện tại: {Math.Max(age, 0)} tuổi).");

            // 5. Phải đồng ý Điều khoản dịch vụ
            if (!chkTerms.Checked)
                ShowError(chkTerms, "Bạn phải đồng ý với Điều khoản dịch vụ.");

            if (firstError != null)
            {
                firstError.Focus();
                return false;
            }
            return true;
        }

        // Tính tuổi chính xác (chưa đến sinh nhật năm nay thì trừ 1)
        private static int CalculateAge(DateTime birthDate, DateTime today)
        {
            int age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }

        private void ResetForm()
        {
            txtUsername.Clear();
            txtPassword.Clear();
            txtConfirm.Clear();
            dtpBirthDate.Value = DateTime.Today;
            rdoMale.Checked = true;
            chkTerms.Checked = false;
            epCheck.Clear();
            txtUsername.Focus();
        }
    }
}
