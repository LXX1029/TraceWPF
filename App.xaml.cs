using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.Configuration;

namespace TraceWPF;

public partial class App : System.Windows.Application
{
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
            "TraceWPF.Views",
            "TraceWPF.ViewModels",
            "TraceWPF.Application.UseCases",
            "TraceWPF.Infrastructure.Services"
        );

        var provider = Microsoft.Extensions.DependencyInjection.ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(services);

        //var mainWindow = (Views.MainWindow)provider.GetService(typeof(Views.MainWindow))!;
        //mainWindow.Show();
        var mainWindow = (Views.DataMigrationView)provider.GetService(typeof(Views.DataMigrationView))!;
        mainWindow.Show();
    }
}
