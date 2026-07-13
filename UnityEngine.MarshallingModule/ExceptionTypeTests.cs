using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[ExcludeFromDocs]
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	internal class ExceptionTypeTests
	{
		[NativeThrows]
		public unsafe static void NullReferenceException(string nativeFormat, string values)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(nativeFormat, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = nativeFormat.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(values, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = values.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				ExceptionTypeTests.NullReferenceException_Injected(ref managedSpanWrapper, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
		}

		[NativeThrows]
		public unsafe static void ArgumentNullException(string argumentName)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(argumentName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = argumentName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ExceptionTypeTests.ArgumentNullException_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[NativeThrows]
		public unsafe static void ArgumentException(string nativeFormat, string values)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(nativeFormat, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = nativeFormat.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(values, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = values.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				ExceptionTypeTests.ArgumentException_Injected(ref managedSpanWrapper, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
		}

		[NativeThrows]
		public unsafe static void InvalidOperationException(string nativeFormat, string values)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(nativeFormat, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = nativeFormat.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(values, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = values.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				ExceptionTypeTests.InvalidOperationException_Injected(ref managedSpanWrapper, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
		}

		[NativeThrows]
		public unsafe static void IndexOutOfRangeException(string nativeFormat, int index)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(nativeFormat, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = nativeFormat.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ExceptionTypeTests.IndexOutOfRangeException_Injected(ref managedSpanWrapper, index);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void NullReferenceException_Injected(ref ManagedSpanWrapper nativeFormat, ref ManagedSpanWrapper values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ArgumentNullException_Injected(ref ManagedSpanWrapper argumentName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ArgumentException_Injected(ref ManagedSpanWrapper nativeFormat, ref ManagedSpanWrapper values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InvalidOperationException_Injected(ref ManagedSpanWrapper nativeFormat, ref ManagedSpanWrapper values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void IndexOutOfRangeException_Injected(ref ManagedSpanWrapper nativeFormat, int index);
	}
}
