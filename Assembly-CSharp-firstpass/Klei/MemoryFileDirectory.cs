using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Klei
{
	public class MemoryFileDirectory : IFileDirectory
	{
		public string GetID()
		{
			return this.id;
		}

		public MemoryFileDirectory(string id, string mount_point = "")
		{
			this.id = id;
			this.mountPoint = FileSystem.Normalize(mount_point);
		}

		public string GetRoot()
		{
			return this.mountPoint;
		}

		public byte[] ReadBytes(string filename)
		{
			byte[] array = null;
			this.dataMap.TryGetValue(filename, out array);
			return array;
		}

		private string GetFullFilename(string filename)
		{
			string text = FileSystem.Normalize(filename);
			return Path.Combine(this.mountPoint, text);
		}

		public void Map(string filename, byte[] data)
		{
			string fullFilename = this.GetFullFilename(filename);
			if (this.dataMap.ContainsKey(fullFilename))
			{
				throw new ArgumentException(string.Format("MemoryFileSystem: '{0}' is already mapped.", Array.Empty<object>()));
			}
			this.dataMap[fullFilename] = data;
		}

		public void Unmap(string filename)
		{
			string fullFilename = this.GetFullFilename(filename);
			this.dataMap.Remove(fullFilename);
		}

		public void Clear()
		{
			this.dataMap.Clear();
		}

		public void GetFiles(Regex re, string path, ICollection<string> result)
		{
			foreach (string text in this.dataMap.Keys)
			{
				if (re.IsMatch(text) && !Path.GetFileName(text).StartsWith("._"))
				{
					result.Add(text);
				}
			}
		}

		public bool FileExists(string path)
		{
			return this.dataMap.ContainsKey(path);
		}

		public FileHandle FindFileHandle(string path)
		{
			if (this.FileExists(path))
			{
				return new FileHandle
				{
					full_path = path,
					source = this
				};
			}
			return default(FileHandle);
		}

		public bool IsModded()
		{
			return false;
		}

		private string id;

		private string mountPoint;

		private Dictionary<string, byte[]> dataMap = new Dictionary<string, byte[]>();
	}
}
