using System.ComponentModel;

namespace Project.Models
{
    public class OrderWrap
    {
        private Torder _order ;

        public Torder order
        {
            get { return _order; }
            set { _order = value; }
        }

        public OrderWrap()
        {
            _order = new Torder();
        }
        [DisplayName("訂單編號")]
        public int Oid 
        { 
            get { return _order.Oid; }
            set { _order.Oid = value; }
        }
        [DisplayName("訂購人名稱")]
        public string Oname
        {
            get { return _order.Oname; }
            set { _order.Oname = value; }
        }
        [DisplayName("訂單價格")]
        public int Oprice
        {
            get { return _order.Oprice; }
            set { _order.Oprice = value; }
        }
        [DisplayName("訂單折扣價")]
        public int? Odiscountedprice
        {
            get { return _order.Odiscountedprice; }
            set { _order.Odiscountedprice = value; }
        }
        [DisplayName("訂單總價")]
        public int OtotalPrice
        {
            get { return _order.OtotalPrice; }
            set { _order.OtotalPrice = value; }
        }
        [DisplayName("訂單日期")]
        public DateTime Odate
        {
            get { return _order.Odate; }
            set { _order.Odate = value; }
        }
        [DisplayName("會員編號")]
        public int Mid
        {
            get { return _order.Mid; }
            set { _order.Mid = value; }
        }
        [DisplayName("訂單地址")]
        public string Oaddress
        {
            get { return _order.Oaddress; }
            set { _order.Oaddress = value; }
        }
        [DisplayName("訂單電話")]
        public string Ophone
        {
            get { return _order.Ophone; }
            set { _order.Ophone = value; }
        }
        [DisplayName("訂單電子郵件")]
        public string Oemail
        {
            get { return _order.Oemail; }
            set { _order.Oemail = value; }
        }
        [DisplayName("訂單狀態")]
        public string Ostatus
        {
            get { return _order.Ostatus; }
            set { _order.Ostatus = value; }
        }
        [DisplayName("付款狀態")]
        public bool Opayment
        {
            get { return _order.Opayment; }
            set { _order.Opayment = value; }
        }
        [DisplayName("訂單取消原因")]
        public string? OcancelReason
        {
            get { return _order.OcancelReason; }
            set { _order.OcancelReason = value; }
        }

        [DisplayName("訂單取消描述")]
        public string? OcancelDescription
        {
            get { return _order.OcancelDescription; }
            set { _order.OcancelDescription = value; }
        }
        [DisplayName("訂單取消")]
        public string OcancelStatus
        {
            get { return _order.OcancelStatus; }
            set { _order.OcancelStatus = value; }
        }
        [DisplayName("申請時間")]
        public DateTime? OreturnDate
        {
            get { return _order.OreturnDate; }
            set { _order.OreturnDate = value; }
        }
        [DisplayName("退貨申請")]
        public string? OreturnStatus
        {
            get { return _order.OreturnStatus; }
            set { _order.OreturnStatus = value; }
        }
        [DisplayName("退貨編號")]
        public string? OreturnNo
        {
            get { return _order.OreturnNo; }
            set { _order.OreturnNo = value; }
        }
    }
}
