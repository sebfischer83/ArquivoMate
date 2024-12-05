namespace ArquivoMate.Infrastructure.Services.Document
{
    public record OcrPdfResponse
    {
        public string Message { get; init; } = null!;

        public bool Success { get; init; }

        public byte[]? GeneratedPdf { get; init; }

        public string Content { get; init; }
    }
}
