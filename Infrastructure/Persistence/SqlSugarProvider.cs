namespace TraceWPF.Infrastructure.Persistence
{
    using SqlSugar;

    /// <summary>
    /// SqlSugar ORM 客户端工厂类，提供创建 SqlSugarClient 实例的静态方法。
    /// SqlSugar ORM client factory class that provides a static method to create SqlSugarClient instances.
    /// </summary>
    public static class SqlSugarProvider
    {
        /// <summary>
        /// 根据给定的连接字符串创建一个 SQLite 数据库的 SqlSugarClient 实例，启用自动关闭连接。
        /// Creates a SqlSugarClient instance for a SQLite database using the given connection string, with auto-close connection enabled.
        /// </summary>
        /// <param name="connectionString">SQLite 数据库连接字符串 / The SQLite database connection string.</param>
        /// <returns>配置好的 SqlSugarClient 实例 / A configured SqlSugarClient instance.</returns>
        public static SqlSugarClient CreateClient(string connectionString)
        {
            return new SqlSugarClient(new ConnectionConfig
            {
                ConnectionString = connectionString,
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true
            });
        }
    }
}

