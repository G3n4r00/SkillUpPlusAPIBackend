using Microsoft.EntityFrameworkCore;
using SkillUpPlus.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SkillUpPlus.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var jwtOptions = builder.Configuration.GetSection("JwtOptions");
var firebaseProjectId = jwtOptions["Audience"];
var firebaseIssuer = jwtOptions["Issuer"];

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // Define Bearer como padrão
    .AddJwtBearer(options =>
    {
        // Define Firebase como a autoridade. 
        // O ASP.NET vai baixar automaticamente as chaves públicas do Google para validar a assinatura.
        options.Authority = firebaseIssuer;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = firebaseIssuer,

            ValidateAudience = true,
            ValidAudience = firebaseProjectId,

            ValidateLifetime = true // Garante que o token não expirou
        };
    });

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITrackService, TrackService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Popular o Db
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
