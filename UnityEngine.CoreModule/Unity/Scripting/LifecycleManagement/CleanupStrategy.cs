using System;
using UnityEngine.Bindings;

namespace Unity.Scripting.LifecycleManagement
{
	[VisibleToOtherModules]
	internal enum CleanupStrategy
	{
		Unset,
		Auto,
		Clear,
		CaptureInitializationExpression,
		ResetToDefaultValue
	}
}
