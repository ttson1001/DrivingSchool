using System.ComponentModel.DataAnnotations.Schema;
using TutorDrive.Entities.Enum;

namespace TutorDrive.Entities
{
    public class Transaction : IEntity
    {
        public long Id { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Amount { get; set; }
        public long UserId { get; set; }
        public Account User { get; set; }
        public string? PaymentMethod{ get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public long? RegistrationId { get; set; }
        public Registration? Registration { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
