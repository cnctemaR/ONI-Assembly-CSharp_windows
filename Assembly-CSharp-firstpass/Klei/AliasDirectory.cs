using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Klei
{
	public class AliasDirectory : IFileDirectory
	{
		public string GetID()
		{
			return this.id;
		}

		public AliasDirectory(string id, string actual_location, string path_prefix, bool isModded = false)
		{
			this.id = id;
			actual_location = FileSystem.Normalize(actual_location);
			path_prefix = FileSystem.Normalize(path_prefix);
			this.isModded = isModded;
			this.root = actual_location;
			this.prefix = path_prefix;
		}

		private string GetActualPath(string filename)
		{
			if (filename.StartsWith(this.prefix))
			{
				string text = filename.Substring(this.prefix.Length);
				return FileSystem.Normalize(this.root + text);
			}
			return filename;
		}

		private string GetVirtualPath(string filename)
		{
			if (filename.StartsWith(this.root))
			{
				string text = filename.Substring(this.root.Length);
				return FileSystem.Normalize(this.prefix + text);
			}
			return filename;
		}

		public string GetRoot()
		{
			return this.root;
		}

		public byte[] ReadBytes(string src_filename)
		{
			string actualPath = this.GetActualPath(src_filename);
			if (!File.Exists(actualPath))
			{
				return null;
			}
			return File.ReadAllBytes(actualPath);
		}

		public void GetFiles(Regex re, string src_path, ICollection<string> result)
		{
			string actualPath = this.GetActualPath(src_path);
			if (!Directory.Exists(actualPath))
			{
				return;
			}
			foreach (string text in Directory.GetFiles(actualPath))
			{
				string text2 = FileSystem.Normalize(text);
				string virtualPath = this.GetVirtualPath(text2);
				if (re.IsMatch(virtualPath) && !Path.GetFileName(text).StartsWith("._"))
				{
					result.Add(virtualPath);
				}
			}
		}

		public bool FileExists(string path)
		{
			return File.Exists(this.GetActualPath(path));
		}

		public FileHandle FindFileHandle(string path)
		{
			if (this.FileExists(path))
			{
				path = this.GetVirtualPath(FileSystem.Normalize(path));
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
			return this.isModded;
		}

		private string id;

		private string root;

		private string prefix;

		private bool isModded;
	}
}
