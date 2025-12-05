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
Hãy phân nhóm feedback tiếng Việt.

Output JSON:
[
  {{
    ""clusterName"": ""Tên nhóm"",
    ""count"": số feedback,
    ""examples"": [ danh sách feedback ]
  }}
]

Feedback:
- {text}

Chỉ trả về JSON thuần, không giải thích thêm.
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
