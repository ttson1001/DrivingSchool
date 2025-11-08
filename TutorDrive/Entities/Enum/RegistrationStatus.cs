namespace TutorDrive.Entities.Enum
{
    public enum RegistrationStatus
    {
        Pending = 1,       // Đang chờ duyệt
        Approved = 2,      // Đã duyệt / được chấp nhận
        Rejected = 3,      // Bị từ chối
        Cancelled = 4,     // Học viên hủy đăng ký
        Paid = 5
    }
}
