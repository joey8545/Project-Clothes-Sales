using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Project.Models;

public partial class Tmember
{
    public int Mid { get; set; }
    [DisplayName("姓名")]
    public string Mname { get; set; } = null!;
    [DisplayName("性別")]
    public int Mgender { get; set; }
    [DisplayName("帳號")]
    public string Maccount { get; set; } = null!;
    [DisplayName("密碼")]
    public string Mpassword { get; set; } = null!;
    [DisplayName("電子郵件")]
    public string Memail { get; set; } = null!;
    [DisplayName("地址")]
    public string Maddress { get; set; } = null!;
    [DisplayName("生日")]
    public DateOnly Mbirthday { get; set; }
    [DisplayName("電話")]
    public string Mphone { get; set; } = null!;
    [DisplayName("點數")]
    public int Mpoints { get; set; }
    [DisplayName("權限")]
    public int Mpermissions { get; set; }
    [DisplayName("創立日期")]
    public DateTime McreatedDate { get; set; }
    [DisplayName("照片")]
    public string? Mphoto { get; set; }
    [DisplayName("黑名單")]
    public bool MisHided { get; set; }
}
