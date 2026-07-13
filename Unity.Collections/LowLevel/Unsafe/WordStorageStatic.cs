using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	[Obsolete("This storage will no longer be used. (RemovedAfter 2021-06-01)")]
	internal sealed class WordStorageStatic
	{
		private WordStorageStatic()
		{
		}

		public static WordStorageStatic.Thing Ref;

		public struct Thing
		{
			public WordStorage Data;
		}
	}
}
