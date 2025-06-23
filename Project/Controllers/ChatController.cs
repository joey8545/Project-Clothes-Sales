using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Controllers
{
    public class ChatController : Controller
    {
        private readonly DbuniPayContext _db;

        public ChatController(DbuniPayContext db) 
        { 
            _db= db;
        }
        public IActionResult ChatRoom()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetMemberInfo()
        {
            if (!Int32.TryParse(HttpContext.Session.GetString(CDictionary.SK_LOGEDIN_USER), out int memberId))
            {
                return Json(new { success = false, message = "用戶未登入。" });
            }
            var member = _db.Tmembers.FirstOrDefault(p => p.Mid == memberId);
            if (member == null)
            {
                return Json(new { success = false, message = "會員資料不存在。" });
            }
            return Json(new
            {
                success = true,
                data = new
                {
                    id = member.Mid,
                    name = member.Mname,
                    gender = member.Mgender,
                    account = member.Maccount,
                    email = member.Memail,
                    address = member.Maddress,
                    birthday = member.Mbirthday.ToString("yyyy-MM-dd"),
                    phone = member.Mphone,
                    points = member.Mpoints,
                    permissions = member.Mpermissions,
                    createdDate = member.McreatedDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    photo = member.Mphoto,
                    isHided = member.MisHided
                }
            });
        }

        // 上傳檔案
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Json(new { success = false, message = "檔案無效或為空。" });
            }

            try
            {
                string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                string filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Json(new
                {
                    success = true,
                    fileUrl = $"/uploads/{fileName}",
                    originalName = file.FileName,
                    contentType = file.ContentType
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"檔案上傳錯誤: {ex}");
                return Json(new { success = false, message = "檔案上傳失敗，請稍後再試。" });
            }
        }

        // 儲存聊天訊息
        [HttpPost]
        public async Task<IActionResult> SaveMessage(int chatId, int messageSendId, string messageContent)
        {
            if (string.IsNullOrEmpty(messageContent))
            {
                return Json(new { success = false, message = "訊息內容不能為空。" });
            }

            try
            {
                var message = new Tmessage
                {
                    ChatId = chatId,
                    MessageSendId = messageSendId,
                    MessageContent = messageContent,
                    MessageTime = DateTime.UtcNow
                };

				_db.Tmessages.Add(message);
                await _db.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    messageId = message.MessageId,
                    messageTime = message.MessageTime
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"訊息儲存錯誤: {ex}");
                return Json(new { success = false, message = "訊息保存失敗，請稍後再試。" });
            }
        }
    }
}
