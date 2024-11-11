namespace ArquivoMate.Infrastructure.Services.Document
{
    public record OcrPdfResponse
    {
        public required string Message { get; init; }

        public bool Success { get; init; }

        public required byte[]? GeneratedPdf { get; init; }

        public required string Content { get; init; }
    }
}
