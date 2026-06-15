using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using VendingMachines.Models;
using VendingMachines.ViewModels;

namespace VendingMachines.Tests
{
    public abstract class TestBase
    {
        protected _43pKobzarContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<_43pKobzarContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new _43pKobzarContext(options);
        }

        protected void SeedVendingMachines(_43pKobzarContext context)
        {
            var companyLoc = new CompanyLocation { Id = Guid.NewGuid(), Name = "Офис" };
            var model = new Model { Id = Guid.NewGuid(), Name = "Model X" };
            var company = new Company { Id = Guid.NewGuid(), Name = "Технопром" };
            var place = new Place { Id = Guid.NewGuid(), Name = "Холл" };
            context.CompanyLocations.Add(companyLoc);
            context.Models.Add(model);
            context.Companies.Add(company);
            context.Places.Add(place);
            context.VendingMachines.Add(new VendingMachine
            {
                Id = Guid.NewGuid(),
                SerialNumber = "SN001",
                Companylocation = companyLoc,
                Model = model,
                Company = company,
                Place = place,
                Location = "ул. Ленина, 1",
                InstallDate = new DateOnly(2023, 1, 1)
            });
            context.SaveChanges();
        }

        protected void SeedCompanies(_43pKobzarContext context)
        {
            var highCompanies = context.Highcompanies.ToList();
            var high1 = highCompanies.FirstOrDefault();
            var high2 = highCompanies.Skip(1).FirstOrDefault();

            context.Companies.AddRange(
                new Company { Id = Guid.NewGuid(), Name = "ООО Ромашка", HighCompanyId = high1?.HighcomapnyId },
                new Company { Id = Guid.NewGuid(), Name = "ИП Василёк", HighCompanyId = high2?.HighcomapnyId }
            );
            context.SaveChanges();
        }

        protected void SeedHighCompanies(_43pKobzarContext context)
        {
            context.Highcompanies.AddRange(
                new Highcompany { HighcomapnyId = Guid.NewGuid(), Name = "Холдинг 1" },
                new Highcompany { HighcomapnyId = Guid.NewGuid(), Name = "Холдинг 2" }
            );
            context.SaveChanges();
        }

