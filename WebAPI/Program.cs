using Common.Interfaces;
using Common.Services;

var builder = WebApplication.CreateBuilder(args);

// Just force the web host to listen on all interfaces
builder.WebHost.UseUrls("http://0.0.0.0:5000");

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IScoreManagerService, ScoreManagerService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
