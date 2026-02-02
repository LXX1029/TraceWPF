using System.Windows;
using TraceWPF.DI;
using TraceWPF.ViewModels;

namespace TraceWPF.Views
{
    public partial class DataMigrationView : Window, ITransient
    {
        public DataMigrationView(DataMigrationViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
