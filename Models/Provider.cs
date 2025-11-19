using System;
using System.Collections.Generic;

namespace APIs.Models;

public partial class Provider
{
    public int ProviderId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Addrss { get; set; }

    public string? Specialty { get; set; }

    //public virtual ICollection<BilledEvent> BilledEvents { get; set; } = new List<BilledEvent>();

    //public virtual ICollection<ClinicService> Services { get; set; } = new List<ClinicService>();
}
