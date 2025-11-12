using TutorDrive.Entities.Enum;

namespace TutorDrive.Dtos.Transaction
{
    public class TransactionSearchRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public PaymentStatus? PaymentStatus { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
