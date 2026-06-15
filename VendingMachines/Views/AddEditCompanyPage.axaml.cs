using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using VendingMachines.Models;
using VendingMachines.ViewModels;

namespace VendingMachines;

public partial class AddEditCompanyPage : UserControl
{
    public AddEditCompanyPage()
    {
        InitializeComponent();
        DataContext = new AddEditCompanyPageViewModel();
    }

    public AddEditCompanyPage(Company company)
    {
        InitializeComponent();
        DataContext = new AddEditCompanyPageViewModel(company);
    }
}