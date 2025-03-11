using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Tadvice
{
    public int Id { get; set; }

    public int Mid { get; set; }

    public int Oid { get; set; }

    public string Question { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime DateTime { get; set; }

    public bool IsReply { get; set; }
}
