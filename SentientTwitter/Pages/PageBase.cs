using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SentientTwitter.CosmosDB;
using SentientTwitter.Services;
using System;

namespace SentientTwitter.Pages
{
    public class PageBase<T> : ComponentBase
    {
        [Inject]
        protected ILogger<T> Logger { get; set; }
        [Inject]
        protected TwitterService _twitterService { get; set; }
        [Inject]
        protected TextSentimentService _textAnalyzerService { get; set; }
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }
        [Inject]
        protected TwitterRecordDbContext _dbContext { get; set; }
    }
}
