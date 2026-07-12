using System;

namespace UnityEngine.Bindings
{
	[VisibleToOtherModules]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Delegate, Inherited = false)]
	internal class VisibleToOtherModulesAttribute : Attribute
	{
		public VisibleToOtherModulesAttribute()
		{
		}

		public VisibleToOtherModulesAttribute(params string[] modules)
		{
		}
	}
}
