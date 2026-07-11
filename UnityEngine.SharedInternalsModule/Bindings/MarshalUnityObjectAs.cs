using System;

namespace UnityEngine.Bindings
{
	[VisibleToOtherModules]
	[AttributeUsage(AttributeTargets.Class)]
	internal class MarshalUnityObjectAs : Attribute, IBindingsAttribute
	{
		public MarshalUnityObjectAs(Type marshalAsType)
		{
			this.MarshalAsType = marshalAsType;
		}

		public Type MarshalAsType { get; set; }
	}
}
