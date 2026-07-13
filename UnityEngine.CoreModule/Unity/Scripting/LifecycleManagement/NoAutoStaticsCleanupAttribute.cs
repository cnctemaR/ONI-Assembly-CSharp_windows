using System;
using UnityEngine.Bindings;

namespace Unity.Scripting.LifecycleManagement
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event, AllowMultiple = false)]
	[VisibleToOtherModules]
	internal sealed class NoAutoStaticsCleanupAttribute : Attribute
	{
	}
}
