using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Bindings
{
	[VisibleToOtherModules]
	[UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(OutArray<>.BindingsMarshaller))]
	internal ref struct OutArray<[IsUnmanaged] T> where T : struct, ValueType
	{
		public static Array CreateArray(int length)
		{
			return new T[length];
		}

		public T[] Value
		{
			get
			{
				return this.array;
			}
		}

		[Ignore]
		private T[] array;

		public static class BindingsMarshaller
		{
			public static OutArrayNativeData ConvertToUnmanaged(ref OutArray<T> marshalled)
			{
				return new OutArrayNativeData
				{
					createAndCallback = (IntPtr)ldftn(CreateAndCallbackPinned1),
					arrayRef = (IntPtr)UnsafeUtility.AsPointer<T[]>(ref marshalled.array),
					createArray = (IntPtr)ldftn(CreateArray)
				};
			}

			public unsafe static OutArray<T> ConvertToManaged(in OutArrayNativeData unmanaged)
			{
				return new OutArray<T>
				{
					array = *UnsafeUtility.ClassAsRef<T[]>((void*)unmanaged.arrayRef)
				};
			}
		}
	}
}
