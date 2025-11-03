using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.InnovationHub.Domain.Common
{
    public class AuditableEntity : Entity
    {
        // Audit Properties - Common to all aggregates
        public string CreatedBy { get;  set; }
        public DateTime CreatedAt { get;  set; }
        public string? UpdatedBy { get;  set; }
        public DateTime? UpdatedAt { get;  set; }
    }
}
