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
    public enum ViewMode
    {
        Table,
        Tile
    }

    public class CompanyPageViewModel : ReactiveObject
    {
        private readonly _43pKobzarContext _context;
        private readonly bool _skipConfirmation;
        private List<Company> _allCompanies = new();
        private ObservableCollection<Company> _pagedCompanies = new();

        private int _pageSize = 50;
        private int _currentPage = 1;
        private int _totalItems;
        private string _searchText = string.Empty;
        private ViewMode _viewMode = ViewMode.Table;

        public ReactiveCommand<Unit, Unit> AddCompanyCommand { get; }
        public ReactiveCommand<Company, Unit> EditCompanyCommand { get; }
        public ReactiveCommand<Company, Unit> DeleteCompanyCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }
        public ReactiveCommand<Unit, Unit> PrevPageCommand { get; }
        public ReactiveCommand<Unit, Unit> NextPageCommand { get; }
        public ReactiveCommand<Unit, Unit> GoToMainPageCommand { get; }
        public ReactiveCommand<Unit, Unit> GoToAdminPageCommand { get; }
        public ReactiveCommand<Unit, Unit> GoToUsersCommand { get; }
        public ReactiveCommand<Unit, Unit> SetTileViewCommand { get; }
        public ReactiveCommand<Unit, Unit> SetTableViewCommand { get; }

        public ObservableCollection<Company> PagedCompanies
        {
            get => _pagedCompanies;
            set => this.RaiseAndSetIfChanged(ref _pagedCompanies, value);
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
                if (PagedCompanies.Count == 0)
                    return $"Найдено: 0 из {TotalItems}";

                int start = (CurrentPage - 1) * (PageSize == int.MaxValue ? TotalItems : PageSize) + 1;
                int end = Math.Min(start + PagedCompanies.Count - 1, TotalItems);
                return $"Показано {start}–{end} из {TotalItems}";
            }
        }

        public CompanyPageViewModel() : this(new _43pKobzarContext(), false) { }

        public CompanyPageViewModel(_43pKobzarContext context, bool skipConfirmation = false)
        {
            _context = context;
            _skipConfirmation = skipConfirmation;
            LoadCompanies();

            AddCompanyCommand = ReactiveCommand.Create(AddCompany);
            EditCompanyCommand = ReactiveCommand.Create<Company>(EditCompany);
            DeleteCompanyCommand = ReactiveCommand.Create<Company>(DeleteCompany);
            ExportCommand = ReactiveCommand.CreateFromTask(Export);
            PrevPageCommand = ReactiveCommand.Create(PrevPage, this.WhenAnyValue(x => x.CanGoPrev));
            NextPageCommand = ReactiveCommand.Create(NextPage, this.WhenAnyValue(x => x.CanGoNext));
            GoToMainPageCommand = ReactiveCommand.Create(GoToMainPage);
            GoToAdminPageCommand = ReactiveCommand.Create(GoToAdminPage);
            GoToUsersCommand = ReactiveCommand.Create(GoToUsers);

            SetTileViewCommand = ReactiveCommand.Create(() => { ViewMode = ViewMode.Tile; });
            SetTableViewCommand = ReactiveCommand.Create(() => { ViewMode = ViewMode.Table; });

            this.WhenAnyValue(
                x => x.PagedCompanies.Count,
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

        private void LoadCompanies()
        {
            try
            {
                _allCompanies = _context.Companies
                    .Include(c => c.HighCompany)
                    .ToList();

                ApplyFilterAndPaging();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки компаний: {ex.Message}");
            }
        }

        private void ApplyFilterAndPaging()
        {
            IEnumerable<Company> filtered = _allCompanies;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var search = SearchText.ToLower();
                filtered = filtered.Where(c =>
                    (c.Name?.ToLower().Contains(search) ?? false) ||
                    (c.Address?.ToLower().Contains(search) ?? false) ||
                    (c.Contacts?.ToLower().Contains(search) ?? false) ||
                    (c.HighCompany?.Name?.ToLower().Contains(search) ?? false));
            }

            TotalItems = filtered.Count();

            IEnumerable<Company> paged;
            if (PageSize == int.MaxValue)
            {
                paged = filtered;
            }
            else
            {
                paged = filtered.Skip((CurrentPage - 1) * PageSize).Take(PageSize);
            }

            PagedCompanies = new ObservableCollection<Company>(paged);
            this.RaisePropertyChanged(nameof(TotalPages));
            this.RaisePropertyChanged(nameof(CanGoPrev));
            this.RaisePropertyChanged(nameof(CanGoNext));
        }

        private void PrevPage() => CurrentPage--;
        private void NextPage() => CurrentPage++;

        private void AddCompany()
        {
            if (MainWindowViewModel.Instance != null)
                MainWindowViewModel.Instance.Uc = new AddEditCompanyPage();
        }

        private void EditCompany(Company company)
        {
            if (MainWindowViewModel.Instance != null)
                MainWindowViewModel.Instance.Uc = new AddEditCompanyPage(company);
        }

        private async void DeleteCompany(Company company)
        {
            try
            {
                var companyToDelete = await _context.Companies
                    .Include(c => c.VendingMachines)
                    .FirstOrDefaultAsync(c => c.Id == company.Id);

                if (companyToDelete == null)
                    return;

                if (companyToDelete.VendingMachines?.Any() == true)
                {
                    if (!_skipConfirmation)
                    {
                        var errorBox = MessageBoxManager.GetMessageBoxStandard(
                            "Ошибка удаления",
                            "Невозможно удалить компанию, так как с ней связаны торговые автоматы.",
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
                        $"Вы действительно хотите удалить компанию \"{company.Name}\"?",
                        ButtonEnum.YesNo,
                        Icon.Question);

                    var result = await box.ShowAsync();
                    shouldDelete = result == ButtonResult.Yes;
                }

                if (shouldDelete)
                {
                    _context.Companies.Remove(companyToDelete);
                    await _context.SaveChangesAsync();
                    LoadCompanies();
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
                csv.AppendLine("ID;Название;Адрес;Контакты;В работе с;Вышестоящая компания");

                foreach (var c in _allCompanies)
                {
                    csv.AppendLine($"{c.Id};{EscapeCsv(c.Name)};{EscapeCsv(c.Address)};{EscapeCsv(c.Contacts)};{c.InWorkingWith?.ToString("dd.MM.yyyy")};{EscapeCsv(c.HighCompany?.Name)}");
                }

                var topLevel = Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
                    ? desktop.MainWindow
                    : null;

                if (topLevel != null)
                {
                    var file = await topLevel.StorageProvider.SaveFilePickerAsync(new Avalonia.Platform.Storage.FilePickerSaveOptions
                    {
                        Title = "Сохранить CSV",
                        SuggestedFileName = "companies.csv",
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

        private void GoToUsers()
        {
            if (MainWindowViewModel.Instance != null)
                MainWindowViewModel.Instance.Uc = new UsersPage();
        }
    }
}