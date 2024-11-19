using ArquivoMate.Domain.Common;

namespace ArquivoMate.Domain.Entities
{
    public class DocumentVersion : BaseAuditableEntity<Guid>
    {
        public Guid DocumentId { get; set; }
        public Document Document { get; set; } = null!;

        public string OriginalFilePath { get; set; } = null!;
    }
}
