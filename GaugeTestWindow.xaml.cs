using System.Windows;
using TraceWPF.Domain.Interfaces;
using TraceWPF.DI;
namespace TraceWPF;

/// <summary>
/// 刻度盘控件测试窗口代码隐藏。
/// Code-behind for the GaugeTestWindow.
/// </summary>
public partial class GaugeTestWindow : Window, ITransient
{
    /// <summary>
    /// 初始化刻度盘测试窗口。
    /// </summary>
    public GaugeTestWindow()
    {
        InitializeComponent();
    }
}
