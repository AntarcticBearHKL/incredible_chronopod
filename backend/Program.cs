var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.WebHost.UseUrls("http://*:5000", "https://*:5001");

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();