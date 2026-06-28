using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;

namespace System.IO
{
	[ComVisible(true)]
	public static class Path
	{
		public static string ChangeExtension(string path, string extension)
		{
			if (path == null)
			{
				return null;
			}
			if (path.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("Illegal characters in path.");
			}
			int num = Path.findExtension(path);
			if (extension == null)
			{
				return (num >= 0) ? path.Substring(0, num) : path;
			}
			if (extension.Length == 0)
			{
				return (num >= 0) ? path.Substring(0, num + 1) : (path + '.');
			}
			if (path.Length != 0)
			{
				if (extension.Length > 0 && extension[0] != '.')
				{
					extension = "." + extension;
				}
			}
			else
			{
				extension = string.Empty;
			}
			if (num < 0)
			{
				return path + extension;
			}
			if (num > 0)
			{
				string text = path.Substring(0, num);
				return text + extension;
			}
			return extension;
		}

		public static string Combine(string path1, string path2)
		{
			if (path1 == null)
			{
				throw new ArgumentNullException("path1");
			}
			if (path2 == null)
			{
				throw new ArgumentNullException("path2");
			}
			if (path1.Length == 0)
			{
				return path2;
			}
			if (path2.Length == 0)
			{
				return path1;
			}
			if (path1.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("Illegal characters in path.");
			}
			if (path2.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("Illegal characters in path.");
			}
			if (Path.IsPathRooted(path2))
			{
				return path2;
			}
			char c = path1[path1.Length - 1];
			if (c != Path.DirectorySeparatorChar && c != Path.AltDirectorySeparatorChar && c != Path.VolumeSeparatorChar)
			{
				return path1 + Path.DirectorySeparatorStr + path2;
			}
			return path1 + path2;
		}

		internal static string CleanPath(string s)
		{
			int length = s.Length;
			int num = 0;
			int num2 = 0;
			char c = s[0];
			if (length > 2 && c == '\\' && s[1] == '\\')
			{
				num2 = 2;
			}
			if (length == 1 && (c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar))
			{
				return s;
			}
			for (int i = num2; i < length; i++)
			{
				char c2 = s[i];
				if (c2 == Path.DirectorySeparatorChar || c2 == Path.AltDirectorySeparatorChar)
				{
					if (i + 1 == length)
					{
						num++;
					}
					else
					{
						c2 = s[i + 1];
						if (c2 == Path.DirectorySeparatorChar || c2 == Path.AltDirectorySeparatorChar)
						{
							num++;
						}
					}
				}
			}
			if (num == 0)
			{
				return s;
			}
			char[] array = new char[length - num];
			if (num2 != 0)
			{
				array[0] = '\\';
				array[1] = '\\';
			}
			int j = num2;
			int num3 = num2;
			while (j < length && num3 < array.Length)
			{
				char c3 = s[j];
				if (c3 != Path.DirectorySeparatorChar && c3 != Path.AltDirectorySeparatorChar)
				{
					array[num3++] = c3;
				}
				else if (num3 + 1 != array.Length)
				{
					array[num3++] = Path.DirectorySeparatorChar;
					while (j < length - 1)
					{
						c3 = s[j + 1];
						if (c3 != Path.DirectorySeparatorChar && c3 != Path.AltDirectorySeparatorChar)
						{
							break;
						}
						j++;
					}
				}
				j++;
			}
			return new string(array);
		}

		public static string GetDirectoryName(string path)
		{
			if (path == string.Empty)
			{
				throw new ArgumentException("Invalid path");
			}
			if (path == null || Path.GetPathRoot(path) == path)
			{
				return null;
			}
			if (path.Trim().Length == 0)
			{
				throw new ArgumentException("Argument string consists of whitespace characters only.");
			}
			if (path.IndexOfAny(Path.InvalidPathChars) > -1)
			{
				throw new ArgumentException("Path contains invalid characters");
			}
			int num = path.LastIndexOfAny(Path.PathSeparatorChars);
			if (num == 0)
			{
				num++;
			}
			if (num <= 0)
			{
				return string.Empty;
			}
			string text = path.Substring(0, num);
			int length = text.Length;
			if (length >= 2 && Path.DirectorySeparatorChar == '\\' && text[length - 1] == Path.VolumeSeparatorChar)
			{
				return text + Path.DirectorySeparatorChar;
			}
			return Path.CleanPath(text);
		}

		public static string GetExtension(string path)
		{
			if (path == null)
			{
				return null;
			}
			if (path.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("Illegal characters in path.");
			}
			int num = Path.findExtension(path);
			if (num > -1 && num < path.Length - 1)
			{
				return path.Substring(num);
			}
			return string.Empty;
		}

