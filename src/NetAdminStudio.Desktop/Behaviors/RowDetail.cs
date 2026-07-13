using System.Windows;
using System.Windows.Controls;
using NetAdminStudio.Desktop.Controls;

namespace NetAdminStudio.Desktop.Behaviors;

/// <summary>
/// Propiedad adjunta: cuando se activa en un DataGrid, el doble clic sobre una fila
/// abre una ventana de detalle con toda la información de ese elemento.
/// </summary>
public static class RowDetail
{
    public static readonly DependencyProperty EnabledProperty =
        DependencyProperty.RegisterAttached(
            "Enabled", typeof(bool), typeof(RowDetail),
            new PropertyMetadata(false, OnEnabledChanged));

    public static bool GetEnabled(DependencyObject obj) => (bool)obj.GetValue(EnabledProperty);
    public static void SetEnabled(DependencyObject obj, bool value) => obj.SetValue(EnabledProperty, value);

    private static void OnEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not DataGrid grid) return;

        if (e.NewValue is true)
            grid.MouseDoubleClick += OnDoubleClick;
        else
            grid.MouseDoubleClick -= OnDoubleClick;
    }

    private static void OnDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (sender is DataGrid { SelectedItem: { } item })
            DetailWindow.ShowFor(item);
    }
}
