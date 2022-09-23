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
using System;
using System.Diagnostics.Tracing;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace SentientTwitter.Services;

public interface ITwitterService
{
    Task StartStream();
    Task StopStream();
    Task ToggleStream(bool toggle);
    Task CalculateTrending(TweetStreamData tweetStream);
    TweetModel CreateTweetFromStream(TweetStreamData stream);
}

public class TwitterService
{
    public List<TweetModel> AllTweets { get; set; } = new();
    public ConcurrentQueue<List<TweetModel>> coll = new ConcurrentQueue<List<TweetModel>>();
    public ConcurrentQueue<TweetModel> coll2 = new ConcurrentQueue<TweetModel>();

    public delegate void OnTweetReceived(object sender, TweetEventReceived e);
    public event OnTweetReceived? TweetReceived;

    private IConfiguration _configuration;

    private HttpClient? httpClient;
    private string bearerToken;

    public Dictionary<Hashtag, int> HashTags { get; set; } = new();
    public Dictionary<Domain, int> Topics { get; set; } = new();
    public Dictionary<Entity, int> Subjects { get; set; } = new();

    public List<string> TopHashtags { get; set; } = new();
    public List<string> FilteredHashtags { get; set; } = new();
    public string TrendingTopic { get; set; } = string.Empty;
    public string TrendingSubject { get; set; } = string.Empty;

    public bool IsStreamOpen
    {
        get
        {
            return _isStreamOpen;
        }
        set
        {
            _isStreamOpen = value;
            ToggleStream(value);
        }
    }
    private bool _isStreamOpen { get; set; } = true;

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

            // Stream
            using (httpClient)
            {
                var response = await httpClient.GetAsync("https://api.twitter.com/2/tweets/sample/stream?tweet.fields=context_annotations,lang,entities", HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var streamReader = new StreamReader(stream))
                {
                    // Process the response.
                    while (!streamReader.EndOfStream && IsStreamOpen)
                    {
                        try
                        {
                            var tweetStream = JsonConvert.DeserializeObject<TweetStreamData>(streamReader.ReadLine());
                            if (tweetStream is null || tweetStream.data is null) continue;

                            var newTweet = CreateTweetFromStream(tweetStream);
                            AllTweets.Add(newTweet);

                            CalculateTrending(tweetStream).ConfigureAwait(false);

                            TweetReceived?.Invoke(this, new TweetEventReceived(tweetStream, "received"));

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

    public async Task ToggleStream(bool isOpen)
    {
        IsStreamOpen = isOpen;

        //if (IsStreamOpen)
        //{
        //    await this.StartStream();
        //}
        //else
        //{
        //    await this.StopStream();
        //}
    }

    // Calculate Hashtags, Domains, and Subjects
    public async Task CalculateTrending(TweetStreamData tweetStream)
    {
        try
        {
            // Hashtags
            if (tweetStream.data.entities is not null)
            {
                var hashtags = tweetStream.data.entities.hashtags;
                if (hashtags is not null)
                {
                    foreach (var tag in hashtags)
                    {
                        var matchTag = HashTags.Where(x => x.Key.tag.ToUpper().Trim() == tag?.tag.ToUpper().Trim()).FirstOrDefault();
                        if (matchTag.Key is null)
                        {
                            HashTags.Add(tag, 1);
                        }
                        else
                        {
                            HashTags[matchTag.Key] = matchTag.Value + 1;
                        }
                    }
                }

                TopHashtags = HashTags.OrderByDescending(x => x.Value).Select(y => y.Key.tag).Take(10).ToList();
            }

            // DOMAIN and ENTITY
            if (tweetStream.data.context_annotations is not null)
            {
                foreach (var context in tweetStream.data.context_annotations)
                {
                    // TOPIC
                    var topic = context.domain;
                    if (topic is not null)
                    {
                        var match = Topics.Where(x => x.Key.name.ToUpper().Trim() == topic?.name.ToUpper().Trim()).FirstOrDefault();
                        if (match.Key is null)
                        {
                            Topics.Add(topic, 1);
                        }
                        else
                        {
                            Topics[match.Key] = match.Value + 1;
                        }

                        TrendingTopic = Topics.OrderByDescending(x => x.Value).FirstOrDefault().Key.name.ToString();
                    }

                    // SUBJECT
                    var subject = context.entity;
                    if (subject is not null)
                    {
                        var match = Subjects.Where(x => x.Key.name.ToUpper().Trim() == subject?.name.ToUpper().Trim()).FirstOrDefault();
                        if (match.Key is null)
                        {
                            Subjects.Add(subject, 1);
                        }
                        else
                        {
                            Subjects[match.Key] = match.Value + 1;
                        }

                        TrendingSubject = Subjects.OrderByDescending(x => x.Value).FirstOrDefault().Key.name.ToString();
                    }
                }
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Could not finish trending tasks: {ex}");
        }
    }

    // Convert from Stream to Tweet Model
    public TweetModel CreateTweetFromStream(TweetStreamData stream)
    {
        return new TweetModel()
        {
            Id = stream.data.id,
            Text = stream.data.text,
            Language = stream.data.lang,
            Name = $"Tweet-{stream.data.id}",
            Domain = stream.data.context_annotations is not null ? stream.data.context_annotations.FirstOrDefault()?.domain : null,
            Entity = stream.data.context_annotations is not null ? stream.data.context_annotations.FirstOrDefault()?.entity : null,
            Entities = stream.data.entities
        };
    }
}