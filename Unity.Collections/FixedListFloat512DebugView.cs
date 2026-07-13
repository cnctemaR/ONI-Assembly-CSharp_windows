using System;

namespace Unity.Collections
{
	[Obsolete("FixedListFloat512DebugView is deprecated. (UnityUpgradable) -> FixedList512BytesDebugView<float>", true)]
	internal sealed class FixedListFloat512DebugView
	{
		public FixedListFloat512DebugView(FixedList512Bytes<float> list)
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

		private FixedList512Bytes<float> m_List;
	}
}
