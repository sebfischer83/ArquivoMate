namespace ArquivoMate.Infrastructure.Services.Files.Bunny
{
    internal static class TimeExtensions
    {
        internal static string ToUnixTimestamp(this DateTimeOffset time)
            => time.ToUnixTimeSeconds().ToString();
    }
}
