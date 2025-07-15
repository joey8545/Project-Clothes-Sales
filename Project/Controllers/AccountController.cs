using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.ViewModel;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization; 
using System.Linq;

namespace Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly DbuniPayContext _context;

        public AccountController(DbuniPayContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Login([FromBody] FloginViewModel m)
        {
            
            if (m == null || string.IsNullOrEmpty(m.faccount) || string.IsNullOrEmpty(m.fpassword))
            {
                return Json(new { success = false, message = "帳號和密碼不能為空" });
            }

            try
            {
                
                Tmember? member = _context.Tmembers.FirstOrDefault(
                    c => c.Maccount == m.faccount && c.Mpassword == m.fpassword
                );

                if (member != null)
                {
                    string json = JsonSerializer.Serialize(member);
                    HttpContext.Session.SetString(CDictionary.SK_LOGEDIN_USER, json);

                    return Json(new
                    {
                        success = true,
                        redirectUrl = Url.Action("FrontIndex", "FrontHome")
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "帳號或密碼錯誤"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "系統錯誤，請稍後再試"
                });
            }
        }

        
        [HttpGet]
        public IActionResult CheckLoginStatus()
        {
            try
            {
                var memberJson = HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER);
                if (!string.IsNullOrEmpty(memberJson))
                {
                    try
                    {
                        
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
                HttpContext.Session.Remove(CDictionary.SK_LOGEDIN_USER);
                return Json(new
                {
                    success = true,
                    redirectUrl = "FrontIndex/FrontHome" 
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "登出過程發生錯誤"
                });
            }
        }
    }
}