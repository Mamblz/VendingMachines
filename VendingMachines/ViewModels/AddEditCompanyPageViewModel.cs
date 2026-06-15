using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using VendingMachines.Models;

namespace VendingMachines.ViewModels
{
    public class AddEditCompanyPageViewModel : ReactiveObject
    {
        private readonly _43pKobzarContext _readContext;
        private bool _isEditMode;

        public bool IsEditMode
        {
            get => _isEditMode;
            private set => this.RaiseAndSetIfChanged(ref _isEditMode, value);
        }

        private Company _currentCompany;
        public Company CurrentCompany
        {
            get => _currentCompany;
            set => this.RaiseAndSetIfChanged(ref _currentCompany, value);
        }

        private DateTimeOffset? _selectedDate;
        public DateTimeOffset? SelectedDate
        {
            get => _selectedDate;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedDate, value);
                CurrentCompany.InWorkingWith = value?.DateTime is DateTime dt ? DateOnly.FromDateTime(dt) : null;
            }
        }

        public string PageTitle => IsEditMode ? "Редактирование компании" : "Добавление компании";
        public string ButtonText => IsEditMode ? "Сохранить" : "Добавить";

        private ObservableCollection<Highcompany> _highCompanies = new();
        public ObservableCollection<Highcompany> HighCompanies
        {
            get => _highCompanies;
            set => this.RaiseAndSetIfChanged(ref _highCompanies, value);
        }

        private Highcompany? _selectedHighCompany;
        public Highcompany? SelectedHighCompany
        {
            get => _selectedHighCompany;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedHighCompany, value);
                CurrentCompany.HighCompanyId = value?.HighcomapnyId;
            }
        }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public AddEditCompanyPageViewModel(_43pKobzarContext context) : this(null, context) { }
        public AddEditCompanyPageViewModel(Company? company = null) : this(company, new _43pKobzarContext()) { }

        public AddEditCompanyPageViewModel(Company? company, _43pKobzarContext context)
        {
            _readContext = context;
            IsEditMode = company != null;

            if (company != null)
            {
                _currentCompany = new Company
                {
                    Id = company.Id,
                    Name = company.Name,
                    Address = company.Address,
                    Contacts = company.Contacts,
                    Notes = company.Notes,
                    InWorkingWith = company.InWorkingWith,
                    HighCompanyId = company.HighCompanyId
                };
                if (company.InWorkingWith.HasValue)
                    _selectedDate = new DateTimeOffset(company.InWorkingWith.Value.ToDateTime(TimeOnly.MinValue));
            }
            else
            {
                _currentCompany = new Company();
            }

            LoadLookups();
            SaveCommand = ReactiveCommand.Create(Save);
            CancelCommand = ReactiveCommand.Create(Cancel);
        }

        private void LoadLookups()
        {
            try
            {
                var highCompanies = _readContext.Highcompanies.AsNoTracking().ToList();
                HighCompanies = new ObservableCollection<Highcompany>(highCompanies);
                SelectedHighCompany = HighCompanies.FirstOrDefault(x => x.HighcomapnyId == CurrentCompany.HighCompanyId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки справочников: {ex.Message}");
            }
        }

        private void Save()
        {
            if (!Validate()) return;

            try
            {
                if (IsEditMode)
                {
                    var existing = _readContext.Companies.FirstOrDefault(x => x.Id == CurrentCompany.Id);
                    if (existing == null) return;

                    existing.Name = CurrentCompany.Name;
                    existing.Address = CurrentCompany.Address;
                    existing.Contacts = CurrentCompany.Contacts;
                    existing.Notes = CurrentCompany.Notes;
                    existing.InWorkingWith = CurrentCompany.InWorkingWith;
                    existing.HighCompanyId = CurrentCompany.HighCompanyId;

                    _readContext.Entry(existing).State = EntityState.Modified;
                }
                else
                {
                    CurrentCompany.Id = Guid.NewGuid();
                    _readContext.Companies.Add(CurrentCompany);
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
            return !string.IsNullOrWhiteSpace(CurrentCompany.Name) &&
                   !string.IsNullOrWhiteSpace(CurrentCompany.Address) &&
                   SelectedHighCompany != null;
        }

        private void Cancel() => NavigateBack();

        private void NavigateBack()
        {
            if (MainWindowViewModel.Instance != null)
                MainWindowViewModel.Instance.Uc = new CompanyPage();
        }
    }
}