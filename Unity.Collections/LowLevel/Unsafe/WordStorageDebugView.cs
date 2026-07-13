using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	[Obsolete("This storage will no longer be used. (RemovedAfter 2021-06-01)")]
	internal sealed class WordStorageDebugView
	{
		public WordStorageDebugView(WordStorage wordStorage)
		{
			this.m_wordStorage = wordStorage;
		}

		public FixedString128Bytes[] Table
		{
			get
			{
				FixedString128Bytes[] array = new FixedString128Bytes[this.m_wordStorage.Entries];
				for (int i = 0; i < this.m_wordStorage.Entries; i++)
				{
					this.m_wordStorage.GetFixedString<FixedString128Bytes>(i, ref array[i]);
				}
				return array;
			}
		}

		private WordStorage m_wordStorage;
	}
}
