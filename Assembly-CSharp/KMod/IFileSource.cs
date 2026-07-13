using System;
using System.Collections.Generic;
using Klei;

namespace KMod
{
	public interface IFileSource
	{
		string GetRoot();

		bool Exists();

		bool Exists(string relative_path);

		void GetTopLevelItems(List<FileSystemItem> file_system_items, string relative_root = "");

		IFileDirectory GetFileSystem();

		bool TryCopyTo(string path, List<string> extensions = null);

		void CopyTo(string path, List<string> extensions = null)
		{
			this.TryCopyTo(path, extensions);
		}

		string Read(string relative_path);

		void Dispose();
	}
}
