using System.ComponentModel;

namespace Project.Models
{
    public class CProductWrap
    {
        private Tproduct _product;

        public Tproduct product
        {
            get { return _product; }
            set { _product = value; }
        }

        // **改為 List<TproductInventory>，支援多筆庫存**
        public List<TproductInventory> TproductInventories { get; set; } = new List<TproductInventory>();

        public CProductWrap()
        {
            _product = new Tproduct();
        }

        public int Pid
        {
            get { return _product.Pid; }
            set { _product.Pid = value; }
        }

        [DisplayName("商品名稱")]
        public string Pname
        {
            get { return _product.Pname; }
            set { _product.Pname = value; }
        }

        [DisplayName("價格")]
        public int Pprice
        {
            get { return _product.Pprice; }
            set { _product.Pprice = value; }
        }

        [DisplayName("系列")]
        public string Ptype
        {
            get { return _product.Ptype; }
            set { _product.Ptype = value; }
        }

        [DisplayName("分類")]
        public string Pcategory
        {
            get { return _product.Pcategory; }
            set { _product.Pcategory = value; }
        }
        // 新增 PtypeList 和 PcategoryList
        public List<string> PtypeList { get; set; } = new List<string>();  // 存系列
        public List<string> PcategoryList { get; set; } = new List<string>();  // 存分類

        [DisplayName("商品描述")]
        public string Pdescription
        {
            get { return _product.Pdescription; }
            set { _product.Pdescription = value; }
        }

        [DisplayName("商品庫存總量")]
        public int Pinventory
        {
            get { return _product.Pinventory; }
            set { _product.Pinventory = value; }
        }

        [DisplayName("新增日期")]
        public DateTime PcreatedDate
        {
            get { return _product.PcreatedDate; }
            set { _product.PcreatedDate = value; }
        }

        [DisplayName("照片")]
        public string? Pphoto
        {
            get { return _product.Pphoto; }
            set { _product.Pphoto = value; }
        }

        public List<string> Images { get; set; } = new List<string>(); // 存圖片名稱

        [DisplayName("照片路徑")]
        public IFormFile? photoPath { get; set; }

        public List<IFormFile> Photos { get; set; } = new List<IFormFile>(); // 多張圖片

        public List<Tpimage> ImgList { get; set; } = new List<Tpimage>();

        public List<string> Colors { get; set; } = new List<string>(); //存顏色

        public List<string> Sizes { get; set; } = new List<string>();  //存尺寸

        public Dictionary<string, int> StockMap { get; set; } = new Dictionary<string, int>();
        // 存放顏色+尺寸的庫存數量

        [DisplayName("是否隱藏")]
        public bool PisHided
        {
            get { return _product.PisHided; }
            set { _product.PisHided = value; }
        }

        public List<Tcomment> Comments { get; set; } // 新增評價列表

        public int CommentCount { get; set; } // 新增評價數量屬性
    }
}

