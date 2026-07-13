using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[ExcludeFromDocs]
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	internal class NullCheckTests
	{
		public unsafe static void StringParameterNullAllowed(string param)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(param, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = param.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				NullCheckTests.StringParameterNullAllowed_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		public unsafe static void StringParameterNullNotAllowed([NotNull] string param)
		{
			if (param == null)
			{
				ThrowHelper.ThrowArgumentNullException(param, "param");
			}
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(param, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = param.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				NullCheckTests.StringParameterNullNotAllowed_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		public unsafe static void ArrayParameterNullAllowed(int[] param)
		{
			Span<int> span = new Span<int>(param);
			fixed (int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				NullCheckTests.ArrayParameterNullAllowed_Injected(ref managedSpanWrapper);
			}
		}

		public unsafe static void ArrayParameterNullNotAllowed([NotNull] int[] param)
		{
			if (param == null)
			{
				ThrowHelper.ThrowArgumentNullException(param, "param");
			}
			Span<int> span = new Span<int>(param);
			fixed (int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				NullCheckTests.ArrayParameterNullNotAllowed_Injected(ref managedSpanWrapper);
			}
		}

		[NativeThrows]
		public static void ObjectParameterNullAllowed(MarshallingTestObject param)
		{
			NullCheckTests.ObjectParameterNullAllowed_Injected(Object.MarshalledUnityObject.Marshal<MarshallingTestObject>(param));
		}

		public static void ObjectParameterNullNotAllowed([NotNull] MarshallingTestObject param)
		{
			if (param == null)
			{
				ThrowHelper.ThrowArgumentNullException(param, "param");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MarshallingTestObject>(param);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(param, "param");
			}
			NullCheckTests.ObjectParameterNullNotAllowed_Injected(intPtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void WritableObjectParameterNullAllowed([Writable] MarshallingTestObject param);

		public static void WritableObjectParameterNullNotAllowed([Writable] [NotNull] MarshallingTestObject param)
		{
			if (param == null)
			{
				ThrowHelper.ThrowArgumentNullException(param, "param");
			}
			NullCheckTests.WritableObjectParameterNullNotAllowed_Injected(param);
		}

		[NativeThrows]
		public static void IntPtrObjectParameterNullAllowed(MyIntPtrObject param)
		{
			NullCheckTests.IntPtrObjectParameterNullAllowed_Injected((param == null) ? ((IntPtr)0) : MyIntPtrObject.BindingsMarshaller.ConvertToNative(param));
		}

		public static void IntPtrObjectParameterNullNotAllowed([NotNull] MyIntPtrObject param)
		{
			if (param == null)
			{
				ThrowHelper.ThrowArgumentNullException(param, "param");
			}
			IntPtr intPtr = MyIntPtrObject.BindingsMarshaller.ConvertToNative(param);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(param, "param");
			}
			NullCheckTests.IntPtrObjectParameterNullNotAllowed_Injected(intPtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StringParameterNullAllowed_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StringParameterNullNotAllowed_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ArrayParameterNullAllowed_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ArrayParameterNullNotAllowed_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ObjectParameterNullAllowed_Injected(IntPtr param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ObjectParameterNullNotAllowed_Injected(IntPtr param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void WritableObjectParameterNullNotAllowed_Injected([Writable] MarshallingTestObject param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void IntPtrObjectParameterNullAllowed_Injected(IntPtr param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void IntPtrObjectParameterNullNotAllowed_Injected(IntPtr param);
	}
}
