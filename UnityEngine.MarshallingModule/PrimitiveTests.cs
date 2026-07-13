using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("MarshallingScriptingClasses.h")]
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	[ExcludeFromDocs]
	internal class PrimitiveTests
	{
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ParameterBool(bool param1, bool param2, int param3);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ParameterInt(int param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ParameterOutInt(out int param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ParameterRefInt(ref int param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int ReturnInt();

		[NativeThrows]
		public unsafe static void ParameterIntVector(int[] param)
		{
			Span<int> span = new Span<int>(param);
			fixed (int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				PrimitiveTests.ParameterIntVector_Injected(ref managedSpanWrapper);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterIntNullableVector(int[] param)
		{
			Span<int> span = new Span<int>(param);
			fixed (int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				PrimitiveTests.ParameterIntNullableVector_Injected(ref managedSpanWrapper);
			}
		}

		public static int[] ReturnIntVector()
		{
			int[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				PrimitiveTests.ReturnIntVector_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				int[] array;
				blittableArrayWrapper.Unmarshal<int>(ref array);
				array2 = array;
			}
			return array2;
		}

		public static int[] ReturnNullIntVector()
		{
			int[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				PrimitiveTests.ReturnNullIntVector_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				int[] array;
				blittableArrayWrapper.Unmarshal<int>(ref array);
				array2 = array;
			}
			return array2;
		}

		public static bool[] ReturnBoolVector()
		{
			bool[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				PrimitiveTests.ReturnBoolVector_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				bool[] array;
				blittableArrayWrapper.Unmarshal<bool>(ref array);
				array2 = array;
			}
			return array2;
		}

		public static char[] ReturnCharVector()
		{
			char[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				PrimitiveTests.ReturnCharVector_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				char[] array;
				blittableArrayWrapper.Unmarshal<char>(ref array);
				array2 = array;
			}
			return array2;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterIntVector_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterIntNullableVector_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnIntVector_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnNullIntVector_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnBoolVector_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnCharVector_Injected(out BlittableArrayWrapper ret);
	}
}
