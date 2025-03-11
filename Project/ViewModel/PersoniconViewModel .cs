using System.ComponentModel.DataAnnotations; // 引入驗證特性
namespace Project.ViewModel
{
    public class PersoniconViewModel
    {
        [Required(ErrorMessage = "請輸入帳號")] // 必填驗證
        [Display(Name = "帳號")] // 顯示名稱
        public string faccount { get; set; }

        [Required(ErrorMessage = "請輸入密碼")] // 必填驗證
        [Display(Name = "密碼")] // 顯示名稱
        [DataType(DataType.Password)] // 密碼類型
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密碼長度需介於 6-100 字元之間")] // 長度驗證
        public string fpassword { get; set; }
    }
}
