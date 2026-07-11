using System;

namespace Unity.Collections
{
	internal sealed class NativeSliceDebugView<T> where T : struct
	{
		public NativeSliceDebugView(NativeSlice<T> array)
		{
			this.m_Array = array;
		}

		public T[] Items
		{
			get
			{
				return this.m_Array.ToArray();
			}
		}

		private NativeSlice<T> m_Array;
	}
}
