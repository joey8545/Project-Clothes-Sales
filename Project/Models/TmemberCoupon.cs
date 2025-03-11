using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class TmemberCoupon
{
    public int Id { get; set; }

    public int Mid { get; set; }

    public int CouponId { get; set; }

    public bool IsUse { get; set; }
}
