using System.Collections;

namespace Bai54_TreeList
{
    /// <summary>Sắp xếp ListView khi bấm vào tiêu đề cột (ngày vào làm sắp theo ngày, không theo chữ).</summary>
    internal class EmployeeComparer : IComparer
    {
        private readonly int _column;
        private readonly bool _ascending;

        public EmployeeComparer(int column, bool ascending)
        {
            _column = column;
            _ascending = ascending;
        }

        public int Compare(object? x, object? y)
        {
            var a = (ListViewItem)x!;
            var b = (ListViewItem)y!;
            var ea = (Employee)a.Tag!;
            var eb = (Employee)b.Tag!;

            int result = _column switch
            {
                0 => string.Compare(ea.Id, eb.Id, StringComparison.OrdinalIgnoreCase),
                1 => string.Compare(ea.Name, eb.Name, StringComparison.CurrentCultureIgnoreCase),
                2 => string.Compare(ea.Position, eb.Position, StringComparison.CurrentCultureIgnoreCase),
                3 => ea.StartDate.CompareTo(eb.StartDate),
                _ => string.Compare(a.SubItems[4].Text, b.SubItems[4].Text, StringComparison.CurrentCultureIgnoreCase)
            };
            return _ascending ? result : -result;
        }
    }
}
