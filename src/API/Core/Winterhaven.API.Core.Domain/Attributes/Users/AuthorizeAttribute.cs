namespace Winterhaven.API.Core.Domain.Attributes.Users;

using System;

/// <summary>
///   Represents an attribute that can be applied to a request to indicate that authorization is required.
/// </summary>
/// <seealso cref="Attribute"/>
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class AuthorizeAttribute : Attribute
{
}