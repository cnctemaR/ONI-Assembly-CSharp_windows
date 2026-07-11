using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Klei
{
	public static class FSUtil
	{
		public static string Normalize(string filename)
		{
			return filename.Replace("\\", "/");
		}

		public static void GetFilesSearchParams(string path, string filename_glob_pattern, out string normalized_path, out Regex filename_regex)
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
				path = path.Substring(0, num);
			}
			normalized_path = (path = FSUtil.Normalize(path));
			string text = filename_glob_pattern.Replace(".", "\\.").Replace("*", ".*");
			string text2 = path.Replace("\\", "\\\\").Replace("/", "\\/").Replace("(", "\\(")
				.Replace(")", "\\)")
				.Replace("[", "\\[")
				.Replace("]", "\\]")
				.Replace(".", "\\.")
				.Replace("+", "\\+");
			text2 = text2 + "/" + text + "$";
			filename_regex = new Regex(text2);
		}

		public static void GetFiles(IFileSystem fs, string path, string filename_glob_pattern, ICollection<string> result)
		{
			string text;
			Regex regex;
			FSUtil.GetFilesSearchParams(path, filename_glob_pattern, out text, out regex);
			fs.GetFiles(regex, text, result);
		}
	}
}
