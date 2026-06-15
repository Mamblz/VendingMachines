using System;
using System.Collections.Generic;

namespace VendingMachines.Models;

public partial class IssuesFound
{
    public Guid Id { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();
}
