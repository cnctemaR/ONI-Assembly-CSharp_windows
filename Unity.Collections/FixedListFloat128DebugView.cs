using System;

namespace Unity.Collections
{
	[Obsolete("FixedListFloat128DebugView is deprecated. (UnityUpgradable) -> FixedList128BytesDebugView<float>", true)]
	internal sealed class FixedListFloat128DebugView
	{
		public FixedListFloat128DebugView(FixedList128Bytes<float> list)
		{
			this.m_List = list;
		}

		public float[] Items
		{
			get
			{
				return this.m_List.ToArray();
			}
		}

		private FixedList128Bytes<float> m_List;
	}
}
