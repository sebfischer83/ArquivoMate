using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Domain.Common
{
    
    ////<summary>
    /// Represents a base auditable entity with common audit fields.
    /// </summary>
    /// <typeparam name="T">The type of the entity's identifier.</typeparam>
    public class BaseAuditableEntity<T> : BaseEntity<T>
    {
        public Guid Owner { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        public DateTimeOffset Created { get; set; }

        /// <summary>
        /// Gets or sets the user who created the entity.
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was last modified.
        /// </summary>
        public DateTimeOffset LastModified { get; set; }

        /// <summary>
        /// Gets or sets the user who last modified the entity.
        /// </summary>
        public string? LastModifiedBy { get; set; }

    }

}
