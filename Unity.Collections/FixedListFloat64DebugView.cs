using System;

namespace Unity.Collections
{
	[Obsolete("FixedListFloat64DebugView is deprecated. (UnityUpgradable) -> FixedList64BytesDebugView<float>", true)]
	internal sealed class FixedListFloat64DebugView
	{
		public FixedListFloat64DebugView(FixedList64Bytes<float> list)
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

		private FixedList64Bytes<float> m_List;
	}
}
