using System;
using System.Runtime.CompilerServices;

namespace Unity.Collections
{
	internal sealed class FixedList4096BytesDebugView<[global::System.Runtime.CompilerServices.IsUnmanaged] T> where T : struct, ValueType
	{
		public FixedList4096BytesDebugView(FixedList4096Bytes<T> list)
		{
			this.m_List = list;
		}

		public T[] Items
		{
			get
			{
				return this.m_List.ToArray();
			}
		}

		private FixedList4096Bytes<T> m_List;
	}
}
