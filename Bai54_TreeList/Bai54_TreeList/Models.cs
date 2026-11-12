namespace Bai54_TreeList
{
    /// <summary>Một đơn vị trong cây tổ chức: Công ty -> Phòng ban -> Nhóm.</summary>
    public class OrgUnit
    {
        public string Name { get; }
        public string Key { get; }          // đường dẫn đơn vị, VD: "IT", "IT/DEV" ("" = cả công ty)
        public string IconKey { get; }      // "company" | "department" | "group"
        public List<OrgUnit> Children { get; } = new();

        public OrgUnit(string name, string key, string iconKey)
        {
            Name = name;
            Key = key;
            IconKey = iconKey;
        }

        public OrgUnit Add(string name, string key, string iconKey)
        {
            var child = new OrgUnit(name, key, iconKey);
            Children.Add(child);
            return child;
        }

        // Nhân viên thuộc đơn vị này nếu nằm ngay tại đây HOẶC ở bất kỳ đơn vị con nào bên dưới
        public bool Contains(string employeeUnitKey) =>
            Key.Length == 0 ||
            employeeUnitKey == Key ||
            employeeUnitKey.StartsWith(Key + "/");
    }

    public class Employee
    {
        public string Id { get; init; } = "";            // Mã NV
        public string Name { get; init; } = "";          // Họ tên
        public string Position { get; init; } = "";      // Chức vụ
        public DateTime StartDate { get; init; }         // Ngày vào làm
        public string UnitKey { get; init; } = "";       // Đơn vị trực tiếp (khớp OrgUnit.Key)

        // Dùng để chọn biểu tượng riêng cho quản lý
        public bool IsManager =>
            Position.Contains("trưởng", StringComparison.OrdinalIgnoreCase) ||
            Position.StartsWith("Giám đốc", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Dữ liệu mẫu (yêu cầu 1).</summary>
    public static class SampleData
    {
        public static OrgUnit CreateOrganization()
        {
            var company = new OrgUnit("Công ty ABC", "", "company");

            var it = company.Add("Phòng Công nghệ thông tin", "IT", "department");
            it.Add("Nhóm Phát triển phần mềm", "IT/DEV", "group");
            it.Add("Nhóm Kiểm thử", "IT/QA", "group");
            it.Add("Nhóm Hạ tầng hệ thống", "IT/SYS", "group");

            var hr = company.Add("Phòng Nhân sự", "HR", "department");
            hr.Add("Nhóm Tuyển dụng", "HR/REC", "group");
            hr.Add("Nhóm Đào tạo", "HR/TRN", "group");

            var acc = company.Add("Phòng Kế toán", "ACC", "department");
            acc.Add("Nhóm Kế toán tổng hợp", "ACC/GEN", "group");
            acc.Add("Nhóm Kế toán thuế", "ACC/TAX", "group");

            var sal = company.Add("Phòng Kinh doanh", "SAL", "department");
            sal.Add("Nhóm Khu vực miền Bắc", "SAL/N", "group");
            sal.Add("Nhóm Khu vực miền Nam", "SAL/S", "group");

            return company;
        }

        public static List<Employee> CreateEmployees() => new()
        {
            E("NV001", "Trần Minh Quân",   "Giám đốc",                 2015, 3, 1,   ""),

            E("NV002", "Lê Thanh Hải",     "Trưởng phòng CNTT",        2016, 6, 15,  "IT"),
            E("NV003", "Nguyễn Văn An",    "Trưởng nhóm phát triển",   2018, 2, 12,  "IT/DEV"),
            E("NV004", "Phạm Quốc Bảo",    "Lập trình viên",           2020, 7, 1,   "IT/DEV"),
            E("NV005", "Đỗ Thị Hạnh",      "Lập trình viên",           2021, 9, 20,  "IT/DEV"),
            E("NV006", "Vũ Minh Khôi",     "Lập trình viên",           2022, 4, 11,  "IT/DEV"),
            E("NV007", "Hoàng Thu Trang",  "Trưởng nhóm kiểm thử",     2019, 1, 7,   "IT/QA"),
            E("NV008", "Bùi Anh Tuấn",     "Kiểm thử viên",            2021, 3, 15,  "IT/QA"),
            E("NV009", "Đặng Văn Long",    "Quản trị hệ thống",        2017, 11, 5,  "IT/SYS"),
            E("NV010", "Ngô Thị Mai",      "Chuyên viên mạng",         2022, 10, 3,  "IT/SYS"),

            E("NV011", "Lý Thị Hoa",       "Trưởng phòng Nhân sự",     2015, 9, 1,   "HR"),
            E("NV012", "Trịnh Bảo Ngọc",   "Chuyên viên tuyển dụng",   2020, 5, 18,  "HR/REC"),
            E("NV013", "Phan Văn Đức",     "Chuyên viên tuyển dụng",   2023, 2, 6,   "HR/REC"),
            E("NV014", "Mai Thị Lan",      "Trưởng nhóm đào tạo",      2018, 8, 27,  "HR/TRN"),
            E("NV015", "Cao Minh Tâm",     "Giảng viên nội bộ",        2021, 12, 13, "HR/TRN"),

            E("NV016", "Dương Thị Thu",    "Kế toán trưởng",           2016, 1, 11,  "ACC"),
            E("NV017", "Võ Thị Yến",       "Kế toán viên",             2019, 6, 24,  "ACC/GEN"),
            E("NV018", "Lương Văn Phát",   "Kế toán viên",             2022, 8, 15,  "ACC/GEN"),
            E("NV019", "Hồ Thị Diệu",      "Chuyên viên thuế",         2020, 10, 19, "ACC/TAX"),

            E("NV020", "Tô Văn Hùng",      "Trưởng phòng Kinh doanh",  2015, 12, 7,  "SAL"),
            E("NV021", "Kiều Thị Nga",     "Trưởng nhóm kinh doanh",   2017, 4, 3,   "SAL/N"),
            E("NV022", "Lâm Quốc Việt",    "Nhân viên kinh doanh",     2021, 6, 28,  "SAL/N"),
            E("NV023", "Châu Thị Mỹ",      "Trưởng nhóm kinh doanh",   2018, 9, 10,  "SAL/S"),
            E("NV024", "Thái Văn Sơn",     "Nhân viên kinh doanh",     2023, 5, 22,  "SAL/S"),
            E("NV025", "Tạ Thị Quyên",     "Nhân viên kinh doanh",     2024, 1, 15,  "SAL/S"),
        };

        private static Employee E(string id, string name, string position, int y, int m, int d, string unitKey) =>
            new()
            {
                Id = id, Name = name, Position = position,
                StartDate = new DateTime(y, m, d), UnitKey = unitKey
            };
    }
}
