namespace TutorDrive.Dtos.Account
{
    public class AccountDto
    {
        public long Id { get; set; }
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Avatar { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

    public class AccountUpdateDto
    {
        public string? FullName { get; set; }
        public string? Password { get; set; }
        public long? RoleId { get; set; }
    }

    public class AccountSearchRequest
    {
        public string? Keyword { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
