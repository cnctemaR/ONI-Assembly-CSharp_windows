using System;

namespace Unity.Collections
{
	[Obsolete("FixedListInt64DebugView is deprecated. (UnityUpgradable) -> FixedList64BytesDebugView<int>", true)]
	internal sealed class FixedListInt64DebugView
	{
		public FixedListInt64DebugView(FixedList64Bytes<int> list)
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

		private FixedList64Bytes<int> m_List;
	}
}
