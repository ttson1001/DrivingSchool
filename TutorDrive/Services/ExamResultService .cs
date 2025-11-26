using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
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
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

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

    public async Task<List<ExamResult>> SearchAsync(ExamResultSearchDto dto)
    {
        var query = _examResultRepo.Get()
            .Include(x => x.StudentProfile)
            .Include(x => x.Exam)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(dto.Keyword))
        {
            query = query.Where(x =>
                x.ExamCode.Contains(dto.Keyword) ||
                x.StudentProfile.Account.Email.Contains(dto.Keyword)
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

        return await query.ToListAsync();
    }

    public async Task<List<ExamResult>> GetHistoryByAccountId(long accountId)
    {
        return await _examResultRepo.Get()
            .Include(x => x.Exam)
            .Include(x => x.StudentProfile)
            .Where(x => x.StudentProfile.AccountId == accountId)
            .OrderByDescending(x => x.Id)
            .ToListAsync();
    }
}
