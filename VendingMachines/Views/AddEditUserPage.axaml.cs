using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using VendingMachines.Models;
using VendingMachines.ViewModels;

namespace VendingMachines;

public partial class AddEditUserPage : UserControl
{
    public AddEditUserPage()
    {
        InitializeComponent();
        DataContext = new AddEditUserPageViewModel();
    }

    public AddEditUserPage(User user)
    {
        InitializeComponent();
        DataContext = new AddEditUserPageViewModel(user);
    }
}