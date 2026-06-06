using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Text.Json;
using VacuumPro.Api.Data;
using VacuumPro.Api.Middleware;
using VacuumPro.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<VacuumDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

//frontend

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}
app.UseStaticFiles();
// app.UseHttpsRedirection();

// app.MapGet("/test-db", async (VacuumDbContext db) =>
//     {
//         var count = await db.Vacuums.CountAsync();
//         return Results.Ok(new { count, message = "Database connection successful" });
//     });

app.UseCors();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<VacuumDbContext>();
    db.Database.EnsureCreated();

    var uploadFolder = Path.Combine(app.Environment.WebRootPath ?? "wwwroot", "uploads");
    Directory.CreateDirectory(uploadFolder);

    if (!db.Vacuums.Any())
    {
        db.Vacuums.AddRange(
            new Vacuum
            {
                Name = "Amica Bagio Eco VM 3046",
                Price = 299m,
                Power = 900,
                Description = "Opis produktu...",
                PicturePath = "/uploads/1.png"
            },
            new Vacuum
            {
                Name = "Bosch AquaWash&Clean BWD421PRO",
                Price = 869.99m,
                Power = 2100,
                Description = "Opis produktu...",
                PicturePath = "/uploads/2.png"
            }
        );

        db.SaveChanges();
    }
    else
    {
        var defaults = new Dictionary<int, string>
        {
            { 1, "/uploads/1.png" },
            { 2, "/uploads/2.png" }
        };

        var allVacuums = db.Vacuums.ToList();
        var needUpdate = allVacuums
            .Where(v => string.IsNullOrWhiteSpace(v.PicturePath) && defaults.ContainsKey(v.Id))
            .ToList();

        foreach (var vacuum in needUpdate)
        {
            vacuum.PicturePath = defaults[vacuum.Id];
        }

        if (needUpdate.Any())
            db.SaveChanges();
    }
}

app.Run();

