using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Project.Models;
using Project.ViewModel;
using System.Collections.Generic;
using System.Text.Json;

namespace Project.Controllers
{
    public class MemberController : Controller
    {
        IWebHostEnvironment _enviro = null;
        public MemberController(IWebHostEnvironment p)
        {
            _enviro = p;
        }

        public IActionResult List(CKeywordViewModel vm)
        {
            DbuniPayContext db = new DbuniPayContext();
            string keyword = vm.txtKeyword;
            IEnumerable<Tmember> datas = null;

            if (string.IsNullOrEmpty(keyword))
                datas = db.Tmembers.Where(m => m.MisHided == false); // 只顯示未加入黑名單的會員
            else
                datas = db.Tmembers.Where
                    (p => p.MisHided == false &&
                    (p.Mname.Contains(keyword)
                || (keyword == "男" && p.Mgender == 0)
                || (keyword == "女" && p.Mgender == 1)
                || p.Maccount.Contains(keyword)
                || p.Memail.Contains(keyword)
                || p.Maddress.Contains(keyword)
                || p.Mbirthday.ToString().Contains(keyword)
                || p.Mphone.Contains(keyword)));

            List<CMemberWrap> list = new List<CMemberWrap>();
            foreach (var t in datas)
                list.Add(new CMemberWrap() { member = t });
            return View(list);
        }
        public IActionResult BlacklistIndex()
        {
            using (DbuniPayContext db = new DbuniPayContext())
            {
                // 獲取已加入黑名單的會員
                var blacklistedMembers = db.Tmembers.Where(m => m.MisHided == true).ToList();
                List<CMemberWrap> list = blacklistedMembers.Select(t => new CMemberWrap { member = t }).ToList();
                return View("Blacklist", list); // 返回 Blacklist 視圖
            }
        }
        [HttpGet] // 加入黑名單的處理方法
        public IActionResult Blacklist(int id)
        {
            DbuniPayContext db = new DbuniPayContext();
            var member = db.Tmembers.FirstOrDefault(m => m.Mid == id);

            if (member != null)
            {
                member.MisHided = true; // 設定MIsHided為true，將會員加入黑名單
                try
                {
                    db.SaveChanges(); // 保存變更
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"更新失敗: {ex.Message}");
                    return RedirectToAction("List");
                }
            }
            return RedirectToAction("List"); // 重定向回會員列表
        }
        
        public IActionResult RemoveFromBlacklist(int id)
        {
            DbuniPayContext db = new DbuniPayContext();
            var member = db.Tmembers.FirstOrDefault(m => m.Mid == id);

            if (member != null)
            {
                member.MisHided = false; // 移除黑名單
                try
                {
                    db.SaveChanges(); // 保存變更
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"更新失敗: {ex.Message}");
                    return RedirectToAction("Blacklist");
                }
            }

