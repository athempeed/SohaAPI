using System;

namespace WebApplication10.Model
{
    public class QBToken
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiresIn { get; set; }
        public DateTime AccessTokenExpiresIn { get; set; }
    }
}
