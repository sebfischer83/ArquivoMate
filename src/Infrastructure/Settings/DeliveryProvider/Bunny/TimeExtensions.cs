namespace ArquivoMate.Infrastructure.Settings.DeliveryProvider.Bunny
{
    internal static class TimeExtensions
    {
        internal static string ToUnixTimestamp(this DateTimeOffset time)
            => time.ToUnixTimeSeconds().ToString();
    }
}
