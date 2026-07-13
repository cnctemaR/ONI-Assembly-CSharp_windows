using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.UIElements.Layout
{
	[Serializable]
	internal struct FixedBuffer16<[global::System.Runtime.CompilerServices.IsUnmanaged] T> where T : struct, ValueType
	{
		public unsafe ref T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				bool flag = index < 0 || index >= 16;
				if (flag)
				{
					throw new IndexOutOfRangeException("index");
				}
				fixed (FixedBuffer16<T>* ptr = (FixedBuffer16<T>*)(&this))
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

		[SerializeField]
		private T __4;

		[SerializeField]
		private T __5;

		[SerializeField]
		private T __6;

		[SerializeField]
		private T __7;

		[SerializeField]
		private T __8;

		[SerializeField]
		private T __9;

		[SerializeField]
		private T _10;

		[SerializeField]
		private T _11;

		[SerializeField]
		private T _12;

		[SerializeField]
		private T _13;

		[SerializeField]
		private T _14;

		[SerializeField]
		private T _15;

		public const int Length = 16;
	}
}
