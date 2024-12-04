namespace ArquivoMate.Shared
{
    public class UserLogoutRequest
    {
        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
    }
}
