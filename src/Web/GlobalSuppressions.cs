// <copyright file="GlobalSuppressions.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates", Justification = "KISS")]
[assembly: SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "See here: https://www.youtube.com/watch?v=FExr6WMn6T8")]
[assembly: SuppressMessage("Performance", "CA1862", Justification = "Visual Studio and CodeMaid really don't like this.")]
