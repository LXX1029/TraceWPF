using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TDengine.Driver.Client;
using TraceWPF.DI;
using TDengine.Driver;
using System.Diagnostics;
using TraceWPF.Domain.Interfaces;

namespace TraceWPF
{
    /// <summary>
    /// TestClusterWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TestClusterWindow : Window, ITransient
    {
        public TestClusterWindow(IDataMigrationService dataMigrationService)
        {
            InitializeComponent();
            this._dataMigrationService = dataMigrationService;
        }
        /// <summary>
        /// WebSocket 连接字符串示例，适用于通过 Nginx 代理连接 TDengine 集群的场景。
        /// </summary>
        string connectionString = "protocol=WebSocket;host=localhost;port=8080;useSSL=false;username=root;password=taosdata";

        /// <summary>
        /// 原生连接字符串示例，适用于直接连接 TDengine 集群的场景（不通过 Nginx 代理）。
        /// 只需要指定 firstEp
        /// </summary>
        string nativeConnectionString = "Host=192.168.1.156;Port=6030;Username=root;Password=taosdata;";
        private readonly IDataMigrationService _dataMigrationService;

        private async void btnWrite_Click(object sender, RoutedEventArgs e)
        {
            // Nginx 代理地址（你的 localhost:8080）
            // 注意：WebSocket 连接字符串以 Host= 开头

            try
            {
                //string connectionString = "host=192.168.1.156;port=6041;username=root;password=taosdata";
                var builder = new ConnectionStringBuilder(connectionString);
                //{
                //    Host = "localhost",
                //    Port = 8080,
                //    Username = "root",
                //    Password = "taosdata",
                //    AutoReconnect = true,
                //};
                // 1. 建立异步连接
                using (var client = DbDriver.Open(builder))
                {
                    Debug.WriteLine("成功连接到 TDengine 集群 (通过 Nginx)");

                    // 2. 准备环境 (异步执行 SQL)
                    //await Task.Run(()=> client.Exec("CREATE DATABASE IF NOT EXISTS demo_cs PRECISION 'ms' REPLICA 3"));
                    await Task.Run(() => client.Exec("USE demo_cs"));

                    // 创建一张超级表 (STable) 和子表
                    //await Task.Run(() => client.Exec("CREATE STABLE IF NOT EXISTS meters (ts TIMESTAMP, current FLOAT, voltage INT) TAGS (location BINARY(64), groupid INT)"));
                    //await Task.Run(() => client.Exec("CREATE TABLE IF NOT EXISTS d1001 USING meters TAGS ('Beijing', 1)"));

                    Debug.WriteLine("数据库与表结构准备就绪。");

                    // 3. 异步写入数据
                    // 模拟 10 条数据
                    DateTime now = DateTime.UtcNow;
                    List<string> sqlBatch = new List<string>();
                    var sql = "insert into demo_cs.d1001 values ";
                    sqlBatch.Add(sql);
                    for (int i = 0; i < 10; i++)
                    {
                        long ts = ((DateTimeOffset)now.AddSeconds(i)).ToUnixTimeMilliseconds();
                        float current = 10.5f + i;
                        int voltage = 220 + i;
                        if (i == 0)
                            sqlBatch.Add($"({ts}, {current}, {voltage})");
                        else
                            sqlBatch.Add($",({ts}, {current}, {voltage})");
                    }

                    // 合并成一个大 Batch 执行
                    string finalSql = string.Join(" ", sqlBatch);
                    await Task.Run(() =>
                    {
                        client.Exec(finalSql);
                    });

                    Debug.WriteLine($"成功异步写入 {sqlBatch.Count} 条记录。");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误: {ex.Message}");
            }
        }

        private async void btnRead_Click(object sender, RoutedEventArgs e)
        {
            var builder = new ConnectionStringBuilder(connectionString);
            //{
            //    Host = "localhost",
            //    Port = 8080,
            //    Username = "root",
            //    Password = "taosdata",
            //    AutoReconnect = true,
            //};
            // 1. 建立异步连接
            using (var client = DbDriver.Open(builder))
            {
                Debug.WriteLine("成功连接到 TDengine 集群 (通过 Nginx)");

                await Task.Run(() => client.Exec("USE demo_cs"));

                using (var rows = client.Query("SELECT * FROM d1001 ORDER BY ts DESC LIMIT 5"))
                {
                    Debug.WriteLine("\n最近 5 条记录：");
                    while (rows.Read())
                    {
                        // 通用且稳健的读取方式：使用 GetValue 并做转换
                        object tsObj = rows.GetValue(0);
                        DateTime ts;
                        if (tsObj is DateTime dt)
                        {
                            ts = dt;
                        }
                        else if (tsObj is long tsLong)
                        {
                            ts = DateTimeOffset.FromUnixTimeMilliseconds(tsLong).DateTime;
                        }
                        else if (tsObj is int tsInt)
                        {
                            ts = DateTimeOffset.FromUnixTimeMilliseconds(tsInt).DateTime;
                        }
                        else
                        {
                            ts = Convert.ToDateTime(tsObj);
                        }

                        double curr = Convert.ToDouble(rows.GetValue(1));
                        int volt = Convert.ToInt32(rows.GetValue(2));
                        Debug.WriteLine($"{ts:yyyy-MM-dd HH:mm:ss.fff} | 电流: {curr}A | 电压: {volt}V");
                    }
                }
            }
        }

        private async void btnCreateDb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataBaseParam param = new DataBaseParam
                {
                    Buffer = 512,
                };
                await this._dataMigrationService.CreateDatabaseAsync(nativeConnectionString, "demo_cs1", param);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"创建数据库失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
