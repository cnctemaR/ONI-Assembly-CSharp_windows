using System;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	internal struct Long1024 : IIndexable<long>
	{
		public int Length
		{
			get
			{
				return 1024;
			}
			set
			{
			}
		}

		public unsafe ref long ElementAt(int index)
		{
			fixed (Long512* ptr = &this.f0)
			{
				return UnsafeUtility.AsRef<long>((void*)((byte*)ptr + (IntPtr)index * 8));
			}
		}

		internal Long512 f0;

		internal Long512 f1;
	}
}
