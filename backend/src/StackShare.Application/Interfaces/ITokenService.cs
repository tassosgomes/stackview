namespace StackShare.Application.Interfaces;

public interface ITokenService
{
    /// <summary>
    /// Generates a secure random token
    /// </summary>
    string GenerateSecureToken();
    
    /// <summary>
    /// Hashes a token with salt for secure storage
    /// </summary>
    string HashToken(string token);
    
    /// <summary>
    /// Verifies a token against its hash
    /// </summary>
    bool VerifyToken(string token, string hash);
}