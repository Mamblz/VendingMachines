using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using VendingMachines.Models;

namespace VendingMachines.ViewModels
{
    public class AddEditUserPageViewModel : ReactiveObject
    {
        private readonly _43pKobzarContext _readContext;
        private bool _isEditMode;
        private string _confirmPassword = string.Empty;
        private string _passwordError = string.Empty;
        private bool _hasPasswordError;

        public bool IsEditMode
        {
            get => _isEditMode;
            private set => this.RaiseAndSetIfChanged(ref _isEditMode, value);
        }

        private User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            set => this.RaiseAndSetIfChanged(ref _currentUser, value);
        }

        public string PageTitle => IsEditMode
            ? "Редактирование пользователя"
            : "Добавление пользователя";

        public string ButtonText => IsEditMode
            ? "Сохранить"
            : "Добавить";

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => this.RaiseAndSetIfChanged(ref _confirmPassword, value);
        }

        public string PasswordError
        {
            get => _passwordError;
            set => this.RaiseAndSetIfChanged(ref _passwordError, value);
        }

        public bool HasPasswordError
        {
            get => _hasPasswordError;
            set => this.RaiseAndSetIfChanged(ref _hasPasswordError, value);
        }

        private ObservableCollection<UserRole> _roles = new();
        public ObservableCollection<UserRole> Roles
        {
            get => _roles;
            set => this.RaiseAndSetIfChanged(ref _roles, value);
        }
        private UserRole? _selectedRole;
        public UserRole? SelectedRole
        {
            get => _selectedRole;
            set => this.RaiseAndSetIfChanged(ref _selectedRole, value);
        }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public AddEditUserPageViewModel(_43pKobzarContext context) : this(null, context) { }
        public AddEditUserPageViewModel(User? user = null) : this(user, new _43pKobzarContext()) { }

        public AddEditUserPageViewModel(User? user, _43pKobzarContext context)
        {
            {
                _readContext = context;
                IsEditMode = user != null;

                if (user != null)
                {
                    _currentUser = new User
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FullName = user.FullName,
                        Password = user.Password,
                        Phone = user.Phone,
                        RoleId = user.RoleId,
                        IsManager = user.IsManager,
                        IsEngineer = user.IsEngineer,
                        IsOperator = user.IsOperator,
                        Image = user.Image
                    };
                }
                else
                {
                    _currentUser = new User();
                }

                LoadLookups();
                this.WhenAnyValue(x => x.ConfirmPassword)
                    .Subscribe(_ => ValidatePasswords());

                this.WhenAnyValue(x => x.SelectedRole)
                    .Subscribe(_ => UpdateRoleId());

                SaveCommand = ReactiveCommand.Create(Save);
                CancelCommand = ReactiveCommand.Create(Cancel);
            }
        }

        private void LoadLookups()
        {
            try
            {
                var roles = _readContext.UserRoles.AsNoTracking().ToList();
                Roles = new ObservableCollection<UserRole>(roles);

                SelectedRole = Roles.FirstOrDefault(x => x.Id == CurrentUser.RoleId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки справочников: {ex.Message}");
            }
        }

        private void UpdateRoleId()
        {
            if (CurrentUser != null)
            {
                CurrentUser.RoleId = SelectedRole?.Id;
            }
        }

        private void ValidatePasswords()
        {
            if (string.IsNullOrWhiteSpace(CurrentUser.Password) && !IsEditMode)
            {
                PasswordError = "Пароль обязателен";
                HasPasswordError = true;
            }
            else if (CurrentUser.Password != ConfirmPassword)
            {
                PasswordError = "Пароли не совпадают";
                HasPasswordError = true;
            }
            else
            {
                PasswordError = string.Empty;
                HasPasswordError = false;
            }
        }

        private void Save()
        {
            ValidatePasswords();

            if (!Validate())
                return;

            try
            {
                if (IsEditMode)
                {
                    var existingUser = _readContext.Users
                        .FirstOrDefault(x => x.Id == CurrentUser.Id);

                    if (existingUser == null)
                        return;

                    existingUser.Email = CurrentUser.Email;
                    existingUser.FullName = CurrentUser.FullName;
                    existingUser.Phone = CurrentUser.Phone;
                    existingUser.RoleId = CurrentUser.RoleId;
                    existingUser.IsManager = CurrentUser.IsManager;
                    existingUser.IsEngineer = CurrentUser.IsEngineer;
                    existingUser.IsOperator = CurrentUser.IsOperator;

                    if (!string.IsNullOrWhiteSpace(CurrentUser.Password))
                    {
                        existingUser.Password = CurrentUser.Password;
                    }

                    _readContext.Entry(existingUser).State = EntityState.Modified;
                }
                else
                {
                    CurrentUser.Id = Guid.NewGuid();
                    _readContext.Users.Add(CurrentUser);
                }

                _readContext.SaveChanges();
                NavigateBack();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения: {ex.Message}");
            }
        }

        private bool Validate()
        {
            return !string.IsNullOrWhiteSpace(CurrentUser.Email)
                   && !string.IsNullOrWhiteSpace(CurrentUser.FullName)
                   && SelectedRole != null
                   && (IsEditMode || (!string.IsNullOrWhiteSpace(CurrentUser.Password) && !HasPasswordError));
        }

        private void Cancel()
        {
            NavigateBack();
        }

        private void NavigateBack()
        {
            if (MainWindowViewModel.Instance != null)
            {
                MainWindowViewModel.Instance.Uc = new UsersPage();
            }
        }
    }
}