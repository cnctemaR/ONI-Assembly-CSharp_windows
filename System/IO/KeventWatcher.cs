using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.IO
{
	internal class KeventWatcher : IFileWatcher
	{
		private KeventWatcher()
		{
		}

		public static bool GetInstance(out IFileWatcher watcher)
		{
			if (KeventWatcher.failed)
			{
				watcher = null;
				return false;
			}
			if (KeventWatcher.instance != null)
			{
				watcher = KeventWatcher.instance;
				return true;
			}
			KeventWatcher.watches = Hashtable.Synchronized(new Hashtable());
			int num = KeventWatcher.kqueue();
			if (num == -1)
			{
				KeventWatcher.failed = true;
				watcher = null;
				return false;
			}
			KeventWatcher.close(num);
			KeventWatcher.instance = new KeventWatcher();
			watcher = KeventWatcher.instance;
			return true;
		}

		public void StartDispatching(object handle)
		{
			FileSystemWatcher fileSystemWatcher = handle as FileSystemWatcher;
			KqueueMonitor kqueueMonitor;
			if (KeventWatcher.watches.ContainsKey(fileSystemWatcher))
			{
				kqueueMonitor = (KqueueMonitor)KeventWatcher.watches[fileSystemWatcher];
			}
			else
			{
				kqueueMonitor = new KqueueMonitor(fileSystemWatcher);
				KeventWatcher.watches.Add(fileSystemWatcher, kqueueMonitor);
			}
			kqueueMonitor.Start();
		}

		public void StopDispatching(object handle)
		{
			FileSystemWatcher fileSystemWatcher = handle as FileSystemWatcher;
			KqueueMonitor kqueueMonitor = (KqueueMonitor)KeventWatcher.watches[fileSystemWatcher];
			if (kqueueMonitor == null)
			{
				return;
			}
			kqueueMonitor.Stop();
		}

		public void Dispose(object handle)
		{
		}

		[DllImport("libc")]
		private static extern int close(int fd);

		[DllImport("libc")]
		private static extern int kqueue();

		private static bool failed;

		private static KeventWatcher instance;

		private static Hashtable watches;
	}
}
