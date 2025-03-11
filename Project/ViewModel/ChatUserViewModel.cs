namespace Project.ViewModel
{   /// <summary>
    /// 聊天系統用戶資訊 ViewModel
    /// </summary>
    public class ChatUserViewModel
    {      
            /// <summary>
            /// 會員ID
            /// </summary>
            public int MID { get; set; }

            /// <summary>
            /// 會員名稱
            /// </summary>
            public string MName { get; set; }

            /// <summary>
            /// 會員權限等級
            /// </summary>
            public int MPermissions { get; set; }

            /// <summary>
            /// 會員是否被隱藏
            /// </summary>
            public bool MIsHided { get; set; }

            /// <summary>
            /// 建立時間
            /// </summary>
            public DateTime CreatedTime { get; set; }

            /// <summary>
            /// 連接ID
            /// </summary>
            public string ConnectionId { get; set; }
        
    }
}
