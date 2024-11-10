using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Infrastructure.Services.Files
{
    public enum FileProviderType
    {
        Local,
        S3,
        NFS,
        AzureBlob
    }

    public class FileProviderSettings
    {
        public FileProviderType Type { get; set; }
    }

    public class LocalFileProviderSettings : FileProviderSettings
    {
        public string Path { get; set; }
        public string RequestPath { get; set; }
    }

    public class NfsFileProviderSettings : FileProviderSettings
    {
        public string Path { get; set; }
    }

    public class AzureBlobFileProviderSettings : FileProviderSettings
    {
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
    }

    public class S3FileProviderSettings : FileProviderSettings
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string BucketName { get; set; }
    }

    public class FileProviderSettingsFactory
    {
        private readonly IConfiguration _configuration;

        public FileProviderSettingsFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public FileProviderSettings GetFileProviderSettings()
        {
            var providerSection = _configuration.GetSection("FileProvider");
            var type = providerSection.GetValue<FileProviderType?>("Type");

            if (type == null)
            {
                throw new InvalidOperationException("FileProvider type is not configured.");
            }

            return type switch
            {
                FileProviderType.Local => providerSection.Get<LocalFileProviderSettings>() ?? throw new InvalidOperationException("LocalFileProviderSettings is not configured."),
                FileProviderType.AzureBlob => providerSection.Get<AzureBlobFileProviderSettings>() ?? throw new InvalidOperationException("AzureBlobFileProviderSettings is not configured."),
                FileProviderType.S3 => providerSection.Get<S3FileProviderSettings>() ?? throw new InvalidOperationException("S3FileProviderSettings is not configured."),
                FileProviderType.NFS => providerSection.Get<NfsFileProviderSettings>() ?? throw new InvalidOperationException("NfsFileProviderSettings is not configured."),
                _ => throw new InvalidOperationException($"Unsupported provider type: {type}")
            };
        }
    }
}
