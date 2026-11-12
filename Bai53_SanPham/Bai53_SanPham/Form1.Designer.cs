#nullable disable
using System.Drawing;
using System.Windows.Forms;

namespace Bai53_SanPham
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
            grpInfo = new GroupBox();
            lblId = new Label();
            txtId = new TextBox();
            lblName = new Label();
            txtName = new TextBox();
            lblPrice = new Label();
            nudUnitPrice = new NumericUpDown();
            lblQuantity = new Label();
            nudQuantity = new NumericUpDown();
            lblCategory = new Label();
            cboCategory = new ComboBox();
            grpActions = new GroupBox();
            btnAdd = new Button();
            btnEdit = new Button();
            btnDelete = new Button();
            btnSearch = new Button();
            lblSearch = new Label();
            txtSearch = new TextBox();
            btnShowAll = new Button();
            lblCount = new Label();
            dgvProducts = new DataGridView();
            bsProducts = new BindingSource(components);
            epCheck = new ErrorProvider(components);
            grpInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudUnitPrice).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudQuantity).BeginInit();
            grpActions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvProducts).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bsProducts).BeginInit();
            ((System.ComponentModel.ISupportInitialize)epCheck).BeginInit();
            SuspendLayout();

            // grpInfo
            grpInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            grpInfo.Controls.Add(lblId);
            grpInfo.Controls.Add(txtId);
            grpInfo.Controls.Add(lblName);
            grpInfo.Controls.Add(txtName);
            grpInfo.Controls.Add(lblPrice);
            grpInfo.Controls.Add(nudUnitPrice);
            grpInfo.Controls.Add(lblQuantity);
            grpInfo.Controls.Add(nudQuantity);
            grpInfo.Controls.Add(lblCategory);
            grpInfo.Controls.Add(cboCategory);
            grpInfo.Location = new Point(12, 12);
            grpInfo.Name = "grpInfo";
            grpInfo.Size = new Size(480, 245);
            grpInfo.TabIndex = 0;
            grpInfo.TabStop = false;
            grpInfo.Text = "Thông tin sản phẩm";

            lblId.AutoSize = true;
            lblId.Location = new Point(15, 35);
            lblId.Name = "lblId";
            lblId.Text = "Mã SP:";
            txtId.Location = new Point(130, 32);
            txtId.Name = "txtId";
            txtId.Size = new Size(320, 29);
            txtId.TabIndex = 0;

            lblName.AutoSize = true;
            lblName.Location = new Point(15, 75);
            lblName.Name = "lblName";
            lblName.Text = "Tên SP:";
            txtName.Location = new Point(130, 72);
            txtName.Name = "txtName";
            txtName.Size = new Size(320, 29);
            txtName.TabIndex = 1;

            lblPrice.AutoSize = true;
            lblPrice.Location = new Point(15, 115);
            lblPrice.Name = "lblPrice";
            lblPrice.Text = "Đơn giá:";
            nudUnitPrice.Increment = new decimal(new int[] { 1000, 0, 0, 0 });
            nudUnitPrice.Location = new Point(130, 112);
            nudUnitPrice.Maximum = new decimal(new int[] { 1000000000, 0, 0, 0 });
            nudUnitPrice.Name = "nudUnitPrice";
            nudUnitPrice.Size = new Size(320, 29);
            nudUnitPrice.TabIndex = 2;
            nudUnitPrice.TextAlign = HorizontalAlignment.Right;
            nudUnitPrice.ThousandsSeparator = true;

            lblQuantity.AutoSize = true;
            lblQuantity.Location = new Point(15, 155);
            lblQuantity.Name = "lblQuantity";
            lblQuantity.Text = "Số lượng:";
            nudQuantity.Location = new Point(130, 152);
            nudQuantity.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            nudQuantity.Name = "nudQuantity";
            nudQuantity.Size = new Size(320, 29);
            nudQuantity.TabIndex = 3;
            nudQuantity.TextAlign = HorizontalAlignment.Right;
            nudQuantity.ThousandsSeparator = true;

            lblCategory.AutoSize = true;
            lblCategory.Location = new Point(15, 195);
            lblCategory.Name = "lblCategory";
            lblCategory.Text = "Danh mục:";
            cboCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCategory.FormattingEnabled = true;
            cboCategory.Location = new Point(130, 192);
            cboCategory.Name = "cboCategory";
            cboCategory.Size = new Size(320, 29);
            cboCategory.TabIndex = 4;

            // grpActions
            grpActions.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            grpActions.Controls.Add(btnAdd);
            grpActions.Controls.Add(btnEdit);
            grpActions.Controls.Add(btnDelete);
            grpActions.Controls.Add(btnSearch);
            grpActions.Controls.Add(lblSearch);
            grpActions.Controls.Add(txtSearch);
            grpActions.Controls.Add(btnShowAll);
            grpActions.Controls.Add(lblCount);
            grpActions.Location = new Point(505, 12);
            grpActions.Name = "grpActions";
            grpActions.Size = new Size(343, 245);
            grpActions.TabIndex = 1;
            grpActions.TabStop = false;
            grpActions.Text = "Chức năng";

            btnAdd.Location = new Point(20, 35);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(140, 40);
            btnAdd.TabIndex = 5;
            btnAdd.Text = "Thêm";
            btnAdd.UseVisualStyleBackColor = true;

            btnEdit.Location = new Point(175, 35);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new Size(140, 40);
            btnEdit.TabIndex = 6;
            btnEdit.Text = "Sửa";
            btnEdit.UseVisualStyleBackColor = true;

            btnDelete.Location = new Point(20, 90);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(140, 40);
            btnDelete.TabIndex = 7;
            btnDelete.Text = "Xóa";
            btnDelete.UseVisualStyleBackColor = true;

            btnSearch.Location = new Point(175, 90);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(140, 40);
            btnSearch.TabIndex = 8;
            btnSearch.Text = "Tìm kiếm";
            btnSearch.UseVisualStyleBackColor = true;

            lblSearch.AutoSize = true;
            lblSearch.Location = new Point(20, 148);
            lblSearch.Name = "lblSearch";
            lblSearch.Text = "Tìm theo Tên SP:";
            txtSearch.Location = new Point(20, 175);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(190, 29);
            txtSearch.TabIndex = 9;
            btnShowAll.Location = new Point(220, 172);
            btnShowAll.Name = "btnShowAll";
            btnShowAll.Size = new Size(95, 34);
            btnShowAll.TabIndex = 10;
            btnShowAll.Text = "Hiện tất cả";
            btnShowAll.UseVisualStyleBackColor = true;

            lblCount.AutoSize = true;
            lblCount.ForeColor = Color.DimGray;
            lblCount.Location = new Point(20, 213);
            lblCount.Name = "lblCount";
            lblCount.Text = "Đang hiển thị: 0 / 0 sản phẩm";

            // dgvProducts
            dgvProducts.AllowUserToAddRows = false;
            dgvProducts.AllowUserToDeleteRows = false;
            dgvProducts.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvProducts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProducts.Location = new Point(12, 270);
            dgvProducts.MultiSelect = false;
            dgvProducts.Name = "dgvProducts";
            dgvProducts.ReadOnly = true;
            dgvProducts.RowHeadersVisible = false;
            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.Size = new Size(836, 278);
            dgvProducts.TabIndex = 11;

            // epCheck
            epCheck.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            epCheck.ContainerControl = this;

            // Form1
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(860, 560);
            Controls.Add(grpInfo);
            Controls.Add(grpActions);
            Controls.Add(dgvProducts);
            Font = new Font("Segoe UI", 10F);
            MinimumSize = new Size(760, 520);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Quản lý danh sách sản phẩm";

            grpInfo.ResumeLayout(false);
            grpInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudUnitPrice).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudQuantity).EndInit();
            grpActions.ResumeLayout(false);
            grpActions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvProducts).EndInit();
            ((System.ComponentModel.ISupportInitialize)bsProducts).EndInit();
            ((System.ComponentModel.ISupportInitialize)epCheck).EndInit();
            ResumeLayout(false);
        }

        private GroupBox grpInfo;
        private Label lblId;
        private TextBox txtId;
        private Label lblName;
        private TextBox txtName;
        private Label lblPrice;
        private NumericUpDown nudUnitPrice;
        private Label lblQuantity;
        private NumericUpDown nudQuantity;
        private Label lblCategory;
        private ComboBox cboCategory;
        private GroupBox grpActions;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnSearch;
        private Label lblSearch;
        private TextBox txtSearch;
        private Button btnShowAll;
        private Label lblCount;
        private DataGridView dgvProducts;
        private BindingSource bsProducts;
        private ErrorProvider epCheck;
    }
}
