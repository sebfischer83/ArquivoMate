namespace ArquivoMate.Infrastructure.Settings.FileProvider
{
    public class S3FileProviderSettings : FileProviderSettings
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string BucketName { get; set; }
    }
}
