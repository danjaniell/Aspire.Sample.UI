using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Weather.UI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddHttpClient<WeatherForecastService>(client =>
{
    var config = builder.Configuration;
    client.BaseAddress = new Uri(
        Environment.GetEnvironmentVariable("APIUrl")
            ?? config.GetValue<string>("APIUrl")
            ?? "http://localhost:7017"
    );
});

builder
    .Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics
            .AddRuntimeInstrumentation()
            .AddMeter(
                "Microsoft.AspNetCore.Hosting",
                "Microsoft.AspNetCore.Server.Kestrel",
                "System.Net.Http"
            );
    })
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation();
    });

var useOtlpExporter = !string.IsNullOrWhiteSpace(
    builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]
);
if (useOtlpExporter)
{
    builder.Services.AddOpenTelemetry().UseOtlpExporter();
}

var app = builder.Build();

app.UseExceptionHandler("/Error");

// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
app.UseHsts();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
