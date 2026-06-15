using System;
using System.Collections.Generic;

namespace VendingMachines.Models;

public partial class Company
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public string? Contacts { get; set; }
    public string? Notes { get; set; }

    public DateOnly? InWorkingWith { get; set; }

    public Guid? HighCompanyId { get; set; }

    public virtual Highcompany? HighCompany { get; set; }

    public virtual ICollection<VendingMachine> VendingMachines { get; set; } = new List<VendingMachine>();
}
