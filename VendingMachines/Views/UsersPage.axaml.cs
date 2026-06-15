using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using VendingMachines.ViewModels;

namespace VendingMachines;

public partial class UsersPage : UserControl
{
    public UsersPage()
    {
        InitializeComponent();
        DataContext = new UsersPageViewModel();
    }
}