using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class ChatMessage
{
    public int ChatId { get; set; }

    public int MemberId { get; set; }

    public int ReceivedMemberId { get; set; }

    public string Message { get; set; } = null!;

    public DateTime Time { get; set; }

    public string Status { get; set; } = null!;
}
