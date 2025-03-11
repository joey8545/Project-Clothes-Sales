using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Tcservice
{
    public int Csid { get; set; }

    public int Mid { get; set; }

    public string Cstexts { get; set; } = null!;

    public DateTime Csmtimes { get; set; }

    public DateTime Csgmtimes { get; set; }

    public string Csstatus { get; set; } = null!;
}
