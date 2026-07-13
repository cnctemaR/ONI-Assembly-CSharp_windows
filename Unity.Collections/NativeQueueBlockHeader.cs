using System;

namespace Unity.Collections
{
	internal struct NativeQueueBlockHeader
	{
		public unsafe NativeQueueBlockHeader* m_NextBlock;

		public int m_NumItems;
	}
}
