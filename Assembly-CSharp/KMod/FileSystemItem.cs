using System;

namespace KMod
{
	public struct FileSystemItem
	{
		public string name;

		public FileSystemItem.ItemType type;

		public enum ItemType
		{
			Directory,
			File
		}
	}
}
