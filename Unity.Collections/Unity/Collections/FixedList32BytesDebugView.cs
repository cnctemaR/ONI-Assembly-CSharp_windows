using System;
using System.Runtime.CompilerServices;

namespace Unity.Collections
{
	internal sealed class FixedList32BytesDebugView<[global::System.Runtime.CompilerServices.IsUnmanaged] T> where T : struct, ValueType
	{
		public FixedList32BytesDebugView(FixedList32Bytes<T> list)
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

		private FixedList32Bytes<T> m_List;
	}
}
