using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using PartnersHub.Communities.Apis.Common;
using PartnersHub.Communities.Application.Common.Interfaces;
using PartnersHub.Communities.Application.Common.Interfaces.Rpository;
using PartnersHub.Communities.Infrastructure.Persistence;
using PartnersHub.Communities.Infrastructure.Persistence.Repositories;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("ar") };


// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Partners Hub Communities API", Version = "v1" });
});

// Add MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(PartnersHub.Communities.Application.Communities.Commands.CreateCommunityCommand).Assembly);
});

// Add DbContext with InMemory database
builder.Services.AddDbContext<CommunitiesDbContext>(options =>
{
    options.UseInMemoryDatabase("CommunitiesDb");
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors(); // Add support for value conversions in InMemoryDatabase
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddLocalization();

// Add Services
builder.Services.AddScoped<ICommunitiesRepository, CommunitiesRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Partners Hub Communities API V1");
        c.RoutePrefix = "swagger";
    });
    
    app.UseDeveloperExceptionPage();
}


app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();


// Initialize Database
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<CommunitiesDbContext>();
        await DbInitializer.InitializeDatabaseAsync(context);
        app.Logger.LogInformation("Database initialized successfully");
    }
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "An error occurred while initializing the database");
}

app.Run();