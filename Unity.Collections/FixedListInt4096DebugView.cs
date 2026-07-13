using System;

namespace Unity.Collections
{
	[Obsolete("FixedListInt4096DebugView is deprecated. (UnityUpgradable) -> FixedList4096BytesDebugView<int>", true)]
	internal sealed class FixedListInt4096DebugView
	{
		public FixedListInt4096DebugView(FixedList4096Bytes<int> list)
		{
			this.m_List = list;
		}

		public int[] Items
		{
			get
			{
				return this.m_List.ToArray();
			}
		}

		private FixedList4096Bytes<int> m_List;
	}
}
