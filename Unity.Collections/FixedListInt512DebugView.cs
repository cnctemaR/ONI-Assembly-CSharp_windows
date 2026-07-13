using System;

namespace Unity.Collections
{
	[Obsolete("FixedListInt512DebugView is deprecated. (UnityUpgradable) -> FixedList512BytesDebugView<int>", true)]
	internal sealed class FixedListInt512DebugView
	{
		public FixedListInt512DebugView(FixedList512Bytes<int> list)
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

		private FixedList512Bytes<int> m_List;
	}
}
