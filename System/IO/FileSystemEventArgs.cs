using System;

namespace System.IO
{
	public class FileSystemEventArgs : EventArgs
	{
		public FileSystemEventArgs(WatcherChangeTypes changeType, string directory, string name)
		{
			this.changeType = changeType;
			this.directory = directory;
			this.name = name;
		}

		internal void SetName(string name)
		{
			this.name = name;
		}

		public WatcherChangeTypes ChangeType
		{
			get
			{
				return this.changeType;
			}
		}

		public string FullPath
		{
			get
			{
				return Path.Combine(this.directory, this.name);
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		private WatcherChangeTypes changeType;

		private string directory;

		private string name;
	}
}
