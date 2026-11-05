namespace ClinicAppointmentManager;

/// <summary>Một lịch hẹn khám bệnh.</summary>
public class Appointment
{
    public string AppointmentId { get; set; } = "";      // Mã lịch hẹn (duy nhất)
    public string PatientName { get; set; } = "";        // Họ tên bệnh nhân
    public string Phone { get; set; } = "";              // Số điện thoại
    public DateTime AppointmentTime { get; set; }        // Ngày giờ hẹn
    public string Doctor { get; set; } = "";             // Bác sĩ
    public string Reason { get; set; } = "";             // Lý do khám
    public string Status { get; set; } = AppointmentStatus.Waiting;   // Trạng thái
}

public static class AppointmentStatus
{
    public const string Waiting = "Chờ khám";
    public const string InProgress = "Đang khám";
    public const string Done = "Hoàn tất";
    public const string Cancelled = "Đã hủy";

    public static readonly string[] All = { Waiting, InProgress, Done, Cancelled };
}
