using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Klei
{
	public class RootDirectory : IFileDirectory
	{
		public string GetID()
		{
			return this.id;
		}

		public string GetRoot()
		{
			return "";
		}

		public byte[] ReadBytes(string filename)
		{
			return File.ReadAllBytes(filename);
		}

		public string ReadText(string filename)
		{
			byte[] array = this.ReadBytes(filename);
			return Encoding.UTF8.GetString(array);
		}

		public void GetFiles(Regex re, string path, ICollection<string> result)
		{
			if (!Directory.Exists(path))
			{
				return;
			}
			foreach (string text in Directory.GetFiles(path))
			{
				string text2 = FileSystem.Normalize(text);
				if (re.IsMatch(text2) && !Path.GetFileName(text).StartsWith("._"))
				{
					result.Add(text2);
				}
			}
		}

		public bool FileExists(string path)
		{
			return File.Exists(path);
		}

		public FileHandle FindFileHandle(string path)
		{
			if (this.FileExists(path))
			{
				return new FileHandle
				{
					full_path = FileSystem.Normalize(path),
					source = this
				};
			}
			return default(FileHandle);
		}

		public bool IsModded()
		{
			return false;
		}

		private string id = "StandardFS";
	}
}
