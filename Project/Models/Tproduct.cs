using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Tproduct
{
    public int Pid { get; set; }

    public string Pname { get; set; } = null!;

    public int Pprice { get; set; }

    public string Ptype { get; set; } = null!;

    public string Pcategory { get; set; } = null!;

    public string Pdescription { get; set; } = null!;

    public int Pinventory { get; set; }

    public DateTime PcreatedDate { get; set; }

    public string Pphoto { get; set; } = null!;

    public bool PisHided { get; set; }
}
