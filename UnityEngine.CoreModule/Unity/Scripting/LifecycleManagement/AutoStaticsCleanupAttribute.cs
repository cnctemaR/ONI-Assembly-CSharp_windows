using System;
using UnityEngine.Bindings;

namespace Unity.Scripting.LifecycleManagement
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event, AllowMultiple = true)]
	[VisibleToOtherModules]
	internal sealed class AutoStaticsCleanupAttribute : Attribute
	{
		public Type ScopeType { get; set; }

		public ScopeTransitionType TransitionType { get; set; }

		public CleanupStrategy CleanupStrategy { get; set; }
	}
}
