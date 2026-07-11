using System;
using System.Collections.Generic;
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

		public bool FileExists(string fullpath)
		{
			foreach (IFileSystem fileSystem in this.filesystems)
			{
				if (fileSystem.FileExists(fullpath))
				{
					return true;
				}
			}
			return false;
		}

		private string id = "LayeredFS";

		private List<IFileSystem> filesystems = new List<IFileSystem>();

		public static LayeredFileSystem instance;
	}
}
