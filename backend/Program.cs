using backend.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 添加服务
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// 添加数据库服务
builder.Services.AddDatabase(builder.Configuration);

// 自动注册应用服务
builder.Services.AddApplicationServices();

// 添加CORS策略
builder.Services.AddCorsPolicy();

// 配置服务器端口
builder.WebHost.UseUrls("http://*:5000");

var app = builder.Build();

// 配置中间件管道
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors("Development");
}
else
{
    app.UseCors("AllowAll");
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();