using System;
using System.Runtime.CompilerServices;

namespace Unity.Collections
{
	internal sealed class NativeArrayDebugView<T> where T : struct
	{
		public NativeArrayDebugView(NativeArray<T> array)
		{
			this.m_Array = array;
		}

		public T[] Items
		{
			[CompilerGenerated]
			get
			{
				return this.m_Array.ToArray();
			}
		}

		private NativeArray<T> m_Array;
	}
}
