using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;

[assembly: AssemblyVersion("0.0.0.0")]
[assembly: InternalsVisibleTo("Unity.Burst.CodeGen")]
[assembly: InternalsVisibleTo("Unity.Burst.Editor")]
[assembly: InternalsVisibleTo("Unity.Burst.Tests.UnitTests")]
[assembly: InternalsVisibleTo("Unity.Burst.Editor.Tests")]
[assembly: InternalsVisibleTo("Unity.Burst.Benchmarks")]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
