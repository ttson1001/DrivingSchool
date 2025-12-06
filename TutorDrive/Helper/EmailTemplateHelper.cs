public static class EmailTemplateHelper
{
    public static string BuildAccountStatusEmail(string username, bool isActive)
    {
        string title = isActive ? "Tài khoản đã được mở khóa" : "Tài khoản đã bị khóa";
        string statusColor = isActive ? "#20bf6b" : "#eb3b5a";

        string message = isActive
            ? @"Xin chúc mừng! Tài khoản của bạn đã được mở khóa và có thể sử dụng bình thường.<br/>
               Chúc bạn có trải nghiệm học tập tuyệt vời cùng TutorDrive!"
            : @"Tài khoản của bạn đã bị khóa do vi phạm quy định hoặc theo yêu cầu từ hệ thống.<br/>
               Nếu bạn cần hỗ trợ, vui lòng liên hệ đội ngũ TutorDrive.";

        string template = @"
<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>TutorDrive Notification</title>
</head>

<body style=""margin:0; padding:0; background:#f5f7fa; font-family:Arial, sans-serif;"">
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""padding: 30px 0;"">
        <tr>
            <td align=""center"">
                <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 4px 12px rgba(0,0,0,0.1);"">
                    
                    <tr>
                        <td style=""background:#4b7bec; padding:20px; text-align:center;"">
                            <h2 style=""color:white; margin:0; font-size:24px; font-weight:600;"">
                                TutorDrive
                            </h2>
                        </td>
                    </tr>

                    <tr>
                        <td style=""background:{STATUS_COLOR}; padding:12px 0; text-align:center;"">
                            <span style=""color:white; font-size:16px; font-weight:bold;"">
                                {TITLE}
                            </span>
                        </td>
                    </tr>

                    <tr>
                        <td style=""padding:30px;"">
                            <p style=""font-size:16px; color:#333;"">
                                Xin chào <b>{USERNAME}</b>,
                            </p>

                            <p style=""font-size:15px; color:#555; line-height:1.6;"">
                                {MESSAGE}
                            </p>

                            <p style=""font-size:13px; color:#999; margin-top:30px;"">
                                Nếu bạn không thực hiện hành động này, vui lòng liên hệ hỗ trợ.
                            </p>
                        </td>
                    </tr>

                    <tr>
                        <td style=""background:#f1f1f1; text-align:center; padding:15px;"">
                            <p style=""font-size:12px; color:#666;"">
                                &copy; 2025 TutorDrive. All rights reserved.
                            </p>
                        </td>
                    </tr>

                </table>
            </td>
        </tr>
    </table>
</body>
</html>";

        return template
            .Replace("{TITLE}", title)
            .Replace("{STATUS_COLOR}", statusColor)
            .Replace("{USERNAME}", username)
            .Replace("{MESSAGE}", message);
    }
}
