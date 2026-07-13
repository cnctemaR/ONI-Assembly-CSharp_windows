using System;

namespace Unity.Collections
{
	[Obsolete("FixedListFloat4096DebugView is deprecated. (UnityUpgradable) -> FixedList4096BytesDebugView<float>", true)]
	internal sealed class FixedListFloat4096DebugView
	{
		public FixedListFloat4096DebugView(FixedList4096Bytes<float> list)
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

		private FixedList4096Bytes<float> m_List;
	}
}
