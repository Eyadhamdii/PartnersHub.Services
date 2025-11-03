using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PartnersHub.Synergy.Apis.Common;
using PartnersHub.Synergy.Application.Interfaces;
using PartnersHub.Synergy.Application.Interfaces.Common;
using PartnersHub.Synergy.Application.Interfaces.Repository;
using PartnersHub.Synergy.Domain.Aggregates.SynergyCompanyAggregate;
using PartnersHub.Synergy.Domain.Resources;
using PartnersHub.Synergy.Infrastructure.Persistence;
using PartnersHub.Synergy.Infrastructure.Persistence.Repositories;
using PartnersHub.Synergy.Infrastructure.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.WriteIndented = true;
    });
// Add MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(PartnersHub.Synergy.Application.SynergyCompany.Queries.GetRegisteredCompaniesQuery).Assembly);
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Partners Hub Synergy API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            // 1. Validation Checks
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,

//            // 2. Set Expected Values
//            ValidIssuer = builder.Configuration["Jwt:Issuer"], // The server that created the token
//            ValidAudience = builder.Configuration["Jwt:Audience"], // The recipient of the token
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // The secret key for signing
//        };
//    });
// Add DbContext with SQL Server
builder.Services.AddDbContext<SynergyDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddTransient<ICacheWrapper, CacheWrapper>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<HandlerMessages>();
builder.Services.AddLocalization();
builder.Services.AddScoped<ISuccessStoryRepository, SuccessStoryRepository>();  
builder.Services.AddScoped<ICollaborationRequirementRepository, CollaborationRequirementRepository>();
builder.Services.AddScoped<IExpectedOutcomesRepository, ExpectedOutcomesRepository>();
builder.Services.AddScoped<IOpportunityTypeRepository, OpportunityTypeRepository>();
builder.Services.AddScoped<ISynergyCompanyRepository, SynergyCompanyRepository>();
builder.Services.AddScoped<IThematicAreaRepository, ThematicAreaRepository>();
builder.Services.AddScoped<IOpportunityRepository, OpportunityRepository>();
builder.Services.AddScoped<ISuccessStoryRepository, SuccessStoryRepository>();
builder.Services.AddScoped<ISuccessStroyTypeRepository,  SuccessStroyTypeRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(PartnersHub.Synergy.Application.AssemblyReference).Assembly);
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Partners Hub Synergy API V1");
        c.RoutePrefix = "swagger";
    });

    app.UseDeveloperExceptionPage();
}

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<SynergyDbContext>();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

try
{
    logger.LogInformation("Checking for pending migrations...");
    var database = dbContext.Database;
    
    if (database.GetPendingMigrations().Any())
    {
        logger.LogInformation("Applying pending migrations...");
        await database.MigrateAsync();
        logger.LogInformation("Migrations applied successfully.");
    }
    else
    {
        logger.LogInformation("Database is up to date.");
    }
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred while migrating the database: {Message}", ex.Message);
    throw;
}

