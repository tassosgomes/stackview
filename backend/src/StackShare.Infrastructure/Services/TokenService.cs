using StackShare.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace StackShare.Infrastructure.Services;

public class TokenService : ITokenService
{
    private const int TokenLength = 64; // 64 bytes = 512 bits
    private const int SaltLength = 32; // 32 bytes = 256 bits

    public string GenerateSecureToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var tokenBytes = new byte[TokenLength];
        rng.GetBytes(tokenBytes);
        
        // Use URL-safe base64 encoding
        return Convert.ToBase64String(tokenBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .Replace("=", "");
    }

    public string HashToken(string token)
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[SaltLength];
        rng.GetBytes(salt);

        var tokenBytes = Encoding.UTF8.GetBytes(token);
        using var sha256 = SHA256.Create();
        
        // Combine salt and token
        var combined = new byte[salt.Length + tokenBytes.Length];
        Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
        Buffer.BlockCopy(tokenBytes, 0, combined, salt.Length, tokenBytes.Length);
        
        var hash = sha256.ComputeHash(combined);
        
        // Store salt + hash together
        var result = new byte[salt.Length + hash.Length];
        Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
        Buffer.BlockCopy(hash, 0, result, salt.Length, hash.Length);
        
        return Convert.ToBase64String(result);
    }

    public bool VerifyToken(string token, string hash)
    {
        try
        {
            var hashBytes = Convert.FromBase64String(hash);
            
            if (hashBytes.Length < SaltLength + 32) // 32 is SHA256 hash length
                return false;
                
            var salt = new byte[SaltLength];
            var storedHash = new byte[32];
            
            Buffer.BlockCopy(hashBytes, 0, salt, 0, SaltLength);
            Buffer.BlockCopy(hashBytes, SaltLength, storedHash, 0, 32);
            
            var tokenBytes = Encoding.UTF8.GetBytes(token);
            using var sha256 = SHA256.Create();
            
            var combined = new byte[salt.Length + tokenBytes.Length];
            Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
            Buffer.BlockCopy(tokenBytes, 0, combined, salt.Length, tokenBytes.Length);
            
            var computedHash = sha256.ComputeHash(combined);
            
            return storedHash.SequenceEqual(computedHash);
        }
        catch
        {
            return false;
        }
    }
}