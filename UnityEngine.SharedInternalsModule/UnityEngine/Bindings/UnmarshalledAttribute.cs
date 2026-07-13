using System;

namespace UnityEngine.Bindings
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	[VisibleToOtherModules]
	[Obsolete("This attribute is not supported - consider using blittable types or supported marshaling - or if native code requires a ScriptingObjectPtr use [UnityMarshalAs(NativeType.ScriptingObjectPtr)]", true)]
	internal class UnmarshalledAttribute : Attribute, IBindingsAttribute
	{
	}
}
