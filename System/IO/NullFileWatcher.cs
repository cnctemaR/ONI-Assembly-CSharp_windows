using System;

namespace System.IO
{
	internal class NullFileWatcher : IFileWatcher
	{
		public void StartDispatching(object handle)
		{
		}

		public void StopDispatching(object handle)
		{
		}

		public void Dispose(object handle)
		{
		}

		public static bool GetInstance(out IFileWatcher watcher)
		{
			if (NullFileWatcher.instance != null)
			{
				watcher = NullFileWatcher.instance;
				return true;
			}
			IFileWatcher fileWatcher;
			watcher = (fileWatcher = new NullFileWatcher());
			NullFileWatcher.instance = fileWatcher;
			return true;
		}

		private static IFileWatcher instance;
	}
}
