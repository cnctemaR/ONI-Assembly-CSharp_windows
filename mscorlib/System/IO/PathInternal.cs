using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.IO
{
	internal static class PathInternal
	{
		internal static bool IsValidDriveChar(char value)
		{
			return (value >= 'A' && value <= 'Z') || (value >= 'a' && value <= 'z');
		}

		internal static bool EndsWithPeriodOrSpace(string path)
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

		internal static string EnsureExtendedPrefixOverMaxPath(string path)
		{
			if (path != null && path.Length >= 260)
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

		internal unsafe static bool IsDevice(ReadOnlySpan<char> path)
		{
			return PathInternal.IsExtended(path) || (path.Length >= 4 && PathInternal.IsDirectorySeparator((char)(*path[0])) && PathInternal.IsDirectorySeparator((char)(*path[1])) && (*path[2] == 46 || *path[2] == 63) && PathInternal.IsDirectorySeparator((char)(*path[3])));
		}

		internal unsafe static bool IsDeviceUNC(ReadOnlySpan<char> path)
		{
			return path.Length >= 8 && PathInternal.IsDevice(path) && PathInternal.IsDirectorySeparator((char)(*path[7])) && *path[4] == 85 && *path[5] == 78 && *path[6] == 67;
		}

		internal unsafe static bool IsExtended(ReadOnlySpan<char> path)
		{
			return path.Length >= 4 && *path[0] == 92 && (*path[1] == 92 || *path[1] == 63) && *path[2] == 63 && *path[3] == 92;
		}

		internal unsafe static bool HasWildCardCharacters(ReadOnlySpan<char> path)
		{
			for (int i = (PathInternal.IsDevice(path) ? "\\\\?\\".Length : 0); i < path.Length; i++)
			{
				char c = (char)(*path[i]);
				if (c <= '?' && (c == '"' || c == '<' || c == '>' || c == '*' || c == '?'))
				{
					return true;
				}
			}
			return false;
		}

		internal unsafe static int GetRootLength(ReadOnlySpan<char> path)
		{
			int length = path.Length;
			int i = 0;
			bool flag = PathInternal.IsDevice(path);
			bool flag2 = flag && PathInternal.IsDeviceUNC(path);
			if ((!flag || flag2) && length > 0 && PathInternal.IsDirectorySeparator((char)(*path[0])))
			{
				if (flag2 || (length > 1 && PathInternal.IsDirectorySeparator((char)(*path[1]))))
				{
					i = (flag2 ? 8 : 2);
					int num = 2;
					while (i < length)
					{
						if (PathInternal.IsDirectorySeparator((char)(*path[i])) && --num <= 0)
						{
							break;
						}
						i++;
					}
				}
				else
				{
					i = 1;
				}
			}
			else if (flag)
			{
				i = 4;
				while (i < length && !PathInternal.IsDirectorySeparator((char)(*path[i])))
				{
					i++;
				}
				if (i < length && i > 4 && PathInternal.IsDirectorySeparator((char)(*path[i])))
				{
					i++;
				}
			}
			else if (length >= 2 && *path[1] == 58 && PathInternal.IsValidDriveChar((char)(*path[0])))
			{
				i = 2;
				if (length > 2 && PathInternal.IsDirectorySeparator((char)(*path[2])))
				{
					i++;
				}
			}
			return i;
		}

		internal unsafe static bool IsPartiallyQualified(ReadOnlySpan<char> path)
		{
			if (path.Length < 2)
			{
				return true;
			}
			if (PathInternal.IsDirectorySeparator((char)(*path[0])))
			{
				return *path[1] != 63 && !PathInternal.IsDirectorySeparator((char)(*path[1]));
			}
			return path.Length < 3 || *path[1] != 58 || !PathInternal.IsDirectorySeparator((char)(*path[2])) || !PathInternal.IsValidDriveChar((char)(*path[0]));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool IsDirectorySeparator(char c)
		{
			return c == '\\' || c == '/';
		}

		internal static string NormalizeDirectorySeparators(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return path;
			}
			bool flag = true;
			for (int i = 0; i < path.Length; i++)
			{
				char c = path[i];
				if (PathInternal.IsDirectorySeparator(c) && (c != '\\' || (i > 0 && i + 1 < path.Length && PathInternal.IsDirectorySeparator(path[i + 1]))))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				return path;
			}
			StringBuilder stringBuilder = new StringBuilder(path.Length);
			int num = 0;
			if (PathInternal.IsDirectorySeparator(path[num]))
			{
				num++;
				stringBuilder.Append('\\');
			}
			int j = num;
			while (j < path.Length)
			{
				char c = path[j];
				if (!PathInternal.IsDirectorySeparator(c))
				{
					goto IL_00C1;
				}
				if (j + 1 >= path.Length || !PathInternal.IsDirectorySeparator(path[j + 1]))
				{
					c = '\\';
					goto IL_00C1;
				}
				IL_00C9:
				j++;
				continue;
				IL_00C1:
				stringBuilder.Append(c);
				goto IL_00C9;
			}
			return stringBuilder.ToString();
		}

		internal unsafe static bool IsEffectivelyEmpty(ReadOnlySpan<char> path)
		{
			if (path.IsEmpty)
			{
				return true;
			}
			ReadOnlySpan<char> readOnlySpan = path;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				if (*readOnlySpan[i] != 32)
				{
					return false;
				}
			}
			return true;
		}

		internal unsafe static bool EndsInDirectorySeparator(ReadOnlySpan<char> path)
		{
			return path.Length > 0 && PathInternal.IsDirectorySeparator((char)(*path[path.Length - 1]));
		}

		internal unsafe static bool StartsWithDirectorySeparator(ReadOnlySpan<char> path)
		{
			return path.Length > 0 && PathInternal.IsDirectorySeparator((char)(*path[0]));
		}

		internal static string EnsureTrailingSeparator(string path)
		{
			if (!PathInternal.EndsInDirectorySeparator(path))
			{
				return path + "\\";
			}
			return path;
		}

		internal static string TrimEndingDirectorySeparator(string path)
		{
			if (!PathInternal.EndsInDirectorySeparator(path) || PathInternal.IsRoot(path))
			{
				return path;
			}
			return path.Substring(0, path.Length - 1);
		}

		internal static ReadOnlySpan<char> TrimEndingDirectorySeparator(ReadOnlySpan<char> path)
		{
			if (!PathInternal.EndsInDirectorySeparator(path) || PathInternal.IsRoot(path))
			{
				return path;
			}
			return path.Slice(0, path.Length - 1);
		}

		internal static bool IsRoot(ReadOnlySpan<char> path)
		{
			return path.Length == PathInternal.GetRootLength(path);
		}

		internal static int GetCommonPathLength(string first, string second, bool ignoreCase)
		{
			int num = PathInternal.EqualStartingCharacterCount(first, second, ignoreCase);
			if (num == 0)
			{
				return num;
			}
			if (num == first.Length && (num == second.Length || PathInternal.IsDirectorySeparator(second[num])))
			{
				return num;
			}
			if (num == second.Length && PathInternal.IsDirectorySeparator(first[num]))
			{
				return num;
			}
			while (num > 0 && !PathInternal.IsDirectorySeparator(first[num - 1]))
			{
				num--;
			}
			return num;
		}

		internal unsafe static int EqualStartingCharacterCount(string first, string second, bool ignoreCase)
		{
			if (string.IsNullOrEmpty(first) || string.IsNullOrEmpty(second))
			{
				return 0;
			}
			int num = 0;
			fixed (string text = first)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				fixed (string text2 = second)
				{
					char* ptr2 = text2;
					if (ptr2 != null)
					{
						ptr2 += RuntimeHelpers.OffsetToStringData / 2;
					}
					char* ptr3 = ptr;
					char* ptr4 = ptr2;
					char* ptr5 = ptr3 + first.Length;
					char* ptr6 = ptr4 + second.Length;
					while (ptr3 != ptr5 && ptr4 != ptr6 && (*ptr3 == *ptr4 || (ignoreCase && char.ToUpperInvariant(*ptr3) == char.ToUpperInvariant(*ptr4))))
					{
						num++;
						ptr3++;
						ptr4++;
					}
				}
			}
			return num;
		}

		internal static bool AreRootsEqual(string first, string second, StringComparison comparisonType)
		{
			int rootLength = PathInternal.GetRootLength(first);
			int rootLength2 = PathInternal.GetRootLength(second);
			return rootLength == rootLength2 && string.Compare(first, 0, second, 0, rootLength, comparisonType) == 0;
		}

		internal unsafe static string RemoveRelativeSegments(string path, int rootLength)
		{
			bool flag = false;
			int num = rootLength;
			if (PathInternal.IsDirectorySeparator(path[num - 1]))
			{
				num--;
			}
			Span<char> span = new Span<char>(stackalloc byte[(UIntPtr)520], 260);
			ValueStringBuilder valueStringBuilder = new ValueStringBuilder(span);
			if (num > 0)
			{
				valueStringBuilder.Append(path.AsSpan(0, num));
			}
			int i = num;
			while (i < path.Length)
			{
				char c = path[i];
				if (!PathInternal.IsDirectorySeparator(c) || i + 1 >= path.Length)
				{
					goto IL_0165;
				}
				if (!PathInternal.IsDirectorySeparator(path[i + 1]))
				{
					if ((i + 2 == path.Length || PathInternal.IsDirectorySeparator(path[i + 2])) && path[i + 1] == '.')
					{
						i++;
					}
					else
					{
						if (i + 2 >= path.Length || (i + 3 != path.Length && !PathInternal.IsDirectorySeparator(path[i + 3])) || path[i + 1] != '.' || path[i + 2] != '.')
						{
							goto IL_0165;
						}
						int j;
						for (j = valueStringBuilder.Length - 1; j >= num; j--)
						{
							if (PathInternal.IsDirectorySeparator(*valueStringBuilder[j]))
							{
								valueStringBuilder.Length = ((i + 3 >= path.Length && j == num) ? (j + 1) : j);
								break;
							}
						}
						if (j < num)
						{
							valueStringBuilder.Length = num;
						}
						i += 2;
					}
				}
				IL_0180:
				i++;
				continue;
				IL_0165:
				if (c != '\\' && c == '/')
				{
					c = '\\';
					flag = true;
				}
				valueStringBuilder.Append(c);
				goto IL_0180;
			}
			if (!flag && valueStringBuilder.Length == path.Length)
			{
				valueStringBuilder.Dispose();
				return path;
			}
			if (valueStringBuilder.Length >= rootLength)
			{
				return valueStringBuilder.ToString();
			}
			return path.Substring(0, rootLength);
		}

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

		public static bool IsPartiallyQualified(string path)
		{
			return false;
		}

		public static bool HasIllegalCharacters(string path, bool checkAdditional)
		{
			return path.IndexOfAny(Path.InvalidPathChars) != -1;
		}

		internal const char DirectorySeparatorChar = '\\';

		internal const char AltDirectorySeparatorChar = '/';

		internal const char VolumeSeparatorChar = ':';

		internal const char PathSeparator = ';';

		internal const string DirectorySeparatorCharAsString = "\\";

		internal const string ExtendedPathPrefix = "\\\\?\\";

		internal const string UncPathPrefix = "\\\\";

		internal const string UncExtendedPrefixToInsert = "?\\UNC\\";

		internal const string UncExtendedPathPrefix = "\\\\?\\UNC\\";

		internal const string DevicePathPrefix = "\\\\.\\";

		internal const string ParentDirectoryPrefix = "..\\";

		internal const int MaxShortPath = 260;

		internal const int MaxShortDirectoryPath = 248;

		internal const int DevicePrefixLength = 4;

		internal const int UncPrefixLength = 2;

		internal const int UncExtendedPrefixLength = 8;

		private static readonly bool s_isCaseSensitive = PathInternal.GetIsCaseSensitive();
	}
}
