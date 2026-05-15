// This file is used by Code Analysis to maintain SuppressMessage attributes that are applied to
// this project. Project-level suppressions either have no target or are given a specific target and
// scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Controllers must be public for MVC discovery", Scope = "namespaceanddescendants", Target = "~N:Winterhaven.Gateway.Presentation.Controllers")]
[assembly: SuppressMessage("Maintainability", "CA1812:Avoid uninstantiated internal classes", Justification = "Internal classes are fine in this namespace", Scope = "namespaceanddescendants", Target = "~N:Winterhaven.Gateway")]