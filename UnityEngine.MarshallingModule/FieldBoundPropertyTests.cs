using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeType("Modules/Marshalling/MarshallingTests.h")]
	internal class FieldBoundPropertyTests
	{
		[NativeProperty(TargetType = TargetType.Field)]
		public static extern int StaticProp
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("FieldBoundPropertyTests::GetNativeStaticPropContainer()", StaticAccessorType.Dot)]
		[NativeProperty("foo", false, TargetType.Field)]
		public static extern int StaticAccessorProp
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
