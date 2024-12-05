using Microsoft.Extensions.Configuration;

namespace ArquivoMate.Infrastructure.Settings.FileProvider
{
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
                FileProviderType.Bunny => providerSection.Get<BunnyFileProviderSettings>() ?? throw new InvalidOperationException("BunnyFileProviderSettings is not configured."),
                _ => throw new InvalidOperationException($"Unsupported provider type: {type}")
            };
        }
    }
}
