using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class TproductInventory
{
    public int Piid { get; set; }

    public int Pid { get; set; }

    public string Pcolor { get; set; } = null!;

    public string Psize { get; set; } = null!;

    public int Pstock { get; set; }

    public DateTime PlastUpdated { get; set; }
}
