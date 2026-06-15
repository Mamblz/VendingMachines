using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;
using System.Linq;
using System.Reactive;

namespace VendingMachines.ViewModels
{
    public class AuthPageViewModel : ReactiveObject
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public ReactiveCommand<Unit, Unit> RegCommand { get; }

        public AuthPageViewModel() 
        {
            RegCommand = ReactiveCommand.Create(Reg);
        }
        public async void LoginCommand()
        {
            var user = MainWindowViewModel.myConnection.Users
                .FirstOrDefault(u => u.Email == Login && u.Password == Password);
            if (user == null)
            {
                var box = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Неверный логин или пароль", ButtonEnum.Ok, Icon.Error);
                await box.ShowAsync();
                return;
            }

            MainWindowViewModel.Instance.Uc = new MainPage();
        }

        private void Reg()
        {
            MainWindowViewModel.Instance.Uc = new RegPage();
        }
    }
}
