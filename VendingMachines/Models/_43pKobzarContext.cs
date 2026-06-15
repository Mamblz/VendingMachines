using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace VendingMachines.Models;

public partial class _43pKobzarContext : DbContext
{
    public _43pKobzarContext()
    {
    }

    public _43pKobzarContext(DbContextOptions<_43pKobzarContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<CompanyLocation> CompanyLocations { get; set; }

    public virtual DbSet<CriticalThresholdTemplate> CriticalThresholdTemplates { get; set; }

    public virtual DbSet<Highcompany> Highcompanies { get; set; }

    public virtual DbSet<IssuesFound> IssuesFounds { get; set; }

    public virtual DbSet<Maintenance> Maintenances { get; set; }

    public virtual DbSet<Model> Models { get; set; }

    public virtual DbSet<NotesVending> NotesVendings { get; set; }

    public virtual DbSet<NotificationTemplate> NotificationTemplates { get; set; }

    public virtual DbSet<Operator> Operators { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<PaymentType> PaymentTypes { get; set; }

    public virtual DbSet<Place> Places { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<ServicePriority> ServicePriorities { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Timezone> Timezones { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<VendingMachine> VendingMachines { get; set; }

    public virtual DbSet<VendingMachinePayment> VendingMachinePayments { get; set; }

    public virtual DbSet<WorkMode> WorkModes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=edu.pg.ngknn.ru;Port=5442;Database=43P_Kobzar;Username=43P;Password=444444");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("company_pkey");

            entity.ToTable("company", "EducationalPractice");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Address).HasColumnType("character varying");
            entity.Property(e => e.Notes).HasColumnType("character varying");
            entity.Property(e => e.Contacts).HasColumnType("character varying");
            entity.Property(e => e.HighCompanyId).HasColumnName("HighCompanyID");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");

            entity.HasOne(d => d.HighCompany).WithMany(p => p.Companies)
                .HasForeignKey(d => d.HighCompanyId)
                .HasConstraintName("company_highcompany_fk");
        });

        modelBuilder.Entity<CompanyLocation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("company_location_pkey");

            entity.ToTable("company_location", "EducationalPractice");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<CriticalThresholdTemplate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("critical_threshold_template_pkey");

            entity.ToTable("critical_threshold_template", "EducationalPractice");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Highcompany>(entity =>
        {
            entity.HasKey(e => e.HighcomapnyId).HasName("highcompany_pk");

            entity.ToTable("highcompany", "EducationalPractice");

            entity.Property(e => e.HighcomapnyId)
                .ValueGeneratedNever()
                .HasColumnName("highcomapnyID");
            entity.Property(e => e.Name).HasColumnType("character varying");
        });

        modelBuilder.Entity<IssuesFound>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("issues_found_pkey");

            entity.ToTable("issues_found", "EducationalPractice");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
        });

        modelBuilder.Entity<Maintenance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("maintenance_pkey");

