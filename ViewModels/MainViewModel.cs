namespace TraceWPF.ViewModels
{
    using TraceWPF.Application.UseCases;
    using TraceWPF.DI;
    using TraceWPF.Views;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;

    public partial class MainViewModel : ObservableObject, ISingleton
    {
        private readonly IGetWelcomeMessage? _getWelcomeMessage;
        private readonly EmployeeViewModel? _employeeViewModel;
        private readonly DataMigrationViewModel? _dataMigrationViewModel;

        [ObservableProperty]
        private string title = "Hello MVVM";

        [ObservableProperty]
        private string inputText = "";

        public MainViewModel() { }

        public MainViewModel(IGetWelcomeMessage getWelcomeMessage, EmployeeViewModel employeeViewModel, DataMigrationViewModel dataMigrationViewModel)
        {
            _getWelcomeMessage = getWelcomeMessage;
            _employeeViewModel = employeeViewModel;
            _dataMigrationViewModel = dataMigrationViewModel;
        }

        [RelayCommand]
        private void UpdateTitle()
        {
            Title = string.IsNullOrWhiteSpace(InputText) ? "Hello MVVM" : InputText;
        }

        [RelayCommand]
        private void LoadWelcomeMessage()
        {
            if (_getWelcomeMessage is null) return;
            var msg = _getWelcomeMessage.Execute();
            Title = msg.Content;
        }

        [RelayCommand]
        private void OpenEmployeeView()
        {
            if (_employeeViewModel is null) return;
            var employeeView = new EmployeeView(_employeeViewModel);
            employeeView.Show();
        }

        [RelayCommand]
        private void OpenDataMigration()
        {
            if (_dataMigrationViewModel is null) return;
            var view = new DataMigrationView(_dataMigrationViewModel);
            view.Show();
        }
    }
}
