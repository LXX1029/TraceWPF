using System.Buffers;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;
using SqlSugar;
using TraceWPF.DI;
using TraceWPF.Domain.Interfaces;
using TraceWPF.Domain.Models;

namespace TraceWPF.Infrastructure.Services
{
    /// <summary>
    /// 数据迁移服务实现类，通过 SqlSugar ORM 操作 TDengine 数据库，提供数据库创建/删除、
    /// 表结构迁移、数据迁移（按子表逐一迁移，支持时间过滤和分页）、清空数据等功能。
    /// 
    /// Data migration service implementation that uses SqlSugar ORM to operate on TDengine databases.
    /// Provides database creation/deletion, schema migration, data migration (sub-table by sub-table with time filtering and pagination),
    /// and data clearing functionality.
    /// </summary>
    public class DataMigrationService : IDataMigrationService, ITransient
    {
        /// <summary>
        /// 创建目标 TDengine 数据库，使用指定的参数（BUFFER、CACHESIZE、DURATION、KEEP 等）。
        /// Creates the target TDengine database using specified parameters (BUFFER, CACHESIZE, DURATION, KEEP, etc.).
        /// </summary>
        /// <param name="connectionString">目标数据库连接字符串 / Target database connection string.</param>
        /// <param name="dbName">要创建的数据库名称 / The name of the database to create.</param>
        /// <param name="dataBaseParam">数据库创建参数对象 / Database creation parameters object.</param>
        public async Task CreateDatabaseAsync(string connectionString, string dbName, DataBaseParam dataBaseParam)
        {
            // Connect to server generally.
            var config = new ConnectionConfig()
            {
                ConnectionString = connectionString,
                // Cast 24 to DbType for TDengine if enum is missing in this version
                DbType = SqlSugar.DbType.TDengine,
                IsAutoCloseConnection = true,

            };

            using var db = new SqlSugarClient(config);
            var sql = $"CREATE DATABASE IF NOT EXISTS {dbName} BUFFER {dataBaseParam.Buffer} CACHESIZE {dataBaseParam.Cachesize} CACHEMODEL 'last_row' COMP 2 DURATION {dataBaseParam.Duration}d WAL_FSYNC_PERIOD 3000 MAXROWS {dataBaseParam.Maxrows} MINROWS 100 STT_TRIGGER {dataBaseParam.Stt_trigger} KEEP {dataBaseParam.KeepDays}d PAGES {dataBaseParam.Pages} PAGESIZE {dataBaseParam.Pagesize} PRECISION 'us' REPLICA 1 WAL_LEVEL 1 VGROUPS {dataBaseParam.Vgroups} SINGLE_STABLE 0";
            await Task.Run(async () =>
            {
                await db.Ado.ExecuteCommandAsync(sql);
            });
        }

        /// <summary>
        /// 删除指定的 TDengine 数据库（如果存在）。
        /// Drops the specified TDengine database if it exists.
        /// </summary>
        /// <param name="connectionString">目标数据库连接字符串 / Target database connection string.</param>
        /// <param name="dbName">要删除的数据库名称 / The name of the database to drop.</param>
        public async Task DeleteDatabaseAsync(string connectionString, string dbName)
        {
            var config = new ConnectionConfig()
            {
                ConnectionString = connectionString,
                DbType = SqlSugar.DbType.TDengine,
                IsAutoCloseConnection = true
            };

            using var db = new SqlSugarClient(config);
            var sql = $"DROP DATABASE IF EXISTS {dbName}";
            await db.Ado.ExecuteCommandAsync(sql);
        }

