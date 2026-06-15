using DynamicData;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using VendingMachines.Models;

namespace VendingMachines.ViewModels
{
    public class AddEditMachinePageViewModel : ReactiveObject
    {
        private readonly _43pKobzarContext _readContext;
        private bool _isEditMode;

        public bool IsEditMode
        {
            get => _isEditMode;
            private set => this.RaiseAndSetIfChanged(ref _isEditMode, value);
        }

        private VendingMachine _currentMachine;
        public VendingMachine CurrentMachine
        {
            get => _currentMachine;
            set => this.RaiseAndSetIfChanged(ref _currentMachine, value);
        }

        public string PageTitle => IsEditMode ? "Редактирование автомата" : "Добавление автомата";
        public string ButtonText => IsEditMode ? "Сохранить" : "Добавить";

        private string _companyLocationName = string.Empty;
        public string CompanyLocationName
        {
            get => _companyLocationName;
            set => this.RaiseAndSetIfChanged(ref _companyLocationName, value);
        }

        private ObservableCollection<Model> _models = new();
        public ObservableCollection<Model> Models
        {
            get => _models;
            set => this.RaiseAndSetIfChanged(ref _models, value);
        }

        private ObservableCollection<CriticalThresholdTemplate> _criticalThresholdTemplates = new();
        public ObservableCollection<CriticalThresholdTemplate> CriticalThresholdTemplates
        {
            get => _criticalThresholdTemplates;
            set => this.RaiseAndSetIfChanged(ref _criticalThresholdTemplates, value);
        }

