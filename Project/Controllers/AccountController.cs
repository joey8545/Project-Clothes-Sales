using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.ViewModel;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization; // 引入 ReferenceHandler 命名空間
using System.Linq;

namespace Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly DbuniPayContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(DbuniPayContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // AJAX登入功能 - 接收JSON格式的帳號密碼
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([FromBody] FloginViewModel m)
        {
            // 驗證模型
            if (m == null || string.IsNullOrEmpty(m.faccount) || string.IsNullOrEmpty(m.fpassword))
            {
                _logger.LogWarning("登入失敗: 帳號或密碼為空");
                return Json(new { success = false, message = "帳號和密碼不能為空" });
            }

            try
            {
                // 查詢資料庫
                Tmember? member = _context.Tmembers.FirstOrDefault(
                    c => c.Maccount == m.faccount && c.Mpassword == m.fpassword
                );

                if (member != null)
                {
                    _logger.LogInformation($"登入成功: {m.faccount}");

                    // 登入成功，將會員資料存入Session
                    var options = new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.Preserve,
                    };
                    string json = JsonSerializer.Serialize(member, options);
                    HttpContext.Session.SetString(CDictionary.SK_LOGEDIN_USER, json);

                    return Json(new
                    {
                        success = true,
                        redirectUrl = Url.Action("FrontIndex", "FrontHome")
                    });
                }
                else
                {
                    _logger.LogWarning($"登入失敗: 用戶 {m.faccount} 帳號或密碼錯誤");
                    return Json(new
                    {
                        success = false,
                        message = "帳號或密碼錯誤"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"登入處理錯誤: {m.faccount}");
                return Json(new
                {
                    success = false,
                    message = "系統錯誤，請稍後再試"
                });
            }
        }

        // 檢查登入狀態的API
        [HttpGet]
        public IActionResult CheckLoginStatus()
        {
            try
            {
                var memberJson = HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER);

                // 檢查是否已登入
                if (!string.IsNullOrEmpty(memberJson))
                {
                    try
                    {
                        // 嘗試反序列化會員資料
                        var member = JsonSerializer.Deserialize<Tmember>(memberJson);
                        if (member != null)
                        {
                            return Json(new
                            {
                                success = true,
                                isLoggedIn = true,
                                username = member.Mname,
                                redirectUrl = Url.Action("fprofile", "FrontMember")
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "反序列化會員資料時發生錯誤");
                        HttpContext.Session.Remove(CDictionary.SK_LOGEDIN_USER);
                    }
                }

                // 未登入或登入失效
                return Json(new
                {
                    success = true,
                    isLoggedIn = false,
                    redirectUrl = Url.Action("fcreate", "FrontMember")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "檢查登入狀態時發生錯誤");
                return Json(new
                {
                    success = false,
                    isLoggedIn = false,
                    message = "檢查登入狀態時發生錯誤"
                });
            }
        }
        [HttpPost]
        public IActionResult Logout()
        {
            try
            {
                // 清除Session中的用戶資料
                HttpContext.Session.Remove(CDictionary.SK_LOGEDIN_USER);
                return Json(new
                {
                    success = true,
                    redirectUrl = "FrontIndex/FrontHome" // 添加重定向URL
                });
            }
            catch (Exception ex)
            {
                // 記錄錯誤並返回錯誤訊息
                _logger.LogError(ex, "登出過程發生錯誤");
                return Json(new
                {
                    success = false,
                    message = "登出過程發生錯誤"
                });
            }
        }
    }
}