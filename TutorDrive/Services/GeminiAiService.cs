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
Hãy phân nhóm các feedback tiếng Việt theo QUY TẮC CỐ ĐỊNH dưới đây:

1. Hài lòng → câu khen, tích cực, đánh giá tốt.
2. Cần cải thiện → góp ý nhẹ hoặc yêu cầu cải thiện.
3. Không hài lòng → phàn nàn mạnh, phản ánh trải nghiệm xấu.
4. Giảng viên → nhận xét về giảng viên: thái độ, phong cách, phương pháp.
5. Nội dung khóa học → nhận xét về bài học, tài liệu, ví dụ, chương trình học.
6. Thời gian – Lịch học → nhận xét về lịch học, thời lượng, tốc độ giảng.
7. Khác → nếu không phù hợp nhóm nào ở trên.

YÊU CẦU BẮT BUỘC:
- Mỗi feedback phải được phân vào đúng một nhóm.
- Trong trường 'examples', PHẢI sử dụng NGUYÊN VĂN chính xác từng feedback từ danh sách bên dưới.
- Không được viết lại, diễn giải, rút gọn hoặc tạo thêm câu mới.
- Nếu feedback không khớp quy tắc → đưa vào nhóm 'Khác'.

ĐỊNH DẠNG TRẢ VỀ (chỉ trả JSON, không văn bản khác):
[
  {{
    ""clusterName"": ""Tên nhóm"",
    ""count"": số feedback thuộc nhóm,
    ""examples"": [ danh sách feedback nguyên văn ]
  }}
]

DANH SÁCH FEEDBACK (bắt buộc dùng EXACT):
- {string.Join("\n- ", comments)}

Chỉ trả JSON thuần, không được kèm thêm giải thích hoặc mô tả.
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
