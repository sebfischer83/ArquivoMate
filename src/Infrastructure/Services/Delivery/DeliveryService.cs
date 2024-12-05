using ArquivoMate.Application.Interfaces;
using ArquivoMate.Infrastructure.Services.Files;
using ArquivoMate.Infrastructure.Settings.DeliveryProvider;
using ArquivoMate.Infrastructure.Settings.DeliveryProvider.Bunny;
using ArquivoMate.Infrastructure.Settings.FileProvider;
using Blobject.Disk;
using BunnyCDN.Net.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Infrastructure.Services.Delivery
{
    internal class DeliveryService : IDeliveryService
    {
        private DeliveryProviderSettingsFactory settingsFactory;
        private ILogger<FileService> logger;
        private BunnyTokenDeliveryProviderSettings? bunnyTokenSettings;
        private DeliveryProviderType providerType;

        public DeliveryService(DeliveryProviderSettingsFactory settingsFactory, ILogger<FileService> logger)
        {
            this.settingsFactory = settingsFactory ?? throw new ArgumentNullException(nameof(settingsFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var settings = settingsFactory.GetFileProviderSettings() ?? throw new InvalidOperationException("Delivery provider settings cannot be null");
            providerType = settings.Type;
            switch (settings.Type)
            {
                case DeliveryProviderType.Local:
                    // Implement logic for Local
                    break;
                case DeliveryProviderType.BunnySimple:
                    // Implement logic for BunnySimple
                    break;
                case DeliveryProviderType.BunnyToken:
                    bunnyTokenSettings = settings as BunnyTokenDeliveryProviderSettings ?? throw new InvalidOperationException("Invalid settings type for BunnyToken delivery provider");
                    break;
                case DeliveryProviderType.BunnyS3:
                    // Implement logic for BunnyS3
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task<string> GetUrlAsync(string path)
        {
            if (providerType == DeliveryProviderType.BunnyToken && bunnyTokenSettings != null)
            {
                return await Task.FromResult(TokenSigner.SignUrl(bunnyTokenSettings.TokenSecurityKey, path, DateTimeOffset.Now.AddHours(24)));
            }

            throw new NotImplementedException();
        }
    }
}
