using System.Windows;
using NetAdminStudio.Desktop.ViewModels;

namespace NetAdminStudio.Desktop;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        SourceInitialized += (_, _) => DarkTitleBar.Apply(this);
        Loaded += async (_, _) =>
        {
            await viewModel.RefreshCommand.ExecuteAsync(null);
            viewModel.StartAutoRefresh();
        };
    }
}
