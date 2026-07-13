using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Bindings
{
	[VisibleToOtherModules]
	[UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(OutArray2D<>.BindingsMarshaller))]
	internal ref struct OutArray2D<[IsUnmanaged] T> where T : struct, ValueType
	{
		public static Array CreateArray(int length1, int lenght2)
		{
			return new T[length1, lenght2];
		}

		public T[,] Value
		{
			get
			{
				return this.array;
			}
		}

		[Ignore]
		private T[,] array;

		public static class BindingsMarshaller
		{
			public static OutArrayNativeData ConvertToUnmanaged(ref OutArray2D<T> marshalled)
			{
				return new OutArrayNativeData
				{
					createAndCallback = (IntPtr)ldftn(CreateAndCallbackPinned2),
					arrayRef = (IntPtr)UnsafeUtility.AsPointer<T[,]>(ref marshalled.array),
					createArray = (IntPtr)ldftn(CreateArray)
				};
			}

			public unsafe static OutArray2D<T> ConvertToManaged(in OutArrayNativeData unmanaged)
			{
				return new OutArray2D<T>
				{
					array = *UnsafeUtility.ClassAsRef<T[,]>((void*)unmanaged.arrayRef)
				};
			}
		}
	}
}
