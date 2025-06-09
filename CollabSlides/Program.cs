using Microsoft.EntityFrameworkCore;
using CollabSlides.Models;
using CollabSlides.Hubs;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR(options =>
    {
        options.ClientTimeoutInterval = TimeSpan.FromMinutes(2); 
        options.HandshakeTimeout = TimeSpan.FromSeconds(30); 
        options.KeepAliveInterval = TimeSpan.FromSeconds(15); 
        options.EnableDetailedErrors = true; 
    })
    .AddJsonProtocol(options =>
    {
        
        options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.PayloadSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder
            .SetIsOriginAllowed(_ => true) 
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); 
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