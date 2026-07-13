using System;

namespace Unity.Collections
{
	[Obsolete("FixedListInt128DebugView is deprecated. (UnityUpgradable) -> FixedList128BytesDebugView<int>", true)]
	internal sealed class FixedListInt128DebugView
	{
		public FixedListInt128DebugView(FixedList128Bytes<int> list)
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

		private FixedList128Bytes<int> m_List;
	}
}
