using TutorDrive.Dtos.Common;
using TutorDrive.Dtos.Transaction;

namespace TutorDrive.Services.IService
{
    public interface ITransactionService
    {
        Task<PagedResult<TransactionDto>> GetByUserPagedAsync(long userId, TransactionSearchRequest request);
        Task<PagedResult<TransactionDto>> GetAllPagedAsync(TransactionSearchAdminRequest request);
    }
}
