namespace LabEquipmentCheckout;

public static class LoanStatus
{
    public const string Borrowing = "Đang mượn";
    public const string Returned = "Đã trả";
    public const string Overdue = "Quá hạn";

    public static readonly string[] All = { Borrowing, Overdue, Returned };
}

/// <summary>Một phiếu mượn thiết bị.</summary>
public class Loan
{
    public string LoanId { get; set; } = "";          // Mã phiếu (duy nhất)
    public string StudentId { get; set; } = "";       // Mã sinh viên
    public string StudentName { get; set; } = "";     // Họ tên sinh viên
    public string Equipment { get; set; } = "";       // Thiết bị
    public int Quantity { get; set; } = 1;            // Số lượng
    public DateTime BorrowDate { get; set; } = DateTime.Today;   // Ngày mượn
    public DateTime DueDate { get; set; } = DateTime.Today;      // Hạn trả
    public DateTime? ReturnDate { get; set; }                    // Ngày trả thực tế (null = chưa trả)

    // Tình trạng được TỰ TÍNH, không lưu cứng:
    //   đã có ngày trả          -> Đã trả
    //   chưa trả, hạn < hôm nay -> Quá hạn
    //   còn lại                 -> Đang mượn
    public string Status => ComputeStatus(DueDate, ReturnDate);

    public bool IsOverdue => Status == LoanStatus.Overdue;

    public static string ComputeStatus(DateTime dueDate, DateTime? returnDate)
    {
        if (returnDate.HasValue) return LoanStatus.Returned;
        return dueDate.Date < DateTime.Today ? LoanStatus.Overdue : LoanStatus.Borrowing;
    }
}
