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
			string[] files = Directory.GetFiles(path);
			for (int i = 0; i < files.Length; i++)
			{
				string text = FileSystem.Normalize(files[i]);
				if (re.IsMatch(text))
				{
					result.Add(text);
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

		private string id = "StandardFS";
	}
}
