using System.Collections;
using System.Windows;

namespace NetAdminStudio.Desktop.Controls;

public partial class DetailWindow : Window
{
    private sealed record Field(string Label, string Value);

    // Campos conocidos (propiedad -> etiqueta), en orden de presentación. Cada DTO usa
    // solo los que tiene; los demás se ignoran para mantener el detalle limpio.
    private static readonly (string Prop, string Label)[] KnownFields =
    {
        ("Name", "Nombre"),
        ("Title", "Título"),
        ("Hostname", "Host"),
        ("FullName", "Nombre completo"),
        ("IpAddress", "IP"),
        ("MacAddress", "MAC"),
        ("Vendor", "Fabricante"),
        ("Model", "Modelo"),
        ("TypeText", "Tipo"),
        ("StateText", "Estado"),
        ("PortsText", "Puertos abiertos"),
        ("Origin", "Origen"),
        ("Location", "Ubicación"),
        ("LatencyMs", "Latencia (ms)"),
        ("LastSeenAt", "Última vez visto"),
        ("ConnectionType", "Conexión"),
        ("QueueName", "Cola"),
        ("BlackLevelPercent", "Tóner %"),
        ("PendingJobs", "Trabajos en cola"),
        ("Path", "Ruta"),
        ("ShareType", "Tipo de recurso"),
        ("SeverityText", "Severidad"),
        ("SourceType", "Origen"),
        ("OpenedText", "Abierta"),
        ("TriggerType", "Disparador"),
        ("ActionType", "Acción"),
        ("ConditionJson", "Condición"),
        ("ActionJson", "Parámetros de la acción"),
        ("EnabledText", "Estado"),
        ("Description", "Descripción"),
    };

    private DetailWindow(string title, string? subtitle, IEnumerable<Field> fields)
    {
        InitializeComponent();
        TitleText.Text = title;
        Title = title;
        SubtitleText.Text = subtitle ?? "";
        SubtitleText.Visibility = string.IsNullOrWhiteSpace(subtitle)
            ? Visibility.Collapsed : Visibility.Visible;
        FieldsList.ItemsSource = fields;
    }

    /// <summary>Abre una ventana modal con todos los campos conocidos del objeto.</summary>
    public static void ShowFor(object item)
    {
        var type = item.GetType();
        var fields = new List<Field>();
        string? title = null;
        string? subtitle = null;

        foreach (var (prop, label) in KnownFields)
        {
            var property = type.GetProperty(prop);
            if (property is null) continue;

            var text = Format(property.GetValue(item));
            if (string.IsNullOrWhiteSpace(text)) continue;

            if (prop is "Name" or "Title") title ??= text;
            else if (prop is "IpAddress" or "FullName") subtitle ??= text;

            fields.Add(new Field(label, text));
        }

        var window = new DetailWindow(title ?? "Detalle", subtitle, fields)
        {
            Owner = Application.Current.MainWindow
        };
        window.ShowDialog();
    }

    private static string Format(object? value)
    {
        switch (value)
        {
            case null:
                return "";
            case string s:
                return s;
            case bool b:
                return b ? "Sí" : "No";
            case DateTimeOffset dto:
                return dto.LocalDateTime.ToString("dd/MM/yyyy HH:mm:ss");
            case IEnumerable enumerable:
                return string.Join(", ", enumerable.Cast<object>());
            default:
                return value.ToString() ?? "";
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