            return RedirectToAction("List"); // 重定向回黑名單頁面
        }

        public IActionResult Edit(int id)
        {
            DbuniPayContext db = new DbuniPayContext();
            var member = db.Tmembers.FirstOrDefault(m => m.Mid == id);
            if (member == null)
                return RedirectToAction("List");
            return View(member);
        }

  
        [HttpPost]
        public IActionResult Edit(Tmember p, IFormFile photo)
        {
            DbuniPayContext db = new DbuniPayContext();
            Tmember x = db.Tmembers.FirstOrDefault(c => c.Mid == p.Mid);
            if (x != null)
            {
                x.Mname = p.Mname;
                x.Mgender = p.Mgender;
                x.Maccount = p.Maccount;
                x.Mpassword = p.Mpassword;
                x.Memail = p.Memail;
                x.Maddress = p.Maddress;
                x.Mbirthday = p.Mbirthday;
                x.Mphone = p.Mphone;
                x.Mpoints = p.Mpoints;
                x.Mpermissions = p.Mpermissions;

                // 處理照片上傳
                if (photo != null && photo.Length > 0)
                {
                    // 檢查檔案格式
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                    string extension = Path.GetExtension(photo.FileName).ToLower();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("photo", "僅允許上傳 jpg, jpeg, png, gif 格式的圖片");
                        return View(x);
                    }

                    // 檢查檔案大小 (5MB)
                    if (photo.Length > 1 * 1024 * 1024)
                    {
                        ModelState.AddModelError("photo", "圖片大小不能超過 1MB");
                        return View(x);
                    }

                    try
                    {
                        // 刪除舊圖片
                        if (!string.IsNullOrEmpty(x.Mphoto))
                        {
                            string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", x.Mphoto);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // 上傳新圖片
                        string fileName = Guid.NewGuid().ToString() + extension;  // 使用 GUID 避免檔名重複
                        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            photo.CopyTo(stream);
                        }

                        x.Mphoto = fileName;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"圖片上傳失敗: {ex.Message}");
                        return View(x);
                    }
                }

                try
                {
                    db.SaveChanges();
                    return RedirectToAction("List");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"儲存失敗: {ex.Message}");
                    return View(x);
                }
            }
            return View(x);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CMemberWrap memberWrap, IFormFile photo)
        {

            DbuniPayContext db = new DbuniPayContext();
            if (photo != null && photo.Length > 0)
            {
                // 檢查檔案類型
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                string extension = Path.GetExtension(photo.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("photo", "只接受 jpg, jpeg, png, gif 格式的圖片");
                    return View(memberWrap);
                }

                if (photo.Length > 1 * 1024 * 1024)
                {
                    ModelState.AddModelError("photo", "圖片大小不能超過 1MB");
                    return View(memberWrap);
                }

                try
                {
                    string fileName = Guid.NewGuid().ToString() + extension;
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                    // 確保資料夾存在
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images"));

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        photo.CopyTo(stream);
                    }

                    memberWrap.member.Mphoto = fileName;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("photo", $"圖片上傳失敗: {ex.Message}");
                    return View(memberWrap);
                }
            }

            memberWrap.member.McreatedDate = DateTime.Now;
            db.Tmembers.Add(memberWrap.member);

            try
            {
                db.SaveChanges();
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"儲存失敗: {ex.Message}");
                return View(memberWrap);
            }
            
                        
        }

        public IActionResult Profile()
        {
            DbuniPayContext db = new DbuniPayContext();
            string json = HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER);
            if (json==null) {
                return RedirectToAction("Login", "Home");
            }
            var member = JsonSerializer.Deserialize<Tmember>(json);
            Tmember T = db.Tmembers.FirstOrDefault(c => c.Mid == member.Mid);
            CMemberWrap C = new CMemberWrap() { member = T };
            return View(C);
        }

        [HttpPost]
        public IActionResult Profile(MemberProfilecs t)
        {
            DbuniPayContext db = new DbuniPayContext();
            Tmember T = db.Tmembers.FirstOrDefault(c => c.Mid == t.Mid);
            var Mgender= t.Mgender == "男性" ? 0 : 1;
            if (T!=null) 
            {
                T.Mname = t.Mname;
                T.Mgender = Mgender;
                T.Memail = t.Memail;
                T.Maddress = t.Maddress;
                T.Mbirthday =t.Mbirthdays;
                T.Mphone = t.Mphone;
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
            return RedirectToAction("Profile"); // 重定向到 Profile 页面
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePassword C)
        {
            DbuniPayContext db = new DbuniPayContext();
            var json = HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER);
            Tmember member = JsonSerializer.Deserialize<Tmember>(json);
            Tmember T = db.Tmembers.FirstOrDefault(c => c.Mid == member.Mid);
            if (T!=null) {
               
                    T.Mpassword = C.Mpassword;
                    db.SaveChanges();
            }
            string json2 = JsonSerializer.Serialize(T); // 序列化模型数据
            HttpContext.Session.SetString(CDictionary.SK_LOGEDIN_USER, json2); // 更新 Session
            return RedirectToAction("Profile");
        }

    }

}