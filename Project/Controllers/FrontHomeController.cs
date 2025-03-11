using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.DTO;
using Project.Models;
using Project.ViewModel;

namespace Project.Controllers
{
    public class FrontHomeController : Controller
    {
        private readonly DbuniPayContext _context;

        public FrontHomeController(DbuniPayContext context)
        {
            _context = context;
        }

        public IActionResult Customize()
        {
            return View();
        }
        public IActionResult ChangeClothes()
        {
            return View();
        }

        public IActionResult FrontIndex()
		{
            return View();
		}

        public IActionResult Favorite() 
        {
            return View();
        }
        public IActionResult Cart()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CartToPay(string TotalAmount, string ItemName)
        {
            var orderId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);
            //需填入你的網址
            var website = $"https://localhost:7279";
            var order = new Dictionary<string, string>
            {
                //綠界需要的參數
                { "MerchantTradeNo",  orderId},
                { "MerchantTradeDate",  DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")},
                { "TotalAmount",  TotalAmount.ToString()},
                { "TradeDesc",  "無"},
                { "ItemName", HttpUtility.UrlDecode(ItemName) },
                { "ExpireDate",  "3"},
                { "CustomField1",  ""},
                { "CustomField2",  ""},
                { "CustomField3",  ""},
                { "CustomField4",  ""},
                { "ReturnURL",  $"{website}/api/AddPayInfo"},
                { "OrderResultURL", $"{website}/FrontHome/PayInfo/{orderId}"},
                { "PaymentInfoURL",  $"{website}/api/AddAccountInfo"},
                { "ClientRedirectURL",  $"{website}/FrontHome/AccountInfo/{orderId}"},
                { "MerchantID",  "2000132"},
                { "IgnorePayment",  "GooglePay#WebATM#CVS#BARCODE"},
                { "PaymentType",  "aio"},
                { "ChoosePayment",  "ALL"},
                { "EncryptType",  "1"},
            };
            //檢查碼
            order["CheckMacValue"] = GetCheckMacValue(order);
            return View(order);
        }
        
        public IActionResult CheckOrder()
        {
            return View();
        } 
        public IActionResult CheckOrderDetail()
        {
            return View();
        }
        public IActionResult CancelOrder()
        {
            return View();
        }
        public IActionResult CommentList(int memberId)
        {
            var comments = (from c in _context.Tcomments
                           join od in _context.TorderDetails on new { c.Pid, c.Oid } equals new { od.Pid, od.Oid }
                           where c.Mid == memberId
                           orderby c.ComCreateDate descending
                           select new CommentViewModel
                           {
                               ComID = c.ComId,
                               MID = c.Mid,
                               MName = c.Mname,
                               Rating = c.Rating,
                               OID = c.Oid,
                               PID = c.Pid,
                               PName = od.Pname,
                               PSize = c.Psize,
                               PColor = c.Pcolor,
                               Comment = c.Comment,
                               ComCreateDate = c.ComCreateDate,
                               ComImage1 = c.ComImage1,
                               ComImage2 = c.ComImage2
                           }).ToList();

            return View(comments);
        }

        public IActionResult Advice() {
            return View();
        }

        public IActionResult ReturnPurchase()
        {
            return View();
        }

        public IActionResult Coupon()
        {
            return View();
        }

        public IActionResult EcpayView() 
        {
            return View();        
        }

        public IActionResult Payment()
        {
            var orderId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);
            //需填入你的網址
            var website = $"https://localhost:7279";
            var order = new Dictionary<string, string>
            {
                //綠界需要的參數
                { "MerchantTradeNo",  orderId},
                { "MerchantTradeDate",  DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")},
                { "TotalAmount",  "100"},
                { "TradeDesc",  "無"},
                { "ItemName",  "測試商品#測試商品1"},
                { "ExpireDate",  "3"},
                { "CustomField1",  ""},
                { "CustomField2",  ""},
                { "CustomField3",  ""},
                { "CustomField4",  ""},
                { "ReturnURL",  $"{website}/api/AddPayInfo"},
                { "OrderResultURL", $"{website}/FrontHome/PayInfo/{orderId}"},
                { "PaymentInfoURL",  $"{website}/api/AddAccountInfo"},
                { "ClientRedirectURL",  $"{website}/FrontHome/AccountInfo/{orderId}"},
                { "MerchantID",  "2000132"},
                { "IgnorePayment",  "GooglePay#WebATM#CVS#BARCODE"},
                { "PaymentType",  "aio"},
                { "ChoosePayment",  "ALL"},
                { "EncryptType",  "1"},
            };
            //檢查碼
            order["CheckMacValue"] = GetCheckMacValue(order);
            return View(order);
        }
        private string GetCheckMacValue(Dictionary<string, string> order)
        {
            var param = order.Keys.OrderBy(x => x).Select(key => key + "=" + order[key]).ToList();
            var checkValue = string.Join("&", param);
            //測試用的 HashKey
            var hashKey = "5294y06JbISpM5x9";
            //測試用的 HashIV
            var HashIV = "v77hoKGq4kWxNNIS";
            checkValue = $"HashKey={hashKey}" + "&" + checkValue + $"&HashIV={HashIV}";
            checkValue = HttpUtility.UrlEncode(checkValue).ToLower();
            checkValue = GetSHA256(checkValue);
            return checkValue.ToUpper();
        }
        private string GetSHA256(string value)
        {
            var result = new StringBuilder();
            var sha256 = SHA256.Create();
            var bts = Encoding.UTF8.GetBytes(value);
            var hash = sha256.ComputeHash(bts);
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }

        /// step5 : 取得付款資訊，更新資料庫
        [HttpPost]
        public ActionResult PayInfo(IFormCollection id)
        {
            var data = new Dictionary<string, string>();
            foreach (string key in id.Keys)
            {
                data.Add(key, id[key]);
            }
            DbuniPayContext db = new DbuniPayContext();
            string temp = id["MerchantTradeNo"]; //寫在LINQ(下一行)會出錯，
            var ecpayOrder = db.Torders.Where(m => m.OtradeNo == temp).FirstOrDefault();
            if (ecpayOrder != null)
            {
                ecpayOrder.Opayment = int.Parse(id["RtnCode"]) == 0 ?  false:true;
                //ecpayOrder.OpaymentDate = Convert.ToDateTime(id["PaymentDate"]);  //因為出現錯誤所以我先註解掉,麻煩還原此行註解
                db.SaveChanges();
            }
            return View("EcpayView", data);
        }
        /// step5 : 取得虛擬帳號 資訊
        [HttpPost]
        public ActionResult AccountInfo(IFormCollection id)
        {
            var data = new Dictionary<string, string>();
            foreach (string key in id.Keys)
            {
                data.Add(key, id[key]);
            }
            DbuniPayContext db = new DbuniPayContext();
            string temp = id["MerchantTradeNo"]; //寫在LINQ會出錯
            var ecpayOrder = db.EcpayOrders.Where(m => m.MerchantTradeNo == temp).FirstOrDefault();
            if (ecpayOrder != null)
            {
                ecpayOrder.RtnCode = int.Parse(id["RtnCode"]);
                if (id["RtnMsg"] == "Succeeded") ecpayOrder.RtnMsg = "已付款";
                ecpayOrder.PaymentDate = Convert.ToDateTime(id["PaymentDate"]);
                ecpayOrder.SimulatePaid = int.Parse(id["SimulatePaid"]);
                db.SaveChanges();
            }
            return View("EcpayView", data);
        }

    }
}
