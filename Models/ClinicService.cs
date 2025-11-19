using System;
using System.Collections.Generic;

namespace APIs.Models;

public partial class ClinicService
{
    public int ServiceId { get; set; }

    public string ServiceName { get; set; } = null!;

    public decimal Fee { get; set; }

    public string Cptcode { get; set; } = null!;

    public virtual ICollection<BilledEvent> BilledEvents { get; set; } = new List<BilledEvent>();

    public virtual CptCode CptcodeNavigation { get; set; } = null!;

    public virtual ICollection<Provider> Providers { get; set; } = new List<Provider>();
}
