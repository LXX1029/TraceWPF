using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TraceWPF.Domain.Models;

namespace TraceWPF.Domain.Interfaces
{
    public interface IDataMigrationService
    {
        Task CreateDatabaseAsync(string connectionString, string dbName, DataBaseParam dataBaseParam);
        Task DeleteDatabaseAsync(string connectionString, string dbName);
        Task MigrateSchemaAsync(string sourceConn, string targetConn, string sourceDbName, string targetDbName);
        Task MigrateDataAsync(string sourceConn, string targetConn, string sourceDbName, string targetDbName, DateTime startDateTime, DateTime endDateTime, int tableBatchSize, CancellationToken token, Action<string>? action);
        Task ClearDataAsync(string connectionString, string dbName, Action<string>? action);
    }
}
