namespace TraceWPF.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using TraceWPF.DI;
    using TraceWPF.Domain.Models;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;

    /// <summary>
    /// 雇员视图模型
    /// </summary>
    public partial class EmployeeViewModel : ObservableObject, ISingleton
    {
        /// <summary>
        /// 雇员列表
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Employee> employees = new();

        /// <summary>
        /// 当前选中的雇员
        /// </summary>
        [ObservableProperty]
        private Employee? selectedEmployee;

        /// <summary>
        /// 新雇员姓名（用于添加）
        /// </summary>
        [ObservableProperty]
        private string newEmployeeName = "";

        /// <summary>
        /// 新雇员邮箱（用于添加）
        /// </summary>
        [ObservableProperty]
        private string newEmployeeEmail = "";

        /// <summary>
        /// 新雇员电话（用于添加）
        /// </summary>
        [ObservableProperty]
        private string newEmployeePhone = "";

        /// <summary>
        /// 新雇员部门（用于添加）
        /// </summary>
        [ObservableProperty]
        private string newEmployeeDepartment = "";

        /// <summary>
        /// 新雇员职位（用于添加）
        /// </summary>
        [ObservableProperty]
        private string newEmployeePosition = "";

        /// <summary>
        /// 新雇员薪资（用于添加）
        /// </summary>
        [ObservableProperty]
        private decimal newEmployeeSalary;

        private int _nextId = 1;

        /// <summary>
        /// 构造函数，初始化时加载示例雇员数据。
        /// Constructor that loads sample employee data on initialization.
        /// </summary>
        public EmployeeViewModel()
        {
            // 添加一些示例数据
            LoadSampleData();
        }

        /// <summary>
        /// 加载示例数据
        /// </summary>
        private void LoadSampleData()
        {
            Employees.Add(new Employee
            {
                Id = _nextId++,
                Name = "张三",
                Email = "zhangsan@example.com",
                Phone = "13800138001",
                Department = "技术部",
                Position = "高级工程师",
                HireDate = new DateTime(2020, 3, 15),
                Salary = 15000,
                IsActive = true
            });

            Employees.Add(new Employee
            {
                Id = _nextId++,
                Name = "李四",
                Email = "lisi@example.com",
                Phone = "13800138002",
                Department = "市场部",
                Position = "市场经理",
                HireDate = new DateTime(2019, 7, 20),
                Salary = 18000,
                IsActive = true
            });

            Employees.Add(new Employee
            {
                Id = _nextId++,
                Name = "王五",
                Email = "wangwu@example.com",
                Phone = "13800138003",
                Department = "人力资源部",
                Position = "HR专员",
                HireDate = new DateTime(2021, 1, 10),
                Salary = 10000,
                IsActive = true
            });
        }

        /// <summary>
        /// 添加雇员命令
        /// </summary>
        [RelayCommand]
        private void AddEmployee()
        {
            if (string.IsNullOrWhiteSpace(NewEmployeeName))
                return;

            var employee = new Employee
            {
                Id = _nextId++,
                Name = NewEmployeeName,
                Email = NewEmployeeEmail,
                Phone = NewEmployeePhone,
                Department = NewEmployeeDepartment,
                Position = NewEmployeePosition,
                HireDate = DateTime.Now,
                Salary = NewEmployeeSalary,
                IsActive = true
            };

            Employees.Add(employee);
            ClearInputFields();
        }

        /// <summary>
        /// 删除雇员命令
        /// </summary>
        [RelayCommand]
        private void DeleteEmployee()
        {
            if (SelectedEmployee == null)
                return;

            Employees.Remove(SelectedEmployee);
            SelectedEmployee = null;
        }

        /// <summary>
        /// 清空输入字段
        /// </summary>
        [RelayCommand]
        private void ClearInputFields()
        {
            NewEmployeeName = "";
            NewEmployeeEmail = "";
            NewEmployeePhone = "";
            NewEmployeeDepartment = "";
            NewEmployeePosition = "";
            NewEmployeeSalary = 0;
        }
    }
}
