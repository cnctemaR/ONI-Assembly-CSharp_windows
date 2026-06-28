using System;
using System.Text;
using Mono.Unix.Native;

namespace Mono.Unix
{
	public sealed class UnixPath
	{
		private UnixPath()
		{
		}

		public static char[] GetInvalidPathChars()
		{
			return (char[])UnixPath._InvalidPathChars.Clone();
		}

		public static string Combine(string path1, params string[] paths)
		{
			if (path1 == null)
			{
				throw new ArgumentNullException("path1");
			}
			if (paths == null)
			{
				throw new ArgumentNullException("paths");
			}
			if (path1.IndexOfAny(UnixPath._InvalidPathChars) != -1)
			{
				throw new ArgumentException("Illegal characters in path", "path1");
			}
			int num = path1.Length;
			int num2 = -1;
			for (int i = 0; i < paths.Length; i++)
			{
				if (paths[i] == null)
				{
					throw new ArgumentNullException("paths[" + i + "]");
				}
				if (paths[i].IndexOfAny(UnixPath._InvalidPathChars) != -1)
				{
					throw new ArgumentException("Illegal characters in path", "paths[" + i + "]");
				}
				if (UnixPath.IsPathRooted(paths[i]))
				{
					num = 0;
					num2 = i;
				}
				num += paths[i].Length + 1;
			}
			StringBuilder stringBuilder = new StringBuilder(num);
			if (num2 == -1)
			{
				stringBuilder.Append(path1);
				num2 = 0;
			}
			for (int j = num2; j < paths.Length; j++)
			{
				UnixPath.Combine(stringBuilder, paths[j]);
			}
			return stringBuilder.ToString();
		}

		private static void Combine(StringBuilder path, string part)
		{
			if (path.Length > 0 && part.Length > 0)
			{
				char c = path[path.Length - 1];
				if (c != UnixPath.DirectorySeparatorChar && c != UnixPath.AltDirectorySeparatorChar && c != UnixPath.VolumeSeparatorChar)
				{
					path.Append(UnixPath.DirectorySeparatorChar);
				}
			}
			path.Append(part);
		}

		public static string GetDirectoryName(string path)
		{
			UnixPath.CheckPath(path);
			int num = path.LastIndexOf(UnixPath.DirectorySeparatorChar);
			if (num > 0)
			{
				return path.Substring(0, num);
			}
			if (num == 0)
			{
				return "/";
			}
			return string.Empty;
		}

		public static string GetFileName(string path)
		{
			if (path == null || path.Length == 0)
			{
				return path;
			}
			int num = path.LastIndexOf(UnixPath.DirectorySeparatorChar);
			if (num >= 0)
			{
				return path.Substring(num + 1);
			}
			return path;
		}

		public static string GetFullPath(string path)
		{
			path = UnixPath._GetFullPath(path);
			return UnixPath.GetCanonicalPath(path);
		}

		private static string _GetFullPath(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (!UnixPath.IsPathRooted(path))
			{
				path = UnixDirectoryInfo.GetCurrentDirectory() + UnixPath.DirectorySeparatorChar + path;
			}
			return path;
		}

		public static string GetCanonicalPath(string path)
		{
			string[] array;
			int num;
			UnixPath.GetPathComponents(path, out array, out num);
			string text = string.Join("/", array, 0, num);
			return (!UnixPath.IsPathRooted(path)) ? text : ("/" + text);
		}

		private static void GetPathComponents(string path, out string[] components, out int lastIndex)
		{
			string[] array = path.Split(new char[] { UnixPath.DirectorySeparatorChar });
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (!(array[i] == ".") && !(array[i] == string.Empty))
				{
					if (array[i] == "..")
					{
						if (num != 0)
						{
							num--;
						}
						else
						{
							num++;
						}
					}
					else
					{
						array[num++] = array[i];
					}
				}
			}
			components = array;
			lastIndex = num;
		}

		public static string GetPathRoot(string path)
		{
			if (path == null)
			{
				return null;
			}
			if (!UnixPath.IsPathRooted(path))
			{
				return string.Empty;
			}
			return "/";
		}

		public static string GetCompleteRealPath(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			string[] array;
			int num;
			UnixPath.GetPathComponents(path, out array, out num);
			StringBuilder stringBuilder = new StringBuilder();
			if (array.Length > 0)
			{
				string text = ((!UnixPath.IsPathRooted(path)) ? string.Empty : "/");
				text += array[0];
				stringBuilder.Append(UnixPath.GetRealPath(text));
			}
			for (int i = 1; i < num; i++)
			{
				stringBuilder.Append("/").Append(array[i]);
				string realPath = UnixPath.GetRealPath(stringBuilder.ToString());
				stringBuilder.Remove(0, stringBuilder.Length);
				stringBuilder.Append(realPath);
			}
			return stringBuilder.ToString();
		}

		public static string GetRealPath(string path)
		{
			for (;;)
			{
				string text = UnixPath.ReadSymbolicLink(path);
				if (text == null)
				{
					break;
				}
				if (UnixPath.IsPathRooted(text))
				{
					path = text;
				}
				else
				{
					path = UnixPath.GetDirectoryName(path) + UnixPath.DirectorySeparatorChar + text;
					path = UnixPath.GetCanonicalPath(path);
				}
			}
			return path;
		}

		internal static string ReadSymbolicLink(string path)
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			int num;
			for (;;)
			{
				num = Syscall.readlink(path, stringBuilder);
				if (num < 0)
				{
					Errno lastError;
					Errno errno = (lastError = Stdlib.GetLastError());
					if (lastError == Errno.EINVAL)
					{
						break;
					}
					UnixMarshal.ThrowExceptionForError(errno);
				}
				else
				{
					if (num != stringBuilder.Capacity)
					{
						goto IL_0060;
					}
					stringBuilder.Capacity *= 2;
				}
			}
			return null;
			IL_0060:
			return stringBuilder.ToString(0, num);
		}

		private static string ReadSymbolicLink(string path, out Errno errno)
		{
			errno = (Errno)0;
			StringBuilder stringBuilder = new StringBuilder(256);
			int num;
			for (;;)
			{
				num = Syscall.readlink(path, stringBuilder);
				if (num < 0)
				{
					break;
				}
				if (num != stringBuilder.Capacity)
				{
					goto IL_0045;
				}
				stringBuilder.Capacity *= 2;
			}
			errno = Stdlib.GetLastError();
			return null;
			IL_0045:
			return stringBuilder.ToString(0, num);
		}

		public static string TryReadLink(string path)
		{
			Errno errno;
			return UnixPath.ReadSymbolicLink(path, out errno);
		}

		public static string ReadLink(string path)
		{
			Errno errno;
			path = UnixPath.ReadSymbolicLink(path, out errno);
			if (errno != (Errno)0)
			{
				UnixMarshal.ThrowExceptionForError(errno);
			}
			return path;
		}

		public static bool IsPathRooted(string path)
		{
			return path != null && path.Length != 0 && path[0] == UnixPath.DirectorySeparatorChar;
		}

		internal static void CheckPath(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException();
			}
			if (path.Length == 0)
			{
				throw new ArgumentException("Path cannot contain a zero-length string", "path");
			}
			if (path.IndexOfAny(UnixPath._InvalidPathChars) != -1)
			{
				throw new ArgumentException("Invalid characters in path.", "path");
			}
		}

		public static readonly char DirectorySeparatorChar = '/';

		public static readonly char AltDirectorySeparatorChar = '/';

		public static readonly char PathSeparator = ':';

		public static readonly char VolumeSeparatorChar = '/';

		private static readonly char[] _InvalidPathChars = new char[0];
	}
}
