using System;

namespace Unity.Collections
{
	[Obsolete("FixedListByte64DebugView is deprecated. (UnityUpgradable) -> FixedList64BytesDebugView<byte>", true)]
	internal sealed class FixedListByte64DebugView
	{
		public FixedListByte64DebugView(FixedList64Bytes<byte> list)
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

		private FixedList64Bytes<byte> m_List;
	}
}
