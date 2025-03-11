using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Tcoupon
{
    public int Id { get; set; }

    public string CouponName { get; set; } = null!;

    public int CouponDiscount { get; set; }

    public double CouponPercentage { get; set; }

    public DateTime DateStart { get; set; }

    public DateTime DateEnd { get; set; }

    public string PassWord { get; set; } = null!;
}
