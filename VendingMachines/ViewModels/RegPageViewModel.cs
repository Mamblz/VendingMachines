using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VendingMachines.Models;

namespace VendingMachines.ViewModels
{
    public class RegPageViewModel: ViewModelBase
    {
        private readonly _43pKobzarContext _db = new _43pKobzarContext();
        public ReactiveCommand<Unit, Unit> RegisterCommand { get; }
        public ReactiveCommand<Unit, Unit> GoToLoginCommand { get; }

        string _email;
        public string Email
        {
            get => _email;
            set => this.RaiseAndSetIfChanged(ref _email, value);
        }

        string _password;
        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        bool _showPassword;
        public bool ShowPassword
        {
            get => _showPassword;
            set
            {
                this.RaiseAndSetIfChanged(ref _showPassword, value);
                this.RaisePropertyChanged(nameof(ShowPasswordBox));
                this.RaisePropertyChanged(nameof(ShowPasswordText));
            }
        }
        
        public bool ShowPasswordBox => !_showPassword;
        public bool ShowPasswordText => _showPassword;

        string _emailCode;
        public string EmailCode
        {
            get => _emailCode;
            set => this.RaiseAndSetIfChanged(ref _emailCode, value);
        }

        string _franchiseCode;
        public string FranchiseCode
        {
            get => _franchiseCode;
            set => this.RaiseAndSetIfChanged(ref _franchiseCode, value);
        }

        string _captchaText;
        public string CaptchaText
        {
            get => _captchaText;
            set => this.RaiseAndSetIfChanged(ref _captchaText, value);
        }

        string _captchaAnswer;
        public string CaptchaAnswer
        {
            get => _captchaAnswer;
            set => this.RaiseAndSetIfChanged(ref _captchaAnswer, value);
        }


        private string _generatedEmailCode;
        private int _captchaResult;

        public RegPageViewModel()
        {
            GenerateCaptcha();
            RegisterCommand = ReactiveCommand.CreateFromTask(Register);
            GoToLoginCommand = ReactiveCommand.Create(GoToLogin);
        }


        public async Task Register()
        {
            if (!ValidateEmail())
                return;

            if (!ValidatePassword())
                return;

            if (_generatedEmailCode == null)
            {
                await GenerateEmailCode();
                return;
            }

            if (EmailCode != _generatedEmailCode)
            {
                await ShowError("Неверный код подтверждения");
                return;
            }

            if (!int.TryParse(CaptchaAnswer, out int result) || result != _captchaResult)
            {
                await ShowError("Неверная CAPTCHA");
                GenerateCaptcha();
                return;
            }

            if (_db.Users.Any(u => u.Email == Email))
            {
                await ShowError("Пользователь с таким email уже существует");
                return;
            }

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Email = Email,
                Password = Password,
                FullName = "Новый пользователь"
            };

            _db.Users.Add(newUser);
            _db.SaveChanges();

            var box = MessageBoxManager.GetMessageBoxStandard(
                "Успех",
                "Регистрация завершена!",
                ButtonEnum.Ok,
                Icon.Success);

            await box.ShowAsync();
            MainWindowViewModel.Instance.Uc = new AuthPage();
        }

        private void GoToLogin()
        {
            if (MainWindowViewModel.Instance != null)
            {
                MainWindowViewModel.Instance.Uc = new AuthPage();
            }
        }


        private async Task GenerateEmailCode()
        {
            _generatedEmailCode = new Random().Next(100000, 999999).ToString();

            var box = MessageBoxManager.GetMessageBoxStandard(
                "Код подтверждения",
                $"Ваш код подтверждения: {_generatedEmailCode}",
                ButtonEnum.Ok,
                Icon.Info);
            await box.ShowAsync();
        }


        private void GenerateCaptcha()
        {
            var rnd = new Random();
            int a = rnd.Next(1, 10);
            int b = rnd.Next(1, 10);
            int c = rnd.Next(1, 10);

            _captchaResult = a + b * c - 2;
            CaptchaText = $"{a} + {b} * {c} - 2 = ?";
        }

        private bool ValidateEmail()
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            if (!Regex.IsMatch(Email ?? "", pattern))
            {
                ShowError("Некорректный email");
                return false;
            }
            return true;
        }

        private bool ValidatePassword()
        {
            if (Password.Length < 8)
            {
                ShowError("Пароль должен быть минимум 8 символов");
                return false;
            }

            if (!Password.Any(char.IsDigit))
            {
                ShowError("Пароль должен содержать цифру");
                return false;
            }

            if (!Password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                ShowError("Пароль должен содержать спецсимвол");
                return false;
            }

            return true;
        }

        private async Task ShowError(string message)
        {
            var box = MessageBoxManager.GetMessageBoxStandard(
                "Ошибка",
                message,
                ButtonEnum.Ok,
                Icon.Error);

            await box.ShowAsync();
        }
    }
}
