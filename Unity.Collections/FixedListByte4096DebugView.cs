using System;

namespace Unity.Collections
{
	[Obsolete("FixedListByte4096DebugView is deprecated. (UnityUpgradable) -> FixedList4096BytesDebugView<byte>", true)]
	internal sealed class FixedListByte4096DebugView
	{
		public FixedListByte4096DebugView(FixedList4096Bytes<byte> list)
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

		private FixedList4096Bytes<byte> m_List;
	}
}
