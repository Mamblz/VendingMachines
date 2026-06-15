using Avalonia.Controls;
using ReactiveUI;
using VendingMachines.Models;

namespace VendingMachines.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public static MainWindowViewModel Instance;
        public MainWindowViewModel()
        {
            Instance = this;
        }

        public static _43pKobzarContext myConnection = new _43pKobzarContext();
        private UserControl _uc = new AuthPage();
        public UserControl Uc
        {
            get => _uc;
            set => this.RaiseAndSetIfChanged(ref _uc, value);
        }
    }
}
