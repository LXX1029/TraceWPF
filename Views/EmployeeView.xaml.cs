namespace TraceWPF.Views
{
    using System.Windows;
    using TraceWPF.DI;

    /// <summary>
    /// 雇员视图代码隐藏
    /// </summary>
    public partial class EmployeeView : Window, ISingleton
    {
        public EmployeeView()
        {
            InitializeComponent();
        }

        public EmployeeView(ViewModels.EmployeeViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
        private static void RegisterMessage()
        {

        }

    }
}
