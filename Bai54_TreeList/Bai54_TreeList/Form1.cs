namespace Bai54_TreeList
{
    public partial class Form1 : Form
    {
        // Các chế độ xem của ListView (hiển thị trong cboView)
        private static readonly (string Text, View Mode)[] ViewModes =
        {
            ("Chi tiết (Details)",            View.Details),
            ("Danh sách (List)",              View.List),
            ("Biểu tượng nhỏ (SmallIcon)",    View.SmallIcon),
            ("Biểu tượng lớn (LargeIcon)",    View.LargeIcon),
            ("Ô nhân viên (Tile)",            View.Tile),
        };

        private static readonly string[] BaseHeaders = { "Mã NV", "Họ tên", "Chức vụ", "Ngày vào làm", "Đơn vị" };

        private readonly List<Employee> _employees = SampleData.CreateEmployees();
        private readonly Dictionary<string, string> _unitNames = new();   // Key đơn vị -> tên đơn vị

        private int _sortColumn = -1;
        private bool _sortAscending = true;

        public Form1()
        {
            InitializeComponent();

            LoadImages();          // ImageList: biểu tượng cho TreeView và ListView
            BuildTree();           // Yêu cầu 1: dựng cây dữ liệu mẫu

            cboView.Items.AddRange(ViewModes.Select(v => (object)v.Text).ToArray());
            cboView.SelectedIndexChanged += cboView_SelectedIndexChanged;
            tvDepartments.AfterSelect += tvDepartments_AfterSelect;
            lsvEmployees.ColumnClick += lsvEmployees_ColumnClick;

            cboView.SelectedIndex = 0;                         // mặc định: Details
            tvDepartments.ExpandAll();
            tvDepartments.SelectedNode = tvDepartments.Nodes[0];   // chọn "Công ty" -> hiện toàn bộ nhân viên
        }

        // ------------------------------------------------------------
        // ImageList: key giống nhau ở cả hai danh sách (nhỏ và lớn)
        // ------------------------------------------------------------
        private void LoadImages()
        {
            foreach (string key in new[] { "company", "department", "group", "employee", "manager" })
            {
                imgSmall.Images.Add(key, IconFactory.Create(key, imgSmall.ImageSize.Width));
                imgLarge.Images.Add(key, IconFactory.Create(key, imgLarge.ImageSize.Width));
            }
        }

        // ------------------------------------------------------------
        // YÊU CẦU 1: dựng cây Công ty -> Phòng ban -> Nhóm
        // ------------------------------------------------------------
        private void BuildTree()
        {
            OrgUnit company = SampleData.CreateOrganization();
            CollectNames(company);

            tvDepartments.BeginUpdate();
            tvDepartments.Nodes.Clear();
            tvDepartments.Nodes.Add(CreateNode(company));
            tvDepartments.EndUpdate();
        }

        private static TreeNode CreateNode(OrgUnit unit)
        {
            var node = new TreeNode(unit.Name)
            {
                Tag = unit,                       // gắn đơn vị vào Node để lọc nhân viên
                ImageKey = unit.IconKey,
                SelectedImageKey = unit.IconKey
            };
            foreach (var child in unit.Children)
                node.Nodes.Add(CreateNode(child));
            return node;
        }

        private void CollectNames(OrgUnit unit)
        {
            _unitNames[unit.Key] = unit.Name;
            foreach (var child in unit.Children) CollectNames(child);
        }

        // ------------------------------------------------------------
        // YÊU CẦU 2: click Node -> lọc nhân viên thuộc Node đó vào ListView
        // ------------------------------------------------------------
        private void tvDepartments_AfterSelect(object? sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag is OrgUnit unit)
                LoadEmployees(unit, e.Node.FullPath);
        }

        private void LoadEmployees(OrgUnit unit, string path)
        {
            // Nhân viên thuộc đơn vị đang chọn (gồm cả các đơn vị con bên dưới)
            var list = _employees.Where(emp => unit.Contains(emp.UnitKey)).ToList();

            lsvEmployees.BeginUpdate();
            lsvEmployees.Items.Clear();
            foreach (var emp in list)
                lsvEmployees.Items.Add(CreateItem(emp));
            if (_sortColumn >= 0) lsvEmployees.Sort();         // giữ nguyên cách sắp xếp đang dùng
            lsvEmployees.EndUpdate();

            lblStatus.Text = $"Đang xem: {path}   |   {list.Count} nhân viên";
        }

        // ------------------------------------------------------------
        // YÊU CẦU 3: các cột Mã NV, Họ tên, Chức vụ, Ngày vào làm (+ Đơn vị)
        // ------------------------------------------------------------
        private ListViewItem CreateItem(Employee emp)
        {
            var item = new ListViewItem(emp.Id)
            {
                Tag = emp,
                ImageKey = emp.IsManager ? "manager" : "employee"
            };
            item.SubItems.Add(emp.Name);                                 // cột 1: Họ tên
            item.SubItems.Add(emp.Position);                             // cột 2: Chức vụ
            item.SubItems.Add(emp.StartDate.ToString("dd/MM/yyyy"));     // cột 3: Ngày vào làm
            item.SubItems.Add(_unitNames.TryGetValue(emp.UnitKey, out var name) ? name : "");   // cột 4: Đơn vị

            SetItemTexts(item, lsvEmployees.View == View.Details);
            return item;
        }

        // Cột đầu tiên của ListView chính là nhãn của item. Ở chế độ Details cần hiện "Mã NV",
        // còn ở các chế độ biểu tượng thì nhãn nên là "Họ tên" cho dễ đọc -> hoán đổi hai giá trị.
        private static void SetItemTexts(ListViewItem item, bool details)
        {
            var emp = (Employee)item.Tag!;
            item.Text = details ? emp.Id : emp.Name;
            item.SubItems[1].Text = details ? emp.Name : emp.Id;
        }

        // ------------------------------------------------------------
        // ComboBox đổi chế độ xem
        // ------------------------------------------------------------
        private void cboView_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cboView.SelectedIndex < 0) return;
            View mode = ViewModes[cboView.SelectedIndex].Mode;

            lsvEmployees.BeginUpdate();
            lsvEmployees.View = mode;
            foreach (ListViewItem item in lsvEmployees.Items)
                SetItemTexts(item, mode == View.Details);
            lsvEmployees.EndUpdate();
        }

        // ------------------------------------------------------------
        // Bấm tiêu đề cột để sắp xếp (bấm lần nữa để đảo chiều)
        // ------------------------------------------------------------
        private void lsvEmployees_ColumnClick(object? sender, ColumnClickEventArgs e)
        {
            if (e.Column == _sortColumn)
                _sortAscending = !_sortAscending;
            else
            {
                _sortColumn = e.Column;
                _sortAscending = true;
            }

            lsvEmployees.ListViewItemSorter = new EmployeeComparer(_sortColumn, _sortAscending);
            lsvEmployees.Sort();

            for (int i = 0; i < lsvEmployees.Columns.Count; i++)
            {
                string arrow = i == _sortColumn ? (_sortAscending ? "  ▲" : "  ▼") : "";
                lsvEmployees.Columns[i].Text = BaseHeaders[i] + arrow;
            }
        }
    }
}
