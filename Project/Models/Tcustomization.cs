using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Tcustomization
{
    public int Cid { get; set; }

    public string Cname { get; set; } = null!;

    public int Cprice { get; set; }

    public string Ctype { get; set; } = null!;

    public string Ccategory { get; set; } = null!;

    public string Csize { get; set; } = null!;

    public string Ccolor { get; set; } = null!;

    public string Ctext { get; set; } = null!;

    public string Cfont { get; set; } = null!;

    public double CfontSize { get; set; }

    public string Cimage { get; set; } = null!;
}
