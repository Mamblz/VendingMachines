using System;
using System.Collections.Generic;

namespace VendingMachines.Models;

public partial class NotesVending
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<VendingMachine> VendingMachines { get; set; } = new List<VendingMachine>();
}
