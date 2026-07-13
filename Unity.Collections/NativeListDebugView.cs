using System;
using System.Runtime.CompilerServices;

namespace Unity.Collections
{
	internal sealed class NativeListDebugView<[global::System.Runtime.CompilerServices.IsUnmanaged] T> where T : struct, ValueType
	{
		public NativeListDebugView(NativeList<T> array)
		{
			this.m_Array = array;
		}

		public T[] Items
		{
			get
			{
				return this.m_Array.AsArray().ToArray();
			}
		}

		private NativeList<T> m_Array;
	}
}
