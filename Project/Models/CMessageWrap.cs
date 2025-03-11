using System.ComponentModel;

namespace Project.Models
{
    public class CMessageWrap
    {
        private Tmessage _message;

        public Tmessage chat
        {
            get { return _message; }
            set { _message = value; }
        }

        public CMessageWrap()
        {
            _message = new Tmessage();
        }

        [DisplayName("訊息編號")]
        public int MessageId
        {
            get { return _message.MessageId; }
            set { _message.MessageId = value; }
        }

        [DisplayName("聊天室編號")]
        public int ChatId
        {
            get { return _message.ChatId; }
            set { _message.ChatId = value; }
        }

        [DisplayName("傳送者ID")]
        public int MessageSendId
        {
            get { return _message.MessageSendId; }
            set { _message.MessageSendId = value; }
        }

        [DisplayName("訊息內容")]
        public string MessageContent
        {
            get { return _message.MessageContent; }
            set { _message.MessageContent = value; }
        }

        [DisplayName("訊息創立時間")]
        public DateTime MessageTime
        {
            get { return _message.MessageTime; }
            set { _message.MessageTime = value; }
        }
    }
}
