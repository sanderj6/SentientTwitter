using SentientTwitter.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SentientTwitter.CosmosDB;
using SentientTwitter.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<ModalService>();
builder.Services.AddSingleton<TextSentimentService>();
builder.Services.AddSingleton<TwitterService>();

// HttpClient
builder.Services.AddHttpClient("TwitterAPI", client =>
{
    var bearerToken = builder.Configuration.GetSection("Twitter").GetSection("bearerToken").Value;
    client.BaseAddress = new Uri("https://api.twitter.com/2/tweets/sample/stream?tweet.fields=context_annotations,lang,entities");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
});

// Cosmos
builder.Services
    .AddDbContextFactory<TwitterRecordDbContext>(options =>
    options.UseCosmos("AccountEndpoint=https://mood-users.documents.azure.com:443/;AccountKey=MXEkdkP6kvzk3FM1CNjLwHHWLQc5Fk06w0g2WkEtaWVRcIgP2PwXkSr4wZEgCvBmkYNXsG3Zh3iq9iMiiOLG0A==;", "mood-user-db")
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
