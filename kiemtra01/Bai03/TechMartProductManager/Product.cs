namespace TechMartProductManager;

public class Product
{
    public string ProductId { get; set; } = "";
    public string ProductName { get; set; } = "";
    public string Category { get; set; } = "";
    public string CategoryValue { get; set; } = "";
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string ImagePath { get; set; } = "";
}
