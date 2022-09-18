using Azure;

using System.Net.Http;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using SentientTwitter.Data;
using Azure.AI.TextAnalytics;
using Microsoft.Azure.Cosmos;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
using Microsoft.AspNetCore.Identity;
using System.Threading;

namespace SentientTwitter.Services;

public class TwitterService
{
    public delegate void OnTweetReceived(object sender, TweetEventReceived e);
    public event OnTweetReceived TweetReceived;

    private TextAnalyzerService TextAnalyzerService { get; set; }
    private readonly IBackgroundTaskQueue _backgroundTaskQueue;

    public TwitterService()
    {
        var endpoint = "https://api.twitter.com/2/tweets/search/recent?query=from:twitterdev";
        var apiKey = "sz6YBuiFzkMHJRW15S9nVQKqv";
        var apiSecret = "wRk2DaTYYDHpnhRL5lYJLuSD8XYxaWi47shcD4sX1RcCa6vHRW";
        var bearerToken = "AAAAAAAAAAAAAAAAAAAAACMyhAEAAAAAhxbfVkBArjpCnXzMXi%2FuxLuRKK4%3D7VCzjnLTYndhWvUzVW3Azl8VxyB1dpl3WXHACsNjnM2bwKbzi4";

        // Sentiment
        var sentimentEndpoint = "https://moodanalyzer.cognitiveservices.azure.com/";
        var sentimentApiKey = "e26a9d4c01a64468bf1600f5cb104a12";

        //AnalyzeSentimentOptions options = new AnalyzeSentimentOptions()
        //{
        //    IncludeStatistics = true,
        //    IncludeOpinionMining = true
        //};

        TextAnalyzerService = new TextAnalyzerService();
        _backgroundTaskQueue = new BackgroundTaskQueue(null, 1);
    }

    public class TweetEventReceived : EventArgs
    {
        public TweetEventReceived(TweetStreamData tweetStream, string eventName)
        {
            TweetStream = tweetStream;
            EventName = eventName;
        }
        public TweetStreamData TweetStream { get; set; }
        public string EventName { get; set; }
    }

    public async Task<TweetStreamData> GetTweets()
    {
        TweetStreamData tweet = new();

        try
        {
            // Http Client Instantiation
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://api.twitter.com/2/tweets/sample/stream");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "AAAAAAAAAAAAAAAAAAAAACMyhAEAAAAAhxbfVkBArjpCnXzMXi%2FuxLuRKK4%3D7VCzjnLTYndhWvUzVW3Azl8VxyB1dpl3WXHACsNjnM2bwKbzi4");
            httpClient.Timeout = new TimeSpan(0, 0, 30);
            var _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // Serialize Data
            //var myContent = JsonConvert.SerializeObject(stream);
            //StringContent data = new StringContent(myContent, Encoding.UTF8, "application/json");

            using (httpClient)
            {
                var response = await httpClient.GetAsync("https://api.twitter.com/2/tweets/sample/stream?tweet.fields=context_annotations,lang,entities", HttpCompletionOption.ResponseHeadersRead);

                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var streamReader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    var serializer = new JsonSerializer();

                    // Process the response.
                    while (!streamReader.EndOfStream)
                    {
                        try
                        {
                            tweet = JsonConvert.DeserializeObject<TweetStreamData>(streamReader.ReadLine());
                            if (tweet is null || tweet.data is null) continue;

                            TweetReceived?.Invoke(this, new TweetEventReceived(tweet, "received"));

                            await Task.Delay(1);
                        }
                        catch (Exception ex)
                        {
                            // TODO: if disconnected, attempt reconnect
                            System.Diagnostics.Debug.WriteLine($"Could not send HTTP request: {ex}");
                        }
                    }
                }
            }

            return tweet;
        }
        catch (Exception ex)
        {
            // TODO: if disconnected, attempt reconnect
            System.Diagnostics.Debug.WriteLine($"Could not send HTTP request: {ex}");

            return tweet;
        }
    }

    public async ValueTask ProcessTwitterSentiment(TweetStreamData tweetStream, CancellationToken cancellationToken)
    {
        //await Task.Delay(1000);
        var sentiment = await TextAnalyzerService.AnalyzeText(tweetStream.data.text);
        tweetStream.sentiment = sentiment;
        //TweetReceived?.Invoke(this, new TweetEventReceived(tweetStream.data.text, "processed"));
    }
}