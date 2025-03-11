using System.ComponentModel;

namespace Project.Models
{
    public class CChatWrap
    {
        private Tchat _chat;

        public Tchat chat
        {
            get { return _chat; }
            set { _chat = value; }
        }

        public CChatWrap()
        {
            _chat = new Tchat();
        }

        [DisplayName("聊天室編號")]
        public int Chatid
        {
            get { return _chat.ChatId; }
            set { _chat.ChatId = value; }
        }

        [DisplayName("連線編號")]
        public string ChatConnectId
        {
            get { return _chat.ChatConnectId; }
            set { _chat.ChatConnectId = value; }
        }

        [DisplayName("會員編號")]
        public int Mid
        {
            get { return _chat.Mid; }
            set { _chat.Mid = value; }
        }

        [DisplayName("聊天室創立時間")]
        public DateTime ChatCreateTime
        {
            get { return _chat.ChatCreateTime; }
            set { _chat.ChatCreateTime = value; }
        }
       
    }
}
