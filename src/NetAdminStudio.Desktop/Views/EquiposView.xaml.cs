using System.Windows;
using System.Windows.Controls;
using NetAdminStudio.Desktop.ViewModels;

namespace NetAdminStudio.Desktop.Views;

public partial class EquiposView : UserControl
{
    public EquiposView() => InitializeComponent();

    private async void LoadRemote_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel vm)
            return;

        var host = vm.SelectedComputer?.IpAddress;
        if (string.IsNullOrWhiteSpace(host))
        {
            vm.RemoteStatusText = "Seleccioná una computadora de la lista primero.";
            return;
        }

        await vm.LoadRemoteAsync(host, vm.RemoteUser, RemotePasswordBox.Password);
    }

    private async void SaveCredential_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel vm)
            return;

        // Se guarda como credencial por defecto (host vacío) para usarla con cualquier equipo.
        await vm.SaveCredentialAsync("", vm.RemoteUser, RemotePasswordBox.Password);
        RemotePasswordBox.Clear();
    }
}
