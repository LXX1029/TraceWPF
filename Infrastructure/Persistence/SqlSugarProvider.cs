namespace TraceWPF.Infrastructure.Persistence
{
    using SqlSugar;

    public static class SqlSugarProvider
    {
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

