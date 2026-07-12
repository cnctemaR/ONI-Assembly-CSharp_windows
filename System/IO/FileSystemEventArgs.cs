using System;

namespace System.IO
{
	public class FileSystemEventArgs : EventArgs
	{
		public FileSystemEventArgs(WatcherChangeTypes changeType, string directory, string name)
		{
			this._changeType = changeType;
			this._name = name;
			this._fullPath = Path.GetFullPath(FileSystemEventArgs.Combine(directory, name));
		}

		internal static string Combine(string directoryPath, string name)
		{
			bool flag = false;
			if (directoryPath.Length > 0)
			{
				char c = directoryPath[directoryPath.Length - 1];
				flag = c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar;
			}
			if (!flag)
			{
				return directoryPath + Path.DirectorySeparatorChar.ToString() + name;
			}
			return directoryPath + name;
		}

		public WatcherChangeTypes ChangeType
		{
			get
			{
				return this._changeType;
			}
		}

		public string FullPath
		{
			get
			{
				return this._fullPath;
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
		}

		private readonly WatcherChangeTypes _changeType;

		private readonly string _name;

		private readonly string _fullPath;
	}
}
