using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using VendingMachines.ViewModels;

namespace VendingMachines;

public partial class MainPage : UserControl
{
    public MainPage()
    {
        InitializeComponent();
        DataContext = new MainPageViewModel();
    }
}