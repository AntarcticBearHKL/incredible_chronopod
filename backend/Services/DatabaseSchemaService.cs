using MySqlConnector;

namespace backend.Services;

public class DatabaseSchemaService
{
    private readonly string _connectionString;
    private readonly ILogger<DatabaseSchemaService> _logger;

    public DatabaseSchemaService(IConfiguration configuration, ILogger<DatabaseSchemaService> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new ArgumentException("Database connection string not found");
        _logger = logger;
    }

    public async Task InitializeDatabaseAsync()
    {
        try
        {
            _logger.LogInformation("开始初始化数据库...");
            
            string schemaPath = Path.Combine(Directory.GetCurrentDirectory(), "schema.sql");
            
            if (!File.Exists(schemaPath))
            {
                _logger.LogWarning("schema.sql 文件不存在: {path}", schemaPath);
                return;
            }

            string schemaSql = await File.ReadAllTextAsync(schemaPath);
            
            await ExecuteSqlFileAsync(schemaSql);
            
            _logger.LogInformation("数据库初始化完成");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "数据库初始化失败");
            throw;
        }
    }

    private async Task ExecuteSqlFileAsync(string sql)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var statements = sql.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                           .Where(s => !string.IsNullOrWhiteSpace(s))
                           .ToArray();

        foreach (var statement in statements)
        {
            var trimmedStatement = statement.Trim();
            if (string.IsNullOrEmpty(trimmedStatement) || trimmedStatement.StartsWith("--"))
                continue;

            try
            {
                using var command = new MySqlCommand(trimmedStatement, connection);
                await command.ExecuteNonQueryAsync();
                _logger.LogDebug("执行SQL语句成功: {statement}", trimmedStatement.Substring(0, Math.Min(50, trimmedStatement.Length)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "执行SQL语句失败: {statement}", trimmedStatement);
                throw;
            }
        }
    }

    public async Task<bool> CheckDatabaseConnectionAsync()
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "数据库连接失败");
            return false;
        }
    }
}