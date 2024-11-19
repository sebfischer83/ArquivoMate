using ArquivoMate.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Domain.Entities
{
    public class Document : BaseAuditableEntity<Guid>
    {
        public required string FilePath { get; set; }

        public string OriginalFileName { get; set; } = null!;

        public string OriginalFilePath { get; set; } = null!;

        public string FileExtension { get; set; } = null!;

        public long FileSize { get; set; }

        public string ThumbnailImage { get; set; } = null!;

        public string FullImage { get; set; } = null!;

        /// <summary>
        /// Always a pdf, other formats gets converted to pdf
        /// </summary>
        public string PreviewFile { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string? GeneratedContent { get; set; }

        public string? Comment { get; set; }

        public DateTime DocumentDate { get; set; }

        public ICollection<Tag> Tags { get; } = [];

        public ICollection<DocumentVersion> Versions { get; set; } = new List<DocumentVersion>();
    }

}