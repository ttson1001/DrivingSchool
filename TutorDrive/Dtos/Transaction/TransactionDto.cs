using TutorDrive.Dtos.Account;
using TutorDrive.Dtos.Registration;
using TutorDrive.Entities.Enum;

namespace TutorDrive.Dtos.Transaction
{
    public class TransactionDto
    {
        public long Id { get; set; }
        public decimal? Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime CreatedAt { get; set; }

        public AccountDto? User { get; set; }
        public RegistrationListItemDto? Registration { get; set; }
    }

}
