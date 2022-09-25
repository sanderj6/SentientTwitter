using Azure;

using System.Net.Http;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using SentientTwitter.Data;
using Azure.AI.TextAnalytics;

namespace SentientTwitter.Services;

public interface ITextAnalyzerService
{
    Task<DocumentSentiment> AnalyzeText();
}

public class TextSentimentService
{
    private TextAnalyticsClient SentimentClient { get; set; }

    public TextSentimentService()
    {
        var endpoint = "https://moodanalyzer.cognitiveservices.azure.com/";
        var apiKey = "e26a9d4c01a64468bf1600f5cb104a12";

        SentimentClient = new TextAnalyticsClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
    }

    public async Task<DocumentSentiment> AnalyzeText(string text)
    {
        var response = await SentimentClient.AnalyzeSentimentAsync(text);
        return response.Value;
    }

}