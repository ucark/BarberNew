using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class TokenService
{
    private readonly string _secretKey;

    public TokenService(string secretKey)
    {
        _secretKey = secretKey;
    }

    public string GenerateJwtToken(string issuer, string audience, int expireMinutes, string userId)
    {
        // Anahtar boyutunu kontrol et
        if (string.IsNullOrEmpty(_secretKey) || Encoding.UTF8.GetBytes(_secretKey).Length < 16)
        {
            throw new ArgumentOutOfRangeException(nameof(_secretKey), "The encryption algorithm 'HS256' requires a key size of at least '128' bits.");
        }

        // Güvenlik anahtarı oluştur
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

        // Kimlik doğrulama bilgilerini oluştur
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // JWT token oluştur
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: new[] { new Claim(ClaimTypes.NameIdentifier, userId) },
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: credentials
        );

        // JWT token'ı string olarak döndür
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
