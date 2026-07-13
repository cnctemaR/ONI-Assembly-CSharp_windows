using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	[ExcludeFromDocs]
	internal class ManagedObjectTests
	{
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern MyManagedObject ParameterManagedObject(MyManagedObject param);

		[NativeThrows]
		public static StructManagedObject ParameterStructManagedObject(StructManagedObject param)
		{
			StructManagedObject structManagedObject;
			ManagedObjectTests.ParameterStructManagedObject_Injected(ref param, out structManagedObject);
			return structManagedObject;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern MyManagedObject[] ReturnNullManagedObjectArray();

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern MyManagedObject[] ParameterManagedObjectVector(MyManagedObject[] param);

		[NativeThrows]
		public static StructManagedObjectVector ParameterStructManagedObjectVector(StructManagedObjectVector param)
		{
			StructManagedObjectVector structManagedObjectVector;
			ManagedObjectTests.ParameterStructManagedObjectVector_Injected(ref param, out structManagedObjectVector);
			return structManagedObjectVector;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructManagedObject_Injected([In] ref StructManagedObject param, out StructManagedObject ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructManagedObjectVector_Injected([In] ref StructManagedObjectVector param, out StructManagedObjectVector ret);
	}
}
