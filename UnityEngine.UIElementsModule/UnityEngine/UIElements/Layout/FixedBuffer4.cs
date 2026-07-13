using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.UIElements.Layout
{
	[Serializable]
	internal struct FixedBuffer4<[global::System.Runtime.CompilerServices.IsUnmanaged] T> where T : struct, ValueType
	{
		public FixedBuffer4(T x, T y, T z, T w)
		{
			this.__0 = x;
			this.__1 = y;
			this.__2 = z;
			this.__3 = w;
		}

		public unsafe ref T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				bool flag = index < 0 || index >= 4;
				if (flag)
				{
					throw new IndexOutOfRangeException("index");
				}
				fixed (FixedBuffer4<T>* ptr = (FixedBuffer4<T>*)(&this))
				{
					void* ptr2 = (void*)ptr;
					T* ptr3 = (T*)ptr2;
					return ref ptr3[(IntPtr)index * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
				}
			}
		}

		[SerializeField]
		private T __0;

		[SerializeField]
		private T __1;

		[SerializeField]
		private T __2;

		[SerializeField]
		private T __3;

		public const int Length = 4;
	}
}
