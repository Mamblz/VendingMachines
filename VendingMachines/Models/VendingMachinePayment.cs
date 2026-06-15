using System;
using System.Collections.Generic;

namespace VendingMachines.Models;

public partial class VendingMachinePayment
{
    public Guid Id { get; set; }

    public Guid VendingMachineId { get; set; }

    public Guid PaymentTypeId { get; set; }

    public virtual PaymentType PaymentType { get; set; } = null!;

    public virtual VendingMachine VendingMachine { get; set; } = null!;
}
