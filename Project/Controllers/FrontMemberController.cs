using Microsoft.AspNetCore.Mvc;     // 引用 ASP.NET Core MVC 的命名空間，用於建立控制器和動作方法
using Microsoft.Extensions.Hosting; // 引用主機環境相關功能的命名空間 (IWebHostEnvironment 等)
using Project.Models;               // 引用專案中 Models 資料夾的類別（例如 Tmember、DbuniPayContext）
using Project.ViewModel;            // 引用專案中 ViewModel 資料夾的類別
using System;                       // 基本系統功能，包含DateTime等
using System.Collections.Generic;   // 提供集合 (List、Dictionary) 等相關功能
using System.Globalization;
using System.Net.Mail;
using System.Net;
using System.Text.Json;             // 提供 JSON 序列化/反序列化功能

namespace Project.Controllers
{
    public class FrontMemberController : Controller
    {
        private readonly IWebHostEnvironment _enviro;// 存取伺服器環境資訊的變數

        private readonly DbuniPayContext _db;
        public FrontMemberController(IWebHostEnvironment p,DbuniPayContext db)// 建構函式，注入環境變數
        {
            _enviro = p;
            _db = db;
        }
        [HttpGet]
        public IActionResult HandleProfileClick()
        {// 中文：從 Session 取得用戶 JSON         
            var userJson = HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER);
            if (string.IsNullOrEmpty(userJson))
            {// 中文：若未登入，回傳 false 與重定向                
                return Json(new { success = false, redirectUrl = "/FrontMember/fcreate" });
            }           
            var member = JsonSerializer.Deserialize<Tmember>(userJson); // 反序列化為 Tmember 物件，取得 Mpermissions
            int perm = member.Mpermissions; // perm 即會員權限數字                    
            var menuList = new List<object>//建立一個清單來裝選單項目
            {
                new { text = "個人資料", url = "/FrontMember/fprofile" },
                new { text = "我的訂單", url = "/FrontHome/CheckOrder" },
                new { text = "優惠券",   url = "/FrontHome/Coupon" }
            };                     
            var validPerms = new int[] { 1 };//如果權限在 [11,21,31,51,61,71,81,91]，則顯示「後檯」
            if (validPerms.Contains(perm))
            {
                menuList.Add(new { text = "後檯", url = "/Home/Index" });
            }            
            menuList.Add(new { text = "登出", url = "/Account/Logout" });//最後一定都有「登出」          
            return Json(new //回傳 JSON，success = true，data = menuList
            {
                success = true,
                data = menuList.ToArray()
            });
        }

        public IActionResult fcreate()// 會員註冊頁面
        {
            var model = new CMemberWrap
            {
                member = new Tmember()
                {
                    Mgender = -1// 預設性別設定為 -1（未選擇）
                }
            };
            return View(model); // 將 model 傳遞到 fcreate.cshtml 這個 View
        }
        [HttpPost]
        public JsonResult CheckDuplicate([FromBody] CheckDuplicateRequest req)
        {
            bool isDuplicate = false;

            // 中文註解：根據前端傳來的 fieldName 判斷要檢查哪個欄位
            if (req.fieldName == "Maccount")
            {
                isDuplicate = _db.Tmembers.Any(m => m.Maccount == req.fieldValue);
            }
            else if (req.fieldName == "Memail")
            {
                isDuplicate = _db.Tmembers.Any(m => m.Memail == req.fieldValue);
            }
            else if (req.fieldName == "Mphone")
            {
                isDuplicate = _db.Tmembers.Any(m => m.Mphone == req.fieldValue);
            }

            // 回傳 JSON：isDuplicate=true 表示重複；false 表示不重複
            return Json(new { isDuplicate });
        }
        [HttpPost]
        public IActionResult fcreate(CMemberWrap memberWrap)
        {

            // (1)【新增】後端再次檢查帳號/Email/手機是否重複，防止前端繞過JS
            bool isAccountExists = _db.Tmembers.Any(m => m.Maccount == memberWrap.member.Maccount);
            if (isAccountExists)
                return Json(new { success = false, message = "帳號已有人使用" });

            bool isEmailExists = _db.Tmembers.Any(m => m.Memail == memberWrap.member.Memail);
            if (isEmailExists)
                return Json(new { success = false, message = "Email已有人使用" });

            bool isPhoneExists = _db.Tmembers.Any(m => m.Mphone == memberWrap.member.Mphone);
            if (isPhoneExists)
                return Json(new { success = false, message = "手機已有人使用" });
            memberWrap.member.McreatedDate = DateTime.Now; //設定會員的建立日期為目前時間
            try
            {
				_db.Tmembers.Add(memberWrap.member);//將會員資料加入資料庫並儲存變更
				_db.SaveChanges();//成功後導向到「登入」頁面
                return RedirectToAction("fcreate", "FrontMember");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"註冊失敗: {ex.Message}"); // 中文：若發生例外，將錯誤訊息加入 ModelState，並返回表單
                return View(memberWrap);
            }
        }
        public IActionResult Edit(int id)
        {
            var member = _db.Tmembers.FirstOrDefault(m => m.Mid == id);
            if (member == null)
                return RedirectToAction("List");
            return View(member);
        }
        public IActionResult fprofile()
        {
            string json = HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER); // 取得 Session 中的使用者 JSON
            var member = JsonSerializer.Deserialize<Tmember>(json);// 反序列化為 Tmember 物件
            Tmember T = _db.Tmembers.FirstOrDefault(c => c.Mid == member.Mid);// 從資料庫中取得對應的會員資料
            CMemberWrap C = new CMemberWrap() { member = T };// 將會員資料包裝到 ViewModel 中
            return View(C);// 返回個人資料頁面
        }

        [HttpPost]
        public IActionResult fprofile(CMemberWrap t)
        {
            Tmember T = _db.Tmembers.FirstOrDefault(c => c.Mid == t.Mid);
       
            if (T != null)
            {
                T.Mname = t.Mname;
                T.Mgender = t.Mgender.GetValueOrDefault(-1);
                T.Memail = t.Memail;
                T.Maddress = t.Maddress;               
                T.Mphone = t.Mphone;

                if (!string.IsNullOrEmpty(t.Mbirthday))
                {
                    T.Mbirthday = DateOnly.ParseExact(
                        t.Mbirthday,
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture
                    );
                }
                if (t.photoPath != null)
                {
                    string photoName = Guid.NewGuid().ToString() + ".jpg";
                    T.Mphoto = photoName;
                    t.photoPath.CopyTo(new FileStream(_enviro.WebRootPath + "/Images/" + photoName, FileMode.Create));
                }
				_db.SaveChanges();
            }

            string json = JsonSerializer.Serialize(T); // 序列化模型数据
            HttpContext.Session.SetString(CDictionary.SK_LOGEDIN_USER, json); // 更新 Session
            return RedirectToAction("fprofile"); // 重定向到 Profile 页面
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePassword C)
        {
            var json = HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER);
            Tmember member = JsonSerializer.Deserialize<Tmember>(json);
            Tmember T = _db.Tmembers.FirstOrDefault(c => c.Mid == member.Mid);
            if (T != null)
            {

                T.Mpassword = C.Mpassword;
				_db.SaveChanges();
            }
            string json2 = JsonSerializer.Serialize(T); // 序列化模型数据
            HttpContext.Session.SetString(CDictionary.SK_LOGEDIN_USER, json2); // 更新 Session
            return RedirectToAction("fprofile");
        }
        
    }
}
     

    