        /// <summary>
        /// 迁移表结构：从源数据库读取所有超级表（STable）定义并在目标库中重建，
        /// 然后遍历所有子表，读取其 TAG 值并在目标库中创建对应的子表。
        /// 
        /// Migrates schema: reads all super table (STable) definitions from the source database and recreates them in the target database,
        /// then iterates through all sub-tables, reads their TAG values, and creates corresponding sub-tables in the target database.
        /// </summary>
        /// <param name="sourceConn">源数据库连接字符串 / Source database connection string.</param>
        /// <param name="targetConn">目标数据库连接字符串 / Target database connection string.</param>
        /// <param name="sourceDbName">源数据库名称 / Source database name.</param>
        /// <param name="targetDbName">目标数据库名称 / Target database name.</param>
        public async Task MigrateSchemaAsync(string sourceConn, string targetConn, string sourceDbName, string targetDbName)
        {
            var sourceConfig = new ConnectionConfig() { ConnectionString = sourceConn, DbType = SqlSugar.DbType.TDengine, IsAutoCloseConnection = true, LanguageType = LanguageType.Chinese };
            var targetConfig = new ConnectionConfig() { ConnectionString = targetConn, DbType = SqlSugar.DbType.TDengine, IsAutoCloseConnection = true };

            using (var sourceDb = new SqlSugarClient(sourceConfig))
            using (var targetDb = new SqlSugarClient(targetConfig))
            {
                // Open connections explicitly to maintain session for USE command
                sourceDb.Ado.Open();
                targetDb.Ado.Open();

                // TDengine 3.x: Use database-qualified queries instead of USE command
                // This avoids session context issues with the driver.

                // Use target database (for schema creation later)
                if (!string.IsNullOrEmpty(sourceDbName))
                {
                    await sourceDb.Ado.ExecuteCommandAsync($"USE {sourceDbName}");
                }

                // 1. Get STables using INFORMATION_SCHEMA (TDengine 3.0 standard approach)
                //var stableNames = await sourceDb.Ado.SqlQueryAsync<string>("SELECT SERVER_VERSION()");
                var dtStables = await sourceDb.Ado.GetDataTableAsync(
                    $"Show Stables");

                var stableTagsMap = new Dictionary<string, List<string>>();
                //var count = await sourceDb.Ado.GetIntAsync($"SELECT COUNT(*) FROM INFORMATION_SCHEMA.INS_STABLES WHERE db_name = '{sourceDbName}'");
                //var dtAll = await sourceDb.Ado.GetDataTableAsync("SELECT DISTINCT db_name FROM INFORMATION_SCHEMA.INS_STABLES");
                //foreach (DataRow row in dtStables.Rows)
                //{
                var stableRowCount = dtStables.Rows.Count;
                for (int i = 0; i < stableRowCount; i++)
                {
                    var row = dtStables.Rows[i];
                    string stableName = row["stable_name"].ToString();
                    // Get Create SQL
                    var dtCreate = await sourceDb.Ado.GetDataTableAsync($"SHOW CREATE STABLE {stableName}");
                    if (dtCreate.Rows.Count > 0)
                    {
                        // Column 1 is usually 'Create Stable'
                        string createSql = dtCreate.Rows[0][1].ToString();

                        // Fix DB Name reference in SQL: `CREATE STABLE source.tb ...` -> `CREATE STABLE tb ...`
                        if (!string.IsNullOrEmpty(sourceDbName))
                        {
                            createSql = createSql.Replace($"{sourceDbName}.", "");
                        }

                        // Execute on target
                        await targetDb.Ado.ExecuteCommandAsync(createSql);
                    }

                    // Get Schema for Tags
                    var describe = await sourceDb.Ado.GetDataTableAsync($"DESCRIBE {stableName}");
                    var tags = new List<string>();
                    var describeRowCount = describe.Rows.Count;
                    //foreach (DataRow field in describe.Rows)
                    //{
                    for (int j = 0; j < describeRowCount; j++)
                    {
                        var field = describe.Rows[j];
                        // Note column usually contains 'TAG' for tags
                        if (field["Note"].ToString() == "TAG")
                        {
                            tags.Add(field["Field"].ToString());
                        }
                    }
                    //}
                    stableTagsMap[stableName] = tags;
                }

                //}

                // 2. Get SubTables using INFORMATION_SCHEMA.INS_TABLES (TDengine 3.0)
                // This returns: table_name, db_name, create_time, columns, stable_name, ...
                var dtTables = await sourceDb.Ado.GetDataTableAsync(
                    $"SELECT table_name, stable_name FROM INFORMATION_SCHEMA.INS_TABLES WHERE db_name = '{sourceDbName}' AND stable_name IS NOT NULL");
                var tableRowCount = dtTables.Rows.Count;
                //foreach (DataRow row in dtTables.Rows)
                //{
                for (int i = 0; i < tableRowCount; i++)
                {
                    var row = dtTables.Rows[i];
                    string stableName = row["stable_name"]?.ToString() ?? "";
                    string tableName = row["table_name"]?.ToString() ?? "";

                    // Only process if it belongs to a known STable
                    if (!string.IsNullOrEmpty(stableName) && stableTagsMap.ContainsKey(stableName))
                    {
                        var tagCols = stableTagsMap[stableName];

                        string createSubSql;
                        if (tagCols.Count > 0)
                        {
                            string tagSel = string.Join(",", tagCols);
                            var tagValDt = await sourceDb.Ado.GetDataTableAsync($"SELECT {tagSel} FROM {tableName} LIMIT 1");
                            if (tagValDt.Rows.Count > 0)
                            {
                                var values = tagValDt.Rows[0].ItemArray;
                                var valStrList = new List<string>();
                                for (int j = 0; j < values.Length; j++)
                                {
                                    var val = values[j];
                                    if (val is string || val is DateTime) valStrList.Add($"'{val}'");
                                    else valStrList.Add(val.ToString());
                                }
                                string valStr = string.Join(",", valStrList);
                                createSubSql = $"CREATE TABLE IF NOT EXISTS {tableName} USING {stableName} TAGS ({valStr})";
                            }
                            else
                            {
                                // Should not happen if table exists, unless empty? 
                                // Even empty table has tags metadata? 
                                // Actually `SELECT tags FROM table` works even if empty?
                                // In TDengine, tags are metadata, so yes.
                                createSubSql = $"CREATE TABLE IF NOT EXISTS {tableName} USING {stableName} TAGS ({string.Join(",", tagCols.Select(_ => "null"))})"; // Fallback
                            }
                        }
                        else
                        {
                            createSubSql = $"CREATE TABLE IF NOT EXISTS {tableName} USING {stableName} TAGS ()";
                        }

                        if (!string.IsNullOrEmpty(createSubSql))
                        {
                            await targetDb.Ado.ExecuteCommandAsync(createSubSql);
                        }
                    }
                }
                //}
            }
        }

