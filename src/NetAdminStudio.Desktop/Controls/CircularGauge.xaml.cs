using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NetAdminStudio.Desktop.Controls;

/// <summary>Medidor circular (anillo) que muestra un porcentaje 0–100 con una etiqueta.</summary>
public partial class CircularGauge : UserControl
{
    private const double Size = 98;
    private const double Thickness = 9;

    public CircularGauge()
    {
        InitializeComponent();
        Redraw();
    }

    public static readonly DependencyProperty PercentProperty =
        DependencyProperty.Register(nameof(Percent), typeof(double), typeof(CircularGauge),
            new PropertyMetadata(0.0, OnChanged));

    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(CircularGauge),
            new PropertyMetadata("", OnChanged));

    public static readonly DependencyProperty AccentProperty =
        DependencyProperty.Register(nameof(Accent), typeof(Brush), typeof(CircularGauge),
            new PropertyMetadata(Brushes.DeepSkyBlue, OnChanged));

    public double Percent
    {
        get => (double)GetValue(PercentProperty);
        set => SetValue(PercentProperty, value);
    }

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public Brush Accent
    {
        get => (Brush)GetValue(AccentProperty);
        set => SetValue(AccentProperty, value);
    }

    private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((CircularGauge)d).Redraw();

    private void Redraw()
    {
        if (ArcPath is null) return;

        var pct = Math.Clamp(Percent, 0, 100);
        ValueText.Text = $"{Math.Round(pct)}%";
        LabelText.Text = Label;
        ArcPath.Stroke = Accent;

        var r = (Size - Thickness) / 2;
        var cx = Size / 2;
        var cy = Size / 2;

        if (pct <= 0)
        {
            ArcPath.Data = null;
            return;
        }

        if (pct >= 99.99)
        {
            ArcPath.Data = new EllipseGeometry(new Point(cx, cy), r, r);
            return;
        }

        var sweep = 360 * pct / 100;
        var start = PointOnCircle(cx, cy, r, -90);
        var end = PointOnCircle(cx, cy, r, -90 + sweep);

        var figure = new PathFigure { StartPoint = start, IsClosed = false };
        figure.Segments.Add(new ArcSegment(
            end, new Size(r, r), 0, sweep > 180, SweepDirection.Clockwise, true));

        var geometry = new PathGeometry();
        geometry.Figures.Add(figure);
        ArcPath.Data = geometry;
    }

    private static Point PointOnCircle(double cx, double cy, double r, double angleDegrees)
    {
        var a = angleDegrees * Math.PI / 180.0;
        return new Point(cx + r * Math.Cos(a), cy + r * Math.Sin(a));
    }
}
