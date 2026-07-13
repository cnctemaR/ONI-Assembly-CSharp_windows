using System;

namespace Unity.Collections
{
	[Obsolete("FixedListInt32DebugView is deprecated. (UnityUpgradable) -> FixedList32BytesDebugView<int>", true)]
	internal sealed class FixedListInt32DebugView
	{
		public FixedListInt32DebugView(FixedList32Bytes<int> list)
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

		private FixedList32Bytes<int> m_List;
	}
}
