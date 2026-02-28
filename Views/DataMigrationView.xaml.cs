using System.Windows;
using TraceWPF.DI;
using TraceWPF.ViewModels;

namespace TraceWPF.Views
{
    /// <summary>
    /// 数据迁移视图代码隐藏，提供 TDengine 数据库迁移操作的用户界面。
    /// Code-behind for the Data Migration view, provides the UI for TDengine database migration operations.
    /// </summary>
    public partial class DataMigrationView : Window, ITransient
    {
        /// <summary>
        /// 构造函数，通过 DI 注入 DataMigrationViewModel 并设置为 DataContext。
        /// Constructor that injects DataMigrationViewModel via DI and sets it as DataContext.
        /// </summary>
        /// <param name="viewModel">数据迁移视图模型实例 / The DataMigrationViewModel instance.</param>
        public DataMigrationView(DataMigrationViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

        }
    }
}
