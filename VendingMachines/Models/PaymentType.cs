using System;
using System.Collections.Generic;

namespace VendingMachines.Models;

public partial class PaymentType
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<VendingMachinePayment> VendingMachinePayments { get; set; } = new List<VendingMachinePayment>();
}
