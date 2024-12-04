namespace ArquivoMate.Shared
{
    public class UserRefreshTokenRequest
    {
        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
    }
}
