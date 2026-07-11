using System;

namespace UnityEngine.Bindings
{
	[VisibleToOtherModules]
	[AttributeUsage(AttributeTargets.Parameter)]
	internal class UnmarshalledAttribute : Attribute, IBindingsAttribute
	{
	}
}
