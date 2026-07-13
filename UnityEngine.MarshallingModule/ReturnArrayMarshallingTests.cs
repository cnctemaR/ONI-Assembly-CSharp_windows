using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Modules/Marshalling/ReturnArrayMarshallingTests.h")]
	[ExcludeFromDocs]
	internal static class ReturnArrayMarshallingTests
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public static extern float[] ReturnArrayOfPrimitiveTypeWorks_Float1D();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public static extern float[,] ReturnArrayOfPrimitiveTypeWorks_Float2D();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public static extern float[,,] ReturnArrayOfPrimitiveTypeWorks_Float3D();
	}
}
