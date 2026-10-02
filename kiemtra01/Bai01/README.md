# BÀI 01 - LÝ THUYẾT & CÂU HỎI NGẮN

## Câu 1: Phân biệt Value Types và Reference Types trong C#
câu 1: Phân biệt Value Types và Reference Types trong C#
Value Types (Kiểu giá trị):
•	Lưu trữ trực tiếp giá trị của dữ liệu.
•	Khi gán một biến Value Type cho một biến khác, giá trị được sao chép sang biến mới.
•	Hai biến sau khi sao chép hoạt động độc lập với nhau.
•	Ví dụ: int, double, bool, struct, enum.
Reference Types (Kiểu tham chiếu):
•	Biến lưu trữ một tham chiếu đến đối tượng.
•	Đối tượng thường được cấp phát trên Heap.
•	Khi gán một biến Reference Type cho biến khác, hai biến có thể cùng tham chiếu đến một đối tượng.
•	Ví dụ: class, object, string, array.
Về Stack và Heap:
•	Các biến cục bộ thường được lưu trên Stack, trong khi các đối tượng được cấp phát trên Heap.
•	Không nên hiểu tuyệt đối rằng Value Type luôn nằm trên Stack và Reference Type luôn nằm trên Heap; vị trí lưu trữ thực tế phụ thuộc vào ngữ cảnh.
________________________________________
Câu 2: Init-only Properties (init) khác gì thuộc tính có set?
init cho phép một thuộc tính được gán giá trị trong quá trình khởi tạo đối tượng, nhưng sau khi đối tượng được khởi tạo thì thuộc tính đó không thể được thay đổi.
Trong khi đó, thuộc tính sử dụng set có thể được gán và thay đổi giá trị sau khi đối tượng đã được tạo.
Ví dụ:
class SinhVien
{
    public string MaSV { get; init; }
    public string TenSV { get; set; }
}
SinhVien sv = new SinhVien
{
    MaSV = "SV01",
    TenSV = "Nguyen Van A"
};

sv.TenSV = "Nguyen Van B"; // Hợp lệ
sv.MaSV = "SV02";          // Không hợp lệ
Trường hợp sử dụng thực tế:
init phù hợp với những thuộc tính chỉ nên xác định một lần khi tạo đối tượng, ví dụ như Mã sinh viên, ID, Mã hóa đơn, giúp hạn chế việc thay đổi dữ liệu sau khi đối tượng đã được khởi tạo.
________________________________________
Câu 3: Phân biệt virtual và override trong tính Đa hình
•	virtual được khai báo trong lớp cha, cho phép phương thức được lớp con ghi đè.
•	override được khai báo trong lớp con, dùng để ghi đè và cung cấp cách triển khai mới cho phương thức virtual của lớp cha.
Ví dụ:
class DongVat
{
    public virtual void Keu()
    {
        Console.WriteLine("Dong vat keu");
    }
}

class Cho : DongVat
{
    public override void Keu()
    {
        Console.WriteLine("Gau gau");
    }
}
Khi thực hiện:
DongVat dv = new Cho();
dv.Keu();
Kết quả:
Gau gau
Điều này thể hiện tính đa hình (Polymorphism): cùng một phương thức Keu() nhưng chương trình gọi phương thức phù hợp với kiểu đối tượng thực tế.
virtual → lớp cha cho phép ghi đè.
override → lớp con thực hiện ghi đè.
________________________________________
Câu 4: Tại sao thành phần static không thể truy xuất thông qua Object Instance?
Thành phần được khai báo static thuộc về Class, không thuộc về từng Object Instance cụ thể.
Vì vậy, thành phần static được dùng chung cho tất cả các đối tượng của lớp và được truy xuất thông qua tên lớp.
Ví dụ:
class SinhVien
{
    public static string Truong = "EPU";
}
Truy xuất đúng:
Console.WriteLine(SinhVien.Truong);
Không truy xuất thông qua:
SinhVien sv = new SinhVien();
sv.Truong;
Lý do: sv là một Object Instance cụ thể, trong khi Truong là thành viên static thuộc về Class SinhVien, không thuộc riêng đối tượng sv.
Instance member → thuộc về Object → object.TenThanhVien
static member → thuộc về Class → Class.TenThanhVien

