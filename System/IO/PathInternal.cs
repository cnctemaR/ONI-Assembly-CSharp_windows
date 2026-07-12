using System;
using System.Runtime.CompilerServices;

namespace System.IO
{
	internal static class PathInternal
	{
		internal static StringComparison StringComparison
		{
			get
			{
				if (!PathInternal.s_isCaseSensitive)
				{
					return StringComparison.OrdinalIgnoreCase;
				}
				return StringComparison.Ordinal;
			}
		}

		internal static bool IsCaseSensitive
		{
			get
			{
				return PathInternal.s_isCaseSensitive;
			}
		}

		private static bool GetIsCaseSensitive()
		{
			bool flag;
			try
			{
				string text = Path.Combine(Path.GetTempPath(), "CASESENSITIVETEST" + Guid.NewGuid().ToString("N"));
				using (new FileStream(text, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose))
				{
					flag = !File.Exists(text.ToLowerInvariant());
				}
			}
			catch (Exception)
			{
				flag = false;
			}
			return flag;
		}

		internal static bool IsValidDriveChar(char value)
		{
			return (value >= 'A' && value <= 'Z') || (value >= 'a' && value <= 'z');
		}

		private static bool EndsWithPeriodOrSpace(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return false;
			}
			char c = path[path.Length - 1];
			return c == ' ' || c == '.';
		}

		internal static string EnsureExtendedPrefixIfNeeded(string path)
		{
			if (path != null && (path.Length >= 260 || PathInternal.EndsWithPeriodOrSpace(path)))
			{
				return PathInternal.EnsureExtendedPrefix(path);
			}
			return path;
		}

		internal static string EnsureExtendedPrefix(string path)
		{
			if (PathInternal.IsPartiallyQualified(path) || PathInternal.IsDevice(path))
			{
				return path;
			}
			if (path.StartsWith("\\\\", StringComparison.OrdinalIgnoreCase))
			{
				return path.Insert(2, "?\\UNC\\");
			}
			return "\\\\?\\" + path;
		}

		internal static bool IsDevice(string path)
		{
			return PathInternal.IsExtended(path) || (path.Length >= 4 && PathInternal.IsDirectorySeparator(path[0]) && PathInternal.IsDirectorySeparator(path[1]) && (path[2] == '.' || path[2] == '?') && PathInternal.IsDirectorySeparator(path[3]));
		}

		internal static bool IsExtended(string path)
		{
			return path.Length >= 4 && path[0] == '\\' && (path[1] == '\\' || path[1] == '?') && path[2] == '?' && path[3] == '\\';
		}

		internal unsafe static int GetRootLength(ReadOnlySpan<char> path)
		{
			int i = 0;
			int num = 2;
			int num2 = 2;
			bool flag = path.StartsWith<char>("\\\\?\\");
			bool flag2 = path.StartsWith<char>("\\\\?\\UNC\\");
			if (flag)
			{
				if (flag2)
				{
					num2 = "\\\\?\\UNC\\".Length;
				}
				else
				{
					num += "\\\\?\\".Length;
				}
			}
			if ((!flag || flag2) && path.Length > 0 && PathInternal.IsDirectorySeparator((char)(*path[0])))
			{
				i = 1;
				if (flag2 || (path.Length > 1 && PathInternal.IsDirectorySeparator((char)(*path[1]))))
				{
					i = num2;
					int num3 = 2;
					while (i < path.Length)
					{
						if (PathInternal.IsDirectorySeparator((char)(*path[i])) && --num3 <= 0)
						{
							break;
						}
						i++;
					}
				}
			}
			else if (path.Length >= num && *path[num - 1] == (ushort)Path.VolumeSeparatorChar)
			{
				i = num;
				if (path.Length >= num + 1 && PathInternal.IsDirectorySeparator((char)(*path[num])))
				{
					i++;
				}
			}
			return i;
		}

		internal static bool IsPartiallyQualified(string path)
		{
			if (path.Length < 2)
			{
				return true;
			}
			if (PathInternal.IsDirectorySeparator(path[0]))
			{
				return path[1] != '?' && !PathInternal.IsDirectorySeparator(path[1]);
			}
			return path.Length < 3 || path[1] != Path.VolumeSeparatorChar || !PathInternal.IsDirectorySeparator(path[2]) || !PathInternal.IsValidDriveChar(path[0]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool IsDirectorySeparator(char c)
		{
			return c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar;
		}

		private static readonly bool s_isCaseSensitive = PathInternal.GetIsCaseSensitive();

		internal const string ExtendedDevicePathPrefix = "\\\\?\\";

		internal const string UncPathPrefix = "\\\\";

		internal const string UncDevicePrefixToInsert = "?\\UNC\\";

		internal const string UncExtendedPathPrefix = "\\\\?\\UNC\\";

		internal const string DevicePathPrefix = "\\\\.\\";

		internal const int MaxShortPath = 260;

		internal const int DevicePrefixLength = 4;
	}
}
