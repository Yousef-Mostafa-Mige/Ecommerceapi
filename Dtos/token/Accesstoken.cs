namespace Ecommerceapi.Dtos.token
{
    public class Accesstoken
    {
        public string? refreshToken { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}