using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityEngine.Bindings
{
	[VisibleToOtherModules]
	internal ref struct BlittableListWrapper
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BlittableListWrapper(BlittableArrayWrapper arrayWrapper, int listSize)
		{
			this.arrayWrapper = arrayWrapper;
			this.listSize = listSize;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Unmarshal<[IsUnmanaged] T>(List<T> list) where T : struct, ValueType
		{
			bool flag = list == null;
			if (!flag)
			{
				switch (this.arrayWrapper.updateFlags)
				{
				case BlittableArrayWrapper.UpdateFlags.SizeChanged:
				case BlittableArrayWrapper.UpdateFlags.DataIsEmpty:
				case BlittableArrayWrapper.UpdateFlags.DataIsNull:
					NoAllocHelpers.ResetListSize<T>(list, this.listSize);
					break;
				case BlittableArrayWrapper.UpdateFlags.DataIsNativePointer:
					NoAllocHelpers.ResetListContents<T>(list, new ReadOnlySpan<T>(this.arrayWrapper.data, this.arrayWrapper.size));
					break;
				case BlittableArrayWrapper.UpdateFlags.DataIsNativeOwnedMemory:
					NoAllocHelpers.ResetListContents<T>(list, new ReadOnlySpan<T>(BindingsAllocator.GetNativeOwnedDataPointer(this.arrayWrapper.data), this.arrayWrapper.size));
					BindingsAllocator.FreeNativeOwnedMemory(this.arrayWrapper.data);
					break;
				}
			}
		}

		private BlittableArrayWrapper arrayWrapper;

		private int listSize;
	}
}
