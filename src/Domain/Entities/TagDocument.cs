using ArquivoMate.Domain.Entities;

public class TagDocument
{
    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = null!;

    public Guid DocumentId { get; set; }
    public Document Document { get; set; } = null!;
}
