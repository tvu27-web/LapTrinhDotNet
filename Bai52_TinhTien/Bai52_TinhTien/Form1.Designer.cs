#nullable disable
using System.Drawing;
using System.Windows.Forms;

namespace Bai52_TinhTien
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
            lblCategory = new Label();
            cboCategory = new ComboBox();
            lblAvailable = new Label();
            lstAvailableServices = new ListBox();
            btnSelect = new Button();
            btnRemove = new Button();
            btnClearAll = new Button();
            lblSelected = new Label();
            lstSelectedServices = new ListBox();
            grpBilling = new GroupBox();
            lblSubtotal = new Label();
            txtSubtotal = new TextBox();
            lblPromo = new Label();
            txtPromo = new TextBox();
            btnApplyPromo = new Button();
            lblDiscountRate = new Label();
            nudDiscount = new NumericUpDown();
            lblPromoInfo = new Label();
            lblDiscountAmount = new Label();
            txtDiscountAmount = new TextBox();
            lblPayable = new Label();
            txtPayable = new TextBox();
            lblCount = new Label();
            epPromo = new ErrorProvider(components);
            grpBilling.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudDiscount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)epPromo).BeginInit();
            SuspendLayout();

            // lblCategory / cboCategory
            lblCategory.AutoSize = true;
            lblCategory.Location = new Point(15, 18);
            lblCategory.Name = "lblCategory";
            lblCategory.Text = "Loại dịch vụ:";
            cboCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCategory.FormattingEnabled = true;
            cboCategory.Location = new Point(125, 15);
            cboCategory.Name = "cboCategory";
            cboCategory.Size = new Size(260, 29);
            cboCategory.TabIndex = 0;

            // lblAvailable / lstAvailableServices
            lblAvailable.AutoSize = true;
            lblAvailable.Location = new Point(15, 55);
            lblAvailable.Name = "lblAvailable";
            lblAvailable.Text = "Dịch vụ có sẵn:";
            lstAvailableServices.FormattingEnabled = true;
            lstAvailableServices.IntegralHeight = false;
            lstAvailableServices.ItemHeight = 21;
            lstAvailableServices.Location = new Point(15, 80);
            lstAvailableServices.Name = "lstAvailableServices";
            lstAvailableServices.SelectionMode = SelectionMode.MultiExtended;
            lstAvailableServices.Size = new Size(340, 230);
            lstAvailableServices.TabIndex = 1;

            // btnSelect / btnRemove / btnClearAll
            btnSelect.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnSelect.Location = new Point(370, 105);
            btnSelect.Name = "btnSelect";
            btnSelect.Size = new Size(60, 40);
            btnSelect.TabIndex = 2;
            btnSelect.Text = ">";
            btnSelect.UseVisualStyleBackColor = true;

            btnRemove.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnRemove.Location = new Point(370, 160);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(60, 40);
            btnRemove.TabIndex = 3;
            btnRemove.Text = "<";
            btnRemove.UseVisualStyleBackColor = true;

            btnClearAll.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnClearAll.Location = new Point(370, 215);
            btnClearAll.Name = "btnClearAll";
            btnClearAll.Size = new Size(60, 40);
            btnClearAll.TabIndex = 4;
            btnClearAll.Text = "<<";
            btnClearAll.UseVisualStyleBackColor = true;

            // lblSelected / lstSelectedServices
            lblSelected.AutoSize = true;
            lblSelected.Location = new Point(445, 55);
            lblSelected.Name = "lblSelected";
            lblSelected.Text = "Dịch vụ đã chọn:";
            lstSelectedServices.FormattingEnabled = true;
            lstSelectedServices.IntegralHeight = false;
            lstSelectedServices.ItemHeight = 21;
            lstSelectedServices.Location = new Point(445, 80);
            lstSelectedServices.Name = "lstSelectedServices";
            lstSelectedServices.SelectionMode = SelectionMode.MultiExtended;
            lstSelectedServices.Size = new Size(345, 230);
            lstSelectedServices.TabIndex = 5;

            // grpBilling
            grpBilling.Controls.Add(lblSubtotal);
            grpBilling.Controls.Add(txtSubtotal);
            grpBilling.Controls.Add(lblPromo);
            grpBilling.Controls.Add(txtPromo);
            grpBilling.Controls.Add(btnApplyPromo);
            grpBilling.Controls.Add(lblDiscountRate);
            grpBilling.Controls.Add(nudDiscount);
            grpBilling.Controls.Add(lblPromoInfo);
            grpBilling.Controls.Add(lblDiscountAmount);
            grpBilling.Controls.Add(txtDiscountAmount);
            grpBilling.Controls.Add(lblPayable);
            grpBilling.Controls.Add(txtPayable);
            grpBilling.Controls.Add(lblCount);
            grpBilling.Location = new Point(15, 325);
            grpBilling.Name = "grpBilling";
            grpBilling.Size = new Size(775, 185);
            grpBilling.TabIndex = 6;
            grpBilling.TabStop = false;
            grpBilling.Text = "Tính tiền";

            // Cột trái
            lblSubtotal.AutoSize = true;
            lblSubtotal.Location = new Point(15, 35);
            lblSubtotal.Name = "lblSubtotal";
            lblSubtotal.Text = "Tổng tiền chưa giảm:";
            txtSubtotal.Location = new Point(200, 32);
            txtSubtotal.Name = "txtSubtotal";
            txtSubtotal.ReadOnly = true;
            txtSubtotal.Size = new Size(190, 29);
            txtSubtotal.TabIndex = 7;
            txtSubtotal.TabStop = false;
            txtSubtotal.TextAlign = HorizontalAlignment.Right;

            lblPromo.AutoSize = true;
            lblPromo.Location = new Point(15, 75);
            lblPromo.Name = "lblPromo";
            lblPromo.Text = "Mã giảm giá:";
            txtPromo.Location = new Point(200, 72);
            txtPromo.Name = "txtPromo";
            txtPromo.Size = new Size(100, 29);
            txtPromo.TabIndex = 8;
            btnApplyPromo.Location = new Point(310, 70);
            btnApplyPromo.Name = "btnApplyPromo";
            btnApplyPromo.Size = new Size(80, 33);
            btnApplyPromo.TabIndex = 9;
            btnApplyPromo.Text = "Áp dụng";
            btnApplyPromo.UseVisualStyleBackColor = true;

            lblDiscountRate.AutoSize = true;
            lblDiscountRate.Location = new Point(15, 115);
            lblDiscountRate.Name = "lblDiscountRate";
            lblDiscountRate.Text = "Tỷ lệ chiết khấu (%):";
            nudDiscount.Location = new Point(200, 112);
            nudDiscount.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            nudDiscount.Name = "nudDiscount";
            nudDiscount.Size = new Size(100, 29);
            nudDiscount.TabIndex = 10;
            nudDiscount.TextAlign = HorizontalAlignment.Right;

            lblPromoInfo.AutoSize = true;
            lblPromoInfo.ForeColor = Color.Gray;
            lblPromoInfo.Location = new Point(15, 150);
            lblPromoInfo.Name = "lblPromoInfo";
            lblPromoInfo.Text = "Mã mẫu: GIAM10, GIAM20, VIP30";

            // Cột phải
            lblDiscountAmount.AutoSize = true;
            lblDiscountAmount.Location = new Point(415, 35);
            lblDiscountAmount.Name = "lblDiscountAmount";
            lblDiscountAmount.Text = "Số tiền được giảm:";
            txtDiscountAmount.Location = new Point(610, 32);
            txtDiscountAmount.Name = "txtDiscountAmount";
            txtDiscountAmount.ReadOnly = true;
            txtDiscountAmount.Size = new Size(145, 29);
            txtDiscountAmount.TabIndex = 11;
            txtDiscountAmount.TabStop = false;
            txtDiscountAmount.TextAlign = HorizontalAlignment.Right;

            lblPayable.AutoSize = true;
            lblPayable.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPayable.Location = new Point(415, 77);
            lblPayable.Name = "lblPayable";
            lblPayable.Text = "Thành tiền thanh toán:";
            txtPayable.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            txtPayable.ForeColor = Color.Firebrick;
            txtPayable.Location = new Point(610, 72);
            txtPayable.Name = "txtPayable";
            txtPayable.ReadOnly = true;
            txtPayable.Size = new Size(145, 32);
            txtPayable.TabIndex = 12;
            txtPayable.TabStop = false;
            txtPayable.TextAlign = HorizontalAlignment.Right;

            lblCount.AutoSize = true;
            lblCount.Location = new Point(415, 120);
            lblCount.Name = "lblCount";
            lblCount.Text = "Số dịch vụ đã chọn: 0";

            // epPromo
            epPromo.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            epPromo.ContainerControl = this;

            // Form1
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(805, 525);
            Controls.Add(lblCategory);
            Controls.Add(cboCategory);
            Controls.Add(lblAvailable);
            Controls.Add(lstAvailableServices);
            Controls.Add(btnSelect);
            Controls.Add(btnRemove);
            Controls.Add(btnClearAll);
            Controls.Add(lblSelected);
            Controls.Add(lstSelectedServices);
            Controls.Add(grpBilling);
            Font = new Font("Segoe UI", 10F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Bảng tính tiền dịch vụ và chiết khấu đơn hàng";

            grpBilling.ResumeLayout(false);
            grpBilling.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudDiscount).EndInit();
            ((System.ComponentModel.ISupportInitialize)epPromo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private Label lblCategory;
        private ComboBox cboCategory;
        private Label lblAvailable;
        private ListBox lstAvailableServices;
        private Button btnSelect;
        private Button btnRemove;
        private Button btnClearAll;
        private Label lblSelected;
        private ListBox lstSelectedServices;
        private GroupBox grpBilling;
        private Label lblSubtotal;
        private TextBox txtSubtotal;
        private Label lblPromo;
        private TextBox txtPromo;
        private Button btnApplyPromo;
        private Label lblDiscountRate;
        private NumericUpDown nudDiscount;
        private Label lblPromoInfo;
        private Label lblDiscountAmount;
        private TextBox txtDiscountAmount;
        private Label lblPayable;
        private TextBox txtPayable;
        private Label lblCount;
        private ErrorProvider epPromo;
    }
}
