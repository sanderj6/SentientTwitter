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
using System.Configuration;

namespace SentientTwitter.Services;

public interface ITwitterService
{
    Task StartStream();
    Task StopStream();
}

public class TwitterService
{
    public delegate void OnTweetReceived(object sender, TweetEventReceived e);
    public event OnTweetReceived? TweetReceived;

    private IConfiguration _configuration;

    private HttpClient? httpClient;
    private string bearerToken;

    public bool IsStreamOpen { get; set; } = true;

    public TwitterService(IConfiguration configuration)
    {
        _configuration = configuration;

        bearerToken = _configuration.GetSection("Twitter").GetSection("bearerToken").Value;
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

    public async Task StartStream()
    {
        try
        {
            // Http Client Instantiation
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://api.twitter.com/2/tweets/sample/stream?tweet.fields=context_annotations,lang,entities");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            var _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // Stream
            using (httpClient)
            {
                var response = await httpClient.GetAsync("https://api.twitter.com/2/tweets/sample/stream?tweet.fields=context_annotations,lang,entities", HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var streamReader = new StreamReader(stream))
                {
                    var serializer = new JsonSerializer();

                    // Process the response.
                    while (!streamReader.EndOfStream && IsStreamOpen)
                    {
                        try
                        {
                            var tweet = JsonConvert.DeserializeObject<TweetStreamData>(streamReader.ReadLine());
                            if (tweet is null || tweet.data is null) continue;

                            TweetReceived?.Invoke(this, new TweetEventReceived(tweet, "received"));

                            await Task.Delay(1);
                        }
                        catch (Exception ex)
                        {
                            // If disconnected, attempt reconnect
                            Debug.WriteLine($"Could not send HTTP request: {ex}");
                            continue;
                        }
                    }

                    // Redundant
                    stream.Close();
                    streamReader.Close();
                }

                httpClient.Dispose();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Could not send HTTP request: {ex}");
        }
    }

    public async Task StopStream()
    {
        IsStreamOpen = false;
        await this.StartStream();
    }    
}