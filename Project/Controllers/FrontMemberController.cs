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
        public FrontMemberController(IWebHostEnvironment p)// 建構函式，注入環境變數
        {
            _enviro = p;
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
            using var db = new DbuniPayContext();
            bool isDuplicate = false;

            // 中文註解：根據前端傳來的 fieldName 判斷要檢查哪個欄位
            if (req.fieldName == "Maccount")
            {
                isDuplicate = db.Tmembers.Any(m => m.Maccount == req.fieldValue);
            }
            else if (req.fieldName == "Memail")
            {
                isDuplicate = db.Tmembers.Any(m => m.Memail == req.fieldValue);
            }
            else if (req.fieldName == "Mphone")
            {
                isDuplicate = db.Tmembers.Any(m => m.Mphone == req.fieldValue);
            }

            // 回傳 JSON：isDuplicate=true 表示重複；false 表示不重複
            return Json(new { isDuplicate });
        }
        [HttpPost]
        public IActionResult fcreate(CMemberWrap memberWrap)
        {
            using var db = new DbuniPayContext();

            // (1)【新增】後端再次檢查帳號/Email/手機是否重複，防止前端繞過JS
            bool isAccountExists = db.Tmembers.Any(m => m.Maccount == memberWrap.member.Maccount);
            if (isAccountExists)
                return Json(new { success = false, message = "帳號已有人使用" });

            bool isEmailExists = db.Tmembers.Any(m => m.Memail == memberWrap.member.Memail);
            if (isEmailExists)
                return Json(new { success = false, message = "Email已有人使用" });

            bool isPhoneExists = db.Tmembers.Any(m => m.Mphone == memberWrap.member.Mphone);
            if (isPhoneExists)
                return Json(new { success = false, message = "手機已有人使用" });
            memberWrap.member.McreatedDate = DateTime.Now; //設定會員的建立日期為目前時間
            try
            {
                db.Tmembers.Add(memberWrap.member);//將會員資料加入資料庫並儲存變更
                db.SaveChanges();//成功後導向到「登入」頁面
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
            DbuniPayContext db = new DbuniPayContext();
            var member = db.Tmembers.FirstOrDefault(m => m.Mid == id);
            if (member == null)
                return RedirectToAction("List");
            return View(member);
        }
        public IActionResult fprofile()
        {
            DbuniPayContext db = new DbuniPayContext();// 建議使用 using 以確保資源釋放
            string json = HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER); // 取得 Session 中的使用者 JSON
            var member = JsonSerializer.Deserialize<Tmember>(json);// 反序列化為 Tmember 物件
            Tmember T = db.Tmembers.FirstOrDefault(c => c.Mid == member.Mid);// 從資料庫中取得對應的會員資料
            CMemberWrap C = new CMemberWrap() { member = T };// 將會員資料包裝到 ViewModel 中
            return View(C);// 返回個人資料頁面
        }

        [HttpPost]
        public IActionResult fprofile(CMemberWrap t)
        {
            DbuniPayContext db = new DbuniPayContext();
            Tmember T = db.Tmembers.FirstOrDefault(c => c.Mid == t.Mid);
       
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
                db.SaveChanges();
            }

            string json = JsonSerializer.Serialize(T); // 序列化模型数据
            HttpContext.Session.SetString(CDictionary.SK_LOGEDIN_USER, json); // 更新 Session
            return RedirectToAction("fprofile"); // 重定向到 Profile 页面
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePassword C)
        {
            DbuniPayContext db = new DbuniPayContext();
            var json = HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER);
            Tmember member = JsonSerializer.Deserialize<Tmember>(json);
            Tmember T = db.Tmembers.FirstOrDefault(c => c.Mid == member.Mid);
            if (T != null)
            {

                T.Mpassword = C.Mpassword;
                db.SaveChanges();
            }
            string json2 = JsonSerializer.Serialize(T); // 序列化模型数据
            HttpContext.Session.SetString(CDictionary.SK_LOGEDIN_USER, json2); // 更新 Session
            return RedirectToAction("fprofile");
        }
        //[HttpPost]                                              // 指定此方法只接受 POST 請求
        //public IActionResult SendVerificationCode([FromBody] request) // 接收前端傳來的 EmailDto
        //{
        //    // 如果 Email 為空，回傳錯誤 JSON
        //    if (string.IsNullOrEmpty(request.Email))
        //    {
        //        return Json(new { success = false, message = "Email不可為空" });
        //    }

        //    try
        //    {
        //        // 產生 6 位數隨機驗證碼
        //        Random random = new Random();                              // 建立 Random 實例
        //        int verificationCode = random.Next(100000, 999999);        // 產生介於 100000~999999 的數字

        //        // 將驗證碼存入 Session (字串格式)
        //        HttpContext.Session.SetString("VerificationCode", verificationCode.ToString());

        //        // 寄送 Email 的示範 (請自行修改為您的 SMTP / Email)
        //        var senderEmail = "yourEmail@gmail.com";                   // 寄件者 Email
        //        var senderPassword = "yourEmailPassword";                  // 寄件者密碼或應用程式金鑰
        //        var smtpClient = new SmtpClient("smtp.gmail.com")          // 使用 Gmail SMTP
        //        {
        //            Port = 587,                                            // Gmail SMTP 連接埠
        //            Credentials = new NetworkCredential(senderEmail, senderPassword), // 帳密驗證
        //            EnableSsl = true,                                      // 啟用 SSL
        //        };

        //        MailMessage mail = new MailMessage();                      // 建立郵件物件
        //        mail.From = new MailAddress(senderEmail);                  // 寄件者
        //        mail.To.Add(request.Email);                                // 收件者 (前端輸入的 Email)
        //        mail.Subject = "您的驗證碼";                                 // 郵件主旨
        //        mail.Body = $"您的驗證碼是: {verificationCode}";            // 郵件內容

        //        smtpClient.Send(mail);                                     // 寄送郵件

        //        // 回傳成功 JSON
        //        return Json(new { success = true });
        //    }
        //    catch (Exception ex)                                           // 捕捉寄送郵件的任何錯誤
        //    {
        //        // 回傳錯誤 JSON，包含例外訊息
        //        return Json(new { success = false, message = ex.Message });
        //    }
        //}

        //[HttpPost]                                                        // 指定此方法只接受 POST 請求
        //public IActionResult VerifyCode([FromBody] CodeDto request)       // 接收前端傳來的 CodeDto
        //{
        //    // 如果驗證碼為空，回傳錯誤 JSON
        //    if (string.IsNullOrEmpty(request.Code))
        //    {
        //        return Json(new { success = false, message = "驗證碼不可為空" });
        //    }

        //    // 從 Session 取出先前存的驗證碼
        //    var savedCode = HttpContext.Session.GetString("VerificationCode");

        //    // 比對使用者輸入的驗證碼與 Session 中的驗證碼
        //    if (savedCode == request.Code)
        //    {
        //        // 驗證成功
        //        return Json(new { success = true });
        //    }
        //    else
        //    {
        //        // 驗證失敗
        //        return Json(new { success = false, message = "驗證碼錯誤" });
        //    }
        //}
    }
}
     

    
