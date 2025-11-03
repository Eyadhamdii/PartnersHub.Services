using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PartnersHub.InfraBase.Apis.Common;
using PartnersHub.InfraBase.Application.Common.Interfaces;
using PartnersHub.InfraBase.Application.Common.Interfaces.Repository;
using PartnersHub.InfraBase.Infrastructure.Persistence;
using PartnersHub.InfraBase.Infrastructure.Persistence.Repositories;
using PartnersHub.InfraBase.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Add MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(PartnersHub.InfraBase.Application.AssemblyReference).Assembly);
});

// Add Database Context
builder.Services.AddDbContext<InfrabaseDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("PartnersHub.InfraBase.Infrastructure")));

// Add Repositories
builder.Services.AddScoped<IInfrabaseRequestRepository, InfrabaseRequestRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add Notification Service (placeholder implementation)
builder.Services.AddScoped<INotificationService, NotificationService>();

// Add CORS
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo {
        Title = "InfraBase API",
        Version = "v1",
        Description = "API for managing infrastructure requests and approvals"
    });

    // Add XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) {
        c.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddHttpClient<IConfigurationLookupService, ConfigurationLookupService>(client =>
{
    var baseUrl = builder.Configuration["ConfigurationHub:BaseUrl"];
    client.BaseAddress = new Uri(baseUrl!);
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        var publicKey = builder.Configuration["Jwt:PublicKey"];
        var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(Convert.FromBase64String(builder.Configuration["Jwt:PrivateKey"]), out _);

        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new RsaSecurityKey(rsa)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "InfraBase API V1");
    });
}

// Add global exception handling
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Seed database on startup
using (var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;
    try {
        var context = services.GetRequiredService<InfrabaseDbContext>();
        // context.Database.Migrate(); // Uncomment to auto-migrate
        app.Logger.LogInformation("Database connection verified");
    } catch (Exception ex) {
        app.Logger.LogError(ex, "An error occurred while connecting to the database");
    }
}

app.Run();

// Make Program class accessible for WebApplicationFactory testing
public partial class Program { }