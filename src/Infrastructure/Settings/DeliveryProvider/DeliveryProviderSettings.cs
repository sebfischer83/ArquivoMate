using ArquivoMate.Infrastructure.Settings.FileProvider;
using Microsoft.Extensions.Configuration;

namespace ArquivoMate.Infrastructure.Settings.DeliveryProvider
{
    public class DeliveryProviderSettings
    {
        public DeliveryProviderType Type { get; set; }
    }

    public class BunnyTokenDeliveryProviderSettings : DeliveryProviderSettings
    {
        public required string TokenSecurityKey { get; set; }

        public required string CdnHostName { get; set; }
    }

    public class DeliveryProviderSettingsFactory
    {
        private readonly IConfiguration _configuration;

        public DeliveryProviderSettingsFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DeliveryProviderSettings GetFileProviderSettings()
        {
            var providerSection = _configuration.GetSection("DeliveryProvider");
            var type = providerSection.GetValue<DeliveryProviderType?>("Type");

            if (type == null)
            {
                throw new InvalidOperationException("FileProvider type is not configured.");
            }

            return type switch
            {
                DeliveryProviderType.Local => throw new NotImplementedException(),
                DeliveryProviderType.BunnySimple => throw new NotImplementedException(),
                DeliveryProviderType.BunnyToken => providerSection.Get<BunnyTokenDeliveryProviderSettings>() ??
                    throw new InvalidOperationException("BunnyTokenDeliveryProviderSettings is not configured."),
                DeliveryProviderType.BunnyS3 => throw new NotImplementedException(),
                _ => throw new InvalidOperationException($"Unsupported provider type: {type}")
            };
        }
    }
}
