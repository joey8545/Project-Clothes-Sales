using System.ComponentModel;

namespace Project.Models
{
    public class COrderDetailWarp
    {
        private TorderDetail _orderdetail;

        public TorderDetail orderdetail
        {
            get { return _orderdetail; }
            set { _orderdetail = value; }
        }

        public COrderDetailWarp()
        {
            _orderdetail = new TorderDetail();
        }

        [DisplayName("訂單明細編號")]
        public int Odid
        {
            get { return _orderdetail.Odid; }
            set { _orderdetail.Odid = value; }
        }

        [DisplayName("訂單編號")]
        public int Oid
        {
            get { return _orderdetail.Oid; }
            set { _orderdetail.Oid = value; }
        }

        [DisplayName("商品編號")]
        public int Pid
        {
            get { return _orderdetail.Pid; }
            set { _orderdetail.Pid = value; }
        }

        [DisplayName("商品名稱")]
        public string Pname
        {
            get { return _orderdetail.Pname; }
            set { _orderdetail.Pname = value; }
        }

        [DisplayName("數量")]
        public int Pcount
        {
            get { return _orderdetail.Pcount; }
            set { _orderdetail.Pcount = value; }
        }

        [DisplayName("尺寸")]
        public string Psize
        {
            get { return _orderdetail.Psize; }
            set { _orderdetail.Psize = value; }
        }

        [DisplayName("顏色")]
        public string Pcolor
        {
            get { return _orderdetail.Pcolor; }
            set { _orderdetail.Pcolor = value; }
        }

        [DisplayName("客製化文字(正)")]
        public string? CustomText0
        {
            get { return _orderdetail.CustomText0; }
            set { _orderdetail.CustomText0 = value; }
        }

        [DisplayName("客製化文字(反)")]
        public string? CustomText1
        {
            get { return _orderdetail.CustomText1; }
            set { _orderdetail.CustomText1 = value; }
        }

        [DisplayName("客製化圖片(正)")]
        public string? CustomPhoto0
        {
            get { return _orderdetail.CustomPhoto0; }
            set { _orderdetail.CustomPhoto0 = value; }
        }

        [DisplayName("客製化圖片(反)")]
        public string? CustomPhoto1
        {
            get { return _orderdetail.CustomPhoto1; }
            set { _orderdetail.CustomPhoto1 = value; }
        }

        [DisplayName("商品圖片")]
        public string? Photo0
        {
            get { return _orderdetail.Photo0; }
            set { _orderdetail.Photo0 = value; }
        }

        [DisplayName("商品圖片")]
        public string? Photo1
        {
            get { return _orderdetail.Photo1; }
            set { _orderdetail.Photo1 = value; }
        }

        [DisplayName("單價")]
        public int Pprice
        {
            get { return _orderdetail.Pprice; }
            set { _orderdetail.Pprice = value; }
        }

        [DisplayName("退換貨方式")]
        public string? Rmethod
        {
            get { return _orderdetail.Rmethod; }
            set { _orderdetail.Rmethod = value; }
        }

        [DisplayName("退換數量")]
        public int? Rqty
        {
            get { return _orderdetail.Rqty; }
            set { _orderdetail.Rqty = value; }
        }

        [DisplayName("退換原因")]
        public string? Rreason
        {
            get { return _orderdetail.Rreason; }
            set { _orderdetail.Rreason = value; }
        }

        [DisplayName("其他退換原因")]
        public string? RotherReason
        {
            get { return _orderdetail.RotherReason; }
            set { _orderdetail.RotherReason = value; }
        }

        [DisplayName("收件人姓名")]
        public string? Rname
        {
            get { return _orderdetail.Rname; }
            set { _orderdetail.Rname = value; }
        }

        [DisplayName("收件人電話")]
        public string? Rphone
        {
            get { return _orderdetail.Rphone; }
            set { _orderdetail.Rphone = value; }
        }

        [DisplayName("收件人地址")]
        public string? Raddress
        {
            get { return _orderdetail.Raddress; }
            set { _orderdetail.Raddress = value; }
        }

        [DisplayName("退換貨描述")]
        public string? Rdescription
        {
            get { return _orderdetail.Rdescription; }
            set { _orderdetail.Rdescription = value; }
        }
    }
}
