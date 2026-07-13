using System;
using UnityEngine.Bindings;

namespace UnityEngine.Multiplayer.Internal
{
	[Flags]
	[VisibleToOtherModules]
	internal enum MultiplayerRoleFlags
	{
		Client = 1,
		Server = 2,
		ClientAndServer = 3
	}
}