        protected void SeedUsers(_43pKobzarContext context)
        {
            var role = new UserRole { Id = Guid.NewGuid(), Name = "Admin" };
            context.UserRoles.Add(role);
            context.Users.AddRange(
                new User { Id = Guid.NewGuid(), FullName = "Иван Петров", Email = "ivan@test.com", Role = role },
                new User { Id = Guid.NewGuid(), FullName = "Мария Сидорова", Email = "maria@test.com", Role = role }
            );
            context.SaveChanges();
        }
    }

    [TestClass]
    public class AdminPageViewModelTests : TestBase
    {
        private _43pKobzarContext _context;
        private AdminPageViewModel _vm;

        [TestInitialize]
        public void Setup()
        {
            _context = CreateInMemoryContext();
            SeedVendingMachines(_context);
            _vm = new AdminPageViewModel(_context, skipConfirmation: true);
        }

        [TestCleanup] public void Cleanup() => _context.Dispose();

        [TestMethod]
        public void LoadVendingMachines_ShouldPopulatePagedMachines()
        {
            Assert.AreEqual(1, _vm!.PagedMachines.Count);
        }

        [TestMethod]
        public void FilterText_FiltersByName()
        {
            _vm!.FilterText = "Офис";
            Assert.AreEqual(1, _vm.PagedMachines.Count);
        }

        [TestMethod]
        public void PageSize_Change_ResetsCurrentPage()
        {
            for (int i = 0; i < 15; i++)
                _context!.VendingMachines.Add(new VendingMachine { Id = Guid.NewGuid(), SerialNumber = $"SN{i}" });
            _context!.SaveChanges();
            _vm = new AdminPageViewModel(_context);
            _vm.PageSize = 5;
            _vm.NextPageCommand.Execute().Subscribe();
            Assert.AreEqual(2, _vm.CurrentPage);
            _vm.PageSize = 10;
            Assert.AreEqual(1, _vm.CurrentPage);
        }

        [TestMethod]
        public async Task DeleteMachine_RemovesMachine()
        {
            var machine = _context.VendingMachines.First();
            await _vm.DeleteMachineCommand.Execute(machine).ToTask();
            Assert.AreEqual(0, _context.VendingMachines.Count());
        }
    }

    [TestClass]
    public class CompanyPageViewModelTests : TestBase
    {
        private _43pKobzarContext _context;
        private CompanyPageViewModel _vm;

        [TestInitialize]
        public void Setup()
        {
            _context = CreateInMemoryContext();
            SeedHighCompanies(_context);
            SeedCompanies(_context);
            _vm = new CompanyPageViewModel(_context, skipConfirmation: true);
        }

        [TestCleanup] public void Cleanup() => _context?.Dispose();

        [TestMethod]
        public void LoadCompanies_IncludesHighCompany()
        {
            Assert.AreEqual(2, _vm!.PagedCompanies.Count);
            Assert.IsNotNull(_vm.PagedCompanies.First().HighCompany);
        }

        [TestMethod]
        public void SearchText_FiltersByName()
        {
            _vm!.SearchText = "Ромашка";
            Assert.AreEqual(1, _vm.PagedCompanies.Count);
            Assert.AreEqual("ООО Ромашка", _vm.PagedCompanies[0].Name);
        }

        [TestMethod]
        public async Task DeleteCompany_WithNoVendingMachines_RemovesCompany()
        {
            var company = _context.Companies.First();
            await _vm.DeleteCompanyCommand.Execute(company).ToTask();
            Assert.AreEqual(1, _context.Companies.Count());
        }

        [TestMethod]
        public async Task DeleteCompany_WithVendingMachines_DoesNotRemove()
        {
            var company = _context!.Companies.First();
            _context.VendingMachines.Add(new VendingMachine { Id = Guid.NewGuid(), SerialNumber = "SN001", CompanyId = company.Id });
            _context.SaveChanges();
            _vm = new CompanyPageViewModel(_context);
            await _vm.DeleteCompanyCommand.Execute(company).ToTask();
            Assert.AreEqual(2, _context.Companies.Count());
        }
    }

    [TestClass]
    public class UsersPageViewModelTests : TestBase
    {
        private _43pKobzarContext _context;
        private UsersPageViewModel _vm;

        [TestInitialize]
        public void Setup()
        {
            _context = CreateInMemoryContext();
            SeedUsers(_context);
            _vm = new UsersPageViewModel(_context, skipConfirmation: true);
        }

        [TestMethod]
        public void LoadUsers_IncludesRole()
        {
            Assert.AreEqual(2, _vm!.PagedUsers.Count);
            Assert.IsNotNull(_vm.PagedUsers.First().Role);
        }

        [TestMethod]
        public void SearchText_FiltersByName()
        {
            _vm!.SearchText = "Иван";
            Assert.AreEqual(1, _vm.PagedUsers.Count);
        }

        [TestMethod]
        public async Task DeleteUser_WithNoRelated_RemovesUser()
        {
            var user = _context.Users.First();
            await _vm.DeleteUserCommand.Execute(user).ToTask();
            Assert.AreEqual(1, _context.Users.Count());
        }

        [TestMethod]
        public async Task DeleteUser_WithMaintenances_DoesNotRemove()
        {
            var user = _context!.Users.First();
            _context.Maintenances.Add(new Maintenance { Id = Guid.NewGuid(), UserId = user.Id, VendingMachineId = Guid.NewGuid() });
            _context.SaveChanges();
            _vm = new UsersPageViewModel(_context);
            await _vm.DeleteUserCommand.Execute(user).ToTask();
            Assert.AreEqual(2, _context.Users.Count());
        }
    }

    [TestClass]
    public class AddEditUserPageViewModelTests : TestBase
    {
        private _43pKobzarContext? _context;
        private AddEditUserPageViewModel? _vm;

        [TestInitialize]
        public void Setup()
        {
            _context = CreateInMemoryContext();
            _context.UserRoles.Add(new UserRole { Id = Guid.NewGuid(), Name = "Admin" });
            _context.SaveChanges();
            _vm = new AddEditUserPageViewModel(context: _context);
        }

        [TestMethod]
        public void Constructor_LoadsRoles()
        {
            Assert.AreEqual(1, _vm!.Roles.Count);
        }

        [TestMethod]
        public async Task Save_WhenEmailEmpty_DoesNotAddUser()
        {
            _vm!.CurrentUser.Email = "";
            _vm.CurrentUser.FullName = "Test";
            _vm.SelectedRole = _vm.Roles.First();
            _vm.CurrentUser.Password = "pass123";
            _vm.ConfirmPassword = "pass123";
            await _vm.SaveCommand.Execute().ToTask();

            Assert.AreEqual(0, _context!.Users.Count());
        }

        [TestMethod]
        public async Task Save_WhenAllValid_AddsUser()
        {
            _vm!.CurrentUser.Email = "test@test.com";
            _vm.CurrentUser.FullName = "Test";
            _vm.SelectedRole = _vm.Roles.First();
            _vm.CurrentUser.Password = "pass123";
            _vm.ConfirmPassword = "pass123";
            await _vm.SaveCommand.Execute().ToTask();

            Assert.AreEqual(1, _context!.Users.Count(u => u.Email == "test@test.com"));
        }

        [TestMethod]
        public async Task Save_EditUser_UpdatesDatabase()
        {
            var existing = new User { Id = Guid.NewGuid(), Email = "old@test.com", FullName = "Old" };
            _context!.Users.Add(existing);
            _context.SaveChanges();

            _vm = new AddEditUserPageViewModel(existing, _context);
            _vm.SelectedRole = _vm.Roles.First();
            _vm.CurrentUser.FullName = "Updated";
            await _vm.SaveCommand.Execute().ToTask();

            var updated = _context.Users.Find(existing.Id);
            Assert.AreEqual("Updated", updated?.FullName);
        }
    }

    [TestClass]
    public class AddEditCompanyPageViewModelTests : TestBase
    {
        private _43pKobzarContext? _context;
        private AddEditCompanyPageViewModel? _vm;

        [TestInitialize]
        public void Setup()
        {
            _context = CreateInMemoryContext();
            SeedHighCompanies(_context);
            _vm = new AddEditCompanyPageViewModel(context: _context);
        }

        [TestMethod]
        public void Constructor_LoadsHighCompanies()
        {
            Assert.AreEqual(2, _vm!.HighCompanies.Count);
        }

        [TestMethod]
        public async Task Save_WhenNameEmpty_DoesNotAddCompany()
        {
            _vm!.CurrentCompany.Name = "";
            await _vm.SaveCommand.Execute().ToTask();
            Assert.AreEqual(0, _context!.Companies.Count());
        }

        [TestMethod]
        public async Task Save_NewCompany_AddsToDatabase()
        {
            _vm.CurrentCompany.Name = "New Company";
            _vm.CurrentCompany.Address = "Test Address";
            _vm.SelectedHighCompany = _vm.HighCompanies.First();
            await _vm.SaveCommand.Execute().ToTask();

            Assert.AreEqual(1, _context.Companies.Count(c => c.Name == "New Company"));
        }
    }

