// <copyright file="ClientTokenOptions.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Options.Security;

public sealed class ClientTokenOptions
{
    public int ClientTokenExpirySeconds { get; init; }
}