            entity.ToTable("maintenance", "EducationalPractice");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.IssuesFoundId).HasColumnName("issues_found_id");
            entity.Property(e => e.MaintenanceDate).HasColumnName("maintenance_date");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VendingMachineId).HasColumnName("vending_machine_id");
            entity.Property(e => e.WorkDescription).HasColumnName("work_description");

            entity.HasOne(d => d.IssuesFound).WithMany(p => p.Maintenances)
                .HasForeignKey(d => d.IssuesFoundId)
                .HasConstraintName("fk_issues_found_vm");

            entity.HasOne(d => d.User).WithMany(p => p.Maintenances)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_user_vm");

            entity.HasOne(d => d.VendingMachine).WithMany(p => p.Maintenances)
                .HasForeignKey(d => d.VendingMachineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_maintenance_vm");
        });

        modelBuilder.Entity<Model>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("model_pkey");

            entity.ToTable("model", "EducationalPractice");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<NotesVending>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notes_vending_pkey");

            entity.ToTable("notes_vending", "EducationalPractice");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<NotificationTemplate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notification_template_pkey");

            entity.ToTable("notification_template", "EducationalPractice");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Operator>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("operator_pkey");

            entity.ToTable("operator", "EducationalPractice");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("payment_method_pkey");

            entity.ToTable("payment_method", "EducationalPractice");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<PaymentType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("payment_type_pkey");

            entity.ToTable("payment_type", "EducationalPractice");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Place>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("place_pkey");

            entity.ToTable("place", "EducationalPractice");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("products_pkey");

            entity.ToTable("products", "EducationalPractice");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.MinStock).HasColumnName("min_stock");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasPrecision(12, 2)
                .HasColumnName("price");
            entity.Property(e => e.QuantityAvailable).HasColumnName("quantity_available");
            entity.Property(e => e.SalesTrend)
                .HasMaxLength(255)
                .HasColumnName("sales_trend");
            entity.Property(e => e.VendingMachineId).HasColumnName("vending_machine_id");

            entity.HasOne(d => d.VendingMachine).WithMany(p => p.Products)
                .HasForeignKey(d => d.VendingMachineId)
                .HasConstraintName("fk_products_vm");
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sales_pkey");

            entity.ToTable("sales", "EducationalPractice");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SaleTimestamp)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("sale_timestamp");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(12, 2)
                .HasColumnName("total_price");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Sales)
                .HasForeignKey(d => d.PaymentMethodId)
                .HasConstraintName("fk_sales_payment");

            entity.HasOne(d => d.Product).WithMany(p => p.Sales)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_sales_product");
        });

        modelBuilder.Entity<ServicePriority>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("service_priority_pkey");

            entity.ToTable("service_priority", "EducationalPractice");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("status_pkey");

            entity.ToTable("status", "EducationalPractice");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Timezone>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("timezone_pkey");

            entity.ToTable("timezone", "EducationalPractice");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users", "EducationalPractice");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Email)
                .HasColumnType("character varying")
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasColumnType("character varying")
                .HasColumnName("full_name");
            entity.Property(e => e.Image)
                .HasColumnType("character varying")
                .HasColumnName("image");
            entity.Property(e => e.IsEngineer)
                .HasDefaultValue(false)
                .HasColumnName("is_engineer");
            entity.Property(e => e.IsManager)
                .HasDefaultValue(false)
                .HasColumnName("is_manager");
            entity.Property(e => e.IsOperator)
                .HasDefaultValue(false)
                .HasColumnName("is_operator");
            entity.Property(e => e.Password)
                .HasColumnType("character varying")
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasColumnType("character varying")
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("fk_users_role");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_role_pkey");

            entity.ToTable("user_role", "EducationalPractice");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<VendingMachine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("vending_machines_pkey");

            entity.ToTable("vending_machines", "EducationalPractice");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CompanylocationId).HasColumnName("companylocation_id");
            entity.Property(e => e.Coordinates)
                .HasMaxLength(255)
                .HasColumnName("coordinates");
            entity.Property(e => e.CriticalThresholdTemplateId).HasColumnName("critical_threshold_template_id");
            entity.Property(e => e.EngineerId).HasColumnName("engineer_id");
            entity.Property(e => e.InstallDate).HasColumnName("install_date");
            entity.Property(e => e.KitOnlineId)
                .HasMaxLength(255)
                .HasColumnName("kit_online_id");
            entity.Property(e => e.LastMaintenanceDate).HasColumnName("last_maintenance_date");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasColumnName("location");
            entity.Property(e => e.ManagerId).HasColumnName("manager_id");
            entity.Property(e => e.ModelId).HasColumnName("model_id");
            entity.Property(e => e.NotesId).HasColumnName("notes_id");
            entity.Property(e => e.NotificationTemplateId).HasColumnName("notification_template_id");
            entity.Property(e => e.OperatorId).HasColumnName("operator_id");
            entity.Property(e => e.PlaceId).HasColumnName("place_id");
            entity.Property(e => e.RfidCashCollection)
                .HasColumnType("character varying")
                .HasColumnName("rfid_cash_collection");
            entity.Property(e => e.RfidLoading)
                .HasColumnType("character varying")
                .HasColumnName("rfid_loading");
            entity.Property(e => e.RfidService)
                .HasColumnType("character varying")
                .HasColumnName("rfid_service");
            entity.Property(e => e.SerialNumber)
                .HasMaxLength(255)
                .HasColumnName("serial_number");
            entity.Property(e => e.ServicePriorityId).HasColumnName("service_priority_id");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.TechnicianId).HasColumnName("technician_id");
            entity.Property(e => e.TimezoneId).HasColumnName("timezone_id");
            entity.Property(e => e.TotalIncome)
                .HasPrecision(12, 2)
                .HasColumnName("total_income");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.WorkModeId).HasColumnName("work_mode_id");
            entity.Property(e => e.WorkingHours)
                .HasMaxLength(255)
                .HasColumnName("working_hours");

            entity.HasOne(d => d.Company).WithMany(p => p.VendingMachines)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("fk_vm_company");

            entity.HasOne(d => d.Companylocation).WithMany(p => p.VendingMachines)
                .HasForeignKey(d => d.CompanylocationId)
                .HasConstraintName("fk_companylocation_id");

            entity.HasOne(d => d.CriticalThresholdTemplate).WithMany(p => p.VendingMachines)
                .HasForeignKey(d => d.CriticalThresholdTemplateId)
                .HasConstraintName("fk_vm_threshold");

            entity.HasOne(d => d.Model).WithMany(p => p.VendingMachines)
                .HasForeignKey(d => d.ModelId)
                .HasConstraintName("fk_vm_model");

            entity.HasOne(d => d.Notes).WithMany(p => p.VendingMachines)
                .HasForeignKey(d => d.NotesId)
                .HasConstraintName("fk_vm_notes");

            entity.HasOne(d => d.NotificationTemplate).WithMany(p => p.VendingMachines)
                .HasForeignKey(d => d.NotificationTemplateId)
                .HasConstraintName("fk_vm_notification");

            entity.HasOne(d => d.Operator).WithMany(p => p.VendingMachines)
                .HasForeignKey(d => d.OperatorId)
                .HasConstraintName("fk_vm_operator");

            entity.HasOne(d => d.Place).WithMany(p => p.VendingMachines)
                .HasForeignKey(d => d.PlaceId)
                .HasConstraintName("fk_vm_place");

            entity.HasOne(d => d.ServicePriority).WithMany(p => p.VendingMachines)
                .HasForeignKey(d => d.ServicePriorityId)
                .HasConstraintName("fk_vm_priority");

            entity.HasOne(d => d.Status).WithMany(p => p.VendingMachines)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("fk_vm_status");

            entity.HasOne(d => d.Timezone).WithMany(p => p.VendingMachines)
                .HasForeignKey(d => d.TimezoneId)
                .HasConstraintName("fk_vm_timezone");

            entity.HasOne(d => d.User).WithMany(p => p.VendingMachines)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_vm_user");

            entity.HasOne(d => d.WorkMode).WithMany(p => p.VendingMachines)
                .HasForeignKey(d => d.WorkModeId)
                .HasConstraintName("fk_vm_work_mode");
        });

        modelBuilder.Entity<VendingMachinePayment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("vending_machine_payment_pkey");

            entity.ToTable("vending_machine_payment", "EducationalPractice");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.PaymentTypeId).HasColumnName("payment_type_id");
            entity.Property(e => e.VendingMachineId).HasColumnName("vending_machine_id");

            entity.HasOne(d => d.PaymentType).WithMany(p => p.VendingMachinePayments)
                .HasForeignKey(d => d.PaymentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_vmp_type");

            entity.HasOne(d => d.VendingMachine).WithMany(p => p.VendingMachinePayments)
                .HasForeignKey(d => d.VendingMachineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_vmp_vm");
        });

        modelBuilder.Entity<WorkMode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("work_mode_pkey");

            entity.ToTable("work_mode", "EducationalPractice");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
