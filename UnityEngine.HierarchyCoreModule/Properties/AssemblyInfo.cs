using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;
using UnityEngine;

[assembly: AssemblyVersion("0.0.0.0")]
[assembly: InternalsVisibleTo("Unity.Hierarchy.Editor.PerformanceTests")]
[assembly: InternalsVisibleTo("Unity.Hierarchy.Editor.Tests")]
[assembly: InternalsVisibleTo("Unity.Hierarchy.PerformanceTests")]
[assembly: InternalsVisibleTo("Unity.Hierarchy.Tests")]
[assembly: InternalsVisibleTo("UnityEditor.HierarchyModule")]
[assembly: InternalsVisibleTo("UnityEngine.HierarchyModule")]
[assembly: UnityEngineModuleAssembly]
[assembly: InternalsVisibleTo("UnityEngine")]
[assembly: InternalsVisibleTo("Unity.Entities.Editor.Tests")]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