		public static string GetFileName(string path)
		{
			if (path == null || path.Length == 0)
			{
				return path;
			}
			if (path.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("Illegal characters in path.");
			}
			int num = path.LastIndexOfAny(Path.PathSeparatorChars);
			if (num >= 0)
			{
				return path.Substring(num + 1);
			}
			return path;
		}

		public static string GetFileNameWithoutExtension(string path)
		{
			return Path.ChangeExtension(Path.GetFileName(path), null);
		}

		public static string GetFullPath(string path)
		{
			string text = Path.InsecureGetFullPath(path);
			if (SecurityManager.SecurityEnabled)
			{
				new FileIOPermission(FileIOPermissionAccess.PathDiscovery, text).Demand();
			}
			return text;
		}

		internal static string WindowsDriveAdjustment(string path)
		{
			if (path.Length < 2)
			{
				return path;
			}
			if (path[1] != ':' || !char.IsLetter(path[0]))
			{
				return path;
			}
			string currentDirectory = Directory.GetCurrentDirectory();
			if (path.Length == 2)
			{
				if (currentDirectory[0] == path[0])
				{
					path = currentDirectory;
				}
				else
				{
					path += '\\';
				}
			}
			else if (path[2] != Path.DirectorySeparatorChar && path[2] != Path.AltDirectorySeparatorChar)
			{
				if (currentDirectory[0] == path[0])
				{
					path = Path.Combine(currentDirectory, path.Substring(2, path.Length - 2));
				}
				else
				{
					path = path.Substring(0, 2) + Path.DirectorySeparatorStr + path.Substring(2, path.Length - 2);
				}
			}
			return path;
		}

		internal static string InsecureGetFullPath(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Trim().Length == 0)
			{
				string text = Locale.GetText("The specified path is not of a legal form (empty).");
				throw new ArgumentException(text);
			}
			if (Environment.IsRunningOnWindows)
			{
				path = Path.WindowsDriveAdjustment(path);
			}
			char c = path[path.Length - 1];
			if (path.Length >= 2 && Path.IsDsc(path[0]) && Path.IsDsc(path[1]))
			{
				if (path.Length == 2 || path.IndexOf(path[0], 2) < 0)
				{
					throw new ArgumentException("UNC paths should be of the form \\\\server\\share.");
				}
				if (path[0] != Path.DirectorySeparatorChar)
				{
					path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
				}
				path = Path.CanonicalizePath(path);
			}
			else
			{
				if (!Path.IsPathRooted(path))
				{
					path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorStr + path;
				}
				else if (Path.DirectorySeparatorChar == '\\' && path.Length >= 2 && Path.IsDsc(path[0]) && !Path.IsDsc(path[1]))
				{
					string currentDirectory = Directory.GetCurrentDirectory();
					if (currentDirectory[1] == Path.VolumeSeparatorChar)
					{
						path = currentDirectory.Substring(0, 2) + path;
					}
					else
					{
						path = currentDirectory.Substring(0, currentDirectory.IndexOf('\\', currentDirectory.IndexOf("\\\\") + 1));
					}
				}
				path = Path.CanonicalizePath(path);
			}
			if (Path.IsDsc(c) && path[path.Length - 1] != Path.DirectorySeparatorChar)
			{
				path += Path.DirectorySeparatorChar;
			}
			return path;
		}

		private static bool IsDsc(char c)
		{
			return c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar;
		}

		public static string GetPathRoot(string path)
		{
			if (path == null)
			{
				return null;
			}
			if (path.Trim().Length == 0)
			{
				throw new ArgumentException("The specified path is not of a legal form.");
			}
			if (!Path.IsPathRooted(path))
			{
				return string.Empty;
			}
			if (Path.DirectorySeparatorChar == '/')
			{
				return (!Path.IsDsc(path[0])) ? string.Empty : Path.DirectorySeparatorStr;
			}
			int num = 2;
			if (path.Length == 1 && Path.IsDsc(path[0]))
			{
				return Path.DirectorySeparatorStr;
			}
			if (path.Length < 2)
			{
				return string.Empty;
			}
			if (Path.IsDsc(path[0]) && Path.IsDsc(path[1]))
			{
				while (num < path.Length && !Path.IsDsc(path[num]))
				{
					num++;
				}
				if (num < path.Length)
				{
					num++;
					while (num < path.Length && !Path.IsDsc(path[num]))
					{
						num++;
					}
				}
				return Path.DirectorySeparatorStr + Path.DirectorySeparatorStr + path.Substring(2, num - 2).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			}
			if (Path.IsDsc(path[0]))
			{
				return Path.DirectorySeparatorStr;
			}
			if (path[1] == Path.VolumeSeparatorChar)
			{
				if (path.Length >= 3 && Path.IsDsc(path[2]))
				{
					num++;
				}
				return path.Substring(0, num);
			}
			return Directory.GetCurrentDirectory().Substring(0, 2);
		}

