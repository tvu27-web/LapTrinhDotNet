using System.Drawing.Drawing2D;

namespace Bai54_TreeList
{
    /// <summary>
    /// Tự vẽ biểu tượng bằng code để khỏi phải thêm file ảnh vào project.
    /// Mỗi biểu tượng được vẽ trên khung 32x32 rồi co giãn về kích thước ImageList.
    /// </summary>
    public static class IconFactory
    {
        public static Bitmap Create(string kind, int size)
        {
            var bmp = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.ScaleTransform(size / 32f, size / 32f);

                switch (kind)
                {
                    case "company":
                        DrawCompany(g);
                        break;
                    case "department":
                        DrawFolder(g);
                        break;
                    case "group":
                        DrawPerson(g, 11, 6, 9, Color.FromArgb(125, 180, 130));
                        DrawPerson(g, 21, 8, 10, Color.FromArgb(46, 139, 87));
                        break;
                    case "manager":
                        DrawPerson(g, 15, 5, 10, Color.FromArgb(230, 126, 34));
                        DrawStar(g, 25, 7, 6.5f, 2.8f);
                        break;
                    default:   // "employee"
                        DrawPerson(g, 16, 4, 10, Color.FromArgb(52, 101, 164));
                        break;
                }
            }
            return bmp;
        }

        private static void DrawCompany(Graphics g)
        {
            using var body = new SolidBrush(Color.FromArgb(52, 101, 164));
            using var window = new SolidBrush(Color.White);
            using var door = new SolidBrush(Color.FromArgb(35, 60, 105));

            g.FillRectangle(body, 6, 3, 20, 27);
            for (int row = 0; row < 3; row++)
                for (int col = 0; col < 3; col++)
                    g.FillRectangle(window, 9 + col * 6, 6 + row * 5, 3, 3);
            g.FillRectangle(door, 13, 22, 6, 8);
        }

        private static void DrawFolder(Graphics g)
        {
            using var tab = new SolidBrush(Color.FromArgb(222, 160, 40));
            using var front = new SolidBrush(Color.FromArgb(247, 196, 70));
            using var edge = new Pen(Color.FromArgb(190, 130, 20), 1f);

            g.FillRectangle(tab, 3, 6, 12, 6);
            g.FillRectangle(front, 3, 10, 26, 18);
            g.DrawRectangle(edge, 3, 10, 26, 18);
        }

        // Người: đầu tròn + thân (nửa elip phía dưới)
        private static void DrawPerson(Graphics g, float cx, float top, float head, Color color)
        {
            using var brush = new SolidBrush(color);
            g.FillEllipse(brush, cx - head / 2, top, head, head);
            g.FillEllipse(brush, cx - head * 0.85f, top + head + 1, head * 1.7f, head * 1.7f);
        }

        private static void DrawStar(Graphics g, float cx, float cy, float outer, float inner)
        {
            var pts = new PointF[10];
            for (int i = 0; i < 10; i++)
            {
                double angle = -Math.PI / 2 + i * Math.PI / 5;
                float r = i % 2 == 0 ? outer : inner;
                pts[i] = new PointF(cx + (float)(Math.Cos(angle) * r), cy + (float)(Math.Sin(angle) * r));
            }
            using var fill = new SolidBrush(Color.Gold);
            using var line = new Pen(Color.FromArgb(180, 120, 0), 0.8f);
            g.FillPolygon(fill, pts);
            g.DrawPolygon(line, pts);
        }
    }
}
