#nullable disable
using System.Drawing;
using System.Windows.Forms;

namespace Bai54_TreeList
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
            pnlTop = new Panel();
            lblView = new Label();
            cboView = new ComboBox();
            splitMain = new SplitContainer();
            tvDepartments = new TreeView();
            lsvEmployees = new ListView();
            colId = new ColumnHeader();
            colName = new ColumnHeader();
            colPosition = new ColumnHeader();
            colDate = new ColumnHeader();
            colUnit = new ColumnHeader();
            imgSmall = new ImageList(components);
            imgLarge = new ImageList(components);
            statusMain = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            statusMain.SuspendLayout();
            SuspendLayout();

            // imgSmall: biểu tượng nhỏ cho TreeView và ListView (Details/List/SmallIcon)
            imgSmall.ColorDepth = ColorDepth.Depth32Bit;
            imgSmall.ImageSize = new Size(20, 20);

            // imgLarge: biểu tượng lớn cho ListView (LargeIcon/Tile)
            imgLarge.ColorDepth = ColorDepth.Depth32Bit;
            imgLarge.ImageSize = new Size(48, 48);

            // pnlTop
            pnlTop.Controls.Add(lblView);
            pnlTop.Controls.Add(cboView);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 0);
            pnlTop.Name = "pnlTop";
            pnlTop.Size = new Size(980, 48);
            pnlTop.TabIndex = 0;

            lblView.AutoSize = true;
            lblView.Location = new Point(12, 14);
            lblView.Name = "lblView";
            lblView.Text = "Chế độ xem:";

            cboView.DropDownStyle = ComboBoxStyle.DropDownList;
            cboView.FormattingEnabled = true;
            cboView.Location = new Point(110, 10);
            cboView.Name = "cboView";
            cboView.Size = new Size(260, 29);
            cboView.TabIndex = 0;

            // splitMain
            splitMain.Dock = DockStyle.Fill;
            splitMain.Location = new Point(0, 48);
            splitMain.Name = "splitMain";
            splitMain.Panel1.Controls.Add(tvDepartments);
            splitMain.Panel2.Controls.Add(lsvEmployees);
            splitMain.Size = new Size(980, 502);
            splitMain.SplitterDistance = 300;
            splitMain.TabIndex = 1;

            // tvDepartments
            tvDepartments.Dock = DockStyle.Fill;
            tvDepartments.HideSelection = false;
            tvDepartments.ImageList = imgSmall;
            tvDepartments.ItemHeight = 26;
            tvDepartments.Location = new Point(0, 0);
            tvDepartments.Name = "tvDepartments";
            tvDepartments.PathSeparator = " > ";
            tvDepartments.ShowLines = true;
            tvDepartments.Size = new Size(300, 502);
            tvDepartments.TabIndex = 0;

            // lsvEmployees
            lsvEmployees.Columns.AddRange(new ColumnHeader[] { colId, colName, colPosition, colDate, colUnit });
            lsvEmployees.Dock = DockStyle.Fill;
            lsvEmployees.FullRowSelect = true;
            lsvEmployees.GridLines = true;
            lsvEmployees.HideSelection = false;
            lsvEmployees.LargeImageList = imgLarge;
            lsvEmployees.Location = new Point(0, 0);
            lsvEmployees.MultiSelect = false;
            lsvEmployees.Name = "lsvEmployees";
            lsvEmployees.Size = new Size(676, 502);
            lsvEmployees.SmallImageList = imgSmall;
            lsvEmployees.TabIndex = 0;
            lsvEmployees.TileSize = new Size(290, 56);
            lsvEmployees.UseCompatibleStateImageBehavior = false;
            lsvEmployees.View = View.Details;

            colId.Text = "Mã NV";
            colId.Width = 90;
            colName.Text = "Họ tên";
            colName.Width = 200;
            colPosition.Text = "Chức vụ";
            colPosition.Width = 190;
            colDate.Text = "Ngày vào làm";
            colDate.Width = 130;
            colUnit.Text = "Đơn vị";
            colUnit.Width = 230;

            // statusMain
            statusMain.Items.AddRange(new ToolStripItem[] { lblStatus });
            statusMain.Location = new Point(0, 550);
            statusMain.Name = "statusMain";
            statusMain.Size = new Size(980, 26);
            statusMain.TabIndex = 2;

            lblStatus.Name = "lblStatus";
            lblStatus.Text = "Sẵn sàng";

            // Form1
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(980, 576);
            Controls.Add(splitMain);
            Controls.Add(pnlTop);
            Controls.Add(statusMain);
            Font = new Font("Segoe UI", 10F);
            MinimumSize = new Size(720, 440);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Quản lý nhân sự theo cơ cấu tổ chức";

            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            splitMain.Panel1.ResumeLayout(false);
            splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            splitMain.ResumeLayout(false);
            statusMain.ResumeLayout(false);
            statusMain.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private Panel pnlTop;
        private Label lblView;
        private ComboBox cboView;
        private SplitContainer splitMain;
        private TreeView tvDepartments;
        private ListView lsvEmployees;
        private ColumnHeader colId;
        private ColumnHeader colName;
        private ColumnHeader colPosition;
        private ColumnHeader colDate;
        private ColumnHeader colUnit;
        private ImageList imgSmall;
        private ImageList imgLarge;
        private StatusStrip statusMain;
        private ToolStripStatusLabel lblStatus;
    }
}
