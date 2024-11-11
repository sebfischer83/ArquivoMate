using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;

namespace ArquivoMate.Infrastructure.Services.Document
{
    public class OcrPdfRequest
    {
        [NotNull]
        public IFormFile File { get; set; } = null!;

        [NotNull]
        public string[] Languages { get; set; } = ["deu", "eng"];
    }
}
