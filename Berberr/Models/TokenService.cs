using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class TokenService
{
    public string GenerateJwtToken(string secretKey, string issuer, string audience, int expireMinutes, string userId)
    {
        // Anahtar boyutunu kontrol et
        if (string.IsNullOrEmpty(secretKey) || Encoding.UTF8.GetBytes(secretKey).Length < 16)
        {
            throw new ArgumentOutOfRangeException(nameof(secretKey), "The encryption algorithm 'HS256' requires a key size of at least '128' bits.");
        }

        // Güvenlik anahtarı oluştur
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

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
