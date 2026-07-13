using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Bindings
{
	[VisibleToOtherModules]
	internal ref struct BlittableArrayWrapper
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe BlittableArrayWrapper(void* data, int size)
		{
			this.data = data;
			this.size = size;
			this.updateFlags = BlittableArrayWrapper.UpdateFlags.NoUpdateNeeded;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Unmarshal<[IsUnmanaged] T>(ref T[] array) where T : struct, ValueType
		{
			switch (this.updateFlags)
			{
			case BlittableArrayWrapper.UpdateFlags.SizeChanged:
			case BlittableArrayWrapper.UpdateFlags.DataIsNativePointer:
				array = new Span<T>(this.data, this.size).ToArray();
				break;
			case BlittableArrayWrapper.UpdateFlags.DataIsNativeOwnedMemory:
				array = new Span<T>(BindingsAllocator.GetNativeOwnedDataPointer(this.data), this.size).ToArray();
				BindingsAllocator.FreeNativeOwnedMemory(this.data);
				break;
			case BlittableArrayWrapper.UpdateFlags.DataIsEmpty:
				array = Array.Empty<T>();
				break;
			case BlittableArrayWrapper.UpdateFlags.DataIsNull:
				array = null;
				break;
			}
		}

		internal unsafe void* data;

		internal int size;

		internal BlittableArrayWrapper.UpdateFlags updateFlags;

		internal enum UpdateFlags
		{
			NoUpdateNeeded,
			SizeChanged,
			DataIsNativePointer,
			DataIsNativeOwnedMemory,
			DataIsEmpty,
			DataIsNull
		}
	}
}
