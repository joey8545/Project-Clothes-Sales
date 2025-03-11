namespace Project.DTO
{
    public class OrderDetailDTO
    {
        public int Odid { get; set; }
        public int Oid { get; set; }
        public int PId { get; set; }
        public string PName { get; set; } = null!;
        public int PCount { get; set; }
        public string PSize { get; set; } = null!;
        public string PColor { get; set; } = null!;
        public string? CustomText0 { get; set; }
        public string? CustomText1 { get; set; }
        public string? CustomPhoto0 { get; set; }
        public string? CustomPhoto1 { get; set; }
        public string? Photo0 { get; set; }
        public string? Photo1 { get; set; }
        public int PPrice { get; set; }
        public string Ostatus { get; set; }
        public bool IsReviewed { get; set; }
        public string? Rmethod { get; set; } // 退換貨方式
        public int? Rqty { get; set; } // 退換貨數量
        public string? Rreason { get; set; } // 退換貨原因
        public string? RotherReason { get; set; } // 其他退換貨原因
        public string? Rname { get; set; } // 收件人姓名
        public string? Rphone { get; set; } // 收件人電話
        public string? Raddress { get; set; } // 收件人地址
        public string? Rdescription { get; set; } // 退換貨備註
    }

    public class OrderDetailRequest
    {
        public int Oid { get; set; }
    }

}
