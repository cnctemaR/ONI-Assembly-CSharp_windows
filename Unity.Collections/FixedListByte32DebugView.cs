using System;

namespace Unity.Collections
{
	[Obsolete("FixedListByte32DebugView is deprecated. (UnityUpgradable) -> FixedList32BytesDebugView<byte>", true)]
	internal sealed class FixedListByte32DebugView
	{
		public FixedListByte32DebugView(FixedList32Bytes<byte> list)
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

		private FixedList32Bytes<byte> m_List;
	}
}
