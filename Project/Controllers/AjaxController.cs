using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.DTO;
using Project.Models;

namespace Project.Controllers
{
    public class AjaxController : Controller
    {
        private readonly DbuniPayContext _context;
            
        public AjaxController(DbuniPayContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<string> GetCurrentMemberIdAsync()
        {
            int? memberId = HttpContext.Session.GetInt32("MemberId");
            Console.WriteLine($"MemberId from session: {memberId}");

            if (memberId == null)
            {
                return await Task.FromResult("Session 已過期或未登入");
            }

            return await Task.FromResult($"MemberId: {memberId}");
        }

        [HttpPost]
        public async Task<IEnumerable<CustomCartDTO>> GetCartItems([FromBody] object request)
        {
            string json = HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER);
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            var member = JsonSerializer.Deserialize<Tmember>(json);
            Console.WriteLine($"Member MID: {member.Mid}");

            var cartItems = await _context.Tcarts
                .Where(c => c.Mid == member.Mid)
                .Select(c => new CustomCartDTO
                {
                    Id = c.Id,
                    PId = c.Pid,
                    PName = c.Pname,
                    PType = c.Ptype,
                    PCategory = c.Pcategory,
                    PCount = c.Pcount,
                    PSize = c.Psize,
                    PColor = c.Pcolor,
                    CustomText0 = c.CustomText0,
                    CustomText1 = c.CustomText1,
                    CustomPhoto0 = c.CustomPhoto0,
                    CustomPhoto1 = c.CustomPhoto1,
                    Photo0 = c.Photo0,
                    Photo1 = c.Photo1,
                    PPrice = c.Pprice
                }).ToListAsync();

            Console.WriteLine($"Found {cartItems.Count} items.");

            return cartItems;
        }

        [HttpPost]
        public async Task<string> PostFavorite([FromBody] FavoriteDTO favoriteDTO)
        {
            string json = HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER);
            if (string.IsNullOrEmpty(json))
            {
                return "請先登入會員";
            }
            var member = JsonSerializer.Deserialize<Tmember>(json);
            var existFavorite = await _context.Tfavorites.FirstOrDefaultAsync(c => c.Mid == member.Mid && c.Pid == favoriteDTO.PId);
            if (existFavorite == null)
            {
                var newFavorite = new Tfavorite
                {
                    Mid = member.Mid,
                    Pid = favoriteDTO.PId,
                    Pname = favoriteDTO.PName,
                    Pphoto = favoriteDTO.PPhoto,
                    Pprice = favoriteDTO.PPrice
                };
                _context.Tfavorites.Add(newFavorite);
                await _context.SaveChangesAsync();
                return "已加入收藏";
            }
            else
            {
                _context.Tfavorites.Remove(existFavorite);
                await _context.SaveChangesAsync();
                return "已移除收藏";
            }

        }

        [HttpGet]
        public async Task<List<Tfavorite>> GetFavorite()
        {
            string json = HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER);
            if (string.IsNullOrEmpty(json))
            {
                return new List<Tfavorite>();
            }
            var member = JsonSerializer.Deserialize<Tmember>(json);

