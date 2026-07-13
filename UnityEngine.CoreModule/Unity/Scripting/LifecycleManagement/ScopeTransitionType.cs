using System;
using UnityEngine.Bindings;

namespace Unity.Scripting.LifecycleManagement
{
	[VisibleToOtherModules]
	internal enum ScopeTransitionType
	{
		Unset,
		Entering,
		Exiting,
		Both
	}
}
