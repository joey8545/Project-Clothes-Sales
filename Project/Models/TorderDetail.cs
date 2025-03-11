using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class TorderDetail
{
    public int Odid { get; set; }

    public int Oid { get; set; }

    public int Pid { get; set; }

    public string Pname { get; set; } = null!;

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

    public bool IsReviewed { get; set; }

    public string? Rmethod { get; set; }

    public int? Rqty { get; set; }

    public string? Rreason { get; set; }

    public string? RotherReason { get; set; }

    public string? Rname { get; set; }

    public string? Rphone { get; set; }

    public string? Raddress { get; set; }

    public string? Rdescription { get; set; }
}
