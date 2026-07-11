using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;

namespace System.IO
{
	[ComVisible(true)]
	public static class File
	{
		public static void AppendAllText(string path, string contents)
		{
			using (TextWriter textWriter = new StreamWriter(path, true))
			{
				textWriter.Write(contents);
			}
		}

		public static void AppendAllText(string path, string contents, Encoding encoding)
		{
			using (TextWriter textWriter = new StreamWriter(path, true, encoding))
			{
				textWriter.Write(contents);
			}
		}

		public static StreamWriter AppendText(string path)
		{
			return new StreamWriter(path, true);
		}

		public static void Copy(string sourceFileName, string destFileName)
		{
			File.Copy(sourceFileName, destFileName, false);
		}

		public static void Copy(string sourceFileName, string destFileName, bool overwrite)
		{
			if (sourceFileName == null)
			{
				throw new ArgumentNullException("sourceFileName");
			}
			if (destFileName == null)
			{
				throw new ArgumentNullException("destFileName");
			}
			if (sourceFileName.Length == 0)
			{
				throw new ArgumentException("An empty file name is not valid.", "sourceFileName");
			}
			if (sourceFileName.Trim().Length == 0 || sourceFileName.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("The file name is not valid.");
			}
			if (destFileName.Length == 0)
			{
				throw new ArgumentException("An empty file name is not valid.", "destFileName");
			}
			if (destFileName.Trim().Length == 0 || destFileName.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("The file name is not valid.");
			}
			MonoIOError monoIOError;
			if (!MonoIO.Exists(sourceFileName, out monoIOError))
			{
				throw new FileNotFoundException(Locale.GetText("{0} does not exist", new object[] { sourceFileName }), sourceFileName);
			}
			if ((File.GetAttributes(sourceFileName) & FileAttributes.Directory) == FileAttributes.Directory)
			{
				throw new ArgumentException(Locale.GetText("{0} is a directory", new object[] { sourceFileName }));
			}
			if (MonoIO.Exists(destFileName, out monoIOError))
			{
				if ((File.GetAttributes(destFileName) & FileAttributes.Directory) == FileAttributes.Directory)
				{
					throw new ArgumentException(Locale.GetText("{0} is a directory", new object[] { destFileName }));
				}
				if (!overwrite)
				{
					throw new IOException(Locale.GetText("{0} already exists", new object[] { destFileName }));
				}
			}
			string directoryName = Path.GetDirectoryName(destFileName);
			if (directoryName != string.Empty && !Directory.Exists(directoryName))
			{
				throw new DirectoryNotFoundException(Locale.GetText("Destination directory not found: {0}", new object[] { directoryName }));
			}
			if (!MonoIO.CopyFile(sourceFileName, destFileName, overwrite, out monoIOError))
			{
				string text = Locale.GetText("{0}\" or \"{1}", new object[] { sourceFileName, destFileName });
				throw MonoIO.GetException(text, monoIOError);
			}
		}

		public static FileStream Create(string path)
		{
			return File.Create(path, 8192);
		}

		public static FileStream Create(string path, int bufferSize)
		{
			return new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None, bufferSize);
		}

		[MonoTODO("options not implemented")]
		public static FileStream Create(string path, int bufferSize, FileOptions options)
		{
			return File.Create(path, bufferSize, options, null);
		}

