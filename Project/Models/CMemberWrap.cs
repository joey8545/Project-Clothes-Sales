using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Project.Models
{
    public class CMemberWrap
    {
        private Tmember _member = null;
        public Tmember member
        {
            get { return _member; }
            set { _member = value; }
        }
        public CMemberWrap()
        {
            _member = new Tmember();
            _member.Mpoints = 0; // 設定初始點數為 0
        }
        public int Mid
        {
            get { return _member.Mid; }
            set { _member.Mid = value; }
        }
        [Required(ErrorMessage = "姓名必須填寫")]
        [DisplayName("姓名")]
        public string Mname
        {
            get { return _member.Mname; }
            set { _member.Mname = value; }
        }
        [Required(ErrorMessage = "性別必須選擇")]
        [DisplayName("性別")]
        public int? Mgender
        {
            get
            {
                // 中文註解：若 _member.Mgender < 0，表示未選；否則回傳實際值
                if (_member.Mgender < 0)
                    return -1;
                else
                    return _member.Mgender;
            }
            set
            {
                // 中文註解：若 set 進來是 null，表示未選；否則設置對應值
                if (value == null)
                    _member.Mgender = -1; // 表示未選
                else
                    _member.Mgender = value.Value;
            }
        }   
        public string GenderText// 中文註解：根據 Mgender 顯示文字
        {
            get
            {
                // 中文註解：若 Mgender=0 => 男性, =1 => 女性, 其餘 => 空字串
                if (_member.Mgender == 0) return "男性";
                else if (_member.Mgender == 1) return "女性";
                else return "";
            }
        }
        [Required(ErrorMessage = "帳號必須填寫")]
        [DisplayName("帳號")]
        public string Maccount
        {
            get { return _member.Maccount; }
            set { _member.Maccount = value; }
        }
        [Required(ErrorMessage = "密碼必須填寫")]
        [DisplayName("密碼")]
        public string Mpassword
        {
            get { return _member.Mpassword; }
            set { _member.Mpassword = value; }
        }

        [NotMapped] //不對應資料庫欄位
        [Required(ErrorMessage = "請輸入確認密碼")] //必填錯誤訊息
        [Compare("Mpassword", ErrorMessage = "兩次輸入的密碼不相符")]
        //比對 Mpassword，若不同顯示錯誤
        [Display(Name = "確認密碼")] //顯示用標題
        public string? ConfirmPassword { get; set; }
        [Required(ErrorMessage = "E-mail必須填寫")]
        [EmailAddress(ErrorMessage = "請輸入正確的 E-mail 格式")]
        [DisplayName("電子郵件")]
        public string? Memail
        {
            get { return _member.Memail; }
            set { _member.Memail = value; }
        }
        [Required(ErrorMessage = "地址必須填寫")]
        [DisplayName("地址")]
        public string? Maddress
        {
            get { return _member.Maddress; }
            set { _member.Maddress = value; }
        }
        [Required(ErrorMessage = "請選擇生日")]
        [DisplayName("生日")]
        public string? Mbirthday
        {
            get
            {// 中文註解：若 _member.Mbirthday 是預設值(0001-01-01)，則回傳空字串                         
                if (_member.Mbirthday == default)
                    return "";
                else
                    return _member.Mbirthday.ToString("yyyy-MM-dd");
            }
            set
            {// 中文註解：若使用者沒有輸入，則不做 parse
                if (!string.IsNullOrEmpty(value))
                {
                    _member.Mbirthday = DateOnly.Parse(value);
                }
                else
                {// 若真的需要清空, 可將 _member.Mbirthday 設成 default(DateOnly)               
                    _member.Mbirthday = default;
                }
            }
        }        
        [Required(ErrorMessage = "電話必須填寫")]
        [DisplayName("電話")]
        public string? Mphone
        {
            get { return _member.Mphone; }
            set { _member.Mphone = value; }
        }
        [DisplayName("點數")]
        public int Mpoints
        {
            get { return _member.Mpoints; }
            set { _member.Mpoints = value; }
        }
     
        [DisplayName("權限")]
        public int Mpermissions
        {
            get { return _member.Mpermissions; }
            set { _member.Mpermissions = value; }
        }

             public string PermissionText
             {
                get
                {
                    return _member.Mpermissions switch
                    {
                        0 => "會員",
                        1 => "員工",                    
                        _ => "未定義權限"  // 加入預設值
                    };
                }
             }
        
        [DisplayName("創建日期")]
        public string? McreatedDate
        {
            get { return _member.McreatedDate.ToString("yyyy-MM-dd HH:mm:ss"); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _member.McreatedDate = DateTime.Parse(value);
                }
            }
        }
        [DisplayName("會員照片")]
        public string? Mphoto
        {
            get { return _member.Mphoto; }
            set { _member.Mphoto = value; }
        }
        [DisplayName("黑名單")]
        public bool Mishided
        {
            get { return _member.MisHided; }
            set { _member.MisHided = value; }
        }

        public IFormFile photoPath { get; set; }

      
    }
}
    

