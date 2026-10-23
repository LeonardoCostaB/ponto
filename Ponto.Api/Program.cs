using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Ponto.Infra.Database;
using Ponto.Infra.Integrations.Discord;
using Ponto.Infra.Integrations.Tangerino;
using Ponto.Infra.Results;
using Ponto.Infra.Validator;
using Ponto.Shared.Middlewares;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var apiKey = builder.Configuration["ApiKey"];

Console.WriteLine($"[STARTUP] ApiKey encontrada? {!string.IsNullOrEmpty(apiKey)}");
Console.WriteLine($"[STARTUP] ConnectionString encontrada? {!string.IsNullOrEmpty(connectionString)}");

if (string.IsNullOrEmpty(connectionString))
{
    // Força o erro imediato no log do Azure sem esperar timeout de 1 minuto
    throw new InvalidOperationException("CRÍTICO: A ConnectionString 'DefaultConnection' não foi encontrada nas variáveis de ambiente!");
}

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<Program>();
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.Configure<TangerinoOptions>(builder.Configuration.GetSection(TangerinoOptions.SectionName));
builder.Services.Configure<DiscordOptions>(builder.Configuration.GetSection(DiscordOptions.SectionName));

builder.Services.AddHttpClient();
builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient());
builder.Services.AddHttpClient<ITangerinoClient, TangerinoClient>();
builder.Services.AddHttpClient<IDiscordClient, DiscordClient>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsql => npgsql.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5), 
            errorCodesToAdd: new List<string> { "409" }
        )));

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ApiKeyMiddleware>();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
