using System;
using System.Collections.Generic;

namespace VendingMachines.Models;

public partial class Timezone
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<VendingMachine> VendingMachines { get; set; } = new List<VendingMachine>();
}
