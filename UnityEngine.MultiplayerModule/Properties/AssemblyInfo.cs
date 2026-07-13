using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;
using UnityEngine;

[assembly: AssemblyVersion("0.0.0.0")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.PlayMode.Editor")]
[assembly: InternalsVisibleTo("Unity.DedicatedServer.Tests.Editor")]
[assembly: InternalsVisibleTo("Unity.DedicatedServer.MultiplayerRoles.Editor")]
[assembly: InternalsVisibleTo("Unity.DedicatedServer.MultiplayerRoles")]
[assembly: InternalsVisibleTo("UnityEditor.MultiplayerModule")]
[assembly: InternalsVisibleTo("UnityEditor.CoreModule")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Workflows.IntegrationTests.PlayMode.Editor")]
[assembly: InternalsVisibleTo("Unity.Modules.Multiplayer.PlayMode.Tests.Editor")]
[assembly: InternalsVisibleTo("Unity.Modules.Multiplayer.MultiplayerRoles.Tests.Performance")]
[assembly: InternalsVisibleTo("Unity.Modules.Multiplayer.MultiplayerRoles.Tests.Editor")]
[assembly: UnityEngineModuleAssembly]
[assembly: InternalsVisibleTo("UnityEngine")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Playmode.Workflow.Runtime.Tests")]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
