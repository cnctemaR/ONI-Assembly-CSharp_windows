using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;
using UnityEngine;

[assembly: AssemblyVersion("0.0.0.0")]
[assembly: InternalsVisibleTo("UnityEngine")]
[assembly: UnityEngineModuleAssembly]
[assembly: InternalsVisibleTo("Unity.Video.Tests")]
[assembly: InternalsVisibleTo("Unity.Audio.DSPGraph.Tests")]
[assembly: InternalsVisibleTo("Unity.Video.EditModeTests")]
[assembly: InternalsVisibleTo("UnityEditor.VideoModule")]
[assembly: InternalsVisibleTo("UnityEditor.MediaModule")]
[assembly: InternalsVisibleTo("Assembly-CSharp-testable")]
[assembly: InternalsVisibleTo("VideoTesting")]
[assembly: InternalsVisibleTo("Unity.Audio.DSPGraph")]
[assembly: InternalsVisibleTo("Unity.Video.PlayModeTests")]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
