using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Bindings
{
	[VisibleToOtherModules]
	[UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(OutArray3D<>.BindingsMarshaller))]
	internal ref struct OutArray3D<[IsUnmanaged] T> where T : struct, ValueType
	{
		public static Array CreateArray(int length1, int lenght2, int length3)
		{
			return new T[length1, lenght2, length3];
		}

		public T[,,] Value
		{
			get
			{
				return this.array;
			}
		}

		[Ignore]
		private T[,,] array;

		public static class BindingsMarshaller
		{
			public static OutArrayNativeData ConvertToUnmanaged(ref OutArray3D<T> marshalled)
			{
				return new OutArrayNativeData
				{
					createAndCallback = (IntPtr)ldftn(CreateAndCallbackPinned3),
					arrayRef = (IntPtr)UnsafeUtility.AsPointer<T[,,]>(ref marshalled.array),
					createArray = (IntPtr)ldftn(CreateArray)
				};
			}

			public unsafe static OutArray3D<T> ConvertToManaged(in OutArrayNativeData unmanaged)
			{
				return new OutArray3D<T>
				{
					array = *UnsafeUtility.ClassAsRef<T[,,]>((void*)unmanaged.arrayRef)
				};
			}
		}
	}
}
