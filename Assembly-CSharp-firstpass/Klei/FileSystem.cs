using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Klei
{
	public static class FileSystem
	{
		public static void Initialize()
		{
			if (FileSystem.file_sources.Count == 0)
			{
				FileSystem.file_sources.Add(new RootDirectory());
			}
		}

		public static byte[] ReadBytes(string filename)
		{
			FileSystem.Initialize();
			foreach (IFileDirectory fileDirectory in FileSystem.file_sources)
			{
				byte[] array = fileDirectory.ReadBytes(filename);
				if (array != null)
				{
					return array;
				}
			}
			return null;
		}

		public static FileHandle FindFileHandle(string filename)
		{
			FileSystem.Initialize();
			foreach (IFileDirectory fileDirectory in FileSystem.file_sources)
			{
				if (fileDirectory.FileExists(filename))
				{
					return fileDirectory.FindFileHandle(filename);
				}
			}
			return default(FileHandle);
		}

		public static void GetFiles(Regex re, string path, ICollection<FileHandle> result)
		{
			FileSystem.Initialize();
			ListPool<string, IFileDirectory>.PooledList pooledList = ListPool<string, IFileDirectory>.Allocate();
			foreach (IFileDirectory fileDirectory in FileSystem.file_sources)
			{
				pooledList.Clear();
				fileDirectory.GetFiles(re, path, pooledList);
				foreach (string text in pooledList)
				{
					result.Add(new FileHandle
					{
						full_path = text,
						source = fileDirectory
					});
				}
			}
			pooledList.Recycle();
		}

		public static void GetFiles(Regex re, string path, ICollection<string> result)
		{
			FileSystem.Initialize();
			foreach (IFileDirectory fileDirectory in FileSystem.file_sources)
			{
				fileDirectory.GetFiles(re, path, result);
			}
		}

		public static void GetFiles(string path, string filename_glob_pattern, ICollection<string> result)
		{
			string text;
			Regex regex;
			FileSystem.GetFilesSearchParams(path, filename_glob_pattern, out text, out regex);
			FileSystem.GetFiles(regex, text, result);
		}

		public static void GetFiles(string path, string filename_glob_pattern, ICollection<FileHandle> result)
		{
			string text;
			Regex regex;
			FileSystem.GetFilesSearchParams(path, filename_glob_pattern, out text, out regex);
			FileSystem.GetFiles(regex, text, result);
		}

		public static void GetFiles(string filename, ICollection<FileHandle> result)
		{
			string text;
			Regex regex;
			FileSystem.GetFilesSearchParams(Path.GetDirectoryName(filename), Path.GetFileName(filename), out text, out regex);
			FileSystem.GetFiles(regex, text, result);
		}

		public static bool FileExists(string path)
		{
			FileSystem.Initialize();
			using (List<IFileDirectory>.Enumerator enumerator = FileSystem.file_sources.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.FileExists(path))
					{
						return true;
					}
				}
			}
			return false;
		}

		public static void ReadFiles(string filename, ICollection<byte[]> result)
		{
			FileSystem.Initialize();
			foreach (IFileDirectory fileDirectory in FileSystem.file_sources)
			{
				byte[] array = fileDirectory.ReadBytes(filename);
				if (array != null)
				{
					result.Add(array);
				}
			}
		}

		public static bool IsModdedFile(string filename)
		{
			foreach (IFileDirectory fileDirectory in FileSystem.file_sources)
			{
				if (fileDirectory.FileExists(filename))
				{
					return fileDirectory.IsModded();
				}
			}
			return false;
		}

		public static string ConvertToText(byte[] bytes)
		{
			return Encoding.UTF8.GetString(bytes);
		}

		public static string Normalize(string filename)
		{
			return filename.Replace("\\", "/");
		}

		public static string CombineAndNormalize(params string[] paths)
		{
			return FileSystem.Normalize(Path.Combine(paths));
		}

		private static void GetFilesSearchParams(string path, string filename_glob_pattern, out string normalized_path, out Regex filename_regex)
		{
			normalized_path = null;
			filename_regex = null;
			int num = path.Length - 1;
			while ((num >= 0 && path[num] == '\\') || path[num] == '/')
			{
				num--;
			}
			if (num < 0)
			{
				return;
			}
			if (num < path.Length - 1)
			{
				path = path.Substring(0, num + 1);
			}
			normalized_path = (path = FileSystem.Normalize(path));
			string text = filename_glob_pattern.Replace(".", "\\.").Replace("*", ".*");
			string text2 = Regex.Escape(path);
			text2 = text2 + "/" + text + "$";
			filename_regex = new Regex(text2);
		}

		[Conditional("UNITY_EDITOR_WIN")]
		public static void CheckForCaseSensitiveErrors(string filename)
		{
		}

		public static List<IFileDirectory> file_sources = new List<IFileDirectory>();
	}
}
