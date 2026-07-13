using System;
using System.Runtime.CompilerServices;

namespace Unity.Collections
{
	internal sealed class FixedList512BytesDebugView<[global::System.Runtime.CompilerServices.IsUnmanaged] T> where T : struct, ValueType
	{
		public FixedList512BytesDebugView(FixedList512Bytes<T> list)
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

		private FixedList512Bytes<T> m_List;
	}
}
