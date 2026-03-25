using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TraceWPF.Domain.Models;

namespace TraceWPF.Domain.Interfaces
{
    /// <summary>
    /// 数据迁移服务接口，定义 TDengine 数据库迁移操作的契约，包括创建/删除数据库、迁移表结构、迁移数据和清空数据。
    /// Data migration service interface that defines the contract for TDengine database migration operations, including creating/deleting databases, migrating schemas, migrating data, and clearing data.
    /// </summary>
    public interface IDataMigrationService
    {
        /// <summary>
        /// 异步创建目标 TDengine 数据库，使用指定的数据库参数（KEEP、DURATION、CACHESIZE 等）。
        /// Asynchronously creates the target TDengine database with specified parameters (KEEP, DURATION, CACHESIZE, etc.).
        /// </summary>
        /// <param name="connectionString">目标数据库连接字符串 / Target database connection string.</param>
        /// <param name="dbName">目标数据库名称 / Target database name.</param>
        /// <param name="dataBaseParam">数据库创建参数 / Database creation parameters.</param>
        Task CreateDatabaseAsync(string connectionString, string dbName, DataBaseParam dataBaseParam);

        /// <summary>
        /// 异步删除指定的 TDengine 数据库。
        /// Asynchronously drops the specified TDengine database.
        /// </summary>
        /// <param name="connectionString">目标数据库连接字符串 / Target database connection string.</param>
        /// <param name="dbName">要删除的数据库名称 / The name of the database to drop.</param>
        Task DeleteDatabaseAsync(string connectionString, string dbName);

        /// <summary>
        /// 异步迁移表结构：从源数据库读取所有超级表（STable）和子表（Sub Table）的定义，并在目标数据库中重建。
        /// Asynchronously migrates schema: reads all super table and sub-table definitions from the source database and recreates them in the target database.
        /// </summary>
        /// <param name="sourceConn">源数据库连接字符串 / Source database connection string.</param>
        /// <param name="targetConn">目标数据库连接字符串 / Target database connection string.</param>
        /// <param name="sourceDbName">源数据库名称 / Source database name.</param>
        /// <param name="targetDbName">目标数据库名称 / Target database name.</param>
        Task MigrateSchemaAsync(string sourceConn, string targetConn, string sourceDbName, string targetDbName, Action<string>? action);

        /// <summary>
        /// 异步迁移数据：按子表逐一从源数据库读取指定时间范围内的数据，并批量写入目标数据库。
        /// 支持通过 CancellationToken 取消操作，并通过 Action 回调报告迁移进度。
        /// 
        /// Asynchronously migrates data: reads data within the specified time range from the source database sub-table by sub-table,
        /// and batch-inserts into the target database. Supports cancellation via CancellationToken and progress reporting via Action callback.
        /// </summary>
        /// <param name="sourceConn">源数据库连接字符串 / Source database connection string.</param>
        /// <param name="targetConn">目标数据库连接字符串 / Target database connection string.</param>
        /// <param name="sourceDbName">源数据库名称 / Source database name.</param>
        /// <param name="targetDbName">目标数据库名称 / Target database name.</param>
        /// <param name="startDateTime">数据过滤的开始时间 / Start datetime for data filtering.</param>
        /// <param name="endDateTime">数据过滤的结束时间 / End datetime for data filtering.</param>
        /// <param name="tableBatchSize">每批次迁移的行数 / Number of rows per migration batch.</param>
        /// <param name="token">取消令牌 / Cancellation token.</param>
        /// <param name="action">进度回调方法 / Progress callback action.</param>
        Task MigrateDataAsync(string sourceConn, string targetConn, string sourceDbName, string targetDbName, DateTime startDateTime, DateTime endDateTime, int tableBatchSize, CancellationToken token, Action<string>? action);

        /// <summary>
        /// 异步清空目标数据库中所有子表的数据（保留表结构）。
        /// Asynchronously clears all data from sub-tables in the target database (table structures are preserved).
        /// </summary>
        /// <param name="connectionString">目标数据库连接字符串 / Target database connection string.</param>
        /// <param name="dbName">目标数据库名称 / Target database name.</param>
        /// <param name="action">进度回调方法 / Progress callback action.</param>
        Task ClearDataAsync(string connectionString, string dbName, Action<string>? action);
    }
}
