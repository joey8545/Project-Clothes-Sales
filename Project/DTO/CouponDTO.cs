namespace Project.DTO
{
    public class CouponDTO
    {
        public int Id { get; set; }

        public string CouponName { get; set; }

        public int CouponDiscount { get; set; }

        public float CouponPercentage { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        public string CouponPassWord { get; set; }
    }
    public class UseCouponDTO
    {
        public int CouponId { get; set; }
    }
}
