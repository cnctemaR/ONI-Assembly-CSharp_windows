using System;

namespace UnityEngine.Bindings
{
	[VisibleToOtherModules]
	[AttributeUsage(AttributeTargets.Class)]
	internal class NativeAsStructAttribute : Attribute, IBindingsAttribute
	{
	}
}
