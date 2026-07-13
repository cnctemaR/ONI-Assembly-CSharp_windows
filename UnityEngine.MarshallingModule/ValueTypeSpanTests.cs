using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeType("Modules/Marshalling/MarshallingTests.h")]
	internal class ValueTypeSpanTests
	{
		[NativeThrows]
		public unsafe static void ParameterIntReadOnlySpan(ReadOnlySpan<int> param)
		{
			ReadOnlySpan<int> readOnlySpan = param;
			fixed (int* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				ValueTypeSpanTests.ParameterIntReadOnlySpan_Injected(ref managedSpanWrapper);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterIntSpan(Span<int> param)
		{
			Span<int> span = param;
			fixed (int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				ValueTypeSpanTests.ParameterIntSpan_Injected(ref managedSpanWrapper);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterBoolReadOnlySpan(ReadOnlySpan<bool> param)
		{
			ReadOnlySpan<bool> readOnlySpan = param;
			fixed (bool* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				ValueTypeSpanTests.ParameterBoolReadOnlySpan_Injected(ref managedSpanWrapper);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterCharReadOnlySpan(ReadOnlySpan<char> param)
		{
			ReadOnlySpan<char> readOnlySpan = param;
			fixed (char* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				ValueTypeSpanTests.ParameterCharReadOnlySpan_Injected(ref managedSpanWrapper);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterEnumReadOnlySpan(ReadOnlySpan<SomeEnum> param)
		{
			ReadOnlySpan<SomeEnum> readOnlySpan = param;
			fixed (SomeEnum* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				ValueTypeSpanTests.ParameterEnumReadOnlySpan_Injected(ref managedSpanWrapper);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterBlittableCornerCaseStructReadOnlySpan(ReadOnlySpan<BlittableCornerCases> param)
		{
			ReadOnlySpan<BlittableCornerCases> readOnlySpan = param;
			fixed (BlittableCornerCases* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				ValueTypeSpanTests.ParameterBlittableCornerCaseStructReadOnlySpan_Injected(ref managedSpanWrapper);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterStructWithSelfPointerSpan(Span<StructWithSelfPointer> param)
		{
			Span<StructWithSelfPointer> span = param;
			fixed (StructWithSelfPointer* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				ValueTypeSpanTests.ParameterStructWithSelfPointerSpan_Injected(ref managedSpanWrapper);
			}
		}

		public static Span<int> ReturnsArrayRefWritableAsSpan(int val1, int val2, int val3)
		{
			ManagedSpanWrapper managedSpanWrapper;
			ValueTypeSpanTests.ReturnsArrayRefWritableAsSpan_Injected(val1, val2, val3, out managedSpanWrapper);
			return ManagedSpanWrapper.ToSpan<int>(managedSpanWrapper);
		}

		public static Span<int> ReturnsVectorRefAsSpan(int val1, int val2, int val3)
		{
			ManagedSpanWrapper managedSpanWrapper;
			ValueTypeSpanTests.ReturnsVectorRefAsSpan_Injected(val1, val2, val3, out managedSpanWrapper);
			return ManagedSpanWrapper.ToSpan<int>(managedSpanWrapper);
		}

		public static Span<int> ReturnsScriptingSpanAsSpan(int val1, int val2, int val3)
		{
			ManagedSpanWrapper managedSpanWrapper;
			ValueTypeSpanTests.ReturnsScriptingSpanAsSpan_Injected(val1, val2, val3, out managedSpanWrapper);
			return ManagedSpanWrapper.ToSpan<int>(managedSpanWrapper);
		}

		public static ReadOnlySpan<int> ReturnsArrayRefWritableAsReadOnlySpan(int val1, int val2, int val3)
		{
			ManagedSpanWrapper managedSpanWrapper;
			ValueTypeSpanTests.ReturnsArrayRefWritableAsReadOnlySpan_Injected(val1, val2, val3, out managedSpanWrapper);
			return ManagedSpanWrapper.ToReadOnlySpan<int>(managedSpanWrapper);
		}

		public static ReadOnlySpan<int> ReturnsVectorRefAsReadOnlySpan(int val1, int val2, int val3)
		{
			ManagedSpanWrapper managedSpanWrapper;
			ValueTypeSpanTests.ReturnsVectorRefAsReadOnlySpan_Injected(val1, val2, val3, out managedSpanWrapper);
			return ManagedSpanWrapper.ToReadOnlySpan<int>(managedSpanWrapper);
		}

		public static ReadOnlySpan<int> ReturnsArrayRefAsReadOnlySpan(int val1, int val2, int val3)
		{
			ManagedSpanWrapper managedSpanWrapper;
			ValueTypeSpanTests.ReturnsArrayRefAsReadOnlySpan_Injected(val1, val2, val3, out managedSpanWrapper);
			return ManagedSpanWrapper.ToReadOnlySpan<int>(managedSpanWrapper);
		}

		public static ReadOnlySpan<int> ReturnsScriptingReadOnlySpanAsSpan(int val1, int val2, int val3)
		{
			ManagedSpanWrapper managedSpanWrapper;
			ValueTypeSpanTests.ReturnsScriptingReadOnlySpanAsSpan_Injected(val1, val2, val3, out managedSpanWrapper);
			return ManagedSpanWrapper.ToReadOnlySpan<int>(managedSpanWrapper);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterIntReadOnlySpan_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterIntSpan_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterBoolReadOnlySpan_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCharReadOnlySpan_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterEnumReadOnlySpan_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterBlittableCornerCaseStructReadOnlySpan_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructWithSelfPointerSpan_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnsArrayRefWritableAsSpan_Injected(int val1, int val2, int val3, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnsVectorRefAsSpan_Injected(int val1, int val2, int val3, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnsScriptingSpanAsSpan_Injected(int val1, int val2, int val3, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnsArrayRefWritableAsReadOnlySpan_Injected(int val1, int val2, int val3, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnsVectorRefAsReadOnlySpan_Injected(int val1, int val2, int val3, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnsArrayRefAsReadOnlySpan_Injected(int val1, int val2, int val3, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnsScriptingReadOnlySpanAsSpan_Injected(int val1, int val2, int val3, out ManagedSpanWrapper ret);
	}
}
