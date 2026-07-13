using System;
using System.Runtime.CompilerServices;

namespace Unity.Collections
{
	internal sealed class FixedList64BytesDebugView<[global::System.Runtime.CompilerServices.IsUnmanaged] T> where T : struct, ValueType
	{
		public FixedList64BytesDebugView(FixedList64Bytes<T> list)
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

		private FixedList64Bytes<T> m_List;
	}
}
