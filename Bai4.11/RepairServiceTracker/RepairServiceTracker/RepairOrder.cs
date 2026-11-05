namespace RepairServiceTracker;

public static class OrderStatus
{
    public const string New = "Mới tiếp nhận";
    public const string Repairing = "Đang sửa";
    public const string WaitingParts = "Chờ linh kiện";
    public const string Done = "Hoàn tất";
    public const string Cancelled = "Đã hủy";

    public static readonly string[] All = { New, Repairing, WaitingParts, Done, Cancelled };
}

/// <summary>Một đơn đăng ký dịch vụ sửa chữa.</summary>
public class RepairOrder
{
    public string OrderId { get; set; } = "";                 // Mã đơn (duy nhất)
    public string CustomerName { get; set; } = "";            // Tên khách hàng
    public string Phone { get; set; } = "";                   // Số điện thoại
    public string DeviceType { get; set; } = "";              // Loại thiết bị
    public string Description { get; set; } = "";             // Mô tả lỗi
    public DateTime ReceivedDate { get; set; } = DateTime.Today;   // Ngày tiếp nhận
    public decimal EstimatedCost { get; set; }                // Chi phí dự kiến (VNĐ)
    public string Status { get; set; } = OrderStatus.New;     // Trạng thái
    public string ImagePath { get; set; } = "";               // Đường dẫn ảnh thiết bị (có thể rỗng)

    // "Chưa hoàn tất" = đơn còn đang xử lý (không tính đơn Hoàn tất và đơn Đã hủy).
    // Muốn tính cả đơn Đã hủy thì bỏ vế "&& Status != OrderStatus.Cancelled".
    public bool IsOpen => Status != OrderStatus.Done && Status != OrderStatus.Cancelled;
}