            var favorite = await _context.Tfavorites.Where(c => c.Mid == member.Mid)
                                                    .OrderByDescending(c => c.FcreateDate)
                                                    .ToListAsync();
            Console.WriteLine($"Favorite count: {favorite.Count}");
            return favorite;
        }

        [HttpGet]
        public async Task<IActionResult> GetFavoriteID()
        {
            string json = HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER);
            if (json == null)
            {
                return Json(new List<int>());
            }
            var member = JsonSerializer.Deserialize<Tmember>(json);


            var favoriteID = await _context.Tfavorites.Where(c => c.Mid == member.Mid)
                                                      .Select(c => c.Pid)
                                                      .ToListAsync();
            return Json(favoriteID);
        }

        [HttpGet]
        public async Task<IActionResult> Top4Sales()
        {
            var Top4Sales = await (from tod in _context.TorderDetails
                                   join td in _context.Torders on tod.Oid equals td.Oid
                                   where td.Odate >= new DateTime(2025, 1, 1) && td.Odate <= new DateTime(2025, 12, 31) && tod.Pid!=0
                                   group tod by tod.Pid into grouped
                                   select new
                                   {
                                       id = grouped.Key,
                                       pid = grouped.FirstOrDefault().Pid,
                                       count = grouped.Sum(t => t.Pcount),
                                       pphoto = grouped.FirstOrDefault().Photo0,
                                       pname = grouped.FirstOrDefault().Pname,
                                       totalprice = grouped.Sum(t => t.Pprice * t.Pcount),
                                       pprice = grouped.FirstOrDefault().Pprice
                                   })
                                   .OrderByDescending(x => x.count)
                                   .Take(4)
                                   .ToListAsync();
            return Json(Top4Sales);
        }

        [HttpGet]
        public async Task<IActionResult> New4Sales(string keyword)
        {
            var Newest4 = await _context.Tproducts.OrderByDescending(x => x.PcreatedDate).Take(4).ToListAsync();
            return Json(Newest4);
        }

        [HttpPost]
        public async Task<IActionResult> SaveToCart([FromBody] CustomCartDTO cart)
        {
            string json = HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER);
            if (string.IsNullOrEmpty(json))
            {
                return Json(new { success = false, message = "請先登入會員" });
            }
            var member = JsonSerializer.Deserialize<Tmember>(json);
            var Cart = new Tcart
            {
                Mid = member.Mid,
                Pid = 0,
                Pname = cart.PName,
                Ptype = cart.PType,
                Pcategory = cart.PCategory,
                Pcount = cart.PCount,
                Psize = cart.PSize,
                Pcolor = cart.PColor,
                CustomText0 = cart.CustomText0,
                CustomText1 = cart.CustomText1,
                CustomPhoto0 = cart.CustomPhoto0,
                CustomPhoto1 = cart.CustomPhoto1,
                Photo0 = cart.Photo0,
                Photo1 = cart.Photo1,
                Pprice = cart.PPrice * cart.PCount,
            };
            if (Cart.Photo0 == "")
            {
                return Json(new { success = false, message = "請儲存正面圖案" });
            }
            else if (Cart.Photo1 == "")
            {
                return Json(new { success = false, message = "請儲存背面圖案" });
            }
            _context.Tcarts.Add(Cart);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "已加入購物車" });
        }

        [HttpPost]
        public async Task<IActionResult> SaveToOrder([FromBody] OrderDTO order)
        {

            string json = HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER);
            if (string.IsNullOrEmpty(json))
            {
                return BadRequest("請先登入會員");
            }

            var member = JsonSerializer.Deserialize<Tmember>(json);

            try
            {
                var newOrder = new Torder
                {
                    Oprice = order.OPrice,
                    Odiscountedprice = order.ODiscountedprice,
                    OtotalPrice = order.OTotalPrice,
                    Oname = order.OName,
                    Oaddress = order.OAddress,
                    Ophone = order.OPhone,
                    Oemail = order.OEmail,
                    Ostatus = order.OStatus,
                    Opayment = false,
                    Odate = DateTime.Now,
                    Mid = member.Mid,
                    OtradeNo = order.orderId,
                    OtradeDate = order.MerchantTradeDate
                };
                _context.Torders.Add(newOrder);
                await _context.SaveChangesAsync();

                return Ok(new { OrderID = newOrder.Oid, message = "訂單成立" });
            }
            catch (Exception ex)
            {
                return BadRequest("訂單成立失敗");
            }
        }
         
        [HttpPost]
        public async Task<IActionResult> SaveToOrderDetail([FromBody] List<OrderDetailDTO> orderDetails)
        {
            string json = HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER);
            if (string.IsNullOrEmpty(json))
            {
                return Unauthorized("請先登入會員");
            }

            var member = JsonSerializer.Deserialize<Tmember>(json);

            try
            {
                foreach (var detail in orderDetails)
                {
                    var orderDetail = new TorderDetail
                    {
                        Oid = detail.Oid,
                        Pid = detail.PId,
                        Pname = detail.PName,
                        Pcount = detail.PCount,
                        Psize = detail.PSize,
                        Pcolor = detail.PColor,
                        CustomText0 = detail.CustomText0,
                        CustomText1 = detail.CustomText1,
                        CustomPhoto0 = detail.CustomPhoto0,
                        CustomPhoto1 = detail.CustomPhoto1,
                        Photo0 = detail.Photo0,
                        Photo1 = detail.Photo1,
                        Pprice = detail.PPrice
                    };
                    _context.TorderDetails.Add(orderDetail);
                }

                await _context.SaveChangesAsync();
                return Ok("訂單詳情儲存成功");
            }
            catch (Exception ex)
            {
                return BadRequest($"訂單詳情儲存失敗: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCartItems([FromBody] int[] cartItemIds)
        { 
            string json = HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER);
            if (string.IsNullOrEmpty(json))
            {
                return Unauthorized("請先登入會員");
            }

            var member = JsonSerializer.Deserialize<Tmember>(json);
             
            var itemsToRemove = _context.Tcarts.Where(c => c.Mid == member.Mid && cartItemIds.Contains(c.Id));
            if (!itemsToRemove.Any())
            {
                return Ok("沒有符合刪除條件的購物車資料");
            }

            _context.Tcarts.RemoveRange(itemsToRemove);
            await _context.SaveChangesAsync();

            return Ok("購物車資料已刪除");
        }

        [HttpPost]
        public async Task<string> addToCart([FromBody] CustomCartDTO cart)
        {
            string json = HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER);
            if (string.IsNullOrEmpty(json))
            {
                return "請先登入會員";
            }

            var member = JsonSerializer.Deserialize<Tmember>(json);
            var existingCartItem = await _context.Tcarts
                .FirstOrDefaultAsync(c => c.Mid == member.Mid
                                       && c.Pid == cart.PId
                                       && c.Psize == cart.PSize
                                       && c.Pcolor == cart.PColor);

            if (existingCartItem != null)
            {
                existingCartItem.Pcount += cart.PCount;
                existingCartItem.Pprice = existingCartItem.Pcount * cart.PPrice;
            }
            else
            {
                var newCart = new Tcart
                {
                    Mid = member.Mid,
                    Pid = cart.PId,
                    Pname = cart.PName,
                    Ptype = cart.PType,
                    Pcategory = cart.PCategory,
                    Pcount = cart.PCount,
                    Psize = cart.PSize,
                    Pcolor = cart.PColor,
                    CustomText0 = null,
                    CustomText1 = null,
                    CustomPhoto0 = null,
                    CustomPhoto1 = null,
                    Photo0 = cart.Photo0,
                    Photo1 = null,
                    Pprice = cart.PPrice * cart.PCount,
                };

                _context.Tcarts.Add(newCart);
            }

            await _context.SaveChangesAsync();
            return "已加入購物車";
        }


        [HttpPost]
            public async Task<IActionResult> PostAdvice([FromBody] AdviceDTO Ad)
            {
                string json = HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER);
                if (string.IsNullOrEmpty(json))
                {
                    return Json(new { success = false, message = "請重新登入會員", redirectUrl = Url.Action("FrontIndex", "FrontHome") });
                }
                var member = JsonSerializer.Deserialize<Tmember>(json);
  
                    var Advice = new Tadvice
                {
                    Mid = member.Mid,
                    Oid = Ad.OId,
                    Question = Ad.Question,
                    Title = Ad.Title,
                    Description = Ad.Description,
                    DateTime = DateTime.Now
                };
                _context.Tadvices.Add(Advice);
                await _context.SaveChangesAsync();
                return Json(new { success = true, redirectUrl = Url.Action("CheckOrder", "FrontHome") });
            }

        [HttpPut]
        public async Task<string> ReplyAdvice([FromBody] ReplyAdviceDTO replyadvice)
        {
            var advice = await _context.Tadvices.FindAsync(replyadvice.Id);
            advice.IsReply = replyadvice.IsReply;
            _context.Tadvices.Update(advice);
            await _context.SaveChangesAsync();
            return "回覆成功";
        }

        [HttpGet]

        public async Task<IActionResult> GetCoupon()
        {
            string json = HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER);
            if (string.IsNullOrEmpty(json))
            {
                return Json(new { success = false, message = "請重新登入會員", redirectUrl = Url.Action("FrontIndex", "FrontHome") });
            }
            var member = JsonSerializer.Deserialize<Tmember>(json);
            var Coupon = await (from MCou in _context.TmemberCoupons
                                join Cou in _context.Tcoupons on MCou.CouponId equals Cou.Id
                                where MCou.Mid == member.Mid && MCou.IsUse == false && Cou.DateEnd >= DateTime.Now
                                select new
                                {
                                    CouponName = Cou.CouponName,
                                    CouponDiscount = Cou.CouponDiscount,
                                    CouponPercentage = (Cou.CouponPercentage * 10).ToString("F0"),
                                    DateStart = Cou.DateStart.ToString("yyyy-MM-dd"),
                                    DateEnd = Cou.DateEnd.ToString("yyyy-MM-dd"),
                                }
                               ).ToListAsync();
            return Json(new { success = true, data = Coupon });
        }

        [HttpPost]

        public async Task<IActionResult> SendCoupon([FromBody] CouponDTO CouponPassWord)
        {

            var Coupon = await _context.Tcoupons.Where(c => c.PassWord == CouponPassWord.CouponPassWord).FirstOrDefaultAsync();
            string json = HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER);
            var member = JsonSerializer.Deserialize<Tmember>(json);
            var MemberCoupon = await _context.TmemberCoupons.Where(c => c.Mid == member.Mid).ToListAsync();
            if (string.IsNullOrEmpty(CouponPassWord.CouponPassWord))
            {
                return Json(new {success=false,message= "請輸入優惠碼" });
            }
            else if (Coupon == null)
            {
                return Json(new { success = false, message = "無效的優惠碼" });
            }
            else if (MemberCoupon.Any(c => c.CouponId == Coupon.Id))
            {
                return Json(new { success = false, message = "已領取過該優惠" });
            }
            else
            {
                var AddCoupon = new TmemberCoupon
                {
                    Mid = member.Mid,
                    CouponId = Coupon.Id,
                };
                _context.TmemberCoupons.Add(AddCoupon);
                _context.SaveChanges();
                return Json(new { success = true, message = "已加入該優惠" });
            }
        }

        [HttpGet]

        public async Task<IActionResult> GetAllCoupon()
        {
            var coupon = await (from Cou in _context.Tcoupons
                                select new
                                {
                                    Id = Cou.Id,
                                    CouponName = Cou.CouponName,
                                    CouponDiscount = Cou.CouponDiscount,
                                    CouponPercentage = (Cou.CouponPercentage * 10).ToString("F0"),
                                    DateStart = Cou.DateStart,
                                    DateEnd = Cou.DateEnd,
                                    Password = Cou.PassWord
                                }
                                ).OrderByDescending(c => c.DateStart)
                                 .ToListAsync();
            return Json(coupon);
        }

        [HttpDelete]

        public async Task<string> Delete(int id)
        {
            var CouponToDelete = await _context.Tcoupons.FindAsync(id);
            if (CouponToDelete == null)
            {
                return "查不到該筆資料，刪除失敗";
            }
            _context.Tcoupons.Remove(CouponToDelete);
            await _context.SaveChangesAsync();
            return "已刪除資料";
        }

        [HttpPut]

        public async Task<IActionResult> saveEdit(int id, [FromBody] CouponDTO coupon)
        {

            var CouponEdit = await _context.Tcoupons.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (CouponEdit != null)
            {
                CouponEdit.CouponName = coupon.CouponName;
                CouponEdit.CouponDiscount = coupon.CouponDiscount;
                CouponEdit.CouponPercentage = coupon.CouponPercentage;
                CouponEdit.DateStart = coupon.DateStart;
                CouponEdit.DateEnd = coupon.DateEnd;
                CouponEdit.PassWord = coupon.CouponPassWord;

                _context.Tcoupons.Update(CouponEdit);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "更新成功" });
            }

            return Json(new { success = false, message = "更新失敗", redirectUrl = Url.Action("Coupon", "Other") });
        }

        [HttpPost]

        public async Task<string> addCoupon([FromBody] CouponDTO coupon)
        {
            var Coupon = new Tcoupon
            {
                CouponName = coupon.CouponName,
                CouponDiscount = coupon.CouponDiscount,
                CouponPercentage = coupon.CouponPercentage,
                DateStart = coupon.DateStart,
                DateEnd = coupon.DateEnd,
                PassWord = coupon.CouponPassWord
            };
            _context.Tcoupons.Add(Coupon);
            await _context.SaveChangesAsync();
            return "已新增此優惠券";
        }

        [HttpGet("Ajax/openEdit/{id}")]

        public async Task<IActionResult> openEdit(int id)
        {

            var coupon = await _context.Tcoupons.FindAsync(id);
            return Json(coupon);
        }

        [HttpGet("Ajax/openAdvice/{id}")]

        public async Task<IActionResult> openAdvice(int id)
        {

            var advice = await _context.Tadvices.FindAsync(id);
            var Email = await _context.Tmembers.Where(c => c.Mid == advice.Mid).Select(c => c.Memail).FirstOrDefaultAsync();
            var openAdvice = new OpenAdviceDTO
            {
                Id = advice.Id,
                Mid = advice.Mid,
                Oid = advice.Oid,
                Question = advice.Question,
                Title = advice.Title,
                Description = advice.Description,
                DateTime = advice.DateTime,
                IsReply = advice.IsReply,
                email = Email
            };
            return Json(openAdvice);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCart([FromBody] CustomCartDTO cart)
        {
            try
            {
                using (var db = new DbuniPayContext())
                {
                    var existingCart = db.Tcarts.FirstOrDefault(c => c.Id == cart.Id);
                    if (existingCart != null)
                    {
                        if (existingCart.Pcount > 0)  
                        { 
                            var unitPrice = existingCart.Pprice / existingCart.Pcount;
                             
                            existingCart.Pcount = cart.PCount;
                             
                            existingCart.Pprice = unitPrice * cart.PCount;

                            await db.SaveChangesAsync(); 
                            return Ok(new { newPrice = existingCart.Pprice });
                        }
                        return BadRequest("商品數量異常");
                    }
                    return NotFound("找不到購物車項目");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"更新失敗，錯誤: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> ReturnExchange([FromBody] List<OrderDetailDTO> requestList)
        {
            if (requestList == null || requestList.Count == 0)
            {
                return BadRequest(new { success = false, message = "無效的請求資料" });
            }

            using (var db = new DbuniPayContext())
            {
                var odidList = requestList.Select(r => r.Odid).ToList();

                var orderDetails = await db.TorderDetails
                    .Where(od => odidList.Contains(od.Odid))
                    .ToListAsync();

                if (orderDetails.Count == 0)
                {
                    return NotFound(new { success = false, message = "找不到對應的訂單明細" });
                }

                foreach (var orderDetail in orderDetails)
                {
                    var updateData = requestList.FirstOrDefault(r => r.Odid == orderDetail.Odid);
                    if (updateData != null)
                    {
                        orderDetail.Rmethod = updateData.Rmethod;
                        orderDetail.Rqty = updateData.Rqty;
                        orderDetail.Rreason = updateData.Rreason;
                        orderDetail.RotherReason = updateData.RotherReason;
                        orderDetail.Rname = updateData.Rname;
                        orderDetail.Rphone = updateData.Rphone;
                        orderDetail.Raddress = updateData.Raddress;
                        orderDetail.Rdescription = updateData.Rdescription;
                    }
                }

                await db.SaveChangesAsync();
            }

            return Ok(new { success = true, message = "退換貨資料已更新" });
        }

        [HttpPut]
        public async Task<IActionResult> ReturnedExchange([FromBody] ReturnedExchangeDTO request)
        {
            if (string.IsNullOrEmpty(request.OrderId))
            {
                return BadRequest("沒有提供訂單ID");
            }

            if (!int.TryParse(request.OrderId, out int oid))
            {
                return BadRequest("訂單ID格式錯誤");
            }

            using (var context = new DbuniPayContext())
            {
                var order = await context.Torders.FirstOrDefaultAsync(o => o.Oid == oid);
                if (order == null)
                {
                    return NotFound("找不到該訂單");
                }
                 
                order.OreturnDate = DateTime.Now;
                order.OreturnStatus = "申請中";
                 
                int taiwanYear = DateTime.Now.Year - 1911;
                string today = $"{taiwanYear}{DateTime.Now:MMdd}";
                 
                var latestReturnNo = await context.Torders
                    .Where(o => o.OreturnNo != null && o.OreturnNo.StartsWith("R" + today))
                    .OrderByDescending(o => o.OreturnNo)
                    .Select(o => o.OreturnNo)
                    .FirstOrDefaultAsync();
                 
                int newSequence = 1;
                if (!string.IsNullOrEmpty(latestReturnNo))
                {
                    string seqStr = latestReturnNo.Substring(8);  
                    if (int.TryParse(seqStr, out int seq))
                    {
                        newSequence = seq + 1;
                    }
                }
                 
                order.OreturnNo = $"RT{today}{newSequence:D3}";

                try
                {
                    await context.SaveChangesAsync();
                    return Ok(new { success = true, message = "訂單退貨資料更新成功" });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { success = false, message = ex.Message });
                }
            }
        }
         

        [HttpGet]
        public async Task<IActionResult> GetMemberCoupons()
        {
            string json = HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER);
            if (string.IsNullOrEmpty(json))
            {
                return Unauthorized("請先登入會員");
            }
            var member = JsonSerializer.Deserialize<Tmember>(json);

            var coupons = await _context.TmemberCoupons
                .Where(mc => mc.Mid == member.Mid && !mc.IsUse)
                .Join(_context.Tcoupons,
                      mc => mc.CouponId,
                      c => c.Id,
                      (mc, c) => new CouponDTO
                      {
                          Id = c.Id,
                          CouponName = c.CouponName,
                          CouponDiscount = c.CouponDiscount,
                          CouponPercentage = (float)c.CouponPercentage,
                          DateStart = c.DateStart,
                          DateEnd = c.DateEnd,
                          CouponPassWord = c.PassWord
                      })
                .ToListAsync();

            return Ok(coupons);
        }

        [HttpPut]
        public async Task<IActionResult> UseMemberCoupon([FromBody] UseCouponDTO request)
        { 
            if (request == null || request.CouponId <= 0)
            {
                return BadRequest("無效的折價券 ID");
            } 

            string json = HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER);
            if (string.IsNullOrEmpty(json))
            {
                return Unauthorized("請先登入會員");
            }

            var member = JsonSerializer.Deserialize<Tmember>(json);
             
            var memberCoupon = await _context.TmemberCoupons
                .FirstOrDefaultAsync(mc => mc.Mid == member.Mid && mc.CouponId == request.CouponId && !mc.IsUse);

            if (memberCoupon == null)
            {
                return NotFound("折價券不存在、已使用，或無權使用");
            }
             
            memberCoupon.IsUse = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "折價券使用成功" });
        }



        [HttpDelete]
        [Route("Ajax/DeleteItem/{id}")]
        public async Task<string> DeleteItem(int id)
        {
            try
            {
                using (var db = new DbuniPayContext())
                {
                    var item = db.Tcarts.FirstOrDefault(c => c.Id == id);
                    if (item != null)
                    {
                        db.Tcarts.Remove(item);
                        await db.SaveChangesAsync();
                        return "刪除成功";
                    }
                    return "找不到該購物車項目";
                }
            }
            catch (Exception ex)
            {
                return $"刪除失敗，錯誤: {ex.Message}";
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMemberInfo(int memberId)
        {
            try
            {
                using (var db = new DbuniPayContext())
                {
                    var member = await db.Tmembers
                        .Where(m => m.Mid == memberId)
                        .Select(m => new
                        {
                            Name = m.Mname,
                            Phone = m.Mphone,
                            Email = m.Memail
                        })
                        .FirstOrDefaultAsync();

                    if (member == null)
                    {
                        return NotFound("會員資料未找到");
                    }

                    return Ok(member);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"獲取會員資料失敗，錯誤: {ex.Message}");
            }
        }


        [HttpPost]
        public async Task<IEnumerable<OrderDTO>> GetOrderItems([FromBody] object request)
        {
            string json = HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER);
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            var member = JsonSerializer.Deserialize<Tmember>(json);
            Console.WriteLine($"Member MID: {member.Mid}");

            var orderItems = await _context.Torders
                .Where(o => o.Mid == member.Mid)
                .OrderByDescending(o => o.Odate)
                .Select(o => new OrderDTO
                {
                    OID = o.Oid,
                    OName = o.Oname,
                    OPrice = o.Oprice,
                    ODiscountedprice = o.Odiscountedprice,
                    OTotalPrice = o.OtotalPrice,
                    Odate = o.Odate.ToString("yyyy-MM-dd HH:mm:ss"),
                    OAddress = o.Oaddress,
                    OPhone = o.Ophone,
                    OEmail = o.Oemail,
                    OStatus = o.Ostatus,
                    OPayment = o.Opayment,
                    OCancelStatus = o.OcancelStatus,
                    OReturnStatus = o.OreturnStatus,
                    OReturnNo = o.OreturnNo
                })
                .ToListAsync();

            Console.WriteLine($"Found {orderItems.Count} orders.");

            return orderItems;
        }

        [HttpPost]
        public async Task<IEnumerable<OrderDTO>> GetOrderPrice([FromBody] OrderRequest request)
        {
            if (request?.Oid == null)
            {
                return null;
            }

            var order = _context.Torders
                .Where(o => o.Oid == request.Oid)
                .FirstOrDefault();

            if (order == null)
            {
                return null;
            }

            var orderPrice = await _context.Torders
                .Where(od => od.Oid == request.Oid)
                .Select(od => new OrderDTO
                {
                    OID = order.Oid,
                    OName = order.Oname,
                    OPrice = order.Oprice,
                    OTotalPrice = order.OtotalPrice,
                    ODiscountedprice = order.Odiscountedprice
                }).ToListAsync();

            Console.WriteLine(orderPrice);
            return orderPrice;
        }

        [HttpPost]
        public async Task<IEnumerable<OrderDetailDTO>> GetOrderDetail([FromBody] OrderDetailRequest request)
        {
            if (request == null || request.Oid <= 0)
            {
                return null;
            }

            int oid = request.Oid;

            var orderInfo = await _context.Torders
                .Where(o => o.Oid == oid)
                .Select(o => new
                {
                    o.Ostatus
                })
                .FirstOrDefaultAsync();

            if (orderInfo == null)
            {
                return null;
            }

            var orderDetails = await _context.TorderDetails
                .Where(od => od.Oid == oid)
                .Select(od => new OrderDetailDTO
                {
                    Odid = od.Odid,
                    Oid = od.Oid,
                    PId = od.Pid,
                    PName = od.Pname,
                    PCount = od.Pcount,
                    PSize = od.Psize,
                    PColor = od.Pcolor,
                    CustomText0 = od.CustomText0,
                    CustomText1 = od.CustomText1,
                    CustomPhoto0 = od.CustomPhoto0,
                    CustomPhoto1 = od.CustomPhoto1,
                    Photo0 = od.Photo0,
                    Photo1 = od.Photo1,
                    PPrice = od.Pprice,
                    Ostatus = orderInfo.Ostatus,
                    IsReviewed = od.IsReviewed
                })
                .ToListAsync();

            Console.WriteLine($"Found {orderDetails.Count} order details for OID: {oid} with Status: {orderInfo.Ostatus}");

            return orderDetails;
        }

        [HttpPost]
        public async Task<IActionResult> SearchOrders([FromBody] SearchOrderRequest request)
        { 
            string json = HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER);
            if (string.IsNullOrEmpty(json))
            {
                return Unauthorized("請先登入");
            }

            var member = JsonSerializer.Deserialize<Tmember>(json);
            Console.WriteLine($"Member MID: {member.Mid}");
             
            var query = _context.Torders.Where(o => o.Mid == member.Mid);
             
            if (request.Condition == "oneMonth")
                query = query.Where(o => o.Odate >= DateTime.Now.AddMonths(-1));
            else if (request.Condition == "notShipped")
                query = query.Where(o => o.Ostatus == "待出貨");
            else if (request.Condition == "returned")
                query = query.Where(o => o.Ostatus == "退貨");
            else if (request.Condition == "sixMonths")
                query = query.Where(o => o.Odate >= DateTime.Now.AddMonths(-6));
             
            if (!string.IsNullOrEmpty(request.OrderNumber))
                query = query.Where(o => o.Oid.ToString().Contains(request.OrderNumber));
             
            var orders = await query
                .OrderByDescending(o => o.Odate)
                .Select(o => new OrderDTO
                {
                    OID = o.Oid,
                    OName = o.Oname,
                    OPrice = o.Oprice,
                    ODiscountedprice = o.Odiscountedprice,
                    OTotalPrice = o.OtotalPrice,
                    Odate = o.Odate.ToString("yyyy-MM-dd HH:mm:ss"),
                    OAddress = o.Oaddress,
                    OPhone = o.Ophone,
                    OEmail = o.Oemail,
                    OStatus = o.Ostatus,
                    OPayment = o.Opayment, 
                    OCancelStatus = o.OcancelStatus,
                    OReturnNo = o.OreturnNo
                })
                .ToListAsync();

            Console.WriteLine($"Found {orders.Count} orders.");

            return Ok(new { orders });
        }

        [HttpGet("Ajax/getDetails/{id}")]

        public async Task<IActionResult> getDetails(int id)
        {
            string json = HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER);
            if (string.IsNullOrEmpty(json))
            {
                return RedirectToAction("FrontIndex", "FrontHome");
            }

            var member = JsonSerializer.Deserialize<Tmember>(json);

            var order = await _context.Torders.Where(c => c.Oid == id && c.Mid == member.Mid).FirstOrDefaultAsync();
            Console.WriteLine(order);
            var orderdetail = await _context.TorderDetails.Where(c => c.Oid == id).Select(c => new AdviceOrderDetailDTO
            {
                Pname = c.Pname,
                Psize = c.Psize,
                Pcolor = c.Pcolor,
                Pcount = c.Pcount,
                Pprice = c.Pprice
            }).ToListAsync();
            Console.WriteLine(orderdetail);
            if (orderdetail == null)
            {
                orderdetail = new List<AdviceOrderDetailDTO>();   
            }
            if (order == null) {
                return RedirectToAction("CheckOrder", "FrontHome");
            }
            var detail = new AdviceDetailDTO
            {
                OName = order.Oname,
                OPhone = order.Ophone,
                OEmail = order.Oemail,
                OAddress = order.Oaddress,
                OId = id,
                Odate = order.Odate.ToString("yyyy-MM-dd"),
                Ostatus = order.Ostatus,
                OPayment = order.Opayment,
                orderdetail = orderdetail,
            };
            return Json(detail); 
        }

		[HttpPost]
        public IActionResult CancelOrder([FromBody] Torder cancelRequest)
        {
            using (var db = new DbuniPayContext())
            {
                var order = db.Torders.FirstOrDefault(o => o.Oid == cancelRequest.Oid);
                if (order == null)
                {
                    return Json(new { success = false, message = "訂單不存在" });
                }
				order.OcancelReason = cancelRequest.OcancelReason;
				order.OcancelDescription = cancelRequest.OcancelDescription;
				order.OcancelDate = DateTime.UtcNow;
				order.OcancelStatus = "申請中";

				db.SaveChanges();
				return Json(new { success = true, message = "訂單已成功取消" });
			}
		}


        [HttpPost]
        public IActionResult AddComment([FromForm] CommentDTO dto)
        {
            try
            {
                using (var db = new DbuniPayContext())
                {
                    string json = HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER);
                    if (string.IsNullOrEmpty(json))
                    {
                        return Json(new { success = false, message = "請先登入" });
                    }

                    var member = JsonSerializer.Deserialize<Tmember>(json);
                    var order = db.Torders.FirstOrDefault(o => o.Oid == dto.Oid && o.Mid == member.Mid && o.Ostatus == "已完成");
                    if (order == null)
                    {
                        return Json(new { success = false, message = "訂單不存在或未完成" });
                    }

                    var orderDetail = db.TorderDetails.FirstOrDefault(od => od.Oid == dto.Oid && od.Pid == dto.Pid);
                    if (orderDetail == null)
                    {
                        return Json(new { success = false, message = "商品不存在於該訂單" });
                    }
                    if (orderDetail.IsReviewed)
                    {
                        return Json(new { success = false, message = "該商品已評論" });
                    }

                    string savePath1 = null, savePath2 = null;
                    if (dto.Image1 != null)
                    {
                        savePath1 = Guid.NewGuid() + Path.GetExtension(dto.Image1.FileName);
                        using (var stream = new FileStream(Path.Combine("wwwroot/uploads", savePath1), FileMode.Create))
                        {
                            dto.Image1.CopyTo(stream);
                        }
                    }
                    if (dto.Image2 != null)
                    {
                        savePath2 = Guid.NewGuid() + Path.GetExtension(dto.Image2.FileName);
                        using (var stream = new FileStream(Path.Combine("wwwroot/uploads", savePath2), FileMode.Create))
                        {
                            dto.Image2.CopyTo(stream);
                        }
                    }

                    var newComment = new Tcomment
                    {
                        Mid = member.Mid,
                        Mname = member.Mname,
                        Oid = dto.Oid,
                        Pid = dto.Pid,
                        Psize = dto.PSize,
                        Pcolor = dto.PColor,
                        Rating = dto.Rating,
                        Comment = dto.CommentText,
                        ComCreateDate = DateTime.Now,
                        ComImage1 = savePath1,
                        ComImage2 = savePath2
                    };

                    db.Tcomments.Add(newComment);
                    orderDetail.IsReviewed = true;
                    db.SaveChanges();

                    return Json(new { success = true, message = "評論已提交" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "發生錯誤: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetMemberId()
        {
            string json = HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER);
            if (string.IsNullOrEmpty(json))
            {
                return Json(new { success = false, message = "Session 已過期或未登入" });
            }

            var member = JsonSerializer.Deserialize<Tmember>(json);
            return Json(new { success = true, memberId = member.Mid });
        }
    }


     
}

