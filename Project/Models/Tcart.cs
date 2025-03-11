using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Tcart
{
    public int Id { get; set; }

    public int Mid { get; set; }

    public int Pid { get; set; }

    public string Pname { get; set; } = null!;

    public string Ptype { get; set; } = null!;

    public string Pcategory { get; set; } = null!;

    public int Pcount { get; set; }

    public string Psize { get; set; } = null!;

    public string Pcolor { get; set; } = null!;

    public string? CustomText0 { get; set; }

    public string? CustomText1 { get; set; }

    public string? CustomPhoto0 { get; set; }

    public string? CustomPhoto1 { get; set; }

    public string? Photo0 { get; set; }

    public string? Photo1 { get; set; }

    public int Pprice { get; set; }
}
