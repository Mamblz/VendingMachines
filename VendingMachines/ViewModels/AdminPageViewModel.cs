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
    public class AdminPageViewModel : ReactiveObject
    {
        private readonly _43pKobzarContext _context;
        private readonly bool _skipConfirmation;
        private List<VendingMachine> _allMachines = new();
        private ObservableCollection<VendingMachine> _pagedMachines = new();

        private int _pageSize = 50;
        private int _currentPage = 1;
        private int _totalItems;
        private string _filterText = string.Empty;
        private ViewMode _viewMode = ViewMode.Table;

        public ReactiveCommand<Unit, Unit> AddMachineCommand { get; }
        public ReactiveCommand<VendingMachine, Unit> EditMachineCommand { get; }
        public ReactiveCommand<VendingMachine, Unit> DeleteMachineCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }
        public ReactiveCommand<Unit, Unit> PrevPageCommand { get; }
        public ReactiveCommand<Unit, Unit> NextPageCommand { get; }
        public ReactiveCommand<Unit, Unit> GoToMainPageCommand { get; }
        public ReactiveCommand<Unit, Unit> GoToCompaniesCommand { get; }
        public ReactiveCommand<Unit, Unit> GoToUsersCommand { get; }
        public ReactiveCommand<Unit, Unit> SetTileViewCommand { get; }
        public ReactiveCommand<Unit, Unit> SetTableViewCommand { get; }

        public ObservableCollection<VendingMachine> PagedMachines
        {
            get => _pagedMachines;
            set => this.RaiseAndSetIfChanged(ref _pagedMachines, value);
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

        public string FilterText
        {
            get => _filterText;
            set
            {
                this.RaiseAndSetIfChanged(ref _filterText, value);
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
                if (PagedMachines.Count == 0)
                    return $"Найдено: 0 из {TotalItems}";

                int start = (CurrentPage - 1) * (PageSize == int.MaxValue ? TotalItems : PageSize) + 1;
                int end = Math.Min(start + PagedMachines.Count - 1, TotalItems);
                return $"Показано {start}–{end} из {TotalItems}";
            }
        }

        public AdminPageViewModel(_43pKobzarContext? context = null) : this(context, false) { }

        public AdminPageViewModel(_43pKobzarContext? context, bool skipConfirmation)
        {
            _context = context ?? new _43pKobzarContext();
            _skipConfirmation = skipConfirmation;
            LoadVendingMachines();

            AddMachineCommand = ReactiveCommand.Create(AddMachine);
            EditMachineCommand = ReactiveCommand.Create<VendingMachine>(EditMachine);
            DeleteMachineCommand = ReactiveCommand.Create<VendingMachine>(DeleteMachine);
            ExportCommand = ReactiveCommand.Create(Export);

            PrevPageCommand = ReactiveCommand.Create(PrevPage, this.WhenAnyValue(x => x.CanGoPrev));
            NextPageCommand = ReactiveCommand.Create(NextPage, this.WhenAnyValue(x => x.CanGoNext));
            GoToMainPageCommand = ReactiveCommand.Create(GoToMainPage);
            GoToCompaniesCommand = ReactiveCommand.Create(GoToCompanies);
            GoToUsersCommand = ReactiveCommand.Create(GoToUsers);

            SetTileViewCommand = ReactiveCommand.Create(() => { ViewMode = ViewMode.Tile; });
            SetTableViewCommand = ReactiveCommand.Create(() => { ViewMode = ViewMode.Table; });

            this.WhenAnyValue(
                x => x.PagedMachines.Count,
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

        public void LoadVendingMachines()
        {
            try
            {
                _allMachines = _context.VendingMachines
                    .Include(vm => vm.Company)
                    .Include(vm => vm.Companylocation)
                    .Include(vm => vm.Model)
                    .Include(vm => vm.Place)
                    .Include(vm => vm.ServicePriority)
                    .Include(vm => vm.Timezone)
                    .Include(vm => vm.WorkMode)
                    .Include(vm => vm.Notes)
                    .ToList();

                ApplyFilterAndPaging();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void ApplyFilterAndPaging()
        {
            IEnumerable<VendingMachine> filtered = _allMachines;

            if (!string.IsNullOrWhiteSpace(FilterText))
            {
                var filter = FilterText.ToLower();
                filtered = filtered.Where(vm =>
                    (vm.Companylocation?.Name?.ToLower().Contains(filter) ?? false));
            }

            TotalItems = filtered.Count();

            IEnumerable<VendingMachine> paged;
            if (PageSize == int.MaxValue)
            {
                paged = filtered;
            }
            else
            {
                paged = filtered.Skip((CurrentPage - 1) * PageSize).Take(PageSize);
            }

            PagedMachines = new ObservableCollection<VendingMachine>(paged);
            this.RaisePropertyChanged(nameof(TotalPages));
            this.RaisePropertyChanged(nameof(CanGoPrev));
            this.RaisePropertyChanged(nameof(CanGoNext));
        }

        private void PrevPage() => CurrentPage--;
        private void NextPage() => CurrentPage++;

        private void AddMachine() => MainWindowViewModel.Instance.Uc = new AddEditMachinePage();
        private void EditMachine(VendingMachine machine) => MainWindowViewModel.Instance.Uc = new AddEditMachinePage(machine);

        private async void DeleteMachine(VendingMachine machine)
        {
            try
            {
                var machineToDelete = await _context.VendingMachines
                    .Include(vm => vm.Products)
                    .Include(vm => vm.Maintenances)
                    .Include(vm => vm.VendingMachinePayments)
                    .FirstOrDefaultAsync(vm => vm.Id == machine.Id);

                if (machineToDelete == null)
                    return;

                bool shouldDelete = _skipConfirmation;

                if (!shouldDelete)
                {
                    var box = MessageBoxManager.GetMessageBoxStandard(
                        "Подтверждение удаления",
                        $"Вы действительно хотите удалить автомат \"{machine.Companylocation?.Name}\"?\nВсе связанные записи (продукты, обслуживание и т.д.) будут также удалены.",
                        ButtonEnum.YesNo,
                        Icon.Question);
                    var result = await box.ShowAsync();
                    shouldDelete = result == ButtonResult.Yes;
                }

                if (shouldDelete)
                {
                    if (machineToDelete.Products?.Any() == true)
                        _context.Products.RemoveRange(machineToDelete.Products);
                    if (machineToDelete.Maintenances?.Any() == true)
                        _context.Maintenances.RemoveRange(machineToDelete.Maintenances);
                    if (machineToDelete.VendingMachinePayments?.Any() == true)
                        _context.VendingMachinePayments.RemoveRange(machineToDelete.VendingMachinePayments);

                    _context.VendingMachines.Remove(machineToDelete);
                    await _context.SaveChangesAsync();
                    LoadVendingMachines();
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

        private async void Export()
        {
            try
            {
                var csv = new StringBuilder();
                csv.AppendLine("ID;Название автомата;Модель;Компания;Адрес;Место;Дата установки");

                foreach (var vm in _allMachines)
                {
                    csv.AppendLine($"{vm.Id};{EscapeCsv(vm.Companylocation?.Name)};{EscapeCsv(vm.Model?.Name)};{EscapeCsv(vm.Company?.Name)};{EscapeCsv(vm.Location)};{EscapeCsv(vm.Place?.Name)};{vm.InstallDate?.ToString("dd.MM.yyyy")}");
                }

                var topLevel = Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
                    ? desktop.MainWindow
                    : null;

                if (topLevel != null)
                {
                    var file = await topLevel.StorageProvider.SaveFilePickerAsync(new Avalonia.Platform.Storage.FilePickerSaveOptions
                    {
                        Title = "Сохранить CSV",
                        SuggestedFileName = "vending_machines.csv",
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

        private void GoToCompanies()
        {
            if (MainWindowViewModel.Instance != null)
                MainWindowViewModel.Instance.Uc = new CompanyPage();
        }

        private void GoToUsers()
        {
            if (MainWindowViewModel.Instance != null)
                MainWindowViewModel.Instance.Uc = new UsersPage();
        }
    }
}