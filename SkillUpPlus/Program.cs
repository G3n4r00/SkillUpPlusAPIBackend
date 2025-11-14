using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using SkillUpPlus.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SkillUpPlus.Services;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// --- CONFIGURAÇÃO DE SERVIÇOS ---

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração de JWT (Seu código estava perfeito)
var jwtOptions = builder.Configuration.GetSection("JwtOptions");
var firebaseProjectId = jwtOptions["Audience"];
var firebaseIssuer = jwtOptions["Issuer"];

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = firebaseIssuer;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = firebaseIssuer,
            ValidateAudience = true,
            ValidAudience = firebaseProjectId,
            ValidateLifetime = true
        };
    });

// Injeção de Dependência
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITrackService, TrackService>();
builder.Services.AddScoped<IProgressService, ProgressService>();
builder.Services.AddScoped<IOnboardingService, OnboardingService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// --- CONSTRUÇÃO DO APP E PIPELINE DE MIDDLEWARE ---

var app = builder.Build();

// Tratamento Global de Erros
app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { message = "Ocorreu um erro interno no servidor." });
    });
});


// População do Db
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao popular o banco de dados.");
    }
}

// Configuração do Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Ordem correta da Pipeline de Produção:
// 1. Encaminhar headers do Proxy (para sabermos se é HTTPS)
app.UseForwardedHeaders();

// 2. Aplicar política de CORS
app.UseCors("AllowAll");

// 3. Autenticar (Saber quem é)
app.UseAuthentication();

// 4. Autorizar (Saber o que pode fazer)
app.UseAuthorization();

// 5. Mapear para o Controller
app.MapControllers();

app.Run();