using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Tchat
{
    public int ChatId { get; set; }

    public string ChatConnectId { get; set; } = null!;

    public int Mid { get; set; }

    public DateTime ChatCreateTime { get; set; }
}
