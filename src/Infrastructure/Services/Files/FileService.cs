﻿using ArquivoMate.Application.Interfaces;
using ArquivoMate.Infrastructure.Settings.FileProvider;
using Blobject.Core;
using Blobject.Disk;
using BunnyCDN.Net.Storage;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<FileService> logger;
        private readonly FileProviderSettings settings;
        private readonly BlobClientBase? blobClient;
        private readonly BunnyCDNStorage? bunnyCDNStorage;

        private readonly InternalFileProviderType fileProviderType;

        public FileService(FileProviderSettingsFactory settingsFactory, ILogger<FileService> logger)
        {
            this.settingsFactory = settingsFactory ?? throw new ArgumentNullException(nameof(settingsFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.settings = settingsFactory.GetFileProviderSettings() ?? throw new InvalidOperationException("File provider settings cannot be null");
            switch (settings.Type)
            {
                case FileProviderType.Local:
                    var localSettings = settings as LocalFileProviderSettings ?? throw new InvalidOperationException("Invalid settings type for Local file provider");
                    DiskSettings diskSettings = new(localSettings.Path);
                    blobClient = new DiskBlobClient(diskSettings);
                    fileProviderType = InternalFileProviderType.Blobject;
                    break;
                case FileProviderType.S3:
                    var s3Settings = settings as S3FileProviderSettings ?? throw new InvalidOperationException("Invalid settings type for S3 file provider");
                    fileProviderType = InternalFileProviderType.Blobject;
                    break;
                case FileProviderType.NFS:
                    var nfsSettings = settings as NfsFileProviderSettings ?? throw new InvalidOperationException("Invalid settings type for NFS file provider");
                    fileProviderType = InternalFileProviderType.Blobject;
                    break;
                case FileProviderType.AzureBlob:
                    var azureBlobSettings = settings as AzureBlobFileProviderSettings ?? throw new InvalidOperationException("Invalid settings type for AzureBlob file provider");
                    fileProviderType = InternalFileProviderType.Blobject;
                    break;
                case FileProviderType.Bunny:
                    var bunnySettings = settings as BunnyFileProviderSettings ?? throw new InvalidOperationException("Invalid settings type for Bunny file provider");
                    fileProviderType = InternalFileProviderType.Bunny;
                    bunnyCDNStorage = new BunnyCDNStorage(bunnySettings.StorageZoneName, bunnySettings.AccessKey, "de");
                    break;
                default:
                    throw new InvalidOperationException("Invalid file provider type");
            }

            if (fileProviderType == InternalFileProviderType.Blobject && blobClient != null)
            {
                blobClient.Logger = (s) => { logger.LogInformation(s); };
            }
        }

        public bool NeedPrefix => fileProviderType == InternalFileProviderType.Bunny ? true : false;

        public string GetPrefix()
        {
            if (fileProviderType == InternalFileProviderType.Bunny)
            {
                return (settings as BunnyFileProviderSettings)!.StorageZoneName;
            }
            return string.Empty;
        }

        public async Task WriteAsync(string path, string contentType, byte[] content)
        {
            try
            {
                if (fileProviderType == InternalFileProviderType.Bunny)
                {
                    await using var ms = new MemoryStream(content);
                    await bunnyCDNStorage!.UploadAsync(ms, path);
                }
                else if (fileProviderType == InternalFileProviderType.Blobject)
                {
                    await blobClient!.WriteAsync(path, contentType, content);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error writing file");
                throw;
            }
           
        }
    }

    public enum InternalFileProviderType
    {
        Blobject,
        Bunny
    }
}
