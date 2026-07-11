using System;

namespace UnityEngine.Bindings
{
	[VisibleToOtherModules]
	[AttributeUsage(AttributeTargets.Parameter)]
	internal class NotNullAttribute : Attribute, IBindingsAttribute
	{
	}
}
