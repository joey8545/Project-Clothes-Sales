namespace Project.ViewModel
{
    public class ChatRoomViewModel
    {
        /// <summary>
        /// 聊天室ID
        /// </summary>
        public int ChatId { get; set; }

        /// <summary>
        /// 聊天室連接ID
        /// </summary>
        public string ChatConnectId { get; set; }

        /// <summary>
        /// 會員ID
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
