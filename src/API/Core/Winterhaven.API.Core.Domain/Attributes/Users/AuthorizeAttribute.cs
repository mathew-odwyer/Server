namespace Winterhaven.API.Core.Domain.Attributes.Users;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents an attribute that can be applied to a request to indicate that authorization is required.
/// </summary>
/// <seealso cref="Attribute"/>
[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class AuthorizeAttribute : Attribute
{
}