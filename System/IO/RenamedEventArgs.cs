using System;

namespace System.IO
{
	public class RenamedEventArgs : FileSystemEventArgs
	{
		public RenamedEventArgs(WatcherChangeTypes changeType, string directory, string name, string oldName)
			: base(changeType, directory, name)
		{
			this.oldName = oldName;
			this.oldFullPath = Path.Combine(directory, oldName);
		}

		public string OldFullPath
		{
			get
			{
				return this.oldFullPath;
			}
		}

		public string OldName
		{
			get
			{
				return this.oldName;
			}
		}

		private string oldName;

		private string oldFullPath;
	}
}
