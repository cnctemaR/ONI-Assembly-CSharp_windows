using System;

namespace System.IO
{
	public class RenamedEventArgs : FileSystemEventArgs
	{
		public RenamedEventArgs(WatcherChangeTypes changeType, string directory, string name, string oldName)
			: base(changeType, directory, name)
		{
			this._oldName = oldName;
			this._oldFullPath = FileSystemEventArgs.Combine(directory, oldName);
		}

		public string OldFullPath
		{
			get
			{
				return this._oldFullPath;
			}
		}

		public string OldName
		{
			get
			{
				return this._oldName;
			}
		}

		private readonly string _oldName;

		private readonly string _oldFullPath;
	}
}
