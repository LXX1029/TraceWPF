namespace TraceWPF.Views
{
    using System.Windows;
    using TraceWPF.DI;

    public partial class MainWindow : Window, ISingleton
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(ViewModels.MainViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
