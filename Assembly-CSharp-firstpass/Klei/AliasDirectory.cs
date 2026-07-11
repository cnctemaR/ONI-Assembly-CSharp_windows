using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Klei
{
	public class AliasDirectory : IFileDirectory
	{
		public AliasDirectory(string id, string actual_location, string path_prefix)
		{
			this.id = id;
			actual_location = FileSystem.Normalize(actual_location);
			path_prefix = FileSystem.Normalize(path_prefix);
			this.root = actual_location;
			this.prefix = path_prefix;
		}

		public string GetID()
		{
			return this.id;
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
			string[] files = Directory.GetFiles(actualPath);
			foreach (string text in files)
			{
				string text2 = FileSystem.Normalize(text);
				string virtualPath = this.GetVirtualPath(text2);
				if (re.IsMatch(virtualPath))
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

		private string id;

		private string root;

		private string prefix;
	}
}
