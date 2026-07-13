using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	[ExcludeFromDocs]
	internal class BoolStructTests
	{
		[NativeThrows]
		public static void ParameterStructWith8ByteAndBoolFields(StructWith8ByteAndBoolFields param)
		{
			BoolStructTests.ParameterStructWith8ByteAndBoolFields_Injected(ref param);
		}

		[NativeThrows]
		public unsafe static void ParameterStructWith8ByteAndBoolFieldsArray(StructWith8ByteAndBoolFields[] param)
		{
			Span<StructWith8ByteAndBoolFields> span = new Span<StructWith8ByteAndBoolFields>(param);
			fixed (StructWith8ByteAndBoolFields* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				BoolStructTests.ParameterStructWith8ByteAndBoolFieldsArray_Injected(ref managedSpanWrapper);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructWith8ByteAndBoolFields_Injected([In] ref StructWith8ByteAndBoolFields param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructWith8ByteAndBoolFieldsArray_Injected(ref ManagedSpanWrapper param);
	}
}
