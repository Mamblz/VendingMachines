using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using VendingMachines.ViewModels;

namespace VendingMachines;

public partial class AdminPage : UserControl
{
    public AdminPage()
    {
        InitializeComponent();
        DataContext = new AdminPageViewModel();
    }
}