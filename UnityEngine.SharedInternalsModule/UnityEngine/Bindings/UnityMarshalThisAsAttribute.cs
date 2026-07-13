using System;

namespace UnityEngine.Bindings
{
	[AttributeUsage(AttributeTargets.Method)]
	[VisibleToOtherModules]
	internal class UnityMarshalThisAsAttribute : UnityMarshalAsAttribute
	{
		public UnityMarshalThisAsAttribute(NativeType nativeType)
			: base(nativeType)
		{
		}
	}
}
