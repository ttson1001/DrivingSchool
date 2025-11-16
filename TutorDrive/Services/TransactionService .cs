using Microsoft.EntityFrameworkCore;
using TutorDrive.Dtos.Account;
using TutorDrive.Dtos.Common;
using TutorDrive.Dtos.Registration;
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

        public async Task<PagedResult<TransactionDto>> GetByUserPagedAsync(
            long userId,
            TransactionSearchRequest request)
        {
            var query = _transactionRepository.Get()
                .Where(t => t.UserId == userId);

            return await ApplyFilterAndPagingAsync(
                query,
                request.Page,
                request.PageSize,
                request.PaymentStatus,
                request.FromDate,
                request.ToDate
            );
        }

        public async Task<PagedResult<TransactionDto>> GetAllPagedAsync(
            TransactionSearchAdminRequest request)
        {
            var query = _transactionRepository.Get();

            if (request.UserId.HasValue)
                query = query.Where(t => t.UserId == request.UserId);

            return await ApplyFilterAndPagingAsync(
                query,
                request.Page,
                request.PageSize,
                request.PaymentStatus,
                request.FromDate,
                request.ToDate
            );
        }

        private async Task<PagedResult<TransactionDto>> ApplyFilterAndPagingAsync(
            IQueryable<Transaction> query,
            int page,
            int pageSize,
            PaymentStatus? paymentStatus,
            DateTime? fromDate,
            DateTime? toDate)
        {
            if (paymentStatus.HasValue)
                query = query.Where(t => t.PaymentStatus == paymentStatus);

            if (fromDate.HasValue)
                query = query.Where(t => t.CreatedAt >= fromDate);

            if (toDate.HasValue)
                query = query.Where(t => t.CreatedAt <= toDate);

            var totalRecords = await query.CountAsync();

            var sqlData = await query
                .Include(t => t.User).ThenInclude(u => u.Role)
                .Include(t => t.Registration)
                    .ThenInclude(r => r.StudentProfile).ThenInclude(sp => sp.Account)
                .Include(t => t.Registration).ThenInclude(r => r.Course)
                .Include(t => t.Registration).ThenInclude(r => r.Files)

                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = sqlData.Select(t => new TransactionDto
            {
                Id = t.Id,
                Amount = t.Amount,
                PaymentMethod = t.PaymentMethod,
                PaymentStatus = t.PaymentStatus,
                CreatedAt = t.CreatedAt,

                User = t.User == null ? null : new AccountDto
                {
                    Id = t.User.Id,
                    Email = t.User.Email,
                    FullName = t.User.FullName,
                    Avatar = t.User.Avatar,
                    RoleName = t.User.Role.Name,
                    CreatedAt = t.User.CreatedAt
                },

                Registration = t.Registration == null ? null : new RegistrationListItemDto
                {
                    Id = t.Registration.Id,

                    StudentId = t.Registration.StudentProfileId,
                    StudentName = t.Registration.StudentProfile.Account.FullName,
                    StudentEmail = t.Registration.StudentProfile.Account.Email,

                    FullName = t.Registration.FullName,
                    Email = t.Registration.Email,
                    PhoneNumber = t.Registration.PhoneNumber,

                    CourseId = t.Registration.CourseId,
                    CourseName = t.Registration.Course.Name,

                    Status = t.Registration.Status,
                    Note = t.Registration.Note,
                    RegisterDate = t.Registration.RegisterDate,

                    FileUrls = t.Registration.Files.Select(f => f.Url).ToList(),

                    StartDateTime = t.Registration.StartDateTime,

                    StudyDays = string.Join(", ",
                        Enum.GetValues(typeof(StudyDay))
                            .Cast<StudyDay>()
                            .Where(d => d != StudyDay.None &&
                                        t.Registration.StudyDays.HasFlag(d))
                            .Select(d => d.ToString())
                    ),

                    Price = t.Registration.Price
                }
            }).ToList();

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
