using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace System.IO.Compression
{
	public static class ZipFile
	{
		public static ZipArchive OpenRead(string archiveFileName)
		{
			return ZipFile.Open(archiveFileName, ZipArchiveMode.Read);
		}

		public static ZipArchive Open(string archiveFileName, ZipArchiveMode mode)
		{
			return ZipFile.Open(archiveFileName, mode, null);
		}

		public static ZipArchive Open(string archiveFileName, ZipArchiveMode mode, Encoding entryNameEncoding)
		{
			FileMode fileMode;
			FileAccess fileAccess;
			FileShare fileShare;
			switch (mode)
			{
			case ZipArchiveMode.Read:
				fileMode = FileMode.Open;
				fileAccess = FileAccess.Read;
				fileShare = FileShare.Read;
				break;
			case ZipArchiveMode.Create:
				fileMode = FileMode.CreateNew;
				fileAccess = FileAccess.Write;
				fileShare = FileShare.None;
				break;
			case ZipArchiveMode.Update:
				fileMode = FileMode.OpenOrCreate;
				fileAccess = FileAccess.ReadWrite;
				fileShare = FileShare.None;
				break;
			default:
				throw new ArgumentOutOfRangeException("mode");
			}
			FileStream fileStream = new FileStream(archiveFileName, fileMode, fileAccess, fileShare, 4096, false);
			ZipArchive zipArchive;
			try
			{
				zipArchive = new ZipArchive(fileStream, mode, false, entryNameEncoding);
			}
			catch
			{
				fileStream.Dispose();
				throw;
			}
			return zipArchive;
		}

		public static void CreateFromDirectory(string sourceDirectoryName, string destinationArchiveFileName)
		{
			ZipFile.DoCreateFromDirectory(sourceDirectoryName, destinationArchiveFileName, null, false, null);
		}

		public static void CreateFromDirectory(string sourceDirectoryName, string destinationArchiveFileName, CompressionLevel compressionLevel, bool includeBaseDirectory)
		{
			ZipFile.DoCreateFromDirectory(sourceDirectoryName, destinationArchiveFileName, new CompressionLevel?(compressionLevel), includeBaseDirectory, null);
		}

		public static void CreateFromDirectory(string sourceDirectoryName, string destinationArchiveFileName, CompressionLevel compressionLevel, bool includeBaseDirectory, Encoding entryNameEncoding)
		{
			ZipFile.DoCreateFromDirectory(sourceDirectoryName, destinationArchiveFileName, new CompressionLevel?(compressionLevel), includeBaseDirectory, entryNameEncoding);
		}

		public static void ExtractToDirectory(string sourceArchiveFileName, string destinationDirectoryName)
		{
			ZipFile.ExtractToDirectory(sourceArchiveFileName, destinationDirectoryName, null);
		}

		public static void ExtractToDirectory(string sourceArchiveFileName, string destinationDirectoryName, bool overwrite)
		{
			ZipFile.ExtractToDirectory(sourceArchiveFileName, destinationDirectoryName, null, overwrite);
		}

		public static void ExtractToDirectory(string sourceArchiveFileName, string destinationDirectoryName, Encoding entryNameEncoding)
		{
			ZipFile.ExtractToDirectory(sourceArchiveFileName, destinationDirectoryName, entryNameEncoding, false);
		}

		public static void ExtractToDirectory(string sourceArchiveFileName, string destinationDirectoryName, Encoding entryNameEncoding, bool overwrite)
		{
			if (sourceArchiveFileName == null)
			{
				throw new ArgumentNullException("sourceArchiveFileName");
			}
			using (ZipArchive zipArchive = ZipFile.Open(sourceArchiveFileName, ZipArchiveMode.Read, entryNameEncoding))
			{
				zipArchive.ExtractToDirectory(destinationDirectoryName, overwrite);
			}
		}

		private static void DoCreateFromDirectory(string sourceDirectoryName, string destinationArchiveFileName, CompressionLevel? compressionLevel, bool includeBaseDirectory, Encoding entryNameEncoding)
		{
			sourceDirectoryName = Path.GetFullPath(sourceDirectoryName);
			destinationArchiveFileName = Path.GetFullPath(destinationArchiveFileName);
			using (ZipArchive zipArchive = ZipFile.Open(destinationArchiveFileName, ZipArchiveMode.Create, entryNameEncoding))
			{
				bool flag = true;
				DirectoryInfo directoryInfo = new DirectoryInfo(sourceDirectoryName);
				string text = directoryInfo.FullName;
				if (includeBaseDirectory && directoryInfo.Parent != null)
				{
					text = directoryInfo.Parent.FullName;
				}
				char[] array = ArrayPool<char>.Shared.Rent(260);
				try
				{
					foreach (FileSystemInfo fileSystemInfo in directoryInfo.EnumerateFileSystemInfos("*", SearchOption.AllDirectories))
					{
						flag = false;
						int num = fileSystemInfo.FullName.Length - text.Length;
						if (fileSystemInfo is FileInfo)
						{
							string text2 = ZipFile.EntryFromPath(fileSystemInfo.FullName, text.Length, num, ref array, false);
							ZipFileExtensions.DoCreateEntryFromFile(zipArchive, fileSystemInfo.FullName, text2, compressionLevel);
						}
						else
						{
							DirectoryInfo directoryInfo2 = fileSystemInfo as DirectoryInfo;
							if (directoryInfo2 != null && ZipFile.IsDirEmpty(directoryInfo2))
							{
								string text3 = ZipFile.EntryFromPath(fileSystemInfo.FullName, text.Length, num, ref array, true);
								zipArchive.CreateEntry(text3);
							}
						}
					}
					if (includeBaseDirectory && flag)
					{
						zipArchive.CreateEntry(ZipFile.EntryFromPath(directoryInfo.Name, 0, directoryInfo.Name.Length, ref array, true));
					}
				}
				finally
				{
					ArrayPool<char>.Shared.Return(array, false);
				}
			}
		}

		private static string EntryFromPath(string entry, int offset, int length, ref char[] buffer, bool appendPathSeparator = false)
		{
			while (length > 0 && (entry[offset] == Path.DirectorySeparatorChar || entry[offset] == Path.AltDirectorySeparatorChar))
			{
				offset++;
				length--;
			}
			if (length != 0)
			{
				int num = (appendPathSeparator ? (length + 1) : length);
				ZipFile.EnsureCapacity(ref buffer, num);
				entry.CopyTo(offset, buffer, 0, length);
				for (int i = 0; i < length; i++)
				{
					char c = buffer[i];
					if (c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar)
					{
						buffer[i] = '/';
					}
				}
				if (appendPathSeparator)
				{
					buffer[length] = '/';
				}
				return new string(buffer, 0, num);
			}
			if (!appendPathSeparator)
			{
				return string.Empty;
			}
			return '/'.ToString();
		}

		private static void EnsureCapacity(ref char[] buffer, int min)
		{
			if (buffer.Length < min)
			{
				int num = buffer.Length * 2;
				if (num < min)
				{
					num = min;
				}
				ArrayPool<char>.Shared.Return(buffer, false);
				buffer = ArrayPool<char>.Shared.Rent(num);
			}
		}

		private static bool IsDirEmpty(DirectoryInfo possiblyEmptyDir)
		{
			bool flag;
			using (IEnumerator<string> enumerator = Directory.EnumerateFileSystemEntries(possiblyEmptyDir.FullName).GetEnumerator())
			{
				flag = !enumerator.MoveNext();
			}
			return flag;
		}

		private const char PathSeparator = '/';
	}
}
