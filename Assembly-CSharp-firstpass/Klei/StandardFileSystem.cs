using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Klei
{
	public class StandardFileSystem : IFileSystem
	{
		public string GetID()
		{
			return this.id;
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
				string text2 = FSUtil.Normalize(text);
				if (re.IsMatch(text2))
				{
					result.Add(text2);
				}
			}
		}

		private string id = "StandardFS";
	}
}
