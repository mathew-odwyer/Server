// <copyright file="AuthorizeAttribute.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.Attributes;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents an attribute that can be applied to a request to indicate that authorization is required.
/// </summary>
/// <seealso cref="Attribute" />
[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class AuthorizeAttribute : Attribute
{
}
