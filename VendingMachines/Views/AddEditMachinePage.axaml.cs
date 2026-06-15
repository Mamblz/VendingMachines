using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using VendingMachines.Models;
using VendingMachines.ViewModels;

namespace VendingMachines;

public partial class AddEditMachinePage : UserControl
{
    public AddEditMachinePage()
    {
        InitializeComponent();
        DataContext = new AddEditMachinePageViewModel();
    }

    public AddEditMachinePage(VendingMachine machine)
    {
        InitializeComponent();
        DataContext = new AddEditMachinePageViewModel(machine);
    }
}