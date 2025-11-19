using System;
using System.Collections.Generic;

namespace APIs.Models;

public partial class BilledEvent
{
    public int EventId { get; set; }

    public int ServiceId { get; set; }

    public int ProviderId { get; set; }

    public DateOnly DateOfService { get; set; }

    public decimal BilledAmount { get; set; }

    public virtual Provider Provider { get; set; } = null!;

    public virtual ClinicService Service { get; set; } = null!;
}
