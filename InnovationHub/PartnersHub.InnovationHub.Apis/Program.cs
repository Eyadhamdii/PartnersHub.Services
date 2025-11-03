using Microsoft.EntityFrameworkCore;
using PartnersHub.InnovationHub.Apis.Common;
using PartnersHub.InnovationHub.Application.Common.Interfaces;
using PartnersHub.InnovationHub.Application.Common.Interfaces.Persistence;
using PartnersHub.InnovationHub.Infrastructure.Persistence;
using PartnersHub.InnovationHub.Infrastructure.Presistence;
using PartnersHub.InnovationHub.Infrastructure.Presistence.Repositories;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.IncludeFields = true;
    });

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(PartnersHub.InnovationHub.Application.AssemblyReference).Assembly);
});

builder.Services.AddDbContext<InnovationHubDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection") ??
        "Server=(localdb)\\MSSQLLocalDB;Database=InnovationHubDB;Trusted_Connection=True;",
        b => b.MigrationsAssembly(typeof(InnovationHubDbContext).Assembly.FullName));
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
    options.EnableDetailedErrors(builder.Environment.IsDevelopment());
});

builder.Services.AddScoped<IChallengeRequestRepository, ChallengeRequestRepository>();
builder.Services.AddScoped<IChallengeTechnologiesRequestRepository, ChallengeTechnologiesRequestRepository>();
builder.Services.AddScoped<ITechnologyRepository, TechnologyRepository>();
builder.Services.AddScoped<IAssociatedProviderRepository, AssociatedProviderRepository>();
builder.Services.AddScoped<IAssociatedSectorRepository, AssociatedSectorRepository>();
builder.Services.AddScoped<IReviewChallengeRepository, ReviewChallengeRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    try
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<InnovationHubDbContext>();
        await context.Database.EnsureCreatedAsync();
        app.Logger.LogInformation("Database initialized successfully");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred while initializing the database");
    }
}

app.Run();
