using Microsoft.Extensions.Options;
using Munters.Giphy.Api.Clients;
using Munters.Giphy.Api.Options;
using Munters.Giphy.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services
    .AddOptions<GiphyOptions>()
    .Bind(builder.Configuration.GetSection(GiphyOptions.SectionName))
    .Validate(
        options => Uri.TryCreate(
            options.BaseUrl,
            UriKind.Absolute,
            out _),
        "Giphy BaseUrl must be a valid absolute URL.")
    .Validate(
        options => !string.IsNullOrWhiteSpace(options.ApiKey),
        "Giphy API key is required.")
    .Validate(
        options => options.SearchLimit > 0,
        "Giphy SearchLimit must be greater than zero.")
    .Validate(
        options => options.TrendingLimit > 0,
        "Giphy TrendingLimit must be greater than zero.")
    .ValidateOnStart();

builder.Services.AddHttpClient<IGiphyClient, GiphyClient>(
    (serviceProvider, httpClient) =>
    {
        var options = serviceProvider
            .GetRequiredService<IOptions<GiphyOptions>>()
            .Value;

        httpClient.BaseAddress = new Uri(options.BaseUrl);
        httpClient.Timeout = TimeSpan.FromSeconds(10);
    });

builder.Services.AddScoped<IGifService, GifService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "Munters Giphy API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();