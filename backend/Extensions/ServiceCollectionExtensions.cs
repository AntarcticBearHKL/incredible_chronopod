using System.Reflection;
using Microsoft.EntityFrameworkCore;
using backend.Data;

namespace backend.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 自动注册所有服务
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        // 找到所有以Service结尾的接口
        var serviceInterfaces = assembly.GetTypes()
            .Where(t => t.IsInterface && t.Name.StartsWith("I") && t.Name.EndsWith("Service"))
            .ToList();

        foreach (var serviceInterface in serviceInterfaces)
        {
            // 找到对应的实现类（例如：INoteService -> NoteService）
            var implementationType = assembly.GetTypes()
                .FirstOrDefault(t => t.IsClass && !t.IsAbstract && 
                                serviceInterface.IsAssignableFrom(t));

            if (implementationType != null)
            {
                services.AddScoped(serviceInterface, implementationType);
                Console.WriteLine($"自动注册服务: {serviceInterface.Name} -> {implementationType.Name}");
            }
        }

        return services;
    }

    /// <summary>
    /// 添加数据库配置（MySQL）
    /// </summary>
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), mySqlOptions =>
            {
                mySqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });
        });

        return services;
    }

    /// <summary>
    /// 添加CORS策略
    /// </summary>
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });

            options.AddPolicy("Development", builder =>
            {
                builder
                    .WithOrigins("http://localhost:3000", "http://localhost:8080", "http://127.0.0.1:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        return services;
    }
}