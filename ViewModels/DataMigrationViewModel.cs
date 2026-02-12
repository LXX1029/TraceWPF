using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TraceWPF.DI;
using TraceWPF.Domain.Interfaces;
using TraceWPF.Domain.Models;

namespace TraceWPF.ViewModels
{
    public partial class DataMigrationViewModel : ObservableObject, ITransient
    {
        private readonly IDataMigrationService? _migrationService;

        public DataMigrationViewModel(IDataMigrationService migrationService)
        {
            _migrationService = migrationService;
            TimePeriodOptions = new List<TimePeriodItem>
            {
                 new TimePeriodItem { DisplayName = "近1天", Days = 1 },
                new TimePeriodItem { DisplayName = "近2天", Days = 2 },
                new TimePeriodItem { DisplayName = "近7天", Days = 7 },
                new TimePeriodItem { DisplayName = "近半个月", Days = 15 },
                new TimePeriodItem { DisplayName = "近一个月", Days = 30 },
                new TimePeriodItem { DisplayName = "近三个月", Days = 90 },
                new TimePeriodItem { DisplayName = "近半年", Days = 180 },
                new TimePeriodItem { DisplayName = "近一年", Days = 365 },
                new TimePeriodItem { DisplayName = "全部数据", Days = 0 }
            };
            SelectedTimePeriod = TimePeriodOptions[0];
            var logListBox = System.Windows.Application.Current.MainWindow?.FindName("LogListBox") as System.Windows.Controls.ListBox;
            this.Logs.CollectionChanged += async (s, e) =>
            {
                try
                {
                    var count = Interlocked.Increment(ref itemCount);
                    //if (count == 20)
                    {
                        // 自动滚动到最新日志
                        await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            var mainWindow = System.Windows.Application.Current.MainWindow;
                            if (mainWindow != null)
                            {
                                logListBox ??= mainWindow.FindName("LogListBox") as System.Windows.Controls.ListBox;
                                if (logListBox != null && logListBox.Items.Count > 0)
                                {
                                    logListBox.ScrollIntoView(logListBox.Items[logListBox.Items.Count - 1]);
                                }
                            }
                        }, System.Windows.Threading.DispatcherPriority.ContextIdle);
                        //Interlocked.Exchange(ref itemCount, 0);
                    }
                }
                finally
                {

                }


            };
        }

        public DataMigrationViewModel()
        {
            TimePeriodOptions = new List<TimePeriodItem>
            {
                new TimePeriodItem { DisplayName = "近半个月", Days = 15 },
                new TimePeriodItem { DisplayName = "近一个月", Days = 30 },
                new TimePeriodItem { DisplayName = "近三个月", Days = 90 },
                new TimePeriodItem { DisplayName = "近半年", Days = 180 },
                new TimePeriodItem { DisplayName = "近一年", Days = 365 },
                new TimePeriodItem { DisplayName = "全部数据", Days = 0 }
            };
            SelectedTimePeriod = TimePeriodOptions[0];
        }
        private System.Windows.Controls.ListBox logListBox { get; set; }
        private int itemCount = 0;

        public List<TimePeriodItem> TimePeriodOptions { get; }

        [ObservableProperty]
        public DateTime startDate = DateTime.Now.AddDays(-7);

        [ObservableProperty]
        public DateTime endDate = DateTime.Now;

        [ObservableProperty]
        private TimePeriodItem? selectedTimePeriod;

        [ObservableProperty]
        private string sourceConnectionString = "Host=192.168.1.20;Port=6030;Username=root;Password=kkny.com888;Database=onlineanalysisv20;";

        [ObservableProperty]
        private string targetConnectionString = "Host=192.168.1.156;Port=6030;Username=root;Password=kkny.com888;";

        [ObservableProperty]
        private string targetDbName = "onlineanalysisv20";

        //[ObservableProperty]
        //private int keepDays = 90;

        //[ObservableProperty]
        //private int duration = 7;

        //[ObservableProperty]
        //private int cachesize = 512;

        //[ObservableProperty]
        //private int maxrows = 32768;

        [ObservableProperty]
        private DataBaseParam dataBaseParam = new();

        [ObservableProperty]
        private int tableBatchSize = 5000;

        [ObservableProperty]
        private ObservableCollection<string> logs = new();

        [ObservableProperty]
        private bool isCanInteration = true;

        private const string ServerTDengineInstallCfgPath = "C:\\TDengine\\cfg\\taos.cfg";
        [RelayCommand]
        private async Task CreateDatabase()
        {
            try
            {
                AppendLog($"Creating target database '{TargetDbName}'...");
                await Task.Delay(100);
                await _migrationService!.CreateDatabaseAsync(TargetConnectionString, TargetDbName, this.DataBaseParam);
                //var taosdbPath = "D:\\taosdb";
                //if (!Directory.Exists(taosdbPath))
                //    Directory.CreateDirectory(taosdbPath);
                //if (File.Exists(ServerTDengineInstallCfgPath))
                //{
                //    var tdengineCfgLinesList = await File.ReadAllLinesAsync(ServerTDengineInstallCfgPath);
                //    tdengineCfgLinesList[37] = $"dataDir {taosdbPath}";
                //    await File.WriteAllLinesAsync(ServerTDengineInstallCfgPath, tdengineCfgLinesList);
                //}

                AppendLog("Database created successfully.");
            }
            catch (Exception ex)
            {
                AppendLog($"Error creating database: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task DeleteDatabase()
        {
            if (System.Windows.MessageBox.Show($"确定要删除 '{TargetDbName}' 数据库吗?",
                "确认删除",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning) != System.Windows.MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                this.Logs.Clear();
                AppendLog($"Deleting target database '{TargetDbName}'...");
                await Task.Delay(100);
                await _migrationService!.DeleteDatabaseAsync(TargetConnectionString, TargetDbName);
                AppendLog("Database deleted successfully.");

            }
            catch (Exception ex)
            {
                AppendLog($"Error deleting database: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task ClearData()
        {
            if (System.Windows.MessageBox.Show($"确定要清空 '{TargetDbName}' 数据库中的所有数据吗? (结构将保留)",
                "确认清空数据",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning) != System.Windows.MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                this.IsCanInteration = false;
                AppendLog($"Clearing data in database '{TargetDbName}'...");

                if (!TargetConnectionString.Contains("DataBase", StringComparison.OrdinalIgnoreCase))
                {
                    TargetConnectionStringTemp = TargetConnectionString + $";DataBase={TargetDbName}";
                }

                await _migrationService!.ClearDataAsync(TargetConnectionStringTemp, TargetDbName, (msg) =>
                {
                    AppendLog(msg);
                });
                AppendLog("Data cleared successfully.");
            }
            catch (Exception ex)
            {
                AppendLog($"Error clearing data: {ex.Message}");
            }
            finally
            {
                this.IsCanInteration = true;
            }
        }

        private string TargetConnectionStringTemp = string.Empty;
        [RelayCommand]
        private async Task StartMigration()
        {
            try
            {
                AppendLog("Starting migration process...");

                var sourceDb = GetDbName(SourceConnectionString);
                if (string.IsNullOrEmpty(sourceDb))
                {
                    AppendLog("Error: Could not determine Source Database name from Connection String. Please include 'DATABASE=...'");
                    return;
                }

                // Schema
                AppendLog("Migrating Schema (Super Tables and Sub Tables)...");
                if (!TargetConnectionString.Contains("DataBase", StringComparison.OrdinalIgnoreCase))
                {
                    TargetConnectionStringTemp = TargetConnectionString + $";DataBase={TargetDbName}";
                }

                await _migrationService!.MigrateSchemaAsync(SourceConnectionString, TargetConnectionStringTemp, sourceDb, TargetDbName);
                AppendLog("Schema migration completed.");
            }
            catch (Exception ex)
            {
                AppendLog($"Error during migration: {ex.Message}");
            }
        }

        private CancellationTokenSource _migrationDataTokenSource;
        private CancellationToken _migrationDataToken;
        [RelayCommand]
        private async Task StartMigrationData()
        {
            try
            {
                //await this.ClearData();
                AppendLog("Starting migration process...");
                this.IsCanInteration = false;
                _migrationDataTokenSource = new CancellationTokenSource();
                _migrationDataToken = _migrationDataTokenSource.Token;
                var sourceDb = GetDbName(SourceConnectionString);
                if (string.IsNullOrEmpty(sourceDb))
                {
                    AppendLog("Error: Could not determine Source Database name from Connection String. Please include 'DATABASE=...'");
                    return;
                }
                if (this.EndDate < this.StartDate)
                {
                    AppendLog("Error: EndDate cannot be earlier than StartDate. Please check your date settings.");
                    MessageBox.Show("结束时间不能早于开始时间，请检查日期设置。", "日期错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                int filterDays = Convert.ToInt32(this.EndDate.Subtract(this.StartDate).TotalDays);
                this.StartDate = DateTime.Parse(StartDate.ToString("yyyy-MM-dd 00:00:00"));
                this.EndDate = DateTime.Parse(EndDate.ToString("yyyy-MM-dd 23:59:59"));
                AppendLog($"Source DB: {sourceDb}, Target DB: {TargetDbName}, Filter: 近{filterDays}天");
                if (!TargetConnectionString.Contains("DataBase", StringComparison.OrdinalIgnoreCase))
                {
                    TargetConnectionStringTemp = TargetConnectionString + $";DataBase={TargetDbName}";
                }

                // Data
                AppendLog("Migrating Data...");
                await _migrationService!.MigrateDataAsync(SourceConnectionString, TargetConnectionStringTemp, sourceDb, TargetDbName, this.StartDate, this.EndDate, this.TableBatchSize, _migrationDataToken, (msg) =>
                {
                    AppendLog(msg);
                });
                AppendLog("Data migration completed successfully.");
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException)
                {
                    AppendLog("Data migration has been canceled.");
                }
                else
                {
                    AppendLog($"Error during migration: {ex.Message}");
                }
            }
            finally
            {
                this.IsCanInteration = true;
                this._migrationDataTokenSource?.Dispose();
            }
        }

        [RelayCommand]
        private void MigrationEnd()
        {
            try
            {
                this.IsCanInteration = true;
                this._migrationDataTokenSource?.Cancel();

            }
            catch (Exception ex)
            {
                AppendLog($"Error during cancel migration: {ex.Message}");
            }
        }

        [RelayCommand]
        private void ClearLog()
        {
            this.Logs.Clear();
        }
        private void AppendLog(string message)
        {
            if (Logs.Count > 5000)
            {
                Logs.Clear();
            }
            Logs.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}{Environment.NewLine}");
        }

        private string GetDbName(string connStr)
        {
            var parts = connStr.Split(';');
            foreach (var part in parts)
            {
                var kv = part.Split('=');
                if (kv.Length == 2)
                {
                    if (kv[0].Trim().Equals("DATABASE", StringComparison.OrdinalIgnoreCase))
                    {
                        return kv[1].Trim();
                    }
                }
            }
            return "";
        }
    }

    public class TimePeriodItem
    {
        public string DisplayName { get; set; } = "";
        public int Days { get; set; }
        public override string ToString() => DisplayName;
    }
}
