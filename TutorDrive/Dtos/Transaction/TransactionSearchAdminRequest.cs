using TutorDrive.Entities.Enum;

namespace TutorDrive.Dtos.Transaction
{
    public class TransactionSearchAdminRequest
    {
        public long? UserId { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
