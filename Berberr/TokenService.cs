using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class TokenService
{
    private readonly string _secretKey;

    public TokenService(string secretKey)
    {
        // Gelen secretKey değerini _secretKey değişkenine ata
        _secretKey = secretKey;
    }

    // Token oluşturma metodu
    public string GenerateJwtToken(string issuer, string audience, int expireMinutes, string userId, string userRole)
    {
        // Anahtar boyutunu kontrol et
        if (string.IsNullOrEmpty(_secretKey) || _secretKey.Length < 16)
        {
            throw new ArgumentOutOfRangeException(nameof(_secretKey), "The encryption algorithm 'HS256' requires a key size of at least '128' bits.");
        }

        // Güvenlik anahtarı oluştur
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

        // Kimlik doğrulama bilgilerini oluştur
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Kullanıcı kimlik bilgilerini içeren JWT claim'leri oluştur
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim("UserRole", userRole), // Kullanıcı rolü ekle
            new Claim(JwtRegisteredClaimNames.UniqueName, "dvghhfhf"),
            new Claim(JwtRegisteredClaimNames.UniqueName, "dvghhfhf"),
        };

        // JWT token oluştur
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: credentials
        );

        // JWT token'ı string olarak döndür
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
