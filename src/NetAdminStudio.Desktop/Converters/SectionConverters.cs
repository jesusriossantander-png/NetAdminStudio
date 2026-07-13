using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NetAdminStudio.Desktop.Converters;

/// <summary>Muestra el elemento (Visible) solo cuando la sección activa coincide con el parámetro.</summary>
public sealed class SectionToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        string.Equals(value?.ToString(), parameter?.ToString(), StringComparison.OrdinalIgnoreCase)
            ? Visibility.Visible
            : Visibility.Collapsed;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        Binding.DoNothing;
}

/// <summary>Devuelve el estilo de botón de navegación activo o normal según la sección.</summary>
public sealed class SectionToNavStyleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var active = string.Equals(value?.ToString(), parameter?.ToString(),
            StringComparison.OrdinalIgnoreCase);
        var key = active ? "NavButtonActive" : "NavButton";
        return Application.Current.Resources[key];
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        Binding.DoNothing;
}
