using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TraceWPF.DI;
using TraceWPF.Domain.Interfaces;
using TraceWPF.Domain.Models;

namespace TraceWPF.ViewModels
{
    /// <summary>
    /// 数据迁移视图模型，管理 TDengine 数据库之间的数据迁移操作，包括：
    /// 创建/删除数据库、迁移表结构、迁移数据、清空数据等。
    /// 
    /// Data migration ViewModel that manages TDengine database migration operations, including:
    /// creating/deleting databases, migrating table schemas, migrating data, and clearing data.
    /// </summary>
    public partial class DataMigrationViewModel : ObservableObject, ITransient
    {
        /// <summary>
        /// 数据迁移服务实例，通过 DI 注入。
        /// Data migration service instance injected via DI.
        /// </summary>
        private readonly IDataMigrationService? _migrationService;

        /// <summary>
        /// 带依赖注入的构造函数。初始化时间周期选项列表、默认选中第一项，
        /// 并订阅 Logs 集合变更事件以实现自动滚动到最新日志。
        /// 
        /// DI constructor. Initializes time period options, selects the first item by default,
        /// and subscribes to Logs collection changes to auto-scroll to the latest log entry.
        /// </summary>
        /// <param name="migrationService">数据迁移服务接口 / Data migration service interface.</param>
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

        /// <summary>
        /// 默认无参构造函数（设计器使用），初始化时间周期选项列表。
        /// Default parameterless constructor (used by designer), initializes time period options.
        /// </summary>
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

        /// <summary>
        /// 日志列表控件引用（用于自动滚动）。
        /// Reference to the log ListBox control (used for auto-scrolling).
        /// </summary>
        private System.Windows.Controls.ListBox logListBox { get; set; }

        /// <summary>
        /// 日志项计数器（用于控制滚动频率）。
        /// Log item counter (used to control scroll frequency).
        /// </summary>
        private int itemCount = 0;

        /// <summary>
        /// 时间周期选项列表，供用户选择迁移数据的时间范围。
        /// Time period options list for the user to select the data migration time range.
        /// </summary>
        public List<TimePeriodItem> TimePeriodOptions { get; }

        /// <summary>
        /// 迁移数据的开始日期，默认为7天前。
        /// Start date for data migration, defaults to 7 days ago.
        /// </summary>
        [ObservableProperty]
        public DateTime startDate = DateTime.Now.AddDays(-7);

        /// <summary>
        /// 迁移数据的结束日期，默认为当前时间。
        /// End date for data migration, defaults to the current time.
        /// </summary>
        [ObservableProperty]
        public DateTime endDate = DateTime.Now;

        /// <summary>
        /// 当前选中的时间周期选项。
        /// The currently selected time period option.
        /// </summary>
        [ObservableProperty]
        private TimePeriodItem? selectedTimePeriod;

        /// <summary>
        /// 源数据库连接字符串（TDengine）。
        /// Source database connection string (TDengine).
        /// </summary>
        [ObservableProperty]
        private string sourceConnectionString = "Host=192.168.1.20;Port=6030;Username=root;Password=kkny.com888;Database=onlineanalysisv20;";

        /// <summary>
        /// 目标数据库连接字符串（TDengine）。
        /// Target database connection string (TDengine).
        /// </summary>
        [ObservableProperty]
        private string targetConnectionString = "Host=192.168.1.156;Port=6030;Username=root;Password=kkny.com888;";

        /// <summary>
        /// 目标数据库名称。
        /// Target database name.
        /// </summary>
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

        /// <summary>
        /// 数据库参数配置对象，包含 KEEP、DURATION、CACHESIZE 等 TDengine 建库参数。
        /// Database parameter configuration object containing TDengine database creation parameters like KEEP, DURATION, CACHESIZE, etc.
        /// </summary>
        [ObservableProperty]
        private DataBaseParam dataBaseParam = new();

        /// <summary>
        /// 每批次迁移的数据行数。
        /// Number of data rows to migrate per batch.
        /// </summary>
        [ObservableProperty]
        private int tableBatchSize = 5000;

        /// <summary>
        /// 操作日志消息集合，用于在界面上展示迁移进度信息。
        /// Collection of operation log messages displayed on the UI to show migration progress.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<string> logs = new();

        /// <summary>
        /// 是否允许用户交互（迁移运行期间禁用按钮）。
        /// Whether user interaction is allowed (buttons are disabled during migration).
        /// </summary>
        [ObservableProperty]
        private bool isCanInteration = true;

        /// <summary>
        /// TDengine 服务端配置文件路径常量。
        /// Constant path to the TDengine server configuration file.
        /// </summary>
        private const string ServerTDengineInstallCfgPath = "C:\\TDengine\\cfg\\taos.cfg";

