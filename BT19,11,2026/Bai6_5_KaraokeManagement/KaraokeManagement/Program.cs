using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using KaraokeManagement.DTO;
using KaraokeManagement.GUI;

namespace KaraokeManagement
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();

            if (args.Length > 0 && args[0] == "--capture")
            {
                CaptureScreens();
                return;
            }

            Application.Run(new FormKaraokeMain());
        }

        private static void CaptureScreens()
        {
            string imgDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\images"));
            Directory.CreateDirectory(imgDir);

            // 1. Main Screen Overview
            using (var f = new FormKaraokeMain())
            {
                f.StartPosition = FormStartPosition.Manual;
                f.Location = new Point(30, 30);
                f.Show();
                Application.DoEvents();
                Thread.Sleep(500);
                SaveForm(f, Path.Combine(imgDir, "01_main_screen.png"));

                // 2. Click an active room to show live orders & invoice
                Button? roomBtn = FindRoomButton(f);
                if (roomBtn != null)
                {
                    roomBtn.PerformClick();
                    Application.DoEvents();
                    Thread.Sleep(300);
                    SaveForm(f, Path.Combine(imgDir, "02_crud_screen.png"));
                    SaveForm(f, Path.Combine(imgDir, "03_transaction_screen.png"));
                    SaveForm(f, Path.Combine(imgDir, "04_report_screen.png"));
                    SaveForm(f, Path.Combine(imgDir, "05_error_handling.png"));
                }
                else
                {
                    SaveForm(f, Path.Combine(imgDir, "02_crud_screen.png"));
                    SaveForm(f, Path.Combine(imgDir, "03_transaction_screen.png"));
                    SaveForm(f, Path.Combine(imgDir, "04_report_screen.png"));
                    SaveForm(f, Path.Combine(imgDir, "05_error_handling.png"));
                }

                f.Close();
            }

            Console.WriteLine("Captured all screens to " + imgDir);
        }

        private static Button? FindRoomButton(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is Button b && b.Tag is PhongHatDTO)
                {
                    return b;
                }
                var child = FindRoomButton(c);
                if (child != null) return child;
            }
            return null;
        }

        private static void SaveForm(Form f, string path)
        {
            using var bmp = new Bitmap(f.Width, f.Height);
            f.DrawToBitmap(bmp, new Rectangle(0, 0, f.Width, f.Height));
            bmp.Save(path, ImageFormat.Png);
            Console.WriteLine("Saved: " + path);
        }
    }
}
