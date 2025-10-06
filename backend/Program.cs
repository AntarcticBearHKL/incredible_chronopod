using backend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<DatabaseSchemaService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var schemaService = scope.ServiceProvider.GetRequiredService<DatabaseSchemaService>();
    await schemaService.InitializeDatabaseAsync();
}

app.UseUrls("http://localhost:5000");

app.UseHttpsRedirection();
app.MapControllers();

app.Run();