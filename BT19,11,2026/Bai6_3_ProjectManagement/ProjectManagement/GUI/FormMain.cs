using System;
using System.Drawing;
using System.Windows.Forms;

namespace ProjectManagement.GUI
{
    public class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Hệ thống Quản lý Công việc & Tiến độ Dự án (Bài 6.3 - 3-Tier Layered)";
            this.Size = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            this.BackColor = Color.FromArgb(245, 247, 250);

            // Header Banner
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 110,
                BackColor = Color.FromArgb(44, 62, 80),
                Padding = new Padding(20)
            };

            var lblTitle = new Label
            {
                Text = "HỆ THỐNG QUẢN LÝ CÔNG VIỆC & TIẾN ĐỘ DỰ ÁN",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblSub = new Label
            {
                Text = "Sinh viên thực hiện: Phan Công Thành Đạt | MSV: 24810310223 | Lớp: D19CNPM3",
                Font = new Font("Segoe UI", 11F, FontStyle.Italic),
                ForeColor = Color.FromArgb(189, 195, 199),
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter
            };

            pnlHeader.Controls.Add(lblSub);
            pnlHeader.Controls.Add(lblTitle);

            // Center Panel with modern card buttons
            var pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(40)
            };

            var cardDuAn = CreateNavCard(
                "1. QUẢN LÝ DỰ ÁN",
                "Thêm, sửa, xóa dự án, theo dõi tiến độ và trạng thái các dự án trong hệ thống.",
                Color.FromArgb(41, 128, 185),
                (s, e) => new FormDuAn().ShowDialog(this)
            );
            cardDuAn.Location = new Point(100, 40);

            var cardTask = CreateNavCard(
                "2. CÔNG VIỆC & PHÂN CÔNG",
                "Quản lý danh sách công việc, phân công thành viên phụ trách và cập nhật trạng thái.",
                Color.FromArgb(39, 174, 96),
                (s, e) => new FormCongViec().ShowDialog(this)
            );
            cardTask.Location = new Point(520, 40);

            var cardBaoCao = CreateNavCard(
                "3. BÁO CÁO & THỐNG KÊ",
                "Cảnh báo danh sách công việc trễ hạn và thống kê tỷ lệ hoàn thành theo từng dự án.",
                Color.FromArgb(230, 126, 34),
                (s, e) => new FormBaoCao().ShowDialog(this)
            );
            cardBaoCao.Location = new Point(100, 240);

            var cardThoat = CreateNavCard(
                "4. THOÁT HỆ THỐNG",
                "Đóng ứng dụng an toàn và giải phóng các kết nối cơ sở dữ liệu.",
                Color.FromArgb(127, 140, 141),
                (s, e) => this.Close()
            );
            cardThoat.Location = new Point(520, 240);

            pnlContent.Controls.AddRange(new Control[] { cardDuAn, cardTask, cardBaoCao, cardThoat });

            // Status strip at bottom
            var statusStrip = new StatusStrip();
            var lblStatus = new ToolStripStatusLabel
            {
                Text = "Cơ sở dữ liệu: ProjectTaskDB (MSSQLLocalDB / SQL Server) | Trạng thái: Sẵn sàng hoạt động"
            };
            statusStrip.Items.Add(lblStatus);

            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(statusStrip);
        }

        private Panel CreateNavCard(string title, string description, Color accentColor, EventHandler onClick)
        {
            var pnl = new Panel
            {
                Size = new Size(360, 160),
                BackColor = Color.White,
                Cursor = Cursors.Hand
            };

            var pnlAccent = new Panel
            {
                Dock = DockStyle.Left,
                Width = 10,
                BackColor = accentColor
            };

            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = accentColor,
                Location = new Point(25, 20),
                Size = new Size(320, 30)
            };

            var lblDesc = new Label
            {
                Text = description,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(70, 70, 70),
                Location = new Point(25, 55),
                Size = new Size(320, 50)
            };

            var btnOpen = new Button
            {
                Text = "Mở chức năng ➔",
                Location = new Point(25, 115),
                Size = new Size(130, 30),
                BackColor = accentColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnOpen.Click += onClick;
            pnl.Click += onClick;
            lblTitle.Click += onClick;
            lblDesc.Click += onClick;

            pnl.Controls.AddRange(new Control[] { pnlAccent, lblTitle, lblDesc, btnOpen });
            return pnl;
        }
    }
}
