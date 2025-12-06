using Microsoft.EntityFrameworkCore;
using TutorDrive.Dtos.account;
using TutorDrive.Dtos.Complaint;
using TutorDrive.Entities;
using TutorDrive.Entities.Enum;
using TutorDrive.Repositories;
using TutorDrive.Services.IService;

namespace TutorDrive.Services
{
    public class ComplaintService : IComplaintService
    {
        private readonly IRepository<Complaint> _complaintRepo;

        public ComplaintService(IRepository<Complaint> complaintRepo)
        {
            _complaintRepo = complaintRepo;
        }

        public async Task CreateAsync(long accountId, ComplaintCreateDto dto)
        {
            var entity = new Complaint
            {
                AccountId = accountId,
                Title = dto.Title,
                Content = dto.Content,
                Url = dto.Url,
                Status = ComplaintStatus.Pending
            };

            await _complaintRepo.AddAsync(entity);
            await _complaintRepo.SaveChangesAsync();
        }

        public async Task<List<ComplaintDto>> GetAllAsync()
        {
            return await _complaintRepo.Get()
                .Include(x => x.Account).ThenInclude(a => a.StudentProfile)
                .Include(x => x.Account).ThenInclude(a => a.InstructorProfile)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new ComplaintDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Content = x.Content,
                    Url = x.Url,
                    Reply = x.Reply,
                    Status = x.Status,
                    CreatedAt = x.CreatedAt,

                    Account = new MeDto
                    {
                        AccountId = x.Account.Id,
                        Email = x.Account.Email,
                        FullName = x.Account.FullName,
                        PhoneNumber = x.Account.PhoneNumber,
                        Avatar = x.Account.Avatar,

                        CMND = x.Account.StudentProfile == null ? null : x.Account.StudentProfile.CMND,
                        DOB = x.Account.StudentProfile == null ? null : x.Account.StudentProfile.DOB,
                        Status = x.Account.Status,
                    }
                })
                .ToListAsync();
        }

        public async Task<List<ComplaintDto>> GetMyHistoryAsync(long accountId)
        {
            return await _complaintRepo.Get()
                .Where(x => x.AccountId == accountId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new ComplaintDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Content = x.Content,
                    Url = x.Url,
                    Reply = x.Reply,
                    Status = x.Status,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();
        }

        public async Task DeleteAsync(long accountId, long id)
        {
            var entity = await _complaintRepo.Get()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                throw new Exception("Không tìm thấy complaint");

            if (entity.AccountId != accountId)
                throw new Exception("Bạn không có quyền xóa complaint này");

            _complaintRepo.Delete(entity);
            await _complaintRepo.SaveChangesAsync();
        }

        public async Task ReplyAsync(ComplaintReplyDto dto)
        {
            var entity = await _complaintRepo.Get()
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (entity == null)
                throw new Exception("Complaint không tồn tại");

            if (!string.IsNullOrEmpty(entity.Reply))
                throw new Exception("Complaint đã được trả lời rồi");

            entity.Reply = dto.Reply;
            entity.Status = ComplaintStatus.Resolved;

            _complaintRepo.Update(entity);
            await _complaintRepo.SaveChangesAsync();
        }
    }

}
