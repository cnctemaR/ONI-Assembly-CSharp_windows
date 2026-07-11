using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Klei
{
	public class LayeredFileSystem : IFileSystem
	{
		private LayeredFileSystem()
		{
		}

		public string GetID()
		{
			return this.id;
		}

		public IList<IFileSystem> GetFileSystems()
		{
			return this.filesystems;
		}

		public static void CreateInstance()
		{
			LayeredFileSystem.instance = new LayeredFileSystem();
		}

		public static void DestroyInstance()
		{
			LayeredFileSystem.instance = null;
		}

		public void AddFileSystem(IFileSystem fs)
		{
			this.filesystems.Add(fs);
		}

		public void RemoveFileSystem(IFileSystem fs)
		{
			this.filesystems.Remove(fs);
		}

		public byte[] ReadBytes(string filename)
		{
			byte[] array = null;
			for (int i = this.filesystems.Count - 1; i >= 0; i--)
			{
				array = this.filesystems[i].ReadBytes(filename);
				if (array != null)
				{
					break;
				}
			}
			return array;
		}

		public string ReadText(string filename)
		{
			byte[] array = this.ReadBytes(filename);
			return Encoding.UTF8.GetString(array);
		}

		public void GetFiles(string path, string filename_glob_pattern, ICollection<string> result)
		{
			FSUtil.GetFiles(this, path, filename_glob_pattern, result);
		}

		public void GetFiles(Regex re, string path, ICollection<string> result)
		{
			foreach (IFileSystem fileSystem in this.filesystems)
			{
				fileSystem.GetFiles(re, path, result);
			}
		}

		public bool Exists(string fullpath)
		{
			string fileName = Path.GetFileName(fullpath);
			string directoryName = Path.GetDirectoryName(fullpath);
			string text;
			Regex regex;
			FSUtil.GetFilesSearchParams(directoryName, fileName, out text, out regex);
			List<string> list = new List<string>();
			foreach (IFileSystem fileSystem in this.filesystems)
			{
				fileSystem.GetFiles(regex, text, list);
				if (list.Count > 0)
				{
					break;
				}
			}
			this.GetFiles(directoryName, fileName, list);
			return list.Count > 0;
		}

		private string id = "LayeredFS";

		private List<IFileSystem> filesystems = new List<IFileSystem>();

		public static LayeredFileSystem instance;
	}
}
