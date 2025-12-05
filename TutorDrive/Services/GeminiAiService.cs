using Google.GenAI;
using Google.GenAI.Types;
using Newtonsoft.Json;
using TutorDrive.Dtos.Feedbacks;

public class GeminiAiService
{
    private readonly Client _client;

    public GeminiAiService()
    {
        _client = new Client();
    }

    public async Task<List<FeedbackClusterDto>> ClusterAsync(List<string> comments)
    {
        string text = string.Join("\n- ", comments);

        string prompt = $@"
Hãy phân nhóm feedback theo QUY TẮC CỐ ĐỊNH:

1. Hài lòng → câu khen, tích cực, đánh giá tốt.
2. Cần cải thiện → góp ý nhẹ hoặc yêu cầu cải thiện.
3. Không hài lòng → phàn nàn mạnh, trải nghiệm xấu.
4. Giảng viên → nhận xét về chất lượng giảng dạy, thái độ, phương pháp.
5. Nội dung khóa học → nhận xét về tài liệu, bài học, chương trình học.
6. Thời gian – Lịch học → nhận xét về thời gian học, lịch học, thời lượng.
7. Khác → nếu không thuộc nhóm nào ở trên.

YÊU CẦU:
- Phân nhóm đúng theo 7 quy tắc trên.
- Trả về JSON đúng mẫu, không thêm giải thích.

Output JSON:
[
  {{
    ""clusterName"": ""Tên nhóm"",
    ""count"": số feedback,
    ""examples"": [ danh sách feedback ]
  }}
]

Feedback:
- {string.Join("\n- ", comments)}

Chỉ trả JSON thuần, không thêm chữ nào trước hoặc sau JSON.
";

        var response = await _client.Models.GenerateContentAsync(
            model: "gemini-2.0-flash",
            contents: prompt
        );

        string raw = response.Candidates[0].Content.Parts[0].Text;

        // 🔥 Lọc ra chỉ phần JSON (fix lỗi output có ký tự thừa)
        string json = ExtractJson(raw);

        return JsonConvert.DeserializeObject<List<FeedbackClusterDto>>(json);
    }
    private string ExtractJson(string input)
    {
        int start = input.IndexOf('[');
        int end = input.LastIndexOf(']');

        if (start == -1 || end == -1 || end <= start)
            throw new Exception("AI không trả về JSON hợp lệ.");

        return input.Substring(start, end - start + 1);
    }

}
