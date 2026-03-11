using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace TraceWPF.Controls;

/// <summary>
/// 自定义水平刻度盘控件，支持 Section 区域显示和报警刻度线。
/// Custom horizontal gauge control with Section display and alarm tick lines.
/// </summary>
public class HorizontalGauge : FrameworkElement
{
    #region Dependency Properties

    /// <summary>
    /// 刻度盘最小值。
    /// </summary>
    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(HorizontalGauge),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    /// <summary>
    /// 刻度盘最大值。
    /// </summary>
    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(HorizontalGauge),
            new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    /// <summary>
    /// 当前实际值。
    /// </summary>
    public static readonly DependencyProperty CurrentValueProperty =
        DependencyProperty.Register(nameof(CurrentValue), typeof(double), typeof(HorizontalGauge),
            new FrameworkPropertyMetadata(5.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public double CurrentValue
    {
        get => (double)GetValue(CurrentValueProperty);
        set => SetValue(CurrentValueProperty, value);
    }

    /// <summary>
    /// 流量低报警阈值。
    /// </summary>
    public static readonly DependencyProperty FlowLowLevelAlarmProperty =
        DependencyProperty.Register(nameof(FlowLowLevelAlarm), typeof(double), typeof(HorizontalGauge),
            new FrameworkPropertyMetadata(3.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public double FlowLowLevelAlarm
    {
        get => (double)GetValue(FlowLowLevelAlarmProperty);
        set => SetValue(FlowLowLevelAlarmProperty, value);
    }

    /// <summary>
    /// 流量高报警阈值。
    /// </summary>
    public static readonly DependencyProperty FlowHighLevelAlarmProperty =
        DependencyProperty.Register(nameof(FlowHighLevelAlarm), typeof(double), typeof(HorizontalGauge),
            new FrameworkPropertyMetadata(5.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public double FlowHighLevelAlarm
    {
        get => (double)GetValue(FlowHighLevelAlarmProperty);
        set => SetValue(FlowHighLevelAlarmProperty, value);
    }

    /// <summary>
    /// Section 区域背景画刷。
    /// </summary>
    public static readonly DependencyProperty SectionBackgroundProperty =
        DependencyProperty.Register(nameof(SectionBackground), typeof(Brush), typeof(HorizontalGauge),
            new FrameworkPropertyMetadata(
                new SolidColorBrush(Color.FromArgb(50, 76, 175, 80)),
                FrameworkPropertyMetadataOptions.AffectsRender));

    public Brush SectionBackground
    {
        get => (Brush)GetValue(SectionBackgroundProperty);
        set => SetValue(SectionBackgroundProperty, value);
    }

    /// <summary>
    /// 主刻度间距（值域单位）。
    /// </summary>
    public static readonly DependencyProperty MajorTickIntervalProperty =
        DependencyProperty.Register(nameof(MajorTickInterval), typeof(double), typeof(HorizontalGauge),
            new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public double MajorTickInterval
    {
        get => (double)GetValue(MajorTickIntervalProperty);
        set => SetValue(MajorTickIntervalProperty, value);
    }

    /// <summary>
    /// 两个主刻度之间的次刻度数量。
    /// </summary>
    public static readonly DependencyProperty MinorTickCountProperty =
        DependencyProperty.Register(nameof(MinorTickCount), typeof(int), typeof(HorizontalGauge),
            new FrameworkPropertyMetadata(10, FrameworkPropertyMetadataOptions.AffectsRender));

    public int MinorTickCount
    {
        get => (int)GetValue(MinorTickCountProperty);
        set => SetValue(MinorTickCountProperty, value);
    }

    /// <summary>
    /// 刻度线颜色。
    /// </summary>
    public static readonly DependencyProperty TickBrushProperty =
        DependencyProperty.Register(nameof(TickBrush), typeof(Brush), typeof(HorizontalGauge),
            new FrameworkPropertyMetadata(
                new SolidColorBrush(Color.FromRgb(130, 140, 180)),
                FrameworkPropertyMetadataOptions.AffectsRender));

    public Brush TickBrush
    {
        get => (Brush)GetValue(TickBrushProperty);
        set => SetValue(TickBrushProperty, value);
    }

    /// <summary>
    /// 报警线颜色（低报警）。
    /// </summary>
    public static readonly DependencyProperty AlarmLowBrushProperty =
        DependencyProperty.Register(nameof(AlarmLowBrush), typeof(Brush), typeof(HorizontalGauge),
            new FrameworkPropertyMetadata(
                new SolidColorBrush(Color.FromRgb(255, 87, 34)),
                FrameworkPropertyMetadataOptions.AffectsRender));

    public Brush AlarmLowBrush
    {
        get => (Brush)GetValue(AlarmLowBrushProperty);
        set => SetValue(AlarmLowBrushProperty, value);
    }

    /// <summary>
    /// 报警线颜色（高报警）。
    /// </summary>
    public static readonly DependencyProperty AlarmHighBrushProperty =
        DependencyProperty.Register(nameof(AlarmHighBrush), typeof(Brush), typeof(HorizontalGauge),
            new FrameworkPropertyMetadata(
                new SolidColorBrush(Color.FromRgb(76, 175, 80)),
                FrameworkPropertyMetadataOptions.AffectsRender));

    public Brush AlarmHighBrush
    {
        get => (Brush)GetValue(AlarmHighBrushProperty);
        set => SetValue(AlarmHighBrushProperty, value);
    }

    #endregion

    // Layout constants
    private const double Padding = 30;
    private const double MajorTickHeight = 16;
    private const double MediumTickHeight = 10;
    private const double MinorTickHeight = 6;
    private const double IndicatorRadius = 5;
    private const double BracketHeight = 24;

    public HorizontalGauge()
    {
        // Set a reasonable default size
        MinHeight = 70;
        MinWidth = 200;
    }

    /// <summary>
    /// 将值映射到控件 X 坐标。
    /// </summary>
    private double ValueToX(double value, double trackLeft, double trackWidth)
    {
        double range = Maximum - Minimum;
        if (range <= 0) return trackLeft;
        double ratio = (value - Minimum) / range;
        return trackLeft + ratio * trackWidth;
    }

    protected override void OnRender(DrawingContext dc)
    {
        base.OnRender(dc);

        double width = ActualWidth;
        double height = ActualHeight;
        if (width <= 0 || height <= 0) return;

        double trackLeft = Padding;
        double trackRight = width - Padding;
        double trackWidth = trackRight - trackLeft;
        double centerY = height * 0.45; // axis line vertical center

        if (trackWidth <= 0) return;

        // ---- 1. Draw Section background ----
        DrawSection(dc, trackLeft, trackWidth, centerY);

        // ---- 2. Draw axis line ----
        var axisPen = new Pen(TickBrush, 1);
        axisPen.Freeze();
        dc.DrawLine(axisPen, new Point(trackLeft, centerY), new Point(trackRight, centerY));

        // ---- 3. Draw ticks and labels ----
        DrawTicks(dc, trackLeft, trackWidth, centerY);

        // ---- 4. Draw Section bracket markers ----
        DrawSectionBrackets(dc, trackLeft, trackWidth, centerY);

        // ---- 5. Draw alarm indicators (conditional) ----
        DrawAlarmIndicators(dc, trackLeft, trackWidth, centerY);

        // ---- 6. Draw current value indicator dot ----
        DrawCurrentValueIndicator(dc, trackLeft, trackWidth, centerY);
    }

    /// <summary>
    /// 绘制 Section 背景区域。
    /// </summary>
    private void DrawSection(DrawingContext dc, double trackLeft, double trackWidth, double centerY)
    {
        if (FlowHighLevelAlarm <= FlowLowLevelAlarm) return;

        double lowX = ValueToX(FlowLowLevelAlarm, trackLeft, trackWidth);
        double highX = ValueToX(FlowHighLevelAlarm, trackLeft, trackWidth);
        double sectionHeight = 36;

        var rect = new Rect(lowX, centerY - sectionHeight / 2, highX - lowX, sectionHeight);
        dc.DrawRectangle(SectionBackground, null, rect);
    }

    /// <summary>
    /// 绘制主刻度、次刻度和数字标签。
    /// </summary>
    private void DrawTicks(DrawingContext dc, double trackLeft, double trackWidth, double centerY)
    {
        var tickPen = new Pen(TickBrush, 1);
        tickPen.Freeze();

        var thinTickPen = new Pen(TickBrush, 0.5);
        thinTickPen.Freeze();

        double range = Maximum - Minimum;
        if (range <= 0) return;

        // Draw minor ticks for entire range
        double minorInterval = MajorTickInterval / MinorTickCount;
        int totalMinorTicks = (int)(range / minorInterval) + 1;

        for (int i = 0; i <= totalMinorTicks; i++)
        {
            double value = Minimum + i * minorInterval;
            if (value > Maximum + 0.0001) break;

            double x = ValueToX(value, trackLeft, trackWidth);

            bool isMajor = Math.Abs(value % MajorTickInterval) < 0.0001
                           || Math.Abs(value % MajorTickInterval - MajorTickInterval) < 0.0001;
            bool isMedium = !isMajor && Math.Abs(value % (MajorTickInterval / 2)) < 0.0001;

            double tickH;
            Pen currentPen;

            if (isMajor)
            {
                tickH = MajorTickHeight;
                currentPen = tickPen;
            }
            else if (isMedium)
            {
                tickH = MediumTickHeight;
                currentPen = tickPen;
            }
            else
            {
                tickH = MinorTickHeight;
                currentPen = thinTickPen;
            }

            dc.DrawLine(currentPen, new Point(x, centerY - tickH / 2), new Point(x, centerY + tickH / 2));

            // Draw label for major ticks
            if (isMajor)
            {
                var labelText = new FormattedText(
                    ((int)Math.Round(value)).ToString(),
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Segoe UI"),
                    11,
                    TickBrush,
                    VisualTreeHelper.GetDpi(this).PixelsPerDip);

                dc.DrawText(labelText, new Point(x - labelText.Width / 2, centerY + MajorTickHeight / 2 + 4));
            }
        }
    }

    /// <summary>
    /// 在 Section 边界绘制方括号标记。
    /// </summary>
    private void DrawSectionBrackets(DrawingContext dc, double trackLeft, double trackWidth, double centerY)
    {
        if (FlowHighLevelAlarm <= FlowLowLevelAlarm) return;

        double lowX = ValueToX(FlowLowLevelAlarm, trackLeft, trackWidth);
        double highX = ValueToX(FlowHighLevelAlarm, trackLeft, trackWidth);

        var bracketPen = new Pen(new SolidColorBrush(Color.FromRgb(76, 175, 80)), 1.5);
        bracketPen.Freeze();

        double bracketTop = centerY - BracketHeight / 2;
        double bracketBottom = centerY + BracketHeight / 2;
        double bracketArm = 6;

        // Left bracket [
        dc.DrawLine(bracketPen, new Point(lowX + bracketArm, bracketTop), new Point(lowX, bracketTop));
        dc.DrawLine(bracketPen, new Point(lowX, bracketTop), new Point(lowX, bracketBottom));
        dc.DrawLine(bracketPen, new Point(lowX, bracketBottom), new Point(lowX + bracketArm, bracketBottom));

        // Right bracket ]
        dc.DrawLine(bracketPen, new Point(highX - bracketArm, bracketTop), new Point(highX, bracketTop));
        dc.DrawLine(bracketPen, new Point(highX, bracketTop), new Point(highX, bracketBottom));
        dc.DrawLine(bracketPen, new Point(highX, bracketBottom), new Point(highX - bracketArm, bracketBottom));

        // Small dot at the center-top of the section
        double midX = (lowX + highX) / 2;
        dc.DrawEllipse(
            new SolidColorBrush(Color.FromRgb(76, 175, 80)),
            null,
            new Point(midX, bracketTop - 4),
            3, 3);
    }

    /// <summary>
    /// 根据当前值与报警阈值的关系绘制报警指示。
    /// </summary>
    private void DrawAlarmIndicators(DrawingContext dc, double trackLeft, double trackWidth, double centerY)
    {
        // Low alarm and high alarm visual indicators are handled
        // entirely by the CurrentValue indicator color logic (red/green)
        // in DrawCurrentValueIndicator. No additional markers needed.
    }

    /// <summary>
    /// 绘制当前值指示点。
    /// 当 CurrentValue 在报警范围内（FlowLowLevelAlarm ≤ CurrentValue ≤ FlowHighLevelAlarm）时，指示线为绿色；
    /// 当 CurrentValue 超出报警范围时，指示线为红色。
    /// </summary>
    private void DrawCurrentValueIndicator(DrawingContext dc, double trackLeft, double trackWidth, double centerY)
    {
        double x = ValueToX(CurrentValue, trackLeft, trackWidth);

        // Clamp to track boundaries
        double minX = trackLeft;
        double maxX = trackLeft + trackWidth;
        x = Math.Max(minX, Math.Min(maxX, x));

        // Determine color: RED when outside alarm range, GREEN when within range
        bool isInNormalRange = CurrentValue >= FlowLowLevelAlarm && CurrentValue <= FlowHighLevelAlarm;

        Brush indicatorBrush;
        if (isInNormalRange)
        {
            // Within normal range: green
            indicatorBrush = new SolidColorBrush(Color.FromRgb(76, 175, 80));
        }
        else
        {
            // Outside alarm range (below low or above high): red
            indicatorBrush = new SolidColorBrush(Color.FromRgb(244, 67, 54));
        }

        if (indicatorBrush is SolidColorBrush scb)
        {
            scb.Freeze();
        }

        // Indicator dot
        dc.DrawEllipse(indicatorBrush, null, new Point(x, centerY - MajorTickHeight / 2 - IndicatorRadius - 2), IndicatorRadius, IndicatorRadius);

        // Vertical line at current value position (same color as dot)
        var cursorLine = new Pen(indicatorBrush, 1.5);
        cursorLine.Freeze();
        dc.DrawLine(cursorLine,
            new Point(x, centerY - MajorTickHeight / 2),
            new Point(x, centerY + MajorTickHeight / 2));

        // Value label below axis (same color as indicator)
        var valueText = new FormattedText(
            CurrentValue.ToString("F1"),
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface("Segoe UI"),
            10,
            indicatorBrush,
            VisualTreeHelper.GetDpi(this).PixelsPerDip);

        double labelY = centerY + MajorTickHeight / 2 + 18;
        dc.DrawText(valueText, new Point(x - valueText.Width / 2, labelY));

        // Small dot below the value text (same color)
        dc.DrawEllipse(indicatorBrush, null, new Point(x, labelY + valueText.Height + 4), 3, 3);
    }
}
