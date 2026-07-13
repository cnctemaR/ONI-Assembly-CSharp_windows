using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	[ExcludeFromDocs]
	internal struct StructWithExternTests
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetTimesTwo();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetTimesThree();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int ParameterWritable([Writable] Object unityObject);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ParameterInt(int param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int ReturnInt();

		public int a;
	}
}
