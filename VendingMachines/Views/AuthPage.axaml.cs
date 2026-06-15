using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using VendingMachines.ViewModels;

namespace VendingMachines;

public partial class AuthPage : UserControl
{
    public AuthPage()
    {
        InitializeComponent();
        DataContext = new AuthPageViewModel();
    }
}