using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[ExcludeFromDocs]
	[NativeHeader("Modules/Marshalling/OutArrayMarshallingTests.h")]
	internal static class OutArrayMarshallingTests
	{
		public unsafe static void OutArrayOfPrimitiveTypeWorks([Out] int[] array, int value)
		{
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				if (array != null)
				{
					fixed (int[] array2 = array)
					{
						if (array2.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array2[0]), array2.Length);
						}
					}
				}
				OutArrayMarshallingTests.OutArrayOfPrimitiveTypeWorks_Injected(out blittableArrayWrapper, value);
			}
			finally
			{
				int[] array2;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<int>(ref array2);
			}
		}

		public unsafe static void OutArrayOfStringTypeWorks([Out] string[] array, string value)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = value.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				OutArrayMarshallingTests.OutArrayOfStringTypeWorks_Injected(array, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		public unsafe static void OutArrayOfBlittableStructTypeWorks([Out] StructInt[] array, StructInt value)
		{
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				if (array != null)
				{
					fixed (StructInt[] array2 = array)
					{
						if (array2.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array2[0]), array2.Length);
						}
					}
				}
				OutArrayMarshallingTests.OutArrayOfBlittableStructTypeWorks_Injected(out blittableArrayWrapper, ref value);
			}
			finally
			{
				StructInt[] array2;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<StructInt>(ref array2);
			}
		}

		public static void OutArrayOfIntPtrObjectTypeWorks([Out] MyIntPtrObject[] array, MyIntPtrObject value)
		{
			OutArrayMarshallingTests.OutArrayOfIntPtrObjectTypeWorks_Injected(array, (value == null) ? ((IntPtr)0) : MyIntPtrObject.BindingsMarshaller.ConvertToNative(value));
		}

		public unsafe static void OutArrayOfNestedBlittableStructTypeWorks([Out] StructNestedBlittable[] array, StructNestedBlittable value)
		{
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				if (array != null)
				{
					fixed (StructNestedBlittable[] array2 = array)
					{
						if (array2.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array2[0]), array2.Length);
						}
					}
				}
				OutArrayMarshallingTests.OutArrayOfNestedBlittableStructTypeWorks_Injected(out blittableArrayWrapper, ref value);
			}
			finally
			{
				StructNestedBlittable[] array2;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<StructNestedBlittable>(ref array2);
			}
		}

		public static void OutArrayOfNonBlittableTypeWorks([Out] StructWithStringIntAndFloat[] array, StructWithStringIntAndFloat value)
		{
			OutArrayMarshallingTests.OutArrayOfNonBlittableTypeWorks_Injected(array, ref value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OutArrayOfPrimitiveTypeWorks_Injected(out BlittableArrayWrapper array, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OutArrayOfStringTypeWorks_Injected([Out] string[] array, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OutArrayOfBlittableStructTypeWorks_Injected(out BlittableArrayWrapper array, [In] ref StructInt value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OutArrayOfIntPtrObjectTypeWorks_Injected([Out] MyIntPtrObject[] array, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OutArrayOfNestedBlittableStructTypeWorks_Injected(out BlittableArrayWrapper array, [In] ref StructNestedBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OutArrayOfNonBlittableTypeWorks_Injected([Out] StructWithStringIntAndFloat[] array, [In] ref StructWithStringIntAndFloat value);
	}
}
