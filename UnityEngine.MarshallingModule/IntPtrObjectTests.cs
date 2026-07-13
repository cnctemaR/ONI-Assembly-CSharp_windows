using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	[ExcludeFromDocs]
	internal class IntPtrObjectTests
	{
		[NativeThrows]
		public static void ParameterIntPtrObject(MyIntPtrObject param)
		{
			IntPtrObjectTests.ParameterIntPtrObject_Injected((param == null) ? ((IntPtr)0) : MyIntPtrObject.BindingsMarshaller.ConvertToNative(param));
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ParameterIntPtrObjectVector(MyIntPtrObject[] param);

		[NativeThrows]
		public static void ParameterStructIntPtrObject(StructIntPtrObject param)
		{
			IntPtrObjectTests.ParameterStructIntPtrObject_Injected(ref param);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern MyIntPtrObject[] ReturnIntPtrObjectVector();

		[NativeThrows]
		public static void ParameterStructIntPtrObjectVector(StructIntPtrObjectVector param)
		{
			IntPtrObjectTests.ParameterStructIntPtrObjectVector_Injected(ref param);
		}

		public static MyIntPtrObject ReturnIntPtrObject(int value)
		{
			IntPtr intPtr = IntPtrObjectTests.ReturnIntPtrObject_Injected(value);
			return (intPtr == 0) ? null : MyIntPtrObject.BindingsMarshaller.ConvertToManaged(intPtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterIntPtrObject_Injected(IntPtr param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructIntPtrObject_Injected([In] ref StructIntPtrObject param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructIntPtrObjectVector_Injected([In] ref StructIntPtrObjectVector param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr ReturnIntPtrObject_Injected(int value);
	}
}
