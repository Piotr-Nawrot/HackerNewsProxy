using Api.Endpoints.V1.Stories;
using Api.Middleware;
using Application.Services;
using Domain.Interfaces;
using Microsoft.OpenApi.Models;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(opts =>
{
    opts.EnableAnnotations();
    opts.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hacker News Proxy",
        Version = "v1"
    });
});

builder.Services.AddLogging();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddScoped<IHackerNewsService, HackerNewsService>();


var hackerNewsUri = builder.Configuration["HackerNewsApi:BaseUri"];
ArgumentNullException.ThrowIfNull(hackerNewsUri);
builder.Services
    .AddRefitClient<IHackerNewsApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(hackerNewsUri));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:5053")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

StoriesEndpoints.AddMapping(app);
Domain.Configurations.MappingConfig.RegisterDomainMappings();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors();
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();