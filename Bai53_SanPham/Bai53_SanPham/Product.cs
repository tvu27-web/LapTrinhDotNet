namespace Bai53_SanPham
{
    /// <summary>Lớp đối tượng sản phẩm (yêu cầu 1).</summary>
    public class Product
    {
        public string ProductId { get; set; } = "";      // Mã SP
        public string ProductName { get; set; } = "";    // Tên SP
        public decimal UnitPrice { get; set; }           // Đơn giá
        public int Quantity { get; set; }                // Số lượng
        public string Category { get; set; } = "";       // Danh mục
    }
}
