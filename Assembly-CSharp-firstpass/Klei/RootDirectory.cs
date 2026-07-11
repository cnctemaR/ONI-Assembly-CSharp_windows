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
			return string.Empty;
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
			foreach (string text in files)
			{
				string text2 = FileSystem.Normalize(text);
				if (re.IsMatch(text2))
				{
					result.Add(text2);
				}
			}
		}

		public bool FileExists(string path)
		{
			return File.Exists(path);
		}

		private string id = "StandardFS";
	}
}
