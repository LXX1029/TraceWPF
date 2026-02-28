namespace TraceWPF.Views
{
    using System.Windows;
    using TraceWPF.DI;

    /// <summary>
    /// 雇员视图代码隐藏，展示雇员管理界面。
    /// Code-behind for the Employee view, displays the employee management UI.
    /// </summary>
    public partial class EmployeeView : Window, ISingleton
    {
        /// <summary>
        /// 默认构造函数，仅初始化 XAML 组件（设计器使用）。
        /// Default constructor that only initializes XAML components (used by designer).
        /// </summary>
        public EmployeeView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 带 ViewModel 的构造函数，通过 DI 注入 EmployeeViewModel 并设置为 DataContext。
        /// Constructor with ViewModel injection; sets the injected EmployeeViewModel as DataContext.
        /// </summary>
        /// <param name="vm">雇员视图模型实例 / The EmployeeViewModel instance.</param>
        public EmployeeView(ViewModels.EmployeeViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        /// <summary>
        /// 注册消息通知（预留方法，当前为空实现）。
        /// Registers message notifications (reserved method, currently empty implementation).
        /// </summary>
        private static void RegisterMessage()
        {

        }

    }
}
