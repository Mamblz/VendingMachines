using System;
using System.Collections.Generic;

namespace VendingMachines.Models;

public partial class Product
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public int MinStock { get; set; }

    public Guid? VendingMachineId { get; set; }

    public string? Description { get; set; }

    public int QuantityAvailable { get; set; }

    public string? SalesTrend { get; set; }

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    public virtual VendingMachine? VendingMachine { get; set; }
}
