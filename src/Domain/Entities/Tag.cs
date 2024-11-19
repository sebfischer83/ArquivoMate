using ArquivoMate.Domain.Common;

namespace ArquivoMate.Domain.Entities
{
    public class Tag : BaseEntity<Guid>
    {
        public Guid DocumenId { get; set; }

        public required string Name { get; set; }

        public Guid TagId { get; set; }

        public ICollection<Document> Documents { get; } = [];
    }
}
