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

		public void StartDispatching(FileSystemWatcher fsw)
		{
			KqueueMonitor kqueueMonitor;
			if (KeventWatcher.watches.ContainsKey(fsw))
			{
				kqueueMonitor = (KqueueMonitor)KeventWatcher.watches[fsw];
			}
			else
			{
				kqueueMonitor = new KqueueMonitor(fsw);
				KeventWatcher.watches.Add(fsw, kqueueMonitor);
			}
			kqueueMonitor.Start();
		}

		public void StopDispatching(FileSystemWatcher fsw)
		{
			KqueueMonitor kqueueMonitor = (KqueueMonitor)KeventWatcher.watches[fsw];
			if (kqueueMonitor == null)
			{
				return;
			}
			kqueueMonitor.Stop();
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
