using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.Configuration;

namespace TraceWPF;

/// <summary>
/// 应用程序入口类，负责应用启动时的依赖注入配置和主窗口初始化。
/// Application entry class responsible for configuring dependency injection and initializing the main window on startup.
/// </summary>
public partial class App : System.Windows.Application
{
    /// <summary>
    /// 应用程序启动时的回调方法。
    /// 1. 读取 appsettings.json 获取数据库连接字符串。
    /// 2. 展开环境变量并确保 SQLite 数据目录存在。
    /// 3. 注册 SqlSugarClient 单例及通过 AutoRegister 自动注册 Views、ViewModels、UseCases、Services。
    /// 4. 构建 DI 容器并显示主窗口 (DataMigrationView)。
    /// 
    /// Called when the application starts.
    /// 1. Reads appsettings.json to obtain the database connection string.
    /// 2. Expands environment variables and ensures the SQLite data directory exists.
    /// 3. Registers a SqlSugarClient singleton and auto-registers Views, ViewModels, UseCases, and Services via AutoRegister.
    /// 4. Builds the DI container and shows the main window (DataMigrationView).
    /// </summary>
    /// <param name="e">启动事件参数 / Startup event arguments.</param>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        var config = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
            .SetBasePath(System.AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .Build();
        var rawConn = config.GetConnectionString("Default") ?? "";
        var connStr = System.Environment.ExpandEnvironmentVariables(rawConn);
        var dsPrefix = "Data Source=";
        if (connStr.StartsWith(dsPrefix, System.StringComparison.OrdinalIgnoreCase))
        {
            var path = connStr.Substring(dsPrefix.Length);
            var dir = System.IO.Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir)) System.IO.Directory.CreateDirectory(dir);
        }

        Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddSingleton<SqlSugar.SqlSugarClient>(services, _ =>
            Infrastructure.Persistence.SqlSugarProvider.CreateClient(connStr));

        TraceWPF.DI.ServiceCollectionExtensions.AutoRegister(
            services,
            typeof(App).Assembly,
            "TraceWPF",
            "TraceWPF.Views",
            "TraceWPF.ViewModels",
            "TraceWPF.Application.UseCases",
            "TraceWPF.Infrastructure.Services"
        );

        var provider = Microsoft.Extensions.DependencyInjection.ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(services);

        //var mainWindow = (Views.MainWindow)provider.GetService(typeof(Views.MainWindow))!;
        //mainWindow.Show();
        //var mainWindow = (Views.DataMigrationView)provider.GetService(typeof(Views.DataMigrationView))!;
        //mainWindow.Show();
        var mainWindow = (GaugeTestWindow)provider.GetService(typeof(GaugeTestWindow))!;
        mainWindow.Show();
    }
}
