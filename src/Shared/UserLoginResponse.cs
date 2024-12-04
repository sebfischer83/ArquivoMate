namespace ArquivoMate.Shared
{
    public class UserLoginResponse
    {
        public string AccessToken { get; set; } = "";
        public DateTime AccessTokenValid { get; set; } = DateTime.Now;
        public string RefreshToken { get; set; } = "";
        public DateTime RefreshTokenValid { get; set; } = DateTime.Now;
    }
}
