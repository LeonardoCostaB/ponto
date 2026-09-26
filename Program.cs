using Microsoft.EntityFrameworkCore;
using Ponto.Api.Infra.Database;
using Ponto.Api.Infra.Integrations.Discord;
using Ponto.Api.Infra.Integrations.Tangerino;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.Configure<TangerinoOptions>(builder.Configuration.GetSection(TangerinoOptions.SectionName));
builder.Services.Configure<DiscordOptions>(builder.Configuration.GetSection(DiscordOptions.SectionName));

builder.Services.AddHttpClient();
builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient());
builder.Services.AddHttpClient<ITangerinoClient, TangerinoClient>();
builder.Services.AddHttpClient<IDiscordClient, DiscordClient>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();