using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using VendingMachines.ViewModels;

namespace VendingMachines;

public partial class CompanyPage : UserControl
{
    public CompanyPage()
    {
        InitializeComponent();
        DataContext = new CompanyPageViewModel();
    }
}