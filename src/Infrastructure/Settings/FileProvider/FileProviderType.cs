using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Infrastructure.Settings.FileProvider
{
    public enum FileProviderType
    {
        Local,
        S3,
        NFS,
        AzureBlob,
        Bunny
    }
}
