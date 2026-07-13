using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[ExcludeFromDocs]
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	internal class BlittableStructTests
	{
		[NativeThrows]
		public static void ParameterStructInt(StructInt param)
		{
			BlittableStructTests.ParameterStructInt_Injected(ref param);
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ParameterStructIntByRef(ref StructInt param);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ParameterStructIntIn(in StructInt param);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ParameterStructIntOut(out StructInt param);

		public static void ParameterStructInt2(StructInt2 param)
		{
			BlittableStructTests.ParameterStructInt2_Injected(ref param);
		}

		public static StructInt ReturnStructInt()
		{
			StructInt structInt;
			BlittableStructTests.ReturnStructInt_Injected(out structInt);
			return structInt;
		}

		[NativeThrows]
		public static void ParameterNestedBlittableStruct(StructNestedBlittable s)
		{
			BlittableStructTests.ParameterNestedBlittableStruct_Injected(ref s);
		}

		public static StructNestedBlittable ReturnNestedBlittableStruct()
		{
			StructNestedBlittable structNestedBlittable;
			BlittableStructTests.ReturnNestedBlittableStruct_Injected(out structNestedBlittable);
			return structNestedBlittable;
		}

		[NativeThrows]
		public unsafe static void ParameterStructIntVector(StructInt[] param)
		{
			Span<StructInt> span = new Span<StructInt>(param);
			fixed (StructInt* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				BlittableStructTests.ParameterStructIntVector_Injected(ref managedSpanWrapper);
			}
		}

		public static StructInt[] ReturnStructIntVector()
		{
			StructInt[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				BlittableStructTests.ReturnStructIntVector_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				StructInt[] array;
				blittableArrayWrapper.Unmarshal<StructInt>(ref array);
				array2 = array;
			}
			return array2;
		}

		[NativeThrows]
		public unsafe static void ParameterStructNestedBlittableVector(StructNestedBlittable[] param)
		{
			Span<StructNestedBlittable> span = new Span<StructNestedBlittable>(param);
			fixed (StructNestedBlittable* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				BlittableStructTests.ParameterStructNestedBlittableVector_Injected(ref managedSpanWrapper);
			}
		}

		public static StructNestedBlittable[] ReturnStructNestedBlittableVector()
		{
			StructNestedBlittable[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				BlittableStructTests.ReturnStructNestedBlittableVector_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				StructNestedBlittable[] array;
				blittableArrayWrapper.Unmarshal<StructNestedBlittable>(ref array);
				array2 = array;
			}
			return array2;
		}

		[NativeThrows]
		public static void ParameterStructFixedBuffer(StructFixedBuffer param)
		{
			BlittableStructTests.ParameterStructFixedBuffer_Injected(ref param);
		}

		public static StructFixedBuffer ReturnStructFixedBuffer()
		{
			StructFixedBuffer structFixedBuffer;
			BlittableStructTests.ReturnStructFixedBuffer_Injected(out structFixedBuffer);
			return structFixedBuffer;
		}

		public static StructInt structIntProperty
		{
			get
			{
				StructInt structInt;
				BlittableStructTests.get_structIntProperty_Injected(out structInt);
				return structInt;
			}
			set
			{
				BlittableStructTests.set_structIntProperty_Injected(ref value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructInt_Injected([In] ref StructInt param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructInt2_Injected([In] ref StructInt2 param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnStructInt_Injected(out StructInt ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterNestedBlittableStruct_Injected([In] ref StructNestedBlittable s);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnNestedBlittableStruct_Injected(out StructNestedBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructIntVector_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnStructIntVector_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructNestedBlittableVector_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnStructNestedBlittableVector_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructFixedBuffer_Injected([In] ref StructFixedBuffer param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnStructFixedBuffer_Injected(out StructFixedBuffer ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_structIntProperty_Injected(out StructInt ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_structIntProperty_Injected([In] ref StructInt value);
	}
}
