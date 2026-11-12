namespace Bai52_TinhTien
{
    /// <summary>Một dịch vụ y tế (thuộc một loại, có đơn giá).</summary>
    public class Service
    {
        public string Category { get; }
        public string Name { get; }
        public decimal Price { get; }

        public Service(string category, string name, decimal price)
        {
            Category = category;
            Name = name;
            Price = price;
        }

        // ListBox hiển thị chuỗi này
        public override string ToString() => $"{Name}  -  {Price:N0} đ";
    }
}
