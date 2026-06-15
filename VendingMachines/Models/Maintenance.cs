using System;
using System.Collections.Generic;

namespace VendingMachines.Models;

public partial class Maintenance
{
    public Guid Id { get; set; }

    public DateOnly MaintenanceDate { get; set; }

    public Guid? IssuesFoundId { get; set; }

    public Guid VendingMachineId { get; set; }

    public Guid? UserId { get; set; }

    public string? WorkDescription { get; set; }

    public virtual IssuesFound? IssuesFound { get; set; }

    public virtual User? User { get; set; }

    public virtual VendingMachine VendingMachine { get; set; } = null!;
}
