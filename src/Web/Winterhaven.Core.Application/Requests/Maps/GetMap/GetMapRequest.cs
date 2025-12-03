// <copyright file="GetMapRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.Requests.Maps.GetMap;

using MediatR;
using Winterhaven.Core.Application.Attributes;

[Authorize]
public sealed record GetMapRequest(string Name)
    : IRequest<GetMapResponse>;
