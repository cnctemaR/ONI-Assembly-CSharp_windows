using System;

namespace Unity.Collections
{
	[Obsolete("FixedListByte512DebugView is deprecated. (UnityUpgradable) -> FixedList512BytesDebugView<byte>", true)]
	internal sealed class FixedListByte512DebugView
	{
		public FixedListByte512DebugView(FixedList512Bytes<byte> list)
		{
			this.m_List = list;
		}

		public byte[] Items
		{
			get
			{
				return this.m_List.ToArray();
			}
		}

		private FixedList512Bytes<byte> m_List;
	}
}
