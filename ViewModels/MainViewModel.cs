namespace TraceWPF.ViewModels
{
    using TraceWPF.Application.UseCases;
    using TraceWPF.DI;
    using TraceWPF.Views;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;

    /// <summary>
    /// 主视图模型，管理主窗口的标题显示、欢迎消息加载以及子视图（雇员、数据迁移）的打开。
    /// Main ViewModel that manages the title display, welcome message loading, and opening of sub-views (Employee, DataMigration) on the main window.
    /// </summary>
    public partial class MainViewModel : ObservableObject, ISingleton
    {
        /// <summary>
        /// 获取欢迎消息的用例服务。
        /// Use case service for retrieving the welcome message.
        /// </summary>
        private readonly IGetWelcomeMessage? _getWelcomeMessage;

        /// <summary>
        /// 雇员视图模型（用于打开雇员管理窗口）。
        /// Employee ViewModel (used to open the employee management window).
        /// </summary>
        private readonly EmployeeViewModel? _employeeViewModel;

        /// <summary>
        /// 数据迁移视图模型（用于打开数据迁移窗口）。
        /// Data migration ViewModel (used to open the data migration window).
        /// </summary>
        private readonly DataMigrationViewModel? _dataMigrationViewModel;

        /// <summary>
        /// 主窗口标题文本。
        /// Title text displayed on the main window.
        /// </summary>
        [ObservableProperty]
        private string title = "Hello MVVM";

        /// <summary>
        /// 用户输入的文本，用于更新标题。
        /// User input text used for updating the title.
        /// </summary>
        [ObservableProperty]
        private string inputText = "";

        /// <summary>
        /// 默认无参构造函数（设计器使用）。
        /// Default parameterless constructor (used by designer).
        /// </summary>
        public MainViewModel() { }

        /// <summary>
        /// 带依赖注入的构造函数，注入欢迎消息用例、雇员视图模型和数据迁移视图模型。
        /// DI constructor that injects the welcome message use case, employee ViewModel, and data migration ViewModel.
        /// </summary>
        /// <param name="getWelcomeMessage">获取欢迎消息用例 / Welcome message use case.</param>
        /// <param name="employeeViewModel">雇员视图模型 / Employee ViewModel.</param>
        /// <param name="dataMigrationViewModel">数据迁移视图模型 / Data migration ViewModel.</param>
        public MainViewModel(IGetWelcomeMessage getWelcomeMessage, EmployeeViewModel employeeViewModel, DataMigrationViewModel dataMigrationViewModel)
        {
            _getWelcomeMessage = getWelcomeMessage;
            _employeeViewModel = employeeViewModel;
            _dataMigrationViewModel = dataMigrationViewModel;
        }

        /// <summary>
        /// 更新标题命令：将标题设置为用户输入的文本，若为空则恢复默认标题 "Hello MVVM"。
        /// Update title command: sets the title to the user's input text; reverts to "Hello MVVM" if input is empty.
        /// </summary>
        [RelayCommand]
        private void UpdateTitle()
        {
            Title = string.IsNullOrWhiteSpace(InputText) ? "Hello MVVM" : InputText;
        }

        /// <summary>
        /// 加载欢迎消息命令：通过用例服务从数据库获取欢迎消息并显示在标题上。
        /// Load welcome message command: retrieves the welcome message from the database via the use case service and displays it as the title.
        /// </summary>
        [RelayCommand]
        private void LoadWelcomeMessage()
        {
            if (_getWelcomeMessage is null) return;
            var msg = _getWelcomeMessage.Execute();
            Title = msg.Content;
        }

        /// <summary>
        /// 打开雇员管理视图命令：创建 EmployeeView 窗口并显示。
        /// Open employee view command: creates and shows an EmployeeView window.
        /// </summary>
        [RelayCommand]
        private void OpenEmployeeView()
        {
            if (_employeeViewModel is null) return;
            var employeeView = new EmployeeView(_employeeViewModel);
            employeeView.Show();
        }

        /// <summary>
        /// 打开数据迁移视图命令：创建 DataMigrationView 窗口并显示。
        /// Open data migration view command: creates and shows a DataMigrationView window.
        /// </summary>
        [RelayCommand]
        private void OpenDataMigration()
        {
            if (_dataMigrationViewModel is null) return;
            var view = new DataMigrationView(_dataMigrationViewModel);
            view.Show();
        }
    }
}
