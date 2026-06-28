using System;

namespace System.IO
{
	internal class KeventFileData
	{
		public KeventFileData(FileSystemInfo fsi, DateTime LastAccessTime, DateTime LastWriteTime)
		{
			this.fsi = fsi;
			this.LastAccessTime = LastAccessTime;
			this.LastWriteTime = LastWriteTime;
		}

		public FileSystemInfo fsi;

		public DateTime LastAccessTime;

		public DateTime LastWriteTime;
	}
}
