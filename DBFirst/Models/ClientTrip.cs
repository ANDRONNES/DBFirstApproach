using System;
using System.Collections.Generic;

namespace DBFirst.Models;

public partial class ClientTrip
{
    public int Idclient { get; set; }

    public int Idtrip { get; set; }

    public DateTime Registeredat { get; set; }

    public DateTime? Paymentdate { get; set; }

    public virtual Client IdclientNavigation { get; set; } = null!;

    public virtual Trip IdtripNavigation { get; set; } = null!;
}
