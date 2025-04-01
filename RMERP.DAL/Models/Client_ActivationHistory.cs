using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class Client_ActivationHistory
{
    public int CAH_Id { get; set; }

    public int? CLI_Id { get; set; }

    public DateTime CAH_ActiveOn { get; set; }

    public DateTime? CAH_InactiveOn { get; set; }

    public virtual Client CLI { get; set; }
}