		[MonoTODO("options and fileSecurity not implemented")]
		public static FileStream Create(string path, int bufferSize, FileOptions options, FileSecurity fileSecurity)
		{
			return new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None, bufferSize, options);
		}

		public static StreamWriter CreateText(string path)
		{
			return new StreamWriter(path, false);
		}

		public static void Delete(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Trim().Length == 0 || path.IndexOfAny(Path.InvalidPathChars) >= 0)
			{
				throw new ArgumentException("path");
			}
			if (Directory.Exists(path))
			{
				throw new UnauthorizedAccessException(Locale.GetText("{0} is a directory", new object[] { path }));
			}
			string directoryName = Path.GetDirectoryName(path);
			if (directoryName != string.Empty && !Directory.Exists(directoryName))
			{
				throw new DirectoryNotFoundException(Locale.GetText("Could not find a part of the path \"{0}\".", new object[] { path }));
			}
			MonoIOError monoIOError;
			if (!MonoIO.DeleteFile(path, out monoIOError) && monoIOError != MonoIOError.ERROR_FILE_NOT_FOUND)
			{
				throw MonoIO.GetException(path, monoIOError);
			}
		}

		public static bool Exists(string path)
		{
			MonoIOError monoIOError;
			return path != null && path.Trim().Length != 0 && path.IndexOfAny(Path.InvalidPathChars) < 0 && MonoIO.ExistsFile(path, out monoIOError);
		}

		public static FileSecurity GetAccessControl(string path)
		{
			throw new NotImplementedException();
		}

		public static FileSecurity GetAccessControl(string path, AccessControlSections includeSections)
		{
			throw new NotImplementedException();
		}

		public static FileAttributes GetAttributes(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Trim().Length == 0)
			{
				throw new ArgumentException(Locale.GetText("Path is empty"));
			}
			if (path.IndexOfAny(Path.InvalidPathChars) >= 0)
			{
				throw new ArgumentException(Locale.GetText("Path contains invalid chars"));
			}
			MonoIOError monoIOError;
			FileAttributes fileAttributes = MonoIO.GetFileAttributes(path, out monoIOError);
			if (monoIOError != MonoIOError.ERROR_SUCCESS)
			{
				throw MonoIO.GetException(path, monoIOError);
			}
			return fileAttributes;
		}

		public static DateTime GetCreationTime(string path)
		{
			File.CheckPathExceptions(path);
			MonoIOStat monoIOStat;
			MonoIOError monoIOError;
			if (MonoIO.GetFileStat(path, out monoIOStat, out monoIOError))
			{
				return DateTime.FromFileTime(monoIOStat.CreationTime);
			}
			if (monoIOError == MonoIOError.ERROR_PATH_NOT_FOUND || monoIOError == MonoIOError.ERROR_FILE_NOT_FOUND)
			{
				return File.DefaultLocalFileTime;
			}
			throw new IOException(path);
		}

		public static DateTime GetCreationTimeUtc(string path)
		{
			return File.GetCreationTime(path).ToUniversalTime();
		}

		public static DateTime GetLastAccessTime(string path)
		{
			File.CheckPathExceptions(path);
			MonoIOStat monoIOStat;
			MonoIOError monoIOError;
			if (MonoIO.GetFileStat(path, out monoIOStat, out monoIOError))
			{
				return DateTime.FromFileTime(monoIOStat.LastAccessTime);
			}
			if (monoIOError == MonoIOError.ERROR_PATH_NOT_FOUND || monoIOError == MonoIOError.ERROR_FILE_NOT_FOUND)
			{
				return File.DefaultLocalFileTime;
			}
			throw new IOException(path);
		}

		public static DateTime GetLastAccessTimeUtc(string path)
		{
			return File.GetLastAccessTime(path).ToUniversalTime();
		}

		public static DateTime GetLastWriteTime(string path)
		{
			File.CheckPathExceptions(path);
			MonoIOStat monoIOStat;
			MonoIOError monoIOError;
			if (MonoIO.GetFileStat(path, out monoIOStat, out monoIOError))
			{
				return DateTime.FromFileTime(monoIOStat.LastWriteTime);
			}
			if (monoIOError == MonoIOError.ERROR_PATH_NOT_FOUND || monoIOError == MonoIOError.ERROR_FILE_NOT_FOUND)
			{
				return File.DefaultLocalFileTime;
			}
			throw new IOException(path);
		}

		public static DateTime GetLastWriteTimeUtc(string path)
		{
			return File.GetLastWriteTime(path).ToUniversalTime();
		}

		public static void Move(string sourceFileName, string destFileName)
		{
			if (sourceFileName == null)
			{
				throw new ArgumentNullException("sourceFileName");
			}
			if (destFileName == null)
			{
				throw new ArgumentNullException("destFileName");
			}
			if (sourceFileName.Length == 0)
			{
				throw new ArgumentException("An empty file name is not valid.", "sourceFileName");
			}
			if (sourceFileName.Trim().Length == 0 || sourceFileName.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("The file name is not valid.");
			}
			if (destFileName.Length == 0)
			{
				throw new ArgumentException("An empty file name is not valid.", "destFileName");
			}
			if (destFileName.Trim().Length == 0 || destFileName.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("The file name is not valid.");
			}
			MonoIOError monoIOError;
			if (!MonoIO.Exists(sourceFileName, out monoIOError))
			{
				throw new FileNotFoundException(Locale.GetText("{0} does not exist", new object[] { sourceFileName }), sourceFileName);
			}
			string directoryName = Path.GetDirectoryName(destFileName);
			if (directoryName != string.Empty && !Directory.Exists(directoryName))
			{
				throw new DirectoryNotFoundException(Locale.GetText("Could not find a part of the path."));
			}
			if (MonoIO.MoveFile(sourceFileName, destFileName, out monoIOError))
			{
				return;
			}
			if (monoIOError == MonoIOError.ERROR_ALREADY_EXISTS)
			{
				throw MonoIO.GetException(monoIOError);
			}
			if (monoIOError == MonoIOError.ERROR_SHARING_VIOLATION)
			{
				throw MonoIO.GetException(sourceFileName, monoIOError);
			}
			throw MonoIO.GetException(monoIOError);
		}

		public static FileStream Open(string path, FileMode mode)
		{
			return new FileStream(path, mode, (mode != FileMode.Append) ? FileAccess.ReadWrite : FileAccess.Write, FileShare.None);
		}

		public static FileStream Open(string path, FileMode mode, FileAccess access)
		{
			return new FileStream(path, mode, access, FileShare.None);
		}

		public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share)
		{
			return new FileStream(path, mode, access, share);
		}

		public static FileStream OpenRead(string path)
		{
			return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
		}

		public static StreamReader OpenText(string path)
		{
			return new StreamReader(path);
		}

		public static FileStream OpenWrite(string path)
		{
			return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
		}

		public static void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName)
		{
			File.Replace(sourceFileName, destinationFileName, destinationBackupFileName, false);
		}

		public static void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors)
		{
			if (sourceFileName == null)
			{
				throw new ArgumentNullException("sourceFileName");
			}
			if (destinationFileName == null)
			{
				throw new ArgumentNullException("destinationFileName");
			}
			if (sourceFileName.Trim().Length == 0 || sourceFileName.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("sourceFileName");
			}
			if (destinationFileName.Trim().Length == 0 || destinationFileName.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("destinationFileName");
			}
			string fullPath = Path.GetFullPath(sourceFileName);
			string fullPath2 = Path.GetFullPath(destinationFileName);
			MonoIOError monoIOError;
			if (MonoIO.ExistsDirectory(fullPath, out monoIOError))
			{
				throw new IOException(Locale.GetText("{0} is a directory", new object[] { sourceFileName }));
			}
			if (MonoIO.ExistsDirectory(fullPath2, out monoIOError))
			{
				throw new IOException(Locale.GetText("{0} is a directory", new object[] { destinationFileName }));
			}
			if (!File.Exists(fullPath))
			{
				throw new FileNotFoundException(Locale.GetText("{0} does not exist", new object[] { sourceFileName }), sourceFileName);
			}
			if (!File.Exists(fullPath2))
			{
				throw new FileNotFoundException(Locale.GetText("{0} does not exist", new object[] { destinationFileName }), destinationFileName);
			}
			if (fullPath == fullPath2)
			{
				throw new IOException(Locale.GetText("Source and destination arguments are the same file."));
			}
			string text = null;
			if (destinationBackupFileName != null)
			{
				if (destinationBackupFileName.Trim().Length == 0 || destinationBackupFileName.IndexOfAny(Path.InvalidPathChars) != -1)
				{
					throw new ArgumentException("destinationBackupFileName");
				}
				text = Path.GetFullPath(destinationBackupFileName);
				if (MonoIO.ExistsDirectory(text, out monoIOError))
				{
					throw new IOException(Locale.GetText("{0} is a directory", new object[] { destinationBackupFileName }));
				}
				if (fullPath == text)
				{
					throw new IOException(Locale.GetText("Source and backup arguments are the same file."));
				}
				if (fullPath2 == text)
				{
					throw new IOException(Locale.GetText("Destination and backup arguments are the same file."));
				}
			}
			if (!MonoIO.ReplaceFile(fullPath, fullPath2, text, ignoreMetadataErrors, out monoIOError))
			{
				throw MonoIO.GetException(monoIOError);
			}
		}

		public static void SetAccessControl(string path, FileSecurity fileSecurity)
		{
			throw new NotImplementedException();
		}

		public static void SetAttributes(string path, FileAttributes fileAttributes)
		{
			File.CheckPathExceptions(path);
			MonoIOError monoIOError;
			if (!MonoIO.SetFileAttributes(path, fileAttributes, out monoIOError))
			{
				throw MonoIO.GetException(path, monoIOError);
			}
		}

		public static void SetCreationTime(string path, DateTime creationTime)
		{
			File.CheckPathExceptions(path);
			MonoIOError monoIOError;
			if (!MonoIO.Exists(path, out monoIOError))
			{
				throw MonoIO.GetException(path, monoIOError);
			}
			if (!MonoIO.SetCreationTime(path, creationTime, out monoIOError))
			{
				throw MonoIO.GetException(path, monoIOError);
			}
		}

		public static void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
		{
			File.SetCreationTime(path, creationTimeUtc.ToLocalTime());
		}

		public static void SetLastAccessTime(string path, DateTime lastAccessTime)
		{
			File.CheckPathExceptions(path);
			MonoIOError monoIOError;
			if (!MonoIO.Exists(path, out monoIOError))
			{
				throw MonoIO.GetException(path, monoIOError);
			}
			if (!MonoIO.SetLastAccessTime(path, lastAccessTime, out monoIOError))
			{
				throw MonoIO.GetException(path, monoIOError);
			}
		}

		public static void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
		{
			File.SetLastAccessTime(path, lastAccessTimeUtc.ToLocalTime());
		}

		public static void SetLastWriteTime(string path, DateTime lastWriteTime)
		{
			File.CheckPathExceptions(path);
			MonoIOError monoIOError;
			if (!MonoIO.Exists(path, out monoIOError))
			{
				throw MonoIO.GetException(path, monoIOError);
			}
			if (!MonoIO.SetLastWriteTime(path, lastWriteTime, out monoIOError))
			{
				throw MonoIO.GetException(path, monoIOError);
			}
		}

		public static void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
		{
			File.SetLastWriteTime(path, lastWriteTimeUtc.ToLocalTime());
		}

		private static void CheckPathExceptions(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(Locale.GetText("Path is empty"));
			}
			if (path.Trim().Length == 0)
			{
				throw new ArgumentException(Locale.GetText("Path is empty"));
			}
			if (path.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException(Locale.GetText("Path contains invalid chars"));
			}
		}

		public static byte[] ReadAllBytes(string path)
		{
			byte[] array2;
			using (FileStream fileStream = File.OpenRead(path))
			{
				long length = fileStream.Length;
				if (length > 2147483647L)
				{
					throw new IOException("Reading more than 2GB with this call is not supported");
				}
				int num = 0;
				int i = (int)length;
				byte[] array = new byte[length];
				while (i > 0)
				{
					int num2 = fileStream.Read(array, num, i);
					if (num2 == 0)
					{
						throw new IOException("Unexpected end of stream");
					}
					num += num2;
					i -= num2;
				}
				array2 = array;
			}
			return array2;
		}

		public static string[] ReadAllLines(string path)
		{
			string[] array;
			using (StreamReader streamReader = File.OpenText(path))
			{
				array = File.ReadAllLines(streamReader);
			}
			return array;
		}

		public static string[] ReadAllLines(string path, Encoding encoding)
		{
			string[] array;
			using (StreamReader streamReader = new StreamReader(path, encoding))
			{
				array = File.ReadAllLines(streamReader);
			}
			return array;
		}

		private static string[] ReadAllLines(StreamReader reader)
		{
			List<string> list = new List<string>();
			while (!reader.EndOfStream)
			{
				list.Add(reader.ReadLine());
			}
			return list.ToArray();
		}

		public static string ReadAllText(string path)
		{
			return File.ReadAllText(path, Encoding.UTF8Unmarked);
		}

		public static string ReadAllText(string path, Encoding encoding)
		{
			string text;
			using (StreamReader streamReader = new StreamReader(path, encoding))
			{
				text = streamReader.ReadToEnd();
			}
			return text;
		}

		public static void WriteAllBytes(string path, byte[] bytes)
		{
			using (Stream stream = File.Create(path))
			{
				stream.Write(bytes, 0, bytes.Length);
			}
		}

		public static void WriteAllLines(string path, string[] contents)
		{
			using (StreamWriter streamWriter = new StreamWriter(path))
			{
				File.WriteAllLines(streamWriter, contents);
			}
		}

		public static void WriteAllLines(string path, string[] contents, Encoding encoding)
		{
			using (StreamWriter streamWriter = new StreamWriter(path, false, encoding))
			{
				File.WriteAllLines(streamWriter, contents);
			}
		}

		private static void WriteAllLines(StreamWriter writer, string[] contents)
		{
			foreach (string text in contents)
			{
				writer.WriteLine(text);
			}
		}

		public static void WriteAllText(string path, string contents)
		{
			File.WriteAllText(path, contents, Encoding.UTF8Unmarked);
		}

		public static void WriteAllText(string path, string contents, Encoding encoding)
		{
			using (StreamWriter streamWriter = new StreamWriter(path, false, encoding))
			{
				streamWriter.Write(contents);
			}
		}

		private static DateTime DefaultLocalFileTime
		{
			get
			{
				DateTime? dateTime = File.defaultLocalFileTime;
				if (dateTime == null)
				{
					DateTime dateTime2 = new DateTime(1601, 1, 1);
					File.defaultLocalFileTime = new DateTime?(dateTime2.ToLocalTime());
				}
				return File.defaultLocalFileTime.Value;
			}
		}

		[MonoLimitation("File encryption isn't supported (even on NTFS).")]
		public static void Encrypt(string path)
		{
			throw new NotSupportedException(Locale.GetText("File encryption isn't supported on any file system."));
		}

		[MonoLimitation("File encryption isn't supported (even on NTFS).")]
		public static void Decrypt(string path)
		{
			throw new NotSupportedException(Locale.GetText("File encryption isn't supported on any file system."));
		}

		private static DateTime? defaultLocalFileTime;
	}
}
