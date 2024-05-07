namespace Barber.Models.Settings
{
    public class JwtSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpireMinutes { get; set; }
        public string SecretKey { get; set; } // JWT için gizli anahtar
    }
}
