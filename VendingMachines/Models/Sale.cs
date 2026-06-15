using System;
using System.Collections.Generic;

namespace VendingMachines.Models;

public partial class Sale
{
    public Guid Id { get; set; }

    public DateTime SaleTimestamp { get; set; }

    public Guid ProductId { get; set; }

    public decimal TotalPrice { get; set; }

    public int Quantity { get; set; }

    public Guid? PaymentMethodId { get; set; }

    public virtual PaymentMethod? PaymentMethod { get; set; }

    public virtual Product Product { get; set; } = null!;
}
