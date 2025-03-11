using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Tcomment
{
    public int ComId { get; set; }

    public int Mid { get; set; }

    public string Mname { get; set; } = null!;

    public int Rating { get; set; }

    public int Oid { get; set; }

    public int Pid { get; set; }

    public string Psize { get; set; } = null!;

    public string Pcolor { get; set; } = null!;

    public string Comment { get; set; } = null!;

    public DateTime ComCreateDate { get; set; }

    public string? ComImage1 { get; set; }

    public string? ComImage2 { get; set; }
}
