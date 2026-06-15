using System;
using System.Collections.Generic;

namespace VendingMachines.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public bool IsManager { get; set; }

    public bool IsEngineer { get; set; }

    public bool IsOperator { get; set; }

    public string? Phone { get; set; }

    public Guid? RoleId { get; set; }

    public string? Image { get; set; }

    public string? Password { get; set; }

    public virtual ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();

    public virtual UserRole? Role { get; set; }

    public virtual ICollection<VendingMachine> VendingMachines { get; set; } = new List<VendingMachine>();
}
