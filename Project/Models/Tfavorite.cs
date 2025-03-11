using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Tfavorite
{
    public int Fid { get; set; }

    public int Mid { get; set; }

    public int Pid { get; set; }

    public DateTime FcreateDate { get; set; }

    public string Pname { get; set; } = null!;

    public string Pphoto { get; set; } = null!;

    public int Pprice { get; set; }
}
