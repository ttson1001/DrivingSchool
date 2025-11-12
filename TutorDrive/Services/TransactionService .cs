using Microsoft.EntityFrameworkCore;
using TutorDrive.Dtos.Common;
using TutorDrive.Dtos.Transaction;
using TutorDrive.Entities;
using TutorDrive.Entities.Enum;
using TutorDrive.Repositories;
using TutorDrive.Services.IService;

namespace TutorDrive.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IRepository<Transaction> _transactionRepository;

        public TransactionService(IRepository<Transaction> transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<PagedResult<TransactionDto>> GetByUserPagedAsync(long userId, TransactionSearchRequest request)
        {
            var query = _transactionRepository.Get().Where(t => t.UserId == userId);
            return await ApplyFilterAndPagingAsync(query, request.Page, request.PageSize, request.PaymentStatus, request.FromDate, request.ToDate);
        }

        public async Task<PagedResult<TransactionDto>> GetAllPagedAsync(TransactionSearchAdminRequest request)
        {
            var query = _transactionRepository.Get();

            if (request.UserId.HasValue)
                query = query.Where(t => t.UserId == request.UserId.Value);

            return await ApplyFilterAndPagingAsync(query, request.Page, request.PageSize, request.PaymentStatus, request.FromDate, request.ToDate);
        }
        private async Task<PagedResult<TransactionDto>> ApplyFilterAndPagingAsync(
            IQueryable<Transaction> query, int page, int pageSize,
            PaymentStatus? paymentStatus, DateTime? fromDate, DateTime? toDate)
        {
            if (paymentStatus.HasValue)
                query = query.Where(t => t.PaymentStatus == paymentStatus.Value);

            if (fromDate.HasValue)
                query = query.Where(t => t.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(t => t.CreatedAt <= toDate.Value);

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    Amount = t.Amount,
                    PaymentMethod = t.PaymentMethod,
                    PaymentStatus = t.PaymentStatus,
                    CreatedAt = t.CreatedAt,
                    RegistrationId = t.RegistrationId
                })
                .ToListAsync();

            return new PagedResult<TransactionDto>
            {
                TotalItems = totalRecords,
                Page = page,
                PageSize = pageSize,
                Items = items
            };
        }
    }
}
