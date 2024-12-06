using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Application.Interfaces
{
    public interface IFileService
    {
        bool NeedPrefix { get; }
        string GetPrefix();
        Task WriteAsync(string path, string contentType, byte[] content);
    }
}
