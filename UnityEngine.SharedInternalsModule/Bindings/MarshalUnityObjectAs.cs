using System;

namespace UnityEngine.Bindings
{
	[AttributeUsage(AttributeTargets.Class)]
	[VisibleToOtherModules]
	internal class MarshalUnityObjectAs : Attribute, IBindingsAttribute
	{
		public MarshalUnityObjectAs(Type marshalAsType)
		{
			this.MarshalAsType = marshalAsType;
		}

		public Type MarshalAsType { get; set; }
	}
}