        private CriticalThresholdTemplate? _selectedCriticalThresholdTemplate;
        public CriticalThresholdTemplate? SelectedCriticalThresholdTemplate
        {
            get => _selectedCriticalThresholdTemplate;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedCriticalThresholdTemplate, value);
                CurrentMachine.CriticalThresholdTemplateId = value?.Id;
            }
        }

        private ObservableCollection<NotificationTemplate> _notificationTemplates = new();
        public ObservableCollection<NotificationTemplate> NotificationTemplates
        {
            get => _notificationTemplates;
            set => this.RaiseAndSetIfChanged(ref _notificationTemplates, value);
        }

        private NotificationTemplate? _selectedNotificationTemplate;
        public NotificationTemplate? SelectedNotificationTemplate
        {
            get => _selectedNotificationTemplate;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedNotificationTemplate, value);
                CurrentMachine.NotificationTemplateId = value?.Id;
            }
        }

        private ObservableCollection<PaymentTypeSelection> _paymentTypes = new();
        public ObservableCollection<PaymentTypeSelection> PaymentTypes
        {
            get => _paymentTypes;
            set => this.RaiseAndSetIfChanged(ref _paymentTypes, value);
        }

        private ObservableCollection<Company> _companies = new();
        public ObservableCollection<Company> Companies
        {
            get => _companies;
            set => this.RaiseAndSetIfChanged(ref _companies, value);
        }

        private Company? _selectedCompany;
        public Company? SelectedCompany
        {
            get => _selectedCompany;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedCompany, value);
                CurrentMachine.CompanyId = value?.Id;
            }
        }

        private ObservableCollection<Place> _places = new();
        public ObservableCollection<Place> Places
        {
            get => _places;
            set => this.RaiseAndSetIfChanged(ref _places, value);
        }

        private Place? _selectedPlace;
        public Place? SelectedPlace
        {
            get => _selectedPlace;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedPlace, value);
                CurrentMachine.PlaceId = value?.Id;
            }
        }

        private ObservableCollection<User> _managers = new();
        public ObservableCollection<User> Managers
        {
            get => _managers;
            set => this.RaiseAndSetIfChanged(ref _managers, value);
        }

        private User? _selectedManager;
        public User? SelectedManager
        {
            get => _selectedManager;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedManager, value);
                CurrentMachine.ManagerId = value?.Id;
            }
        }

        private ObservableCollection<User> _engineers = new();
        public ObservableCollection<User> Engineers
        {
            get => _engineers;
            set => this.RaiseAndSetIfChanged(ref _engineers, value);
        }

        private User? _selectedEngineer;
        public User? SelectedEngineer
        {
            get => _selectedEngineer;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedEngineer, value);
                CurrentMachine.EngineerId = value?.Id;
            }
        }

        private ObservableCollection<User> _technicians = new();
        public ObservableCollection<User> Technicians
        {
            get => _technicians;
            set => this.RaiseAndSetIfChanged(ref _technicians, value);
        }

        private User? _selectedTechnician;
        public User? SelectedTechnician
        {
            get => _selectedTechnician;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedTechnician, value);
                CurrentMachine.TechnicianId = value?.Id;
            }
        }

        private ObservableCollection<WorkMode> _workModes = new();
        public ObservableCollection<WorkMode> WorkModes
        {
            get => _workModes;
            set => this.RaiseAndSetIfChanged(ref _workModes, value);
        }

        private WorkMode? _selectedWorkMode;
        public WorkMode? SelectedWorkMode
        {
            get => _selectedWorkMode;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedWorkMode, value);
                CurrentMachine.WorkModeId = value?.Id;
            }
        }

        private ObservableCollection<ServicePriority> _priorities = new();
        public ObservableCollection<ServicePriority> Priorities
        {
            get => _priorities;
            set => this.RaiseAndSetIfChanged(ref _priorities, value);
        }

        private ServicePriority? _selectedPriority;
        public ServicePriority? SelectedPriority
        {
            get => _selectedPriority;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedPriority, value);
                CurrentMachine.ServicePriorityId = value?.Id;
            }
        }

        private ObservableCollection<Timezone> _timezones = new();
        public ObservableCollection<Timezone> Timezones
        {
            get => _timezones;
            set => this.RaiseAndSetIfChanged(ref _timezones, value);
        }

        private Timezone? _selectedTimezone;
        public Timezone? SelectedTimezone
        {
            get => _selectedTimezone;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedTimezone, value);
                CurrentMachine.TimezoneId = value?.Id;
            }
        }

        private Model? _selectedModel;
        public Model? SelectedModel
        {
            get => _selectedModel;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedModel, value);
                CurrentMachine.ModelId = value?.Id;
            }
        }

        private string _noteText = string.Empty;
        public string NoteText
        {
            get => _noteText;
            set => this.RaiseAndSetIfChanged(ref _noteText, value);
        }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public AddEditMachinePageViewModel(_43pKobzarContext context) : this(null, context) { }
        public AddEditMachinePageViewModel(VendingMachine? machine = null) : this(machine, new _43pKobzarContext()) { }

        public AddEditMachinePageViewModel(VendingMachine? machine, _43pKobzarContext context)
        {
            _readContext = context;
            IsEditMode = machine != null;
            _currentMachine = machine ?? new VendingMachine();

            if (machine?.Companylocation != null)
            {
                CompanyLocationName = machine.Companylocation.Name;
            }

            LoadLookups();

            SaveCommand = ReactiveCommand.Create(Save);
            CancelCommand = ReactiveCommand.Create(Cancel);
        }

        private void LoadLookups()
        {
            try
            {
                Models = new ObservableCollection<Model>(_readContext.Models.AsNoTracking().ToList());
                Companies = new ObservableCollection<Company>(_readContext.Companies.AsNoTracking().ToList());
                Places = new ObservableCollection<Place>(_readContext.Places.AsNoTracking().ToList());
                WorkModes = new ObservableCollection<WorkMode>(_readContext.WorkModes.AsNoTracking().ToList());
                Priorities = new ObservableCollection<ServicePriority>(_readContext.ServicePriorities.AsNoTracking().ToList());
                Timezones = new ObservableCollection<Timezone>(_readContext.Timezones.AsNoTracking().ToList());

                var allUsers = _readContext.Users.AsNoTracking().ToList();
                Managers = new ObservableCollection<User>(allUsers.Where(u => u.IsManager));
                Engineers = new ObservableCollection<User>(allUsers.Where(u => u.IsEngineer));
                Technicians = new ObservableCollection<User>(allUsers.Where(u => u.IsOperator));

                CriticalThresholdTemplates = new ObservableCollection<CriticalThresholdTemplate>(_readContext.CriticalThresholdTemplates.AsNoTracking().ToList());
                NotificationTemplates = new ObservableCollection<NotificationTemplate>(_readContext.NotificationTemplates.AsNoTracking().ToList());

                var paymentTypes = _readContext.PaymentTypes.AsNoTracking().ToList();
                var selectedPaymentTypeIds = _readContext.VendingMachinePayments
                    .Where(vmp => vmp.VendingMachineId == CurrentMachine.Id)
                    .Select(vmp => vmp.PaymentTypeId)
                    .ToHashSet();
                PaymentTypes = new ObservableCollection<PaymentTypeSelection>(
                    paymentTypes.Select(pt => new PaymentTypeSelection
                    {
                        PaymentType = pt,
                        IsSelected = selectedPaymentTypeIds.Contains(pt.Id)
                    }));

                SelectedModel = Models.FirstOrDefault(x => x.Id == CurrentMachine.ModelId);
                SelectedCompany = Companies.FirstOrDefault(x => x.Id == CurrentMachine.CompanyId);
                SelectedPlace = Places.FirstOrDefault(x => x.Id == CurrentMachine.PlaceId);
                SelectedManager = Managers.FirstOrDefault(x => x.Id == CurrentMachine.ManagerId);
                SelectedEngineer = Engineers.FirstOrDefault(x => x.Id == CurrentMachine.EngineerId);
                SelectedTechnician = Technicians.FirstOrDefault(x => x.Id == CurrentMachine.TechnicianId);
                SelectedWorkMode = WorkModes.FirstOrDefault(x => x.Id == CurrentMachine.WorkModeId);
                SelectedPriority = Priorities.FirstOrDefault(x => x.Id == CurrentMachine.ServicePriorityId);
                SelectedTimezone = Timezones.FirstOrDefault(x => x.Id == CurrentMachine.TimezoneId);
                SelectedCriticalThresholdTemplate = CriticalThresholdTemplates.FirstOrDefault(x => x.Id == CurrentMachine.CriticalThresholdTemplateId);
                SelectedNotificationTemplate = NotificationTemplates.FirstOrDefault(x => x.Id == CurrentMachine.NotificationTemplateId);

                if (CurrentMachine.Notes != null)
                {
                    NoteText = CurrentMachine.Notes.Name;
                }
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
                CompanyLocation? location = null;
                if (!string.IsNullOrWhiteSpace(CompanyLocationName))
                {
                    location = _readContext.CompanyLocations
                        .FirstOrDefault(cl => cl.Name == CompanyLocationName);

                    if (location == null)
                    {
                        location = new CompanyLocation
                        {
                            Id = Guid.NewGuid(),
                            Name = CompanyLocationName
                        };
                        _readContext.CompanyLocations.Add(location);
                    }
                    CurrentMachine.CompanylocationId = location.Id;
                }

                if (IsEditMode)
                {
                    var existing = _readContext.VendingMachines
                        .Include(vm => vm.Notes)
                        .Include(vm => vm.VendingMachinePayments)
                        .FirstOrDefault(x => x.Id == CurrentMachine.Id);

                    if (existing == null) return;

                    _readContext.Entry(existing).CurrentValues.SetValues(CurrentMachine);

                    if (!string.IsNullOrWhiteSpace(NoteText))
                    {
                        if (existing.Notes == null)
                        {
                            existing.Notes = new NotesVending
                            {
                                Id = Guid.NewGuid(),
                                Name = NoteText
                            };
                        }
                        else
                        {
                            existing.Notes.Name = NoteText;
                            _readContext.Entry(existing.Notes).State = EntityState.Modified;
                        }
                    }
                    else
                    {
                        if (existing.Notes != null)
                        {
                            _readContext.NotesVendings.Remove(existing.Notes);
                            existing.Notes = null;
                            existing.NotesId = null;
                        }
                    }

                    var selectedPaymentTypeIds = PaymentTypes.Where(pt => pt.IsSelected).Select(pt => pt.PaymentType.Id).ToHashSet();
                    var currentPayments = existing.VendingMachinePayments.ToList();

                    foreach (var payment in currentPayments.Where(p => !selectedPaymentTypeIds.Contains(p.PaymentTypeId)).ToList())
                        _readContext.VendingMachinePayments.Remove(payment);
                    foreach (var typeId in selectedPaymentTypeIds.Where(id => !currentPayments.Any(p => p.PaymentTypeId == id)))
                    {
                        _readContext.VendingMachinePayments.Add(new VendingMachinePayment
                        {
                            Id = Guid.NewGuid(),
                            VendingMachineId = existing.Id,
                            PaymentTypeId = typeId
                        });
                    }

                    _readContext.Entry(existing).State = EntityState.Modified;
                }
                else
                {
                    CurrentMachine.Id = Guid.NewGuid();

                    if (!string.IsNullOrWhiteSpace(NoteText))
                    {
                        var notes = new NotesVending
                        {
                            Id = Guid.NewGuid(),
                            Name = NoteText
                        };
                        CurrentMachine.Notes = notes;
                        CurrentMachine.NotesId = notes.Id;
                    }

                    _readContext.VendingMachines.Add(CurrentMachine);

                    foreach (var paymentType in PaymentTypes.Where(pt => pt.IsSelected))
                    {
                        _readContext.VendingMachinePayments.Add(new VendingMachinePayment
                        {
                            Id = Guid.NewGuid(),
                            VendingMachineId = CurrentMachine.Id,
                            PaymentTypeId = paymentType.PaymentType.Id
                        });
                    }
                }

                _readContext.SaveChanges();

                if (MainWindowViewModel.Instance != null)
                    MainWindowViewModel.Instance.Uc = new AdminPage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения: {ex.Message}");
            }
        }

        private bool Validate()
        {
            return !string.IsNullOrWhiteSpace(CompanyLocationName)
                   && SelectedModel != null
                   && SelectedCompany != null
                   && SelectedWorkMode != null
                   && SelectedTimezone != null
                   && SelectedPlace != null
                   && !string.IsNullOrWhiteSpace(CurrentMachine.Location)
                   && !string.IsNullOrWhiteSpace(CurrentMachine.SerialNumber);
        }

        private void Cancel()
        {
            if (MainWindowViewModel.Instance != null)
                MainWindowViewModel.Instance.Uc = new AdminPage();
        }
    }

    public class PaymentTypeSelection
    {
        public PaymentType PaymentType { get; set; } = null!;
        public bool IsSelected { get; set; }
        public string Name => PaymentType.Name;
    }
}