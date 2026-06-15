using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using VendingMachines.Models;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace VendingMachines.ViewModels
{
    public class UsersPageViewModel : ReactiveObject
    {
        private readonly _43pKobzarContext _context;
        private readonly bool _skipConfirmation;
        private List<User> _allUsers = new();
        private ObservableCollection<User> _pagedUsers = new();

        private int _pageSize = 50;
        private int _currentPage = 1;
        private int _totalItems;
        private string _searchText = string.Empty;
        private ViewMode _viewMode = ViewMode.Table;

        public ReactiveCommand<Unit, Unit> AddUserCommand { get; }
        public ReactiveCommand<User, Unit> EditUserCommand { get; }
        public ReactiveCommand<User, Unit> DeleteUserCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }
        public ReactiveCommand<Unit, Unit> PrevPageCommand { get; }
        public ReactiveCommand<Unit, Unit> NextPageCommand { get; }
        public ReactiveCommand<Unit, Unit> GoToMainPageCommand { get; }
        public ReactiveCommand<Unit, Unit> GoToAdminPageCommand { get; }
        public ReactiveCommand<Unit, Unit> GoToCompaniesCommand { get; }
        public ReactiveCommand<Unit, Unit> SetTileViewCommand { get; }
        public ReactiveCommand<Unit, Unit> SetTableViewCommand { get; }

        public ObservableCollection<User> PagedUsers
        {
            get => _pagedUsers;
            set => this.RaiseAndSetIfChanged(ref _pagedUsers, value);
        }

        public List<int> PageSizeOptions { get; } = new() { 5, 10, 20 };

        public int PageSize
        {
            get => _pageSize;
            set
            {
                this.RaiseAndSetIfChanged(ref _pageSize, value);
                CurrentPage = 1;
                ApplyFilterAndPaging();
            }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                this.RaiseAndSetIfChanged(ref _currentPage, value);
                ApplyFilterAndPaging();
            }
        }

        public int TotalItems
        {
            get => _totalItems;
            private set => this.RaiseAndSetIfChanged(ref _totalItems, value);
        }

        public int TotalPages
        {
            get
            {
                if (PageSize == int.MaxValue) return 1;
                return (int)Math.Ceiling((double)TotalItems / PageSize);
            }
        }

        public bool CanGoPrev => CurrentPage > 1;
        public bool CanGoNext => CurrentPage < TotalPages;

        public string SearchText
        {
            get => _searchText;
            set
            {
                this.RaiseAndSetIfChanged(ref _searchText, value);
                CurrentPage = 1;
                ApplyFilterAndPaging();
            }
        }

        public ViewMode ViewMode
        {
            get => _viewMode;
            set => this.RaiseAndSetIfChanged(ref _viewMode, value);
        }

        public bool IsTileView => ViewMode == ViewMode.Tile;
        public bool IsTableView => ViewMode == ViewMode.Table;

        public string DisplayRangeText
        {
            get
            {
                if (PagedUsers.Count == 0)
                    return $"Найдено: 0 из {TotalItems}";

                int start = (CurrentPage - 1) * (PageSize == int.MaxValue ? TotalItems : PageSize) + 1;
                int end = Math.Min(start + PagedUsers.Count - 1, TotalItems);
                return $"Показано {start}–{end} из {TotalItems}";
            }
        }

        public UsersPageViewModel() : this(new _43pKobzarContext(), false) { }

        public UsersPageViewModel(_43pKobzarContext context, bool skipConfirmation = false)
        {
            _context = context;
            _skipConfirmation = skipConfirmation;
            LoadUsers();

            AddUserCommand = ReactiveCommand.Create(AddUser);
            EditUserCommand = ReactiveCommand.Create<User>(EditUser);
            DeleteUserCommand = ReactiveCommand.Create<User>(DeleteUser);
            ExportCommand = ReactiveCommand.CreateFromTask(Export);
            PrevPageCommand = ReactiveCommand.Create(PrevPage, this.WhenAnyValue(x => x.CanGoPrev));
            NextPageCommand = ReactiveCommand.Create(NextPage, this.WhenAnyValue(x => x.CanGoNext));
            GoToMainPageCommand = ReactiveCommand.Create(GoToMainPage);
            GoToAdminPageCommand = ReactiveCommand.Create(GoToAdminPage);
            GoToCompaniesCommand = ReactiveCommand.Create(GoToCompanies);

            SetTileViewCommand = ReactiveCommand.Create(() => { ViewMode = ViewMode.Tile; });
            SetTableViewCommand = ReactiveCommand.Create(() => { ViewMode = ViewMode.Table; });

            this.WhenAnyValue(
                x => x.PagedUsers.Count,
                x => x.TotalItems,
                x => x.CurrentPage,
                x => x.PageSize)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(DisplayRangeText)));

            this.WhenAnyValue(x => x.ViewMode)
                .Subscribe(_ =>
                {
                    this.RaisePropertyChanged(nameof(IsTileView));
                    this.RaisePropertyChanged(nameof(IsTableView));
                });
        }

        private void LoadUsers()
        {
            try
            {
                _allUsers = _context.Users
                    .Include(u => u.Role)
                    .ToList();

                ApplyFilterAndPaging();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки пользователей: {ex.Message}");
            }
        }

        private void ApplyFilterAndPaging()
        {
            IEnumerable<User> filtered = _allUsers;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var search = SearchText.ToLower();
                filtered = filtered.Where(u =>
                    (u.FullName?.ToLower().Contains(search) ?? false) ||
                    (u.Email?.ToLower().Contains(search) ?? false) ||
                    (u.Phone?.ToLower().Contains(search) ?? false) ||
                    (u.Role?.Name?.ToLower().Contains(search) ?? false));
            }

            TotalItems = filtered.Count();

            IEnumerable<User> paged;
            if (PageSize == int.MaxValue)
            {
                paged = filtered;
            }
            else
            {
                paged = filtered.Skip((CurrentPage - 1) * PageSize).Take(PageSize);
            }

            PagedUsers = new ObservableCollection<User>(paged);
            this.RaisePropertyChanged(nameof(TotalPages));
            this.RaisePropertyChanged(nameof(CanGoPrev));
            this.RaisePropertyChanged(nameof(CanGoNext));
        }

        private void PrevPage() => CurrentPage--;
        private void NextPage() => CurrentPage++;

        private void AddUser()
        {
            MainWindowViewModel.Instance.Uc = new AddEditUserPage();
        }

        private void EditUser(User user)
        {
            MainWindowViewModel.Instance.Uc = new AddEditUserPage(user);
        }

        private async void DeleteUser(User user)
        {
            try
            {
                var userToDelete = await _context.Users
                    .Include(u => u.Maintenances)
                    .Include(u => u.VendingMachines)
                    .FirstOrDefaultAsync(u => u.Id == user.Id);

                if (userToDelete == null)
                    return;

                if (userToDelete.Maintenances?.Any() == true || userToDelete.VendingMachines?.Any() == true)
                {
                    if (!_skipConfirmation)
                    {
                        var errorBox = MessageBoxManager.GetMessageBoxStandard(
                            "Ошибка удаления",
                            "Невозможно удалить пользователя, так как с ним связаны записи.",
                            ButtonEnum.Ok,
                            Icon.Error);
                        await errorBox.ShowAsync();
                    }
                    return;
                }

                bool shouldDelete = _skipConfirmation;

                if (!shouldDelete)
                {
                    var box = MessageBoxManager.GetMessageBoxStandard(
                        "Подтверждение удаления",
                        $"Вы действительно хотите удалить пользователя \"{user.FullName}\"?",
                        ButtonEnum.YesNo,
                        Icon.Question);

                    var result = await box.ShowAsync();
                    shouldDelete = result == ButtonResult.Yes;
                }

                if (shouldDelete)
                {
                    _context.Users.Remove(userToDelete);
                    await _context.SaveChangesAsync();
                    LoadUsers();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка удаления: {ex.Message}");

                if (!_skipConfirmation)
                {
                    var errorBox = MessageBoxManager.GetMessageBoxStandard(
                        "Ошибка",
                        $"Произошла ошибка при удалении: {ex.Message}",
                        ButtonEnum.Ok,
                        Icon.Error);
                    await errorBox.ShowAsync();
                }
            }
        }

        private async Task Export()
        {
            try
            {
                var csv = new StringBuilder();
                csv.AppendLine("ID;ФИО;Email;Телефон;Роль;Изображение");

                foreach (var u in _allUsers)
                {
                    csv.AppendLine($"{u.Id};{EscapeCsv(u.FullName)};{EscapeCsv(u.Email)};{EscapeCsv(u.Phone)};{EscapeCsv(u.Role?.Name)};{EscapeCsv(u.Image)}");
                }

                var topLevel = Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
                    ? desktop.MainWindow
                    : null;

                if (topLevel != null)
                {
                    var file = await topLevel.StorageProvider.SaveFilePickerAsync(new Avalonia.Platform.Storage.FilePickerSaveOptions
                    {
                        Title = "Сохранить CSV",
                        SuggestedFileName = "users.csv",
                        DefaultExtension = "csv"
                    });

                    if (file != null)
                    {
                        await using var stream = await file.OpenWriteAsync();
                        using var writer = new StreamWriter(stream, new UTF8Encoding(true));
                        await writer.WriteAsync(csv.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка экспорта: {ex.Message}");
            }
        }

        private string EscapeCsv(string? value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        private void GoToMainPage()
        {
            if (MainWindowViewModel.Instance != null)
                MainWindowViewModel.Instance.Uc = new MainPage();
        }

        private void GoToAdminPage()
        {
            if (MainWindowViewModel.Instance != null)
                MainWindowViewModel.Instance.Uc = new AdminPage();
        }

        private void GoToCompanies()
        {
            if (MainWindowViewModel.Instance != null)
                MainWindowViewModel.Instance.Uc = new CompanyPage();
        }
    }
}