        /// <summary>
        /// 查询目标数据库的最大 SQL 语句长度限制（maxsqllength 变量）。
        /// 如果查询失败则返回默认值 1048576 (1MB)。
        /// 
        /// Queries the target database for the maximum SQL statement length limit (maxsqllength variable).
        /// Returns a default value of 1048576 (1MB) if the query fails.
        /// </summary>
        /// <param name="db">SqlSugar 数据库客户端实例 / The SqlSugar database client instance.</param>
        /// <returns>最大 SQL 语句长度 / The maximum SQL statement length.</returns>
        private async Task<int> GetMaxSqlLengthAsync(SqlSugarClient db)
        {
            try
            {
                var dt = await db.Ado.GetDataTableAsync("SHOW VARIABLES");
                foreach (DataRow row in dt.Rows)
                {
                    if (row[0]?.ToString()?.Equals("maxsqllength", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        if (int.TryParse(row[1]?.ToString(), out int length))
                        {
                            return length;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error querying maxSQLLength: {ex.Message}");
            }
            return 1048576; // Default to 1MB if query fails
        }

        /// <summary>
        /// [旧版] 数据迁移方法（基于 filterDays 整数过滤天数）：按子表逐一迁移数据，
        /// 使用 TS 分页策略和 SQL 语句长度检查以避免超长 SQL。
        /// 已被新版 MigrateDataAsync（基于 DateTime 起止时间）替代。
        /// 
        /// [Legacy] Data migration method (filter by integer filterDays): migrates data sub-table by sub-table,
        /// using TS-based pagination and SQL length checking to avoid oversized SQL statements.
        /// Superseded by the new MigrateDataAsync (DateTime-based start/end time).
        /// </summary>
        /// <param name="sourceConn">源数据库连接字符串 / Source database connection string.</param>
        /// <param name="targetConn">目标数据库连接字符串 / Target database connection string.</param>
        /// <param name="sourceDbName">源数据库名称 / Source database name.</param>
        /// <param name="targetDbName">目标数据库名称 / Target database name.</param>
        /// <param name="filterDays">数据过滤天数（0 表示全部数据）/ Filter days (0 means all data).</param>
        /// <param name="token">取消令牌 / Cancellation token.</param>
        /// <param name="action">进度回调方法 / Progress callback action.</param>
        public async Task MigrateDataAsync1(string sourceConn, string targetConn, string sourceDbName, string targetDbName, int filterDays, CancellationToken token, Action<string>? action)
        {
            var sourceConfig = new ConnectionConfig() { ConnectionString = sourceConn, DbType = SqlSugar.DbType.TDengine, IsAutoCloseConnection = true };
            var targetConfig = new ConnectionConfig() { ConnectionString = targetConn, DbType = SqlSugar.DbType.TDengine, IsAutoCloseConnection = true };

            using var sourceDb = new SqlSugarClient(sourceConfig);
            using var targetDb = new SqlSugarClient(targetConfig);
            // Open connections
            sourceDb.Ado.Open();
            targetDb.Ado.Open();

            // Use source database
            if (!string.IsNullOrEmpty(sourceDbName))
            {
                await sourceDb.Ado.ExecuteCommandAsync($"USE {sourceDbName}");
            }
            if (!string.IsNullOrEmpty(targetDbName))
            {
                await targetDb.Ado.ExecuteCommandAsync($"USE {targetDbName}");
            }

            // Calculate time filter based on filterDays
            DateTime endTime = DateTime.Now;
            DateTime cursorTime = DateTime.MinValue;
            if (filterDays > 0)
            {
                cursorTime = endTime.AddDays(-filterDays);
            }

            // Get max SQL length from target
            int maxSQLLength = await GetMaxSqlLengthAsync(targetDb);
            int safeSQLLength = (int)(maxSQLLength * 0.8); // 80% safety margin

            // Get all super tables
            var dtStables = await sourceDb.Ado.GetDataTableAsync("SHOW STABLES");
            int stableCount = dtStables.Rows.Count;
            StringBuilder sb = new StringBuilder();
            for (int si = 0; si < stableCount; si++)
            {
                DataRow stableRow = dtStables.Rows[si];
                string stableName = stableRow["stable_name"]?.ToString() ?? "";
                if (string.IsNullOrEmpty(stableName)) continue;
                string stableWhere = $" WHERE ts > '{cursorTime:yyyy-MM-dd HH:mm:ss.ffffff}' and ts < '{endTime:yyyy-MM-dd HH:mm:ss.ffffff}'";
                var dtTotalData = await sourceDb.Ado.GetDataTableAsync($"SELECT count(*) FROM {stableName}{stableWhere}");
                if (dtTotalData == null || dtTotalData.Rows.Count == 0) continue;
                long totalStableCount = Convert.ToInt32(dtTotalData.Rows[0][0]);
                // Get TAG column names for this super table using DESCRIBE
                var describe = await sourceDb.Ado.GetDataTableAsync($"DESCRIBE {stableName}");
                var tagColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                int describeCount = describe.Rows.Count;
                for (int sj = 0; sj < describeCount; sj++)
                {
                    DataRow field = describe.Rows[sj];
                    var note = field["Note"]?.ToString() ?? "";
                    if (note.Equals("TAG", StringComparison.OrdinalIgnoreCase))
                    {
                        tagColumns.Add(field["Field"]?.ToString() ?? "");
                    }
                }
                token.ThrowIfCancellationRequested();
                // Sub-table migration strategy
                // 1. Get all child tables for this super table
                var dtChildTables = await sourceDb.Ado.GetDataTableAsync(
                    $"SELECT table_name FROM INFORMATION_SCHEMA.INS_TABLES WHERE db_name = '{sourceDbName}' and stable_name = '{stableName}'");
                int childTableCount = dtChildTables.Rows.Count;
                long totalStableMigrated = 0;
                // 2. Iterate through each child table
                for (int t = 0; t < childTableCount; t++)
                {
                    string tbName = dtChildTables.Rows[t]["table_name"]?.ToString() ?? "";
                    if (string.IsNullOrEmpty(tbName)) continue;
                    token.ThrowIfCancellationRequested();
                    // 3. Migrate data for this child table
                    try
                    {
                        // Basic pagination for safety on large child tables
                        // Using TS pagination per child table is safe and fast
                        DateTime tableCursor = cursorTime;
                        int tableBatchSize = 9000;

                        while (!token.IsCancellationRequested)
                        {
                            string tableWhere = $" WHERE ts > '{tableCursor:yyyy-MM-dd HH:mm:ss.ffffff}' and ts < '{endTime:yyyy-MM-dd HH:mm:ss.ffffff}'";
                            DataTable? dtData = null;
                            try
                            {
                                string fullTableName = string.IsNullOrEmpty(sourceDbName) ? tbName : $"{sourceDbName}.{tbName}";
                                dtData = await sourceDb.Ado.GetDataTableAsync($"SELECT * FROM {fullTableName}{tableWhere} order by ts asc LIMIT {tableBatchSize}");
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Error reading source table {tbName}: {ex.Message}");
                                throw new Exception($"Error reading source table {tbName}: {ex.Message}", ex);
                            }

                            if (dtData == null || dtData.Rows.Count == 0) break;

                            // Update schema info if needed (cols)
                            // Note: SELECT * on subtable does NOT return 'tbname' or TAG columns in TDengine
                            var cols = dtData.Columns.Cast<DataColumn>()
                                        .Select(c => c.ColumnName)
                                        .ToList();

                            if (cols.Count > 0)
                            {
                                int colCount = cols.Count;
                                int rowCount = dtData.Rows.Count;

                                string targetFullTableName = string.IsNullOrEmpty(targetDbName) ? tbName : $"{targetDbName}.{tbName}";
                                string insertPrefix = $"INSERT INTO {targetFullTableName} ({string.Join(",", cols)}) VALUES ";

                                try
                                {
                                    sb.Append(insertPrefix);

                                    for (int ri = 0; ri < rowCount; ri++)
                                    {
                                        var row = dtData.Rows[ri];
                                        string[] rowBuf = ArrayPool<string>.Shared.Rent(colCount);
                                        try
                                        {
                                            for (int ci = 0; ci < colCount; ci++)
                                            {
                                                var val = row[cols[ci]];
                                                if (val is DateTime dt) rowBuf[ci] = $"'{dt:yyyy-MM-dd HH:mm:ss.ffffff}'";
                                                else if (val is string s) rowBuf[ci] = $"'{s.Replace("'", "''")}'";
                                                else if (val == DBNull.Value) rowBuf[ci] = "NULL";
                                                else if (val is bool b) rowBuf[ci] = b ? "true" : "false";
                                                else if (val is int || val is long || val is short || val is byte || val is uint || val is ulong || val is ushort || val is sbyte)
                                                    rowBuf[ci] = val.ToString() ?? "0";
                                                else if (val is float || val is double || val is decimal)
                                                    rowBuf[ci] = Math.Round(double.Parse(val.ToString()!), 4).ToString() ?? "0";
                                                else rowBuf[ci] = val?.ToString() ?? "NULL";
                                                token.ThrowIfCancellationRequested();
                                            }
                                            string rowValues = $"({string.Join(",", rowBuf, 0, colCount)})";

                                            // Check if adding this row would exceed the safe length
                                            if (sb.Length + rowValues.Length + 1 > safeSQLLength && sb.Length > insertPrefix.Length)
                                            {
                                                await targetDb.Ado.ExecuteCommandAsync(sb.ToString());
                                                sb.Clear();
                                                sb.Append(insertPrefix);
                                            }

                                            if (sb.Length > insertPrefix.Length) sb.Append(" ");
                                            sb.Append(rowValues);
                                        }
                                        finally
                                        {
                                            ArrayPool<string>.Shared.Return(rowBuf);
                                        }
                                        token.ThrowIfCancellationRequested();
                                    }

                                    if (sb.Length > insertPrefix.Length)
                                    {
                                        await targetDb.Ado.ExecuteCommandAsync(sb.ToString());
                                    }
                                }
                                catch (Exception ex)
                                {
#if DEBUG
                                    Debug.WriteLine($"Error writing to target table {tbName}: {ex.Message}");
#endif
                                    action?.Invoke($"Error writing to target table {tbName}: {ex.Message}");
                                    if (ex.Message.Contains("SQL statement too long") || ex.Message.Contains("0x219"))
                                    {
                                        tableBatchSize -= 500;
                                    }
                                    if (ex is OperationCanceledException)
                                        throw;
                                    else
                                        continue;
                                }
                                finally
                                {
                                    sb.Clear();
                                }

                                // Update cursor
                                if (dtData.Rows[rowCount - 1]["ts"] is DateTime lastTs)
                                {
                                    tableCursor = lastTs;
                                }

                                totalStableMigrated += rowCount;
                                var migratePercent = (totalStableMigrated * 100.0) / totalStableCount;
                                action?.Invoke($"Migrated {totalStableMigrated}/{totalStableCount} {migratePercent:N2}% rows for stable {stableName} (current: {tbName})");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Debug.WriteLine($"Error migrating table {tbName}: {ex.Message}");
#endif
                        action?.Invoke($"Error migrating table {tbName}: {ex.Message}");
                        if (ex is OperationCanceledException)
                            throw;
                        else
                            continue;
                    }
                }
            }
        }

        /// <summary>
        /// 用于记录迁移结束时间的属性。
        /// Property for recording the migration end time.
        /// </summary>
        private DateTime EndTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 数据迁移方法（基于 DateTime 起止时间）：
        /// 1. 连接源和目标 TDengine 数据库。
        /// 2. 遍历源库中所有超级表，获取每个超级表下的所有子表。
        /// 3. 对每个子表，使用 TS 分页策略按批次读取数据。
        /// 4. 将每行数据格式化为 INSERT VALUES 语句并批量写入目标库。
        /// 5. 使用 ArrayPool 降低内存分配压力，支持取消和错误恢复。
        /// 
        /// Data migration method (DateTime-based start/end time):
        /// 1. Connects to source and target TDengine databases.
        /// 2. Iterates through all super tables in the source database, retrieves all sub-tables for each.
        /// 3. For each sub-table, reads data in batches using TS-based pagination.
        /// 4. Formats each row into INSERT VALUES statements and batch-inserts into the target database.
        /// 5. Uses ArrayPool to reduce memory allocation pressure; supports cancellation and error recovery.
        /// </summary>
        /// <param name="sourceConn">源数据库连接字符串 / Source database connection string.</param>
        /// <param name="targetConn">目标数据库连接字符串 / Target database connection string.</param>
        /// <param name="sourceDbName">源数据库名称 / Source database name.</param>
        /// <param name="targetDbName">目标数据库名称 / Target database name.</param>
        /// <param name="startDateTime">数据过滤的开始时间 / Start datetime for data filtering.</param>
        /// <param name="endDateTime">数据过滤的结束时间 / End datetime for data filtering.</param>
        /// <param name="tableBatchSizet">每批次迁移的行数 / Number of rows per migration batch.</param>
        /// <param name="token">取消令牌 / Cancellation token.</param>
        /// <param name="action">进度回调方法 / Progress callback action.</param>
        public async Task MigrateDataAsync(string sourceConn, string targetConn, string sourceDbName, string targetDbName, DateTime startDateTime, DateTime endDateTime, int tableBatchSizet, CancellationToken token, Action<string>? action)
        {
            var sourceConfig = new ConnectionConfig() { ConnectionString = sourceConn, DbType = SqlSugar.DbType.TDengine, IsAutoCloseConnection = true };
            var targetConfig = new ConnectionConfig() { ConnectionString = targetConn, DbType = SqlSugar.DbType.TDengine, IsAutoCloseConnection = true };

            using var sourceDb = new SqlSugarClient(sourceConfig);
            using var targetDb = new SqlSugarClient(targetConfig);
            // Open connections
            sourceDb.Ado.Open();
            targetDb.Ado.Open();

            // Use source database
            if (!string.IsNullOrEmpty(sourceDbName))
            {
                await sourceDb.Ado.ExecuteCommandAsync($"USE {sourceDbName}");
            }
            if (!string.IsNullOrEmpty(targetDbName))
            {
                await targetDb.Ado.ExecuteCommandAsync($"USE {targetDbName}");
            }

            // Calculate time filter based on filterDays
            //EndTime = DateTime.Now;
            DateTime cursorTime = startDateTime;
            // Get all super tables
            var dtStables = await sourceDb.Ado.GetDataTableAsync("SHOW STABLES");
            int stableCount = dtStables.Rows.Count;
            StringBuilder sb = new StringBuilder();
            for (int si = 0; si < stableCount; si++)
            {
                DataRow stableRow = dtStables.Rows[si];
                string stableName = stableRow["stable_name"]?.ToString() ?? "";
                if (string.IsNullOrEmpty(stableName)) continue;
                string stableWhere = $" WHERE ts > '{cursorTime:yyyy-MM-dd HH:mm:ss.ffffff}' and ts < '{endDateTime:yyyy-MM-dd HH:mm:ss.ffffff}'";
                var dtTotalData = await sourceDb.Ado.GetDataTableAsync($"SELECT count(*) FROM {stableName}{stableWhere}");
                if (dtTotalData == null || dtTotalData.Rows.Count == 0) continue;
                if (int.TryParse(dtTotalData.Rows[0][0]?.ToString(), out int count) == false || count == 0)
                {
                    continue;
                }
                int totalStableCount = count;// Convert.ToInt32(dtTotalData.Rows[0][0]);
                // Get TAG column names for this super table using DESCRIBE
                var describe = await sourceDb.Ado.GetDataTableAsync($"DESCRIBE {stableName}");
                var tagColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                int describeCount = describe.Rows.Count;
                for (int sj = 0; sj < describeCount; sj++)
                {
                    DataRow field = describe.Rows[sj];
                    var note = field["Note"]?.ToString() ?? "";
                    if (note.Equals("TAG", StringComparison.OrdinalIgnoreCase))
                    {
                        tagColumns.Add(field["Field"]?.ToString() ?? "");
                    }
                }
                token.ThrowIfCancellationRequested();
                // Sub-table migration strategy
                // 1. Get all child tables for this super table
                var dtChildTables = await sourceDb.Ado.GetDataTableAsync(
                    $"SELECT table_name FROM INFORMATION_SCHEMA.INS_TABLES WHERE db_name = '{sourceDbName}' and stable_name = '{stableName}'");
                int childTableCount = dtChildTables.Rows.Count;
                long totalStableMigrated = 0;
                // 2. Iterate through each child table
                for (int t = 0; t < childTableCount; t++)
                {
                    string tbName = dtChildTables.Rows[t]["table_name"]?.ToString() ?? "";
                    if (string.IsNullOrEmpty(tbName)) continue;
                    token.ThrowIfCancellationRequested();
                    // 3. Migrate data for this child table
                    try
                    {
                        // Basic pagination for safety on large child tables
                        // Using TS pagination per child table is safe and fast
                        DateTime tableCursor = cursorTime;
                        int tableBatchSize = tableBatchSizet;

                        while (!token.IsCancellationRequested)
                        {
                            string tableWhere = $" WHERE ts > '{tableCursor:yyyy-MM-dd HH:mm:ss.ffffff}' and ts < '{endDateTime:yyyy-MM-dd HH:mm:ss.ffffff}'";
                            DataTable? dtData = null;
                            try
                            {
                                string fullTableName = string.IsNullOrEmpty(sourceDbName) ? tbName : $"{sourceDbName}.{tbName}";
                                dtData = await sourceDb.Ado.GetDataTableAsync($"SELECT * FROM {fullTableName}{tableWhere} order by ts asc LIMIT {tableBatchSize}");
                            }
                            catch (Exception ex)
                            {
#if DEBUG
                                Debug.WriteLine($"Error reading source table {tbName}: {ex.Message}");
#endif
                                //throw new Exception($"Error reading source table {tbName}: {ex.Message}", ex);
                                action?.Invoke($"Error reading source table {tbName}: {ex.Message}");
                                continue;
                            }

                            if (dtData == null || dtData.Rows.Count == 0) break;

                            // Update schema info if needed (cols)
                            // Note: SELECT * on subtable does NOT return 'tbname' or TAG columns in TDengine
                            var cols = dtData.Columns.Cast<DataColumn>()
                                        .Select(c => c.ColumnName)
                                        .ToList();

                            if (cols.Count > 0)
                            {
                                int colCount = cols.Count;
                                int rowCount = dtData.Rows.Count;


                                string targetFullTableName = string.IsNullOrEmpty(targetDbName) ? tbName : $"{targetDbName}.{tbName}";
                                sb.Append($"INSERT INTO {targetFullTableName} ({string.Join(",", cols)}) VALUES ");

                                string[] valuesArray = ArrayPool<string>.Shared.Rent(rowCount);
                                try
                                {
                                    for (int ri = 0; ri < rowCount; ri++)
                                    {
                                        var row = dtData.Rows[ri];
                                        string[] rowBuf = ArrayPool<string>.Shared.Rent(colCount);
                                        try
                                        {
                                            for (int ci = 0; ci < colCount; ci++)
                                            {
                                                var val = row[cols[ci]];
                                                if (val is DateTime dt) rowBuf[ci] = $"'{dt:yyyy-MM-dd HH:mm:ss.ffffff}'";
                                                else if (val is string s) rowBuf[ci] = $"'{s.Replace("'", "''")}'";
                                                else if (val == DBNull.Value) rowBuf[ci] = "NULL";
                                                else if (val is bool b) rowBuf[ci] = b ? "true" : "false";
                                                else if (val is int || val is long || val is short || val is byte || val is uint || val is ulong || val is ushort || val is sbyte)
                                                    rowBuf[ci] = val.ToString() ?? "0";
                                                else if (val is float || val is double || val is decimal)
                                                    rowBuf[ci] = Math.Round(double.Parse(val.ToString()!), 4).ToString() ?? "0";
                                                else rowBuf[ci] = val?.ToString() ?? "NULL";
                                                token.ThrowIfCancellationRequested();
                                            }
                                            valuesArray[ri] = $"({string.Join(",", rowBuf, 0, colCount)})";
                                        }
                                        catch (Exception ex)
                                        {
#if DEBUG
                                            Debug.WriteLine($"Error processing row {ri} in table {tbName}: {ex.Message}");
#endif
                                            action?.Invoke($"Error processing row {ri} in table {tbName}: {ex.Message}");
                                            continue;
                                        }
                                        finally
                                        {
                                            ArrayPool<string>.Shared.Return(rowBuf);
                                        }
                                        token.ThrowIfCancellationRequested();
                                    }

                                    sb.Append(string.Join(" ", valuesArray, 0, rowCount));
#if DEBUG
                                    Debug.WriteLine($"sb length:{sb.Length}");
#endif
                                    await targetDb.Ado.ExecuteCommandAsync(sb.ToString());
                                }
                                catch (Exception ex)
                                {
#if DEBUG
                                    Debug.WriteLine($"Error writing to target table {tbName}: {ex.Message}");
#endif
                                    action?.Invoke($"Error writing to target table {tbName}: {ex.Message}");
                                    if (ex.Message.Contains("SQL statement too long") || ex.Message.Contains("0x219"))
                                    {
                                        tableBatchSize -= 500;
                                    }
                                    if (ex is OperationCanceledException)
                                        throw;
                                    else
                                        continue;
                                }
                                finally
                                {
                                    ArrayPool<string>.Shared.Return(valuesArray);
                                    sb.Clear();
                                }

                                // Update cursor
                                if (dtData.Rows[rowCount - 1]["ts"] is DateTime lastTs)
                                {
                                    tableCursor = lastTs;
                                }

                                totalStableMigrated += rowCount;
                                var migratePercent = (totalStableMigrated * 100.0) / totalStableCount;
                                action?.Invoke($"Migrated {totalStableMigrated}/{totalStableCount} {migratePercent:N2}% rows for stable {stableName} (current: {tbName})");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Debug.WriteLine($"Error migrating table {tbName}: {ex.Message}");
#endif
                        action?.Invoke($"Error migrating table {tbName}: {ex.Message}");
                        if (ex is OperationCanceledException)
                            throw;
                        else
                            continue;
                    }
                }
            }
        }

        /// <summary>
        /// 清空目标数据库中所有子表的数据（保留表结构）：
        /// 1. 遍历数据库中所有超级表。
        /// 2. 对于每个超级表，查找所有子表。
        /// 3. 对每个子表执行 DELETE FROM 语句删除全部数据。
        /// 
        /// Clears all data from sub-tables in the target database (table structures are preserved):
        /// 1. Iterates through all super tables in the database.
        /// 2. For each super table, finds all sub-tables.
        /// 3. Executes DELETE FROM on each sub-table to remove all data.
        /// </summary>
        /// <param name="connectionString">目标数据库连接字符串 / Target database connection string.</param>
        /// <param name="dbName">目标数据库名称 / Target database name.</param>
        /// <param name="action">进度回调方法 / Progress callback action.</param>
        public async Task ClearDataAsync(string connectionString, string dbName, Action<string>? action)
        {
            var config = new ConnectionConfig() { ConnectionString = connectionString, DbType = SqlSugar.DbType.TDengine, IsAutoCloseConnection = true };
            using var db = new SqlSugarClient(config);
            db.Ado.Open();
            if (!string.IsNullOrEmpty(dbName))
            {
                await db.Ado.ExecuteCommandAsync($"USE {dbName}");
            }

            var dtStables = await db.Ado.GetDataTableAsync("SHOW STABLES");
            int stableCount = dtStables.Rows.Count;
            for (int i = 0; i < stableCount; i++)
            {
                var stableRow = dtStables.Rows[i];
                string stableName = stableRow["stable_name"]?.ToString() ?? "";
                if (string.IsNullOrEmpty(stableName)) continue;

                var dtChildTables = await db.Ado.GetDataTableAsync(
                    $"SELECT table_name FROM INFORMATION_SCHEMA.INS_TABLES WHERE db_name = '{dbName}' and stable_name = '{stableName}'");

                int childCount = dtChildTables.Rows.Count;
                for (int j = 0; j < childCount; j++)
                {
                    var childRow = dtChildTables.Rows[j];
                    string tbName = childRow["table_name"]?.ToString() ?? "";
                    if (string.IsNullOrEmpty(tbName)) continue;

                    try
                    {
                        // Delete all data. In TDengine 3.0, DELETE FROM table usually works. 
                        // If a WHERE is required, using a far future timestamp.
                        await db.Ado.ExecuteCommandAsync($"DELETE FROM {tbName}");
                        action?.Invoke($"Cleared data for table: {tbName}");
                        await Task.Delay(50);
                    }
                    catch (Exception ex)
                    {
                        action?.Invoke($"Error clearing table {tbName}: {ex.Message}");
                    }
                }
            }
        }
    }
}
