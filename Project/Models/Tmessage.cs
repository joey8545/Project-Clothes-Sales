using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Tmessage
{
    public int MessageId { get; set; }

    public int ChatId { get; set; }

    public int MessageSendId { get; set; }

    public string MessageContent { get; set; } = null!;

    public DateTime MessageTime { get; set; }
}
