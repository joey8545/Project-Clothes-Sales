using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using Project.Models;
using Project.ViewModel;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EcpayController : ControllerBase
    {
        private readonly DbuniPayContext _db;
        private readonly IMemoryCache _cache;

        // ✅ 合併建構函式，正確注入 DbuniPayContext 和 IMemoryCache
        public EcpayController(DbuniPayContext db, IMemoryCache cache)
        {
            _db = db;
            _cache = cache;
        }

        //step4 : 新增訂單
        [HttpPost("AddOrders")]

        public string AddOrders(GetLocalStorage json)
        {
            string num = "0";
            try
            {
                EcpayOrder Orders = new EcpayOrder();
                Orders.MemberId = json.MerchantID;
                Orders.MerchantTradeNo = json.MerchantTradeNo;
                Orders.RtnCode = 0; //未付款
                Orders.RtnMsg = "訂單成功尚未付款"; 
                Orders.TradeNo = json.MerchantID.ToString();
                Orders.TradeAmt = json.TotalAmount;
                Orders.PaymentDate = Convert.ToDateTime(json.MerchantTradeDate);
                Orders.PaymentType = json.PaymentType;
                Orders.PaymentTypeChargeFee = "0";
                Orders.TradeDate = json.MerchantTradeDate;
                Orders.SimulatePaid = 0;
                _db.EcpayOrders.Add(Orders);
                _db.SaveChanges();
                num = "OK";
            }
            catch (Exception ex)
            {
                num = ex.ToString();
            }
            return num;
        }

        [HttpPost("AddPayInfo")]
        public IActionResult AddPayInfo([FromBody] JObject info)
        {
            try
            {
                string merchantTradeNo = info.Value<string>("MerchantTradeNo");
                if (string.IsNullOrEmpty(merchantTradeNo))
                {
                    return ResponseError();
                }

                _cache.Set(merchantTradeNo, info, TimeSpan.FromMinutes(60));
                return ResponseOK();
            }
            catch (Exception)
            {
                return ResponseError();
            }
        }

        [HttpPost("AddAccountInfo")]
        public IActionResult AddAccountInfo([FromBody] JObject info)
        {
            try
            {
                string merchantTradeNo = info.Value<string>("MerchantTradeNo");
                if (string.IsNullOrEmpty(merchantTradeNo))
                {
                    return ResponseError();
                }

                _cache.Set(merchantTradeNo, info, TimeSpan.FromMinutes(60));
                return ResponseOK();
            }
            catch (Exception)
            {
                return ResponseError();
            }
        }
        
        private IActionResult ResponseError()
        {
            return Content("0|Error", "text/html");
        }

        private IActionResult ResponseOK()
        {
            return Content("1|OK", "text/html");
        }
    }
}
