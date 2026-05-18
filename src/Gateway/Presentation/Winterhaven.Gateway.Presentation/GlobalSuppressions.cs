// This file is used by Code Analysis to maintain SuppressMessage attributes that are applied to this project. Project-level suppressions either have no target or are given a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Controllers must be public for MVC discovery", Scope = "namespaceanddescendants", Target = "~N:Winterhaven.Gateway.Presentation.Controllers")]
[assembly: SuppressMessage("Maintainbility", "CA1812:Internal classes should be made static", Justification = "Internal classes are registered via DI", Scope = "namespaceanddescendants", Target = "~N:Winterhaven.Gateway.Infrastructure")]
[assembly: SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Targets and Parameters must be public for discovery", Scope = "namespaceanddescendants", Target = "~N:Winterhaven.Gateway.Presentation.Targets")]
[assembly: SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Validators must be public for FluentValidation discovery", Scope = "namespaceanddescendants", Target = "~N:Winterhaven.Gateway.Presentation.Validation")]