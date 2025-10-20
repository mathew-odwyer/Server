// <copyright file="SecureTokenHasher.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Infrastructure.Services.Security;

using System.Security.Cryptography;
using System.Text;
using Mantanimus.Core.Application.Services.Security;

internal sealed class SecureTokenHasher : ISecureTokenHasher
{
    public string HashSecureToken(string token)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(token);
        byte[] hashedBytes = SHA256.HashData(bytes);
        string hashedCode = Convert.ToBase64String(hashedBytes);

        return hashedCode;
    }
}
