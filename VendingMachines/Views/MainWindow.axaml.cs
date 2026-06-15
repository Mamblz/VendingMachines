using Avalonia.Controls;
using VendingMachines.ViewModels;

namespace VendingMachines.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
    }
}