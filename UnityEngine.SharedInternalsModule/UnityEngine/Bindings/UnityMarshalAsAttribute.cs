using System;

namespace UnityEngine.Bindings
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = false)]
	[VisibleToOtherModules]
	internal class UnityMarshalAsAttribute : Attribute, IBindingsAttribute
	{
		public NativeType NativeType { get; }

		public Type CustomMarshaller { get; set; }

		public UnityMarshalAsAttribute(NativeType nativeType)
		{
			this.NativeType = nativeType;
		}
	}
}
