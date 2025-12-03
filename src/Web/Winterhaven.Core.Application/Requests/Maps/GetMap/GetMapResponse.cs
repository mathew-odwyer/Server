// <copyright file="GetMapResponse.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.Requests.Maps.GetMap;

public sealed class GetMapResponse
{
    public GetMapResponse(byte[] data)
    {
        this.Data = data ?? throw new ArgumentNullException(nameof(data));
    }

    public byte[] Data { get; }
}
