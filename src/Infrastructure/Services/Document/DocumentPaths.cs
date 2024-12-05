namespace ArquivoMate.Infrastructure.Services.Document
{
    public partial class DocumentProcessor
    {
        private record DocumentPaths(string OriginalPath, string GeneratedPdf, string ThumbnailPath, string ImagePath);
    }
}
