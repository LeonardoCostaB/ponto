using FluentValidation;
using MeuProjetoApi.Infraestrutura.Comportamentos;
using Microsoft.EntityFrameworkCore;
using Ponto.Api.Infra.Database;
using Ponto.Api.Infra.Integrations.Discord;
using Ponto.Api.Infra.Integrations.Tangerino;
using Ponto.Api.Infra.Results;

var builder = WebApplication.CreateBuilder(args);

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
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
