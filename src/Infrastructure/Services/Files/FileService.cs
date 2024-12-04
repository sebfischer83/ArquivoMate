using ArquivoMate.Application.Interfaces;
using Blobject.Core;
using Blobject.Disk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Infrastructure.Services.Files
{
    public class FileService : IFileService
    {
        private readonly FileProviderSettingsFactory settingsFactory;

        private readonly BlobClientBase blobClient;

        public FileService(FileProviderSettingsFactory settingsFactory)
        {
            this.settingsFactory = settingsFactory;

            var settings = settingsFactory.GetFileProviderSettings();
            switch (settings.Type)
            {
                case FileProviderType.Local:
                    var localSettings = settings as LocalFileProviderSettings;
                    DiskSettings diskSettings = new(localSettings.Path);
                    blobClient = new DiskBlobClient(diskSettings);
                    break;
                case FileProviderType.S3:
                    var s3Settings = settings as S3FileProviderSettings;
                    break;
                case FileProviderType.NFS:
                    var nfsSettings = settings as NfsFileProviderSettings;
                    break;
                case FileProviderType.AzureBlob:
                    var azureBlobSettings = settings as AzureBlobFileProviderSettings;
                    break;
                default:
                    throw new InvalidOperationException("Invalid file provider type");
            }
        }

        public async Task WriteAsync(string path, string contentType, byte[] content)
        {
            await blobClient.WriteAsync(path, contentType, content);
        }
    }
}
