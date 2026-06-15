using System;
using System.Collections.Generic;

namespace VendingMachines.Models;

public partial class VendingMachine
{
    public Guid Id { get; set; }

    public string SerialNumber { get; set; } = null!;

    public Guid? CompanylocationId { get; set; }

    public Guid? UserId { get; set; }

    public string? RfidCashCollection { get; set; }

    public Guid? NotesId { get; set; }

    public string? Location { get; set; }

    public Guid? WorkModeId { get; set; }

    public string? RfidLoading { get; set; }

    public Guid? ModelId { get; set; }

    public string? KitOnlineId { get; set; }

    public Guid? CompanyId { get; set; }

    public Guid? CriticalThresholdTemplateId { get; set; }

    public Guid? ServicePriorityId { get; set; }

    public Guid? ManagerId { get; set; }

    public Guid? StatusId { get; set; }

    public Guid? NotificationTemplateId { get; set; }

    public string? WorkingHours { get; set; }

    public Guid? EngineerId { get; set; }

    public DateOnly? InstallDate { get; set; }

    public Guid? PlaceId { get; set; }

    public Guid? OperatorId { get; set; }

    public Guid? TechnicianId { get; set; }

    public DateOnly? LastMaintenanceDate { get; set; }

    public string? RfidService { get; set; }

    public string? Coordinates { get; set; }

    public decimal? TotalIncome { get; set; }

    public Guid? TimezoneId { get; set; }

    public virtual Company? Company { get; set; }

    public virtual CompanyLocation? Companylocation { get; set; }

    public virtual CriticalThresholdTemplate? CriticalThresholdTemplate { get; set; }

    public virtual ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();

    public virtual Model? Model { get; set; }

    public virtual NotesVending? Notes { get; set; }

    public virtual NotificationTemplate? NotificationTemplate { get; set; }

    public virtual Operator? Operator { get; set; }

    public virtual Place? Place { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ServicePriority? ServicePriority { get; set; }

    public virtual Status? Status { get; set; }

    public virtual Timezone? Timezone { get; set; }

    public virtual User? User { get; set; }

    public virtual ICollection<VendingMachinePayment> VendingMachinePayments { get; set; } = new List<VendingMachinePayment>();

    public virtual WorkMode? WorkMode { get; set; }
}
