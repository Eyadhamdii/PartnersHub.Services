using Microsoft.EntityFrameworkCore;
using PartnersHub.ConfigurationHub.Application.Common.Interfaces;
using PartnersHub.ConfigurationHub.Application.Common.Interfaces.Repositories;
using PartnersHub.ConfigurationHub.Infrastructure.Persistence;
using PartnersHub.ConfigurationHub.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Partners Hub ConfigurationHub API", Version = "v1" });
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(PartnersHub.ConfigurationHub.Application.WhiteListIPs.Commands.CreateWhiteListIPCommand).Assembly);
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<ConfigurationHubDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection") ??
        "Server=(localdb)\\MSSQLLocalDB;Database=ConfigurationHubDB;Trusted_Connection=True;");
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
    options.EnableDetailedErrors(builder.Environment.IsDevelopment());
});

builder.Services.AddScoped<IWhiteListIPRepository, WhiteListIPRepository>();
builder.Services.AddScoped<ITermsAndConditionRepository, TermsAndConditionRepository>();
builder.Services.AddScoped<ISectorRepository, SectorRepository>();
builder.Services.AddScoped<ISubSectorRepository, SubSectorRepository>();
builder.Services.AddScoped<IAssetTypeRepository, AssetTypeRepository>();
builder.Services.AddScoped<IUnitOfMeasurementRepository, UnitOfMeasurementRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Partners Hub Configuration Hub API V1");
        c.RoutePrefix = "swagger";
    });
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    try
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ConfigurationHubDbContext>();

        if (dbContext.Database.GetPendingMigrations().Any())
        {
            await dbContext.Database.MigrateAsync();
        }

        await DbInitializer.InitializeDatabaseAsync(dbContext);
        app.Logger.LogInformation("Database initialized successfully");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred while initializing the database");
    }
}

app.Run();