		[PermissionSet(SecurityAction.Assert, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Unrestricted=\"true\"/>\n</PermissionSet>\n")]
		public static string GetTempFileName()
		{
			FileStream fileStream = null;
			Random random = new Random();
			string text;
			do
			{
				int num = random.Next();
				num++;
				text = Path.Combine(Path.GetTempPath(), "tmp" + num.ToString("x") + ".tmp");
				try
				{
					fileStream = new FileStream(text, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read, 8192, false, (FileOptions)1);
				}
				catch (SecurityException)
				{
					throw;
				}
				catch (UnauthorizedAccessException)
				{
					throw;
				}
				catch (DirectoryNotFoundException)
				{
					throw;
				}
				catch
				{
				}
			}
			while (fileStream == null);
			fileStream.Close();
			return text;
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Unrestricted=\"true\"/>\n</PermissionSet>\n")]
		public static string GetTempPath()
		{
			string temp_path = Path.get_temp_path();
			if (temp_path.Length > 0 && temp_path[temp_path.Length - 1] != Path.DirectorySeparatorChar)
			{
				return temp_path + Path.DirectorySeparatorChar;
			}
			return temp_path;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string get_temp_path();

		public static bool HasExtension(string path)
		{
			if (path == null || path.Trim().Length == 0)
			{
				return false;
			}
			if (path.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("Illegal characters in path.");
			}
			int num = Path.findExtension(path);
			return 0 <= num && num < path.Length - 1;
		}

		public static bool IsPathRooted(string path)
		{
			if (path == null || path.Length == 0)
			{
				return false;
			}
			if (path.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("Illegal characters in path.");
			}
			char c = path[0];
			return c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar || (!Path.dirEqualsVolume && path.Length > 1 && path[1] == Path.VolumeSeparatorChar);
		}

		public static char[] GetInvalidFileNameChars()
		{
			if (Environment.IsRunningOnWindows)
			{
				return new char[]
				{
					'\0', '\u0001', '\u0002', '\u0003', '\u0004', '\u0005', '\u0006', '\a', '\b', '\t',
					'\n', '\v', '\f', '\r', '\u000e', '\u000f', '\u0010', '\u0011', '\u0012', '\u0013',
					'\u0014', '\u0015', '\u0016', '\u0017', '\u0018', '\u0019', '\u001a', '\u001b', '\u001c', '\u001d',
					'\u001e', '\u001f', '"', '<', '>', '|', ':', '*', '?', '\\',
					'/'
				};
			}
			return new char[] { '\0', '/' };
		}

		public static char[] GetInvalidPathChars()
		{
			if (Environment.IsRunningOnWindows)
			{
				return new char[]
				{
					'"', '<', '>', '|', '\0', '\u0001', '\u0002', '\u0003', '\u0004', '\u0005',
					'\u0006', '\a', '\b', '\t', '\n', '\v', '\f', '\r', '\u000e', '\u000f',
					'\u0010', '\u0011', '\u0012', '\u0013', '\u0014', '\u0015', '\u0016', '\u0017', '\u0018', '\u0019',
					'\u001a', '\u001b', '\u001c', '\u001d', '\u001e', '\u001f'
				};
			}
			return new char[1];
		}

		public static string GetRandomFileName()
		{
			StringBuilder stringBuilder = new StringBuilder(12);
			RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
			byte[] array = new byte[11];
			randomNumberGenerator.GetBytes(array);
			for (int i = 0; i < array.Length; i++)
			{
				if (stringBuilder.Length == 8)
				{
					stringBuilder.Append('.');
				}
				int num = (int)(array[i] % 36);
				char c = (char)((num >= 26) ? (num - 26 + 48) : (num + 97));
				stringBuilder.Append(c);
			}
			return stringBuilder.ToString();
		}

		private static int findExtension(string path)
		{
			if (path != null)
			{
				int num = path.LastIndexOf('.');
				int num2 = path.LastIndexOfAny(Path.PathSeparatorChars);
				if (num > num2)
				{
					return num;
				}
			}
			return -1;
		}

		private static string GetServerAndShare(string path)
		{
			int num = 2;
			while (num < path.Length && !Path.IsDsc(path[num]))
			{
				num++;
			}
			if (num < path.Length)
			{
				num++;
				while (num < path.Length && !Path.IsDsc(path[num]))
				{
					num++;
				}
			}
			return path.Substring(2, num - 2).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
		}

		private static bool SameRoot(string root, string path)
		{
			if (root.Length < 2 || path.Length < 2)
			{
				return false;
			}
			if (!Path.IsDsc(root[0]) || !Path.IsDsc(root[1]))
			{
				return root[0].Equals(path[0]) && path[1] == Path.VolumeSeparatorChar && (root.Length <= 2 || path.Length <= 2 || (Path.IsDsc(root[2]) && Path.IsDsc(path[2])));
			}
			if (!Path.IsDsc(path[0]) || !Path.IsDsc(path[1]))
			{
				return false;
			}
			string serverAndShare = Path.GetServerAndShare(root);
			string serverAndShare2 = Path.GetServerAndShare(path);
			return string.Compare(serverAndShare, serverAndShare2, true, CultureInfo.InvariantCulture) == 0;
		}

		private static string CanonicalizePath(string path)
		{
			if (path == null)
			{
				return path;
			}
			if (Environment.IsRunningOnWindows)
			{
				path = path.Trim();
			}
			if (path.Length == 0)
			{
				return path;
			}
			string pathRoot = Path.GetPathRoot(path);
			string[] array = path.Split(new char[]
			{
				Path.DirectorySeparatorChar,
				Path.AltDirectorySeparatorChar
			});
			int num = 0;
			bool flag = Environment.IsRunningOnWindows && pathRoot.Length > 2 && Path.IsDsc(pathRoot[0]) && Path.IsDsc(pathRoot[1]);
			int num2 = ((!flag) ? 0 : 3);
			for (int i = 0; i < array.Length; i++)
			{
				if (Environment.IsRunningOnWindows)
				{
					array[i] = array[i].TrimEnd(new char[0]);
				}
				if (!(array[i] == ".") && (i == 0 || array[i].Length != 0))
				{
					if (array[i] == "..")
					{
						if (num > num2)
						{
							num--;
						}
					}
					else
					{
						array[num++] = array[i];
					}
				}
			}
			if (num == 0 || (num == 1 && array[0] == string.Empty))
			{
				return pathRoot;
			}
			string text = string.Join(Path.DirectorySeparatorStr, array, 0, num);
			if (!Environment.IsRunningOnWindows)
			{
				return text;
			}
			if (flag)
			{
				text = Path.DirectorySeparatorStr + text;
			}
			if (!Path.SameRoot(pathRoot, text))
			{
				text = pathRoot + text;
			}
			if (flag)
			{
				return text;
			}
			if (!Path.IsDsc(path[0]) && Path.SameRoot(pathRoot, path))
			{
				if (text.Length <= 2 && !text.EndsWith(Path.DirectorySeparatorStr))
				{
					text += Path.DirectorySeparatorChar;
				}
				return text;
			}
			string currentDirectory = Directory.GetCurrentDirectory();
			if (currentDirectory.Length > 1 && currentDirectory[1] == Path.VolumeSeparatorChar)
			{
				if (text.Length == 0 || Path.IsDsc(text[0]))
				{
					text += '\\';
				}
				return currentDirectory.Substring(0, 2) + text;
			}
			if (Path.IsDsc(currentDirectory[currentDirectory.Length - 1]) && Path.IsDsc(text[0]))
			{
				return currentDirectory + text.Substring(1);
			}
			return currentDirectory + text;
		}

		internal static bool IsPathSubsetOf(string subset, string path)
		{
			if (subset.Length > path.Length)
			{
				return false;
			}
			int num = subset.LastIndexOfAny(Path.PathSeparatorChars);
			if (string.Compare(subset, 0, path, 0, num) != 0)
			{
				return false;
			}
			num++;
			int num2 = path.IndexOfAny(Path.PathSeparatorChars, num);
			if (num2 >= num)
			{
				return string.Compare(subset, num, path, num, path.Length - num2) == 0;
			}
			return subset.Length == path.Length && string.Compare(subset, num, path, num, subset.Length - num) == 0;
		}

		[Obsolete("see GetInvalidPathChars and GetInvalidFileNameChars methods.")]
		public static readonly char[] InvalidPathChars = Path.GetInvalidPathChars();

		public static readonly char AltDirectorySeparatorChar = MonoIO.AltDirectorySeparatorChar;

		public static readonly char DirectorySeparatorChar = MonoIO.DirectorySeparatorChar;

		public static readonly char PathSeparator = MonoIO.PathSeparator;

		internal static readonly string DirectorySeparatorStr = Path.DirectorySeparatorChar.ToString();

		public static readonly char VolumeSeparatorChar = MonoIO.VolumeSeparatorChar;

		internal static readonly char[] PathSeparatorChars = new char[]
		{
			Path.DirectorySeparatorChar,
			Path.AltDirectorySeparatorChar,
			Path.VolumeSeparatorChar
		};

		private static readonly bool dirEqualsVolume = Path.DirectorySeparatorChar == Path.VolumeSeparatorChar;
	}
}
