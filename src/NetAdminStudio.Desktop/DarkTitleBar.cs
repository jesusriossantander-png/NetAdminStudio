using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace NetAdminStudio.Desktop;

/// <summary>
/// Pone la barra de título nativa de Windows en modo oscuro (evita el marco blanco feo).
/// </summary>
public static class DarkTitleBar
{
    private const int DwmwaUseImmersiveDarkMode = 20;
    private const int DwmwaUseImmersiveDarkModeOld = 19;

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(
        IntPtr hwnd, int attribute, ref int value, int size);

    /// <summary>Aplica el modo oscuro a la barra de título de la ventana ya inicializada.</summary>
    public static void Apply(Window window)
    {
        var hwnd = new WindowInteropHelper(window).EnsureHandle();
        var enabled = 1;

        // El atributo cambió de número entre builds de Windows 10/11; probamos ambos.
        if (DwmSetWindowAttribute(hwnd, DwmwaUseImmersiveDarkMode, ref enabled, sizeof(int)) != 0)
            DwmSetWindowAttribute(hwnd, DwmwaUseImmersiveDarkModeOld, ref enabled, sizeof(int));
    }
}
