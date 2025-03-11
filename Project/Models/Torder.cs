using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Torder
{
    public int Oid { get; set; }

    public string Oname { get; set; } = null!;

    public int Oprice { get; set; }

    public int? Odiscountedprice { get; set; }

    public int OtotalPrice { get; set; }

    public DateTime Odate { get; set; }

    public int Mid { get; set; }

    public string Oaddress { get; set; } = null!;

    public string Ophone { get; set; } = null!;

    public string Oemail { get; set; } = null!;

    public string Ostatus { get; set; } = null!;

    public bool Opayment { get; set; }

    public string OtradeNo { get; set; } = null!;

    public string OtradeDate { get; set; } = null!;

    public DateOnly? OpaymentDate { get; set; }

    public string? OcancelReason { get; set; }

    public string? OcancelDescription { get; set; }

    public DateTime? OcancelDate { get; set; }

    public string OcancelStatus { get; set; } = null!;

    public DateTime? OreturnDate { get; set; }

    public string? OreturnStatus { get; set; }

    public string? OreturnNo { get; set; }
}
