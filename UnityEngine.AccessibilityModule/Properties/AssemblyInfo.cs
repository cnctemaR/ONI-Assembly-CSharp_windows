using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;
using UnityEngine;

[assembly: AssemblyVersion("0.0.0.0")]
[assembly: InternalsVisibleTo("Acessibility.VisionUtility.Tests")]
[assembly: InternalsVisibleTo("UnityEditor.CoreModule")]
[assembly: InternalsVisibleTo("UnityEditor.AccessibilityModule")]
[assembly: InternalsVisibleTo("Unity.Modules.Accessibility.Tests.Playmode")]
[assembly: InternalsVisibleTo("Unity.Modules.Accessibility.Tests.Editor")]
[assembly: UnityEngineModuleAssembly]
[assembly: InternalsVisibleTo("UnityEngine")]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
