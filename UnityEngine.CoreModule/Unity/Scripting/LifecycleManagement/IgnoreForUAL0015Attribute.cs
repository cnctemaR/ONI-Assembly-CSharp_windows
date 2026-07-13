using System;
using UnityEngine.Bindings;

namespace Unity.Scripting.LifecycleManagement
{
	[VisibleToOtherModules]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event, AllowMultiple = true)]
	internal sealed class IgnoreForUAL0015Attribute : Attribute
	{
		public string Reason { get; }

		public IgnoreForUAL0015Attribute(string reason)
		{
			this.Reason = reason;
		}
	}
}
