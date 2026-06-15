using System;
using System.Collections.Generic;

namespace VendingMachines.Models;

public partial class Highcompany
{
    public Guid HighcomapnyId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Company> Companies { get; set; } = new List<Company>();
}
