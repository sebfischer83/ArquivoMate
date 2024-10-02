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
        public required string FileName { get; set; }

        public string FileExtension { get; set; }

        public FileType FileType { get; set; }

        public long FileSize { get; set; }

        public string PreviewImage { get; set; }

        /// <summary>
        /// Always a pdf, other formats gets converted to pdf
        /// </summary>
        public string PreviewFile { get; set; }

        public string Content { get; set; }

        public string Comments { get; set; }

        public DateTime DocumentTime { get; set; }

        public List<Tag> Tags { get; } = [];
        public List<DocumenTag> DocumentTags { get; } = [];
    }

    public class Tag : BaseEntity<Guid>
    {
        public List<Document> Documents { get; } = [];
        public List<DocumenTag> DocumenTags { get; } = [];
    }

    public class DocumenTag
    {
        public Guid DocumenId { get; set; }

        public Guid TagId { get; set; }

        public Document Document { get; set; } = null!;

        public Tag Tag { get; set; } = null!;
    }

    public enum FileType
    {
        PDF,
        Image,
        Office,
        Other
    }
}