[TestClass]
    public class AddEditMachinePageViewModelTests : TestBase
    {
        private _43pKobzarContext? _context;
        private AddEditMachinePageViewModel? _vm;

        [TestInitialize]
        public void Setup()
        {
            _context = CreateInMemoryContext();
            _context.CompanyLocations.Add(new CompanyLocation { Id = Guid.NewGuid(), Name = "Loc1" });
            _context.Models.Add(new Model { Id = Guid.NewGuid(), Name = "Model1" });
            _context.Companies.Add(new Company { Id = Guid.NewGuid(), Name = "Comp1" });
            _context.Places.Add(new Place { Id = Guid.NewGuid(), Name = "Place1" });
            _context.WorkModes.Add(new WorkMode { Id = Guid.NewGuid(), Name = "Mode1" });
            _context.Timezones.Add(new Timezone { Id = Guid.NewGuid(), Name = "Zone1" });
            _context.ServicePriorities.Add(new ServicePriority { Id = Guid.NewGuid(), Name = "Priority1" });
            _context.NotesVendings.Add(new NotesVending { Id = Guid.NewGuid(), Name = "Note1" });
            _context.SaveChanges();

            _vm = new AddEditMachinePageViewModel(machine: null, context: _context);
        }

        [TestMethod]
        public void LoadLookups_PopulatesAllCollections()
        {
            Assert.AreEqual(1, _vm.Models.Count);
            Assert.AreEqual(1, _vm.Companies.Count);
            Assert.AreEqual(1, _vm.Places.Count);
            Assert.AreEqual(1, _vm.WorkModes.Count);
            Assert.AreEqual(1, _vm.Timezones.Count);
            Assert.AreEqual(1, _vm.Priorities.Count);
        }

        [TestMethod]
        public async Task Save_WhenAllRequiredSelected_AddsMachine()
        {
            _vm.CompanyLocationName = "Test Location";
            _vm.SelectedModel = _vm.Models.First();
            _vm.SelectedCompany = _vm.Companies.First();
            _vm.SelectedWorkMode = _vm.WorkModes.First();
            _vm.SelectedTimezone = _vm.Timezones.First();
            _vm.SelectedPlace = _vm.Places.First();
            _vm.CurrentMachine.Location = "Address";
            _vm.CurrentMachine.SerialNumber = "SN123";

            await _vm.SaveCommand.Execute().ToTask();

            Assert.AreEqual(1, _context.VendingMachines.Count(vm => vm.SerialNumber == "SN123"));
            var saved = _context.VendingMachines.First(vm => vm.SerialNumber == "SN123");
            Assert.IsNotNull(saved.CompanylocationId);
            var location = _context.CompanyLocations.FirstOrDefault(cl => cl.Name == "Test Location");
            Assert.IsNotNull(location);
            Assert.AreEqual(location.Id, saved.CompanylocationId);
        }
    }
}