if (!dbContext.SynergyCompanies.Any())
{
    var energySectorId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    var technologySectorId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    var healthcareSectorId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    var constructionSectorId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    
    var companies = new[]
    {
        SynergyCompany.Create(
            Guid.NewGuid(),
            "Masdar",
            "UAE",
            "Abu Dhabi",
            "Leading renewable energy company",
            "John Doe",
            "CEO",
            "john.doe@masdar.ae",
            "+971 2 565 5656",
            Guid.NewGuid()
        ).Value,
        SynergyCompany.Create(
            Guid.NewGuid(),
            "Tasaru",
            "UAE",
            "Dubai",
            "AI solutions provider",
            "Jane Doe",
            "CTO",
            "jane.doe@tasaru.com",
            "+971 4 123 4567",
            Guid.NewGuid()
        ).Value,
        SynergyCompany.Create(
            Guid.NewGuid(),
            "SIRC",
            "Saudi Arabia",
            "Riyadh",
            "Leading oil and gas company",
            "Mohammed Al Jaber",
            "President",
            "mohammed.aljaber@sirc.sa",
            "+966 11 123 4567",
            Guid.NewGuid()
        ).Value,
        SynergyCompany.Create(
            Guid.NewGuid(),
            "Tarshid",
            "Saudi Arabia",
            "Jeddah",
            "Construction and real estate development company",
            "Khalid Al Tarshid",
            "CEO",
            "khalid.al-tarshid@tarshid.com",
            "+966 2 123 4567",
            Guid.NewGuid()
        ).Value,
        SynergyCompany.Create(
            Guid.NewGuid(),
            "GCC Labs",
            "Qatar",
            "Doha",
            "Research and development institution",
            "Samer Al Sayed",
            "Director",
            "samer.alsayed@gcclabs.org",
            "+974 3311 1234",
            Guid.NewGuid()
        ).Value,
        SynergyCompany.Create(
            Guid.NewGuid(),
            "Sensetime",
            "China",
            "Beijing",
            "AI solutions provider",
            "Andrew Ng",
            "Founder",
            "andrew.ng@sensetime.com",
            "+86 10 1234 5678",
            Guid.NewGuid()
        ).Value,
        SynergyCompany.Create(
            new Guid("395f4b96-e64f-ef11-a4c6-005056992b12"),
            "SURJ",
            "Egypt",
            "Cairo",
            "Medical research institution",
            "Ronit Sela",
            "Director",
            "ronit.sela@surj.org.eg",
            "+202 2 123 4567",
            Guid.NewGuid()
        ).Value,
        SynergyCompany.Create(
            Guid.NewGuid(),
            "Jasara",
            "Saudi Arabia",
            "Riyadh",
            "Cybersecurity solutions provider",
            "Mohammed Al Ghamdi",
            "CEO",
            "mohammed.alghamdi@jasara.com",
            "+966 11 123 4567",
            Guid.NewGuid()
        ).Value,
        SynergyCompany.Create(
            Guid.NewGuid(),
            "IOT2",
            "Kuwait",
            "Kuwait City",
            "Telecommunications solutions provider",
            "Abdullah Al Otaibi",
            "CEO",
            "abdullah.alotaibi@iot2.com",
            "+965 22 123 4567",
            Guid.NewGuid()
        ).Value,
        SynergyCompany.Create(
            Guid.NewGuid(),
            "SkyPower",
            "Canada",
            "Toronto",
            "Solar energy provider",
            "Alex Johnson",
            "President",
            "alex.johnson@skypower.com",
            "+1 416 123 4567",
            Guid.NewGuid()
        ).Value
    };

    await dbContext.SynergyCompanies.AddRangeAsync(companies);
    await dbContext.SaveChangesAsync();

    companies[0].AddSectors(new Dictionary<Guid, string> 
    { 
        { energySectorId, "Energy" } 
    });
    companies[1].AddSectors(new Dictionary<Guid, string> 
    { 
        { technologySectorId, "Technology" } 
    });
    companies[2].AddSectors(new Dictionary<Guid, string> 
    { 
        { energySectorId, "Energy" } 
    });
    companies[3].AddSectors(new Dictionary<Guid, string> 
    { 
        { constructionSectorId, "Construction" } 
    });
    companies[4].AddSectors(new Dictionary<Guid, string> 
    { 
        { technologySectorId, "Technology" } 
    });
    companies[5].AddSectors(new Dictionary<Guid, string> 
    { 
        { technologySectorId, "Technology" } 
    });
    companies[6].AddSectors(new Dictionary<Guid, string> 
    { 
        { healthcareSectorId, "Healthcare" } 
    });
    companies[7].AddSectors(new Dictionary<Guid, string> 
    { 
        { technologySectorId, "Technology" } 
    });
    companies[8].AddSectors(new Dictionary<Guid, string> 
    { 
        { technologySectorId, "Technology" } 
    });
    companies[9].AddSectors(new Dictionary<Guid, string> 
    { 
        { energySectorId, "Energy" } 
    });

    await dbContext.SaveChangesAsync();
}
var cacheService = scope.ServiceProvider.GetRequiredService<ICacheWrapper>();
await cacheService.LoadLookupsIntoCacheAsync();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();



app.Run();
