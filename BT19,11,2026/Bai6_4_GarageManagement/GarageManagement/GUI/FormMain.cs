using System;
using System.Drawing;
using System.Windows.Forms;

namespace GarageManagement.GUI
{
    public class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Hệ thống Quản lý Garage và Bảo dưỡng Xe Ô tô (Bài 6.4)";
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
                Text = "GARAGE & BẢO DƯỠNG XE Ô TÔ CHUYÊN NGHIỆP",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblSub = new Label
            {
                Text = "Tra cứu SqlParameter | Quản lý Phụ tùng | Lập Phiếu & Trừ Kho Bằng SqlTransaction | Thanh toán Hóa đơn",
                Font = new Font("Segoe UI", 11F, FontStyle.Italic),
                ForeColor = Color.FromArgb(189, 195, 199),
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter
            };

            pnlHeader.Controls.Add(lblSub);
            pnlHeader.Controls.Add(lblTitle);

            // Center Panel
            var pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(40)
            };

            var cardTiepNhan = CreateNavCard(
                "1. TIẾP NHẬN XE & CHỦ XE",
                "Tra cứu biển số bằng SqlParameter; đăng ký chủ xe và xe mới vào hệ thống.",
                Color.FromArgb(41, 128, 185),
                (s, e) => new FormTiepNhanXe().ShowDialog(this)
            );
            cardTiepNhan.Location = new Point(100, 40);

            var cardPhuTung = CreateNavCard(
                "2. DANH MỤC PHỤ TÙNG",
                "Quản lý danh mục phụ tùng, đơn giá và số lượng tồn kho (ràng buộc tồn kho >= 0).",
                Color.FromArgb(52, 73, 94),
                (s, e) => new FormPhuTung().ShowDialog(this)
            );
            cardPhuTung.Location = new Point(520, 40);

            var cardLapPhieu = CreateNavCard(
                "3. LẬP PHIẾU SỬA CHỮA",
                "Chọn xe, lập phiếu thay thế phụ tùng & tính tiền công; thực thi SqlTransaction tự động trừ kho và rollback.",
                Color.FromArgb(142, 68, 173),
                (s, e) => new FormLapPhieuSuaChua().ShowDialog(this)
            );
            cardLapPhieu.Location = new Point(100, 240);

            var cardThanhToan = CreateNavCard(
                "4. THANH TOÁN & HÓA ĐƠN",
                "Tính tổng tiền = (Phụ tùng x Đơn giá) + Tiền công, chuyển trạng thái 'Đã thanh toán' và in hóa đơn.",
                Color.FromArgb(39, 174, 96),
                (s, e) => new FormThanhToan().ShowDialog(this)
            );
            cardThanhToan.Location = new Point(520, 240);

            pnlContent.Controls.AddRange(new Control[] { cardTiepNhan, cardPhuTung, cardLapPhieu, cardThanhToan });

            var statusStrip = new StatusStrip();
            var lblStatus = new ToolStripStatusLabel
            {
                Text = "Cơ sở dữ liệu: GarageDB (MSSQLLocalDB / SQL Server) | Trạng thái: Sẵn sàng phục vụ"
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
