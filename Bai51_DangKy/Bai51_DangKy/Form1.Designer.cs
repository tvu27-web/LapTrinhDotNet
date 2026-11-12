#nullable disable
using System.Drawing;
using System.Windows.Forms;

namespace Bai51_DangKy
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            grpPersonal = new GroupBox();
            lblUsername = new Label();
            lblPassword = new Label();
            lblConfirm = new Label();
            txtUsername = new TextBox();
            txtPassword = new TextBox();
            txtConfirm = new TextBox();
            grpExtra = new GroupBox();
            lblBirth = new Label();
            lblGender = new Label();
            dtpBirthDate = new DateTimePicker();
            rdoMale = new RadioButton();
            rdoFemale = new RadioButton();
            chkTerms = new CheckBox();
            btnRegister = new Button();
            btnReset = new Button();
            epCheck = new ErrorProvider(components);
            ((System.ComponentModel.ISupportInitialize)epCheck).BeginInit();
            grpPersonal.SuspendLayout();
            grpExtra.SuspendLayout();
            SuspendLayout();

            // grpPersonal
            grpPersonal.Controls.Add(lblUsername);
            grpPersonal.Controls.Add(txtUsername);
            grpPersonal.Controls.Add(lblPassword);
            grpPersonal.Controls.Add(txtPassword);
            grpPersonal.Controls.Add(lblConfirm);
            grpPersonal.Controls.Add(txtConfirm);
            grpPersonal.Location = new Point(15, 15);
            grpPersonal.Name = "grpPersonal";
            grpPersonal.Size = new Size(430, 160);
            grpPersonal.TabIndex = 0;
            grpPersonal.TabStop = false;
            grpPersonal.Text = "Thông tin cá nhân";

            // lblUsername / txtUsername
            lblUsername.AutoSize = true;
            lblUsername.Location = new Point(15, 35);
            lblUsername.Name = "lblUsername";
            lblUsername.Text = "Tên đăng nhập:";
            txtUsername.Location = new Point(150, 32);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(250, 27);
            txtUsername.TabIndex = 0;

            // lblPassword / txtPassword
            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(15, 75);
            lblPassword.Name = "lblPassword";
            lblPassword.Text = "Mật khẩu:";
            txtPassword.Location = new Point(150, 72);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(250, 27);
            txtPassword.TabIndex = 1;
            txtPassword.UseSystemPasswordChar = true;

            // lblConfirm / txtConfirm
            lblConfirm.AutoSize = true;
            lblConfirm.Location = new Point(15, 115);
            lblConfirm.Name = "lblConfirm";
            lblConfirm.Text = "Xác nhận mật khẩu:";
            txtConfirm.Location = new Point(150, 112);
            txtConfirm.Name = "txtConfirm";
            txtConfirm.Size = new Size(250, 27);
            txtConfirm.TabIndex = 2;
            txtConfirm.UseSystemPasswordChar = true;

            // grpExtra
            grpExtra.Controls.Add(lblBirth);
            grpExtra.Controls.Add(dtpBirthDate);
            grpExtra.Controls.Add(lblGender);
            grpExtra.Controls.Add(rdoMale);
            grpExtra.Controls.Add(rdoFemale);
            grpExtra.Controls.Add(chkTerms);
            grpExtra.Location = new Point(15, 190);
            grpExtra.Name = "grpExtra";
            grpExtra.Size = new Size(430, 160);
            grpExtra.TabIndex = 1;
            grpExtra.TabStop = false;
            grpExtra.Text = "Thông tin bổ sung";

            // lblBirth / dtpBirthDate
            lblBirth.AutoSize = true;
            lblBirth.Location = new Point(15, 35);
            lblBirth.Name = "lblBirth";
            lblBirth.Text = "Ngày sinh:";
            dtpBirthDate.Format = DateTimePickerFormat.Short;
            dtpBirthDate.Location = new Point(150, 32);
            dtpBirthDate.Name = "dtpBirthDate";
            dtpBirthDate.Size = new Size(250, 27);
            dtpBirthDate.TabIndex = 3;

            // lblGender / rdoMale / rdoFemale
            lblGender.AutoSize = true;
            lblGender.Location = new Point(15, 77);
            lblGender.Name = "lblGender";
            lblGender.Text = "Giới tính:";
            rdoMale.AutoSize = true;
            rdoMale.Checked = true;
            rdoMale.Location = new Point(150, 75);
            rdoMale.Name = "rdoMale";
            rdoMale.TabIndex = 4;
            rdoMale.TabStop = true;
            rdoMale.Text = "Nam";
            rdoFemale.AutoSize = true;
            rdoFemale.Location = new Point(240, 75);
            rdoFemale.Name = "rdoFemale";
            rdoFemale.TabIndex = 5;
            rdoFemale.Text = "Nữ";

            // chkTerms
            chkTerms.AutoSize = true;
            chkTerms.Location = new Point(15, 115);
            chkTerms.Name = "chkTerms";
            chkTerms.TabIndex = 6;
            chkTerms.Text = "Tôi đồng ý với Điều khoản dịch vụ";

            // btnRegister
            btnRegister.Location = new Point(130, 365);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(135, 40);
            btnRegister.TabIndex = 7;
            btnRegister.Text = "Đăng Ký";
            btnRegister.UseVisualStyleBackColor = true;
            btnRegister.Click += btnRegister_Click;

            // btnReset
            btnReset.Location = new Point(280, 365);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(135, 40);
            btnReset.TabIndex = 8;
            btnReset.Text = "Làm Mới";
            btnReset.UseVisualStyleBackColor = true;
            btnReset.Click += btnReset_Click;

            // epCheck
            epCheck.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            epCheck.ContainerControl = this;

            // Form1
            AcceptButton = btnRegister;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(460, 425);
            Controls.Add(grpPersonal);
            Controls.Add(grpExtra);
            Controls.Add(btnRegister);
            Controls.Add(btnReset);
            Font = new Font("Segoe UI", 10F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Đăng ký tài khoản";

            grpPersonal.ResumeLayout(false);
            grpPersonal.PerformLayout();
            grpExtra.ResumeLayout(false);
            grpExtra.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)epCheck).EndInit();
            ResumeLayout(false);
        }

        private GroupBox grpPersonal;
        private Label lblUsername;
        private Label lblPassword;
        private Label lblConfirm;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtConfirm;
        private GroupBox grpExtra;
        private Label lblBirth;
        private Label lblGender;
        private DateTimePicker dtpBirthDate;
        private RadioButton rdoMale;
        private RadioButton rdoFemale;
        private CheckBox chkTerms;
        private Button btnRegister;
        private Button btnReset;
        private ErrorProvider epCheck;
    }
}
