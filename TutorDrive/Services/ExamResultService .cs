using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using TutorDrive.Dtos.account;
using TutorDrive.Dtos.Address.TutorDrive.Dtos.Address;
using TutorDrive.Dtos.Exam;
using TutorDrive.Dtos.ExamResult;
using TutorDrive.Entities;
using TutorDrive.Entities.Enum;
using TutorDrive.Repositories;
using TutorDrive.Services.IService;

public class ExamResultService : IExamResultService
{
    private readonly IRepository<ExamResult> _examResultRepo;
    private readonly IRepository<Exam> _examRepo;
    private readonly IRepository<Account> _accountRepo;
    private readonly IRepository<StudentProfile> _studentRepo;

    public ExamResultService(
        IRepository<ExamResult> examResultRepo,
        IRepository<Exam> examRepo,
        IRepository<Account> accountRepo,
        IRepository<StudentProfile> studentRepo)
    {
        _examResultRepo = examResultRepo;
        _examRepo = examRepo;
        _accountRepo = accountRepo;
        _studentRepo = studentRepo;
    }

    public async Task<object> ImportFromExcelAsync(IFormFile file)
    {
        var errors = new List<string>();
        var imported = 0;

        using (var package = new ExcelPackage(file.OpenReadStream()))
        {
            var ws = package.Workbook.Worksheets[0];
            int rowCount = ws.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    string examCode = ws.Cells[row, 1].GetValue<string>();
                    string studentEmail = ws.Cells[row, 2].GetValue<string>();

                    float? theory = ws.Cells[row, 3].GetValue<float?>();
                    float? simulation = ws.Cells[row, 4].GetValue<float?>();
                    float? track = ws.Cells[row, 5].GetValue<float?>();
                    float? road = ws.Cells[row, 6].GetValue<float?>();
                    string statusStr = ws.Cells[row, 7].GetValue<string>();

                    var exam = await _examRepo.Get().FirstOrDefaultAsync(e => e.ExamCode == examCode);
                    if (exam == null)
                    {
                        errors.Add($"Row {row}: ExamCode '{examCode}' không tồn tại");
                        continue;
                    }

                    var account = await _accountRepo.Get().FirstOrDefaultAsync(a => a.Email == studentEmail);
                    if (account == null)
                    {
                        errors.Add($"Row {row}: Email '{studentEmail}' không tồn tại");
                        continue;
                    }

                    var student = await _studentRepo.Get().FirstOrDefaultAsync(sp => sp.AccountId == account.Id);
                    if (student == null)
                    {
                        errors.Add($"Row {row}: StudentProfile không tồn tại cho email '{studentEmail}'");
                        continue;
                    }

                    var status = ExamResultStatus.Pending;
                    if (!string.IsNullOrEmpty(statusStr))
                    {
                        if (!Enum.TryParse(statusStr, true, out status))
                        {
                            errors.Add($"Row {row}: Status '{statusStr}' không hợp lệ");
                            continue;
                        }
                    }

                    var result = await _examResultRepo.Get()
                        .FirstOrDefaultAsync(r => r.ExamId == exam.Id && r.StudentProfileId == student.Id);

                    if (result == null)
                    {
                        result = new ExamResult
                        {
                            ExamId = exam.Id,
                            StudentProfileId = student.Id,
                            ExamCode = examCode
                        };
                        await _examResultRepo.AddAsync(result);
                    }

                    result.TheoryScore = theory;
                    result.SimulationScore = simulation;
                    result.TrackScore = track;
                    result.RoadTestScore = road;
                    result.Status = status;

                    imported++;
                }
                catch (Exception ex)
                {
                    errors.Add($"Row {row}: Error - {ex.Message}");
                }
            }
        }

        await _examResultRepo.SaveChangesAsync();

        return new
        {
            success = true,
            imported,
            errors
        };
    }

    public async Task<List<ExamResultDto>> SearchAsync(ExamResultSearchDto dto)
    {
        var query = _examResultRepo.Get()
            .Include(x => x.StudentProfile).ThenInclude(s => s.Account)
            .Include(x => x.Exam).ThenInclude(e => e.Course)
            .Include(x => x.StudentProfile.Address).ThenInclude(a => a.Ward)
            .Include(x => x.StudentProfile.Address).ThenInclude(a => a.Province)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(dto.Keyword))
        {
            var keyword = dto.Keyword.Trim().ToLower();

            query = query.Where(x =>
                x.ExamCode.ToLower().Contains(keyword) ||
                x.StudentProfile.Account.Email.ToLower().Contains(keyword)
            );
        }

        if (!string.IsNullOrWhiteSpace(dto.ExamCode))
        {
            query = query.Where(x => x.ExamCode == dto.ExamCode);
        }

        if (dto.FromDate != null)
        {
            query = query.Where(x => x.Exam.ExamDate >= dto.FromDate);
        }

        if (dto.ToDate != null)
        {
            query = query.Where(x => x.Exam.ExamDate <= dto.ToDate);
        }

        var data = await query.ToListAsync();

        return data.Select(x => new ExamResultDto
        {
            Id = x.Id,
            ExamCode = x.ExamCode,

            Exam = new ExamDto
            {
                Id = x.Exam.Id,
                ExamCode = x.Exam.ExamCode,
                CourseId = x.Exam.CourseId,
                CourseName = x.Exam.Course.Name,
                Date = x.Exam.ExamDate,
                Location = x.Exam.Location,
                Theory = x.Exam.Theory,
                Simulation = x.Exam.Simulation,
                Track = x.Exam.Track,
                RoadTest = x.Exam.RoadTest
            },

            Student = new MeDto
            {
                AccountId = x.StudentProfile.Account.Id,
                Email = x.StudentProfile.Account.Email,
                FullName = x.StudentProfile.Account.FullName,
                PhoneNumber = x.StudentProfile.Account.PhoneNumber,
                Avatar = x.StudentProfile.Account.Avatar,

                CMND = x.StudentProfile.CMND,
                DOB = x.StudentProfile.DOB,

                Address = x.StudentProfile.Address == null ? null : new AddressDto
                {
                    FullAddress = x.StudentProfile.Address.FullAddress,
                    Street = x.StudentProfile.Address.Street,
                    WardName = x.StudentProfile.Address.Ward?.Name,
                    ProvinceName = x.StudentProfile.Address.Province?.Name,
                    WardId = x.StudentProfile.Address.WardId,
                    ProvinceId = x.StudentProfile.Address.ProvinceId
                },
            },

            TheoryScore = x.TheoryScore,
            SimulationScore = x.SimulationScore,
            TrackScore = x.TrackScore,
            RoadTestScore = x.RoadTestScore,
            Status = x.Status

        }).ToList();
    }

    public async Task<List<ExamResultDto>> GetHistoryByAccountId(long accountId)
    {
        var data = await _examResultRepo.Get()
            .Include(x => x.StudentProfile).ThenInclude(s => s.Account)
            .Include(x => x.Exam).ThenInclude(e => e.Course)
            .Include(x => x.StudentProfile.Address).ThenInclude(a => a.Ward)
            .Include(x => x.StudentProfile.Address).ThenInclude(a => a.Province)
            .Where(x => x.StudentProfile.AccountId == accountId)
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        return data.Select(x => new ExamResultDto
        {
            Id = x.Id,
            ExamCode = x.ExamCode,

            Exam = new ExamDto
            {
                Id = x.Exam.Id,
                ExamCode = x.Exam.ExamCode,
                CourseId = x.Exam.CourseId,
                CourseName = x.Exam.Course.Name,
                Date = x.Exam.ExamDate,
                Location = x.Exam.Location,
                Theory = x.Exam.Theory,
                Simulation = x.Exam.Simulation,
                Track = x.Exam.Track,
                RoadTest = x.Exam.RoadTest
            },

            Student = new MeDto
            {
                AccountId = x.StudentProfile.Account.Id,
                Email = x.StudentProfile.Account.Email,
                FullName = x.StudentProfile.Account.FullName,
                PhoneNumber = x.StudentProfile.Account.PhoneNumber,
                Avatar = x.StudentProfile.Account.Avatar,

                CMND = x.StudentProfile.CMND,
                DOB = x.StudentProfile.DOB,

                Address = x.StudentProfile.Address == null ? null : new AddressDto
                {
                    FullAddress = x.StudentProfile.Address.FullAddress,
                    Street = x.StudentProfile.Address.Street,
                    WardName = x.StudentProfile.Address.Ward?.Name,
                    ProvinceName = x.StudentProfile.Address.Province?.Name,
                    WardId = x.StudentProfile.Address.WardId,
                    ProvinceId = x.StudentProfile.Address.ProvinceId
                },

                Role = "Student"
            },

            TheoryScore = x.TheoryScore,
            SimulationScore = x.SimulationScore,
            TrackScore = x.TrackScore,
            RoadTestScore = x.RoadTestScore,
            Status = x.Status

        }).ToList();
    }
}
