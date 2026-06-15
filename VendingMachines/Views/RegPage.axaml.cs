using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using VendingMachines.ViewModels;

namespace VendingMachines;

public partial class RegPage : UserControl
{
    public RegPage()
    {
        InitializeComponent();
        DataContext = new RegPageViewModel();
    }
}