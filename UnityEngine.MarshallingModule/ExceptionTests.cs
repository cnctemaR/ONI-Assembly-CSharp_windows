using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	[ExcludeFromDocs]
	internal class ExceptionTests
	{
		[NativeThrows]
		public unsafe static void VoidReturnStringParameter(string param)
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
				ExceptionTests.VoidReturnStringParameter_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int NonUnmarshallingReturn();

		[NativeThrows]
		public static string UnmarshallingReturn()
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				ExceptionTests.UnmarshallingReturn_Injected(out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[NativeThrows]
		public static StructInt BlittableStructReturn()
		{
			StructInt structInt;
			ExceptionTests.BlittableStructReturn_Injected(out structInt);
			return structInt;
		}

		[NativeThrows]
		public static StructCoreString NonblittableStructReturn()
		{
			StructCoreString structCoreString;
			ExceptionTests.NonblittableStructReturn_Injected(out structCoreString);
			return structCoreString;
		}

		[NativeThrows]
		public static extern int PropertyThatCanThrow
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int PropertyGetThatCanThrow
		{
			[NativeThrows]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int PropertySetThatCanThrow
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeThrows]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void VoidReturnStringParameter_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UnmarshallingReturn_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BlittableStructReturn_Injected(out StructInt ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void NonblittableStructReturn_Injected(out StructCoreString ret);
	}
}
