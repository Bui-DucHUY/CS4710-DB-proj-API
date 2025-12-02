using System;
using System.Collections.Generic;

namespace APIs.Models;

public partial class CptCode
{
    public string Cptcode { get; set; } = null!;

    public string Descript { get; set; } = null!;

    public string? Category { get; set; }

    //public virtual ICollection<ClinicService> ClinicServices { get; set; } = new List<ClinicService>();
}
