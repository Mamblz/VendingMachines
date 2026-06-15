using System;
using System.Collections.Generic;

namespace VendingMachines.Models;

public partial class PaymentMethod
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
