using System.Text.Json.Serialization;
namespace WebApplication4.Application.Feedback_Component.Dto
{
    public class FastApiSentimentResponse
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("confidence_score")]
        public double ConfidenceScore { get; set; }
    }
}
