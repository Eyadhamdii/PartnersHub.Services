using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.ConfigurationHub.Domain.Common {
    public abstract class AuditableEntity : Entity {
        public AuditableEntity() {
            CreatedAt = DateTime.Now;
        }
        private DateTime? createdAt;
        private DateTime? updatedAt;
        private DateTime? deletedAt;

        public DateTime? CreatedAt {
            get { return createdAt; }
            set { createdAt = value?.ToUniversalTime(); }
        }

        public DateTime? UpdatedAt {
            get { return updatedAt; }
            set { updatedAt = value?.ToUniversalTime(); }
        }

        public DateTime? DeletedAt {
            get { return deletedAt; }
            set { deletedAt = value?.ToUniversalTime(); }
        }

        public bool IsDeleted { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
    }
}