using Microsoft.EntityFrameworkCore;
using CollabSlides.Models;
using CollabSlides.Hubs;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add SignalR with configuration
builder.Services.AddSignalR(options =>
{
    options.ClientTimeoutInterval = TimeSpan.FromMinutes(2); // Default is 30 seconds
    options.HandshakeTimeout = TimeSpan.FromSeconds(30); // Default is 15 seconds
    options.KeepAliveInterval = TimeSpan.FromSeconds(15); // Default is 15 seconds
    options.EnableDetailedErrors = true; // For debugging
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder
            .SetIsOriginAllowed(_ => true) // Allow any origin for testing
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // Required for SignalR
    });
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapControllers();
app.MapHub<PresentationHub>("/presentationHub");

app.Run();

