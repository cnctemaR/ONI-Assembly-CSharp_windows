using System;

namespace Unity.Collections
{
	[Obsolete("FixedListFloat32DebugView is deprecated. (UnityUpgradable) -> FixedList32BytesDebugView<float>", true)]
	internal sealed class FixedListFloat32DebugView
	{
		public FixedListFloat32DebugView(FixedList32Bytes<float> list)
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

		private FixedList32Bytes<float> m_List;
	}
}
