using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[ExcludeFromDocs]
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	internal class UnityObjectTests
	{
		[NativeThrows]
		public static void ParameterUnityObject(MarshallingTestObject param)
		{
			UnityObjectTests.ParameterUnityObject_Injected(Object.MarshalledUnityObject.Marshal<MarshallingTestObject>(param));
		}

		[NativeThrows]
		public static void ParameterUnityObjectByRef(ref MarshallingTestObject param)
		{
			UnityObjectTests.ParameterUnityObjectByRef_Injected(Object.MarshalledUnityObject.Marshal<MarshallingTestObject>(param));
		}

		[NativeThrows]
		public static void ParameterUnityObjectPPtr(MarshallingTestObject param)
		{
			UnityObjectTests.ParameterUnityObjectPPtr_Injected(Object.MarshalledUnityObject.Marshal<MarshallingTestObject>(param));
		}

		[NativeThrows]
		public static void ParameterStructUnityObject(StructUnityObject param)
		{
			UnityObjectTests.ParameterStructUnityObject_Injected(ref param);
		}

		[NativeThrows]
		public static void ParameterStructUnityObjectPPtr(StructUnityObjectPPtr param)
		{
			UnityObjectTests.ParameterStructUnityObjectPPtr_Injected(ref param);
		}

		[NativeThrows]
		public static void ParameterStructUnityObjectVector(StructUnityObjectVector param)
		{
			UnityObjectTests.ParameterStructUnityObjectVector_Injected(ref param);
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ParameterUnityObjectVector(MarshallingTestObject[] param);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ParameterUnityObjectPPtrVector(MarshallingTestObject[] param);

		public static MarshallingTestObject ReturnUnityObject()
		{
			return Unmarshal.UnmarshalUnityObject<MarshallingTestObject>(UnityObjectTests.ReturnUnityObject_Injected());
		}

		public static MarshallingTestObject ReturnInUnityObject(MarshallingTestObject obj)
		{
			return Unmarshal.UnmarshalUnityObject<MarshallingTestObject>(UnityObjectTests.ReturnInUnityObject_Injected(Object.MarshalledUnityObject.Marshal<MarshallingTestObject>(obj)));
		}

		public static MarshallingTestObject ReturnUnityObjectFakeNull()
		{
			return Unmarshal.UnmarshalUnityObject<MarshallingTestObject>(UnityObjectTests.ReturnUnityObjectFakeNull_Injected());
		}

		public static MarshallingTestObject ReturnUnassignedErrorObject()
		{
			return Unmarshal.UnmarshalUnityObject<MarshallingTestObject>(UnityObjectTests.ReturnUnassignedErrorObject_Injected());
		}

		public static MarshallingTestObject ReturnUnityObjectPPtr()
		{
			return Unmarshal.UnmarshalUnityObject<MarshallingTestObject>(UnityObjectTests.ReturnUnityObjectPPtr_Injected());
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern MarshallingTestObject[] ReturnUnityObjectVector();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern MarshallingTestObject[] ReturnUnityObjectPPtrVector();

		public static StructUnityObject ReturnStructUnityObject()
		{
			StructUnityObject structUnityObject;
			UnityObjectTests.ReturnStructUnityObject_Injected(out structUnityObject);
			return structUnityObject;
		}

		public static StructUnityObjectPPtr ReturnStructUnityObjectPPtr()
		{
			StructUnityObjectPPtr structUnityObjectPPtr;
			UnityObjectTests.ReturnStructUnityObjectPPtr_Injected(out structUnityObjectPPtr);
			return structUnityObjectPPtr;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern StructUnityObject[] ReturnStructUnityObjectVector();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern StructUnityObjectPPtr[] ReturnStructUnityObjectPPtrVector();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern StructUnityObjectVector[] ReturnStructUnityObjectVectorVector();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterUnityObject_Injected(IntPtr param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterUnityObjectByRef_Injected(IntPtr param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterUnityObjectPPtr_Injected(IntPtr param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructUnityObject_Injected([In] ref StructUnityObject param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructUnityObjectPPtr_Injected([In] ref StructUnityObjectPPtr param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructUnityObjectVector_Injected([In] ref StructUnityObjectVector param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr ReturnUnityObject_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr ReturnInUnityObject_Injected(IntPtr obj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr ReturnUnityObjectFakeNull_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr ReturnUnassignedErrorObject_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr ReturnUnityObjectPPtr_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnStructUnityObject_Injected(out StructUnityObject ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnStructUnityObjectPPtr_Injected(out StructUnityObjectPPtr ret);
	}
}
