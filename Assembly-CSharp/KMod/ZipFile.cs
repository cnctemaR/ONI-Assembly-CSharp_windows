using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ionic.Zip;
using Klei;
using UnityEngine;

namespace KMod
{
	internal struct ZipFile : IFileSource
	{
		public ZipFile(string filename)
		{
			this.filename = filename;
			this.zipfile = ZipFile.Read(filename);
			this.file_system = new ZipFileSystem(this.zipfile.Name, this.zipfile, Application.streamingAssetsPath);
		}

		public string GetRoot()
		{
			return this.filename;
		}

		public bool Exists()
		{
			return File.Exists(this.GetRoot());
		}

		public void GetTopLevelItems(List<FileSystemItem> file_system_items)
		{
			HashSetPool<string, ZipFile>.PooledHashSet pooledHashSet = HashSetPool<string, ZipFile>.Allocate();
			foreach (ZipEntry zipEntry in this.zipfile)
			{
				string[] array = FSUtil.Normalize(zipEntry.FileName).Split(new char[] { '/' });
				string text = array[0];
				if (pooledHashSet.Add(text))
				{
					file_system_items.Add(new FileSystemItem
					{
						name = text,
						type = ((1 >= array.Length) ? FileSystemItem.ItemType.File : FileSystemItem.ItemType.Directory)
					});
				}
			}
			pooledHashSet.Recycle();
		}

		public IFileSystem GetFileSystem()
		{
			return this.file_system;
		}

		public void CopyTo(string path, List<string> extensions = null)
		{
			foreach (ZipEntry zipEntry in this.zipfile.Entries)
			{
				bool flag = extensions == null || extensions.Count == 0;
				if (extensions != null)
				{
					foreach (string text in extensions)
					{
						if (zipEntry.FileName.ToLower().EndsWith(text))
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					string text2 = FSUtil.Normalize(Path.Combine(path, zipEntry.FileName));
					string directoryName = Path.GetDirectoryName(text2);
					if (string.IsNullOrEmpty(directoryName) || FileUtil.CreateDirectory(directoryName))
					{
						using (MemoryStream memoryStream = new MemoryStream((int)zipEntry.UncompressedSize))
						{
							zipEntry.Extract(memoryStream);
							using (FileStream fileStream = FileUtil.Create(text2))
							{
								fileStream.Write(memoryStream.GetBuffer(), 0, memoryStream.GetBuffer().Length);
							}
						}
					}
				}
			}
		}

		public string Read(string relative_path)
		{
			ICollection<ZipEntry> collection = this.zipfile.SelectEntries(relative_path);
			if (collection.Count == 0)
			{
				return string.Empty;
			}
			foreach (ZipEntry zipEntry in collection)
			{
				using (MemoryStream memoryStream = new MemoryStream((int)zipEntry.UncompressedSize))
				{
					zipEntry.Extract(memoryStream);
					return Encoding.UTF8.GetString(memoryStream.GetBuffer());
				}
			}
			return string.Empty;
		}

		private string filename;

		private ZipFile zipfile;

		private ZipFileSystem file_system;
	}
}
