using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ProjectManagement.GUI;

namespace ProjectManagement
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

            Application.Run(new FormMain());
        }

        private static void CaptureScreens()
        {
            string imgDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\images"));
            Directory.CreateDirectory(imgDir);

            // 1. FormMain
            using (var f = new FormMain())
            {
                f.StartPosition = FormStartPosition.Manual;
                f.Location = new Point(50, 50);
                f.Show();
                Application.DoEvents();
                Thread.Sleep(300);
                SaveForm(f, Path.Combine(imgDir, "01_main_screen.png"));
                f.Close();
            }

            // 2. FormDuAn (CRUD)
            using (var f = new FormDuAn())
            {
                f.StartPosition = FormStartPosition.Manual;
                f.Location = new Point(50, 50);
                f.Show();
                f.SelectRow(0);
                Application.DoEvents();
                Thread.Sleep(400);
                SaveForm(f, Path.Combine(imgDir, "02_crud_screen.png"));
                f.Close();
            }

            // 3. FormCongViec (Transaction)
            using (var f = new FormCongViec())
            {
                f.StartPosition = FormStartPosition.Manual;
                f.Location = new Point(50, 50);
                f.Show();
                Application.DoEvents();
                Thread.Sleep(400);
                SaveForm(f, Path.Combine(imgDir, "03_transaction_screen.png"));
                f.Close();
            }

            // 4. FormBaoCao (Report - Tab 1 Overdue)
            using (var f = new FormBaoCao())
            {
                f.StartPosition = FormStartPosition.Manual;
                f.Location = new Point(50, 50);
                f.Show();
                Application.DoEvents();
                Thread.Sleep(400);
                SaveForm(f, Path.Combine(imgDir, "04_report_screen.png"));

                // 5. FormBaoCao (Report - Tab 2 Thống kê)
                foreach (Control c in f.Controls)
                {
                    if (c is TabControl tc && tc.TabPages.Count > 1)
                    {
                        tc.SelectedIndex = 1;
                        Application.DoEvents();
                        Thread.Sleep(300);
                        SaveForm(f, Path.Combine(imgDir, "05_error_handling.png"));
                        break;
                    }
                }

                f.Close();
            }

            Console.WriteLine("Captured all screens to " + imgDir);
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