        /// <summary>
        /// 创建目标数据库命令：使用 DataBaseParam 中的参数在目标连接上创建 TDengine 数据库。
        /// Create database command: creates a TDengine database on the target connection using parameters from DataBaseParam.
        /// </summary>
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

        /// <summary>
        /// 删除目标数据库命令：弹出确认对话框后删除指定的 TDengine 数据库。
        /// Delete database command: shows a confirmation dialog and then drops the specified TDengine database.
        /// </summary>
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

        /// <summary>
        /// 清空数据命令：弹出确认对话框后，逐表删除目标数据库中的所有数据（保留表结构）。
        /// Clear data command: shows a confirmation dialog and then deletes all data in the target database table by table (table structures are preserved).
        /// </summary>
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

        /// <summary>
        /// 带有目标数据库名称拼接后的临时连接字符串。
        /// Temporary connection string with the target database name appended.
        /// </summary>
        private string TargetConnectionStringTemp = string.Empty;

        /// <summary>
        /// 迁移表结构命令：从源数据库读取所有超级表和子表的定义，并在目标数据库中重建。
        /// Migrate schema command: reads all super table and sub-table definitions from the source database and recreates them in the target database.
        /// </summary>
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

                await _migrationService!.MigrateSchemaAsync(SourceConnectionString, TargetConnectionStringTemp, sourceDb, TargetDbName, msg =>
                {
                    AppendLog(msg);
                });
                AppendLog("Schema migration completed.");
            }
            catch (Exception ex)
            {
                AppendLog($"Error during migration: {ex.Message}");
            }
        }

        /// <summary>
        /// 数据迁移取消令牌源，用于支持取消正在进行的数据迁移操作。
        /// Cancellation token source for supporting cancellation of an ongoing data migration.
        /// </summary>
        private CancellationTokenSource _migrationDataTokenSource;

        /// <summary>
        /// 数据迁移取消令牌。
        /// Cancellation token for data migration.
        /// </summary>
        private CancellationToken _migrationDataToken;

        /// <summary>
        /// 开始数据迁移命令：验证日期范围后，按子表逐一从源数据库读取数据并批量写入目标数据库。
        /// 支持通过 CancellationToken 取消操作，迁移期间禁用 UI 交互。
        /// 
        /// Start data migration command: validates the date range, then reads data from the source database
        /// sub-table by sub-table and batch-inserts into the target database.
        /// Supports cancellation via CancellationToken; UI interaction is disabled during migration.
        /// </summary>
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

        /// <summary>
        /// 停止数据迁移命令：触发 CancellationToken 取消信号以终止正在运行的数据迁移任务，并恢复 UI 交互。
        /// Stop migration command: triggers the CancellationToken cancellation signal to terminate the running migration task, and re-enables UI interaction.
        /// </summary>
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

        /// <summary>
        /// 清空日志命令：清除界面上所有的操作日志消息。
        /// Clear log command: clears all operation log messages from the UI.
        /// </summary>
        [RelayCommand]
        private void ClearLog()
        {
            this.Logs.Clear();
        }

        /// <summary>
        /// 追加一条带时间戳的日志消息到 Logs 集合。当日志数量超过 5000 条时自动清空以防止内存溢出。
        /// Appends a timestamped log message to the Logs collection. Automatically clears when the count exceeds 5000 to prevent memory overflow.
        /// </summary>
        /// <param name="message">要追加的日志消息 / The log message to append.</param>
        private void AppendLog(string message)
        {
            if (Logs.Count > 5000)
            {
                Logs.Clear();
            }
            Logs.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}{Environment.NewLine}");
        }

        /// <summary>
        /// 从连接字符串中解析出 DATABASE 参数的值。
        /// Parses the DATABASE parameter value from a connection string.
        /// </summary>
        /// <param name="connStr">数据库连接字符串 / The database connection string.</param>
        /// <returns>数据库名称，如果未找到则返回空字符串 / The database name, or empty string if not found.</returns>
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

    /// <summary>
    /// 时间周期选项数据模型，用于下拉框中展示时间范围选项（如"近7天"、"近30天"等）。
    /// Time period option data model, used to display time range options in a dropdown (e.g., "Last 7 days", "Last 30 days").
    /// </summary>
    public class TimePeriodItem
    {
        /// <summary>
        /// 显示名称（如"近7天"）。
        /// Display name (e.g., "Last 7 days").
        /// </summary>
        public string DisplayName { get; set; } = "";

        /// <summary>
        /// 对应的天数（0 表示全部数据）。
        /// Corresponding number of days (0 means all data).
        /// </summary>
        public int Days { get; set; }

        /// <summary>
        /// 重写 ToString 方法返回 DisplayName，用于下拉框显示。
        /// Overrides ToString to return DisplayName for dropdown display.
        /// </summary>
        /// <returns>显示名称 / The display name.</returns>
        public override string ToString() => DisplayName;
    }
}
