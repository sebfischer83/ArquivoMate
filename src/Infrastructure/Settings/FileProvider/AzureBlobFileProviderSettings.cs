namespace ArquivoMate.Infrastructure.Settings.FileProvider
{
    public class AzureBlobFileProviderSettings : FileProviderSettings
    {
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
    }
}
