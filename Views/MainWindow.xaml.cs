namespace TraceWPF.Views
{
    using System.Windows;
    using TraceWPF.DI;

    /// <summary>
    /// 主窗口代码隐藏，承载主界面视图。
    /// Code-behind for the main window, hosts the primary UI view.
    /// </summary>
    public partial class MainWindow : Window, ISingleton
    {
        /// <summary>
        /// 默认构造函数，仅初始化 XAML 组件（设计器使用）。
        /// Default constructor that only initializes XAML components (used by designer).
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 带 ViewModel 的构造函数，通过 DI 注入 MainViewModel 并设置为 DataContext。
        /// Constructor with ViewModel injection; sets the injected MainViewModel as DataContext.
        /// </summary>
        /// <param name="vm">主视图模型实例 / The MainViewModel instance.</param>
        public MainWindow(ViewModels.MainViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
