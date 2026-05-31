using System;

namespace Winterhaven.Gateway.Presentation.Attributes;

/// <summary>
///   Represents an attribute that can be applied to a JSON-RPC method to indicate that an authenticated user session is required.
/// </summary>
/// <seealso cref="Attribute"/>
[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
internal sealed class JsonRpcAuthorizeAttribute : Attribute;
