using System;
using System.Security.Cryptography;
using System.Text;
using Winterhaven.API.Core.Application.Services.Security;

namespace Winterhaven.API.Infrastructure.Services.Security;

internal sealed class SecureTokenHasher : ISecureTokenHasher
{
    public string HashSecureToken(string token)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(token);
        byte[] hashedBytes = SHA256.HashData(bytes);

        return Convert.ToBase64String(hashedBytes);
    }
}