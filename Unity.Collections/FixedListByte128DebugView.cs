using System;

namespace Unity.Collections
{
	[Obsolete("FixedListByte128DebugView is deprecated. (UnityUpgradable) -> FixedList128BytesDebugView<byte>", true)]
	internal sealed class FixedListByte128DebugView
	{
		public FixedListByte128DebugView(FixedList128Bytes<byte> list)
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

		private FixedList128Bytes<byte> m_List;
	}
}
