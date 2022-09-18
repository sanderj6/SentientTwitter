using SentientTwitter.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SentientTwitter.CosmosDB;
using SentientTwitter.Data;
using SentientTwitter.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<ModalService>();
builder.Services.AddSingleton<TextAnalyzerService>();
builder.Services.AddSingleton<TwitterService>();

// Cosmos
builder.Services
    .AddDbContextFactory<SentimentRecordDbContext>(options =>
    options.UseCosmos("AccountEndpoint=https://mood-users.documents.azure.com:443/;AccountKey=MXEkdkP6kvzk3FM1CNjLwHHWLQc5Fk06w0g2WkEtaWVRcIgP2PwXkSr4wZEgCvBmkYNXsG3Zh3iq9iMiiOLG0A==;", "mood-user-db")
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
