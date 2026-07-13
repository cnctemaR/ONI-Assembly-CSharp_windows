using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Klei;
using UnityEngine;

namespace KMod
{
	internal struct Directory : IFileSource
	{
		public Directory(string root)
		{
			this.root = root;
			this.file_system = new AliasDirectory(root, root, Application.streamingAssetsPath, true);
		}

		public string GetRoot()
		{
			return this.root;
		}

		public bool Exists()
		{
			return Directory.Exists(this.GetRoot());
		}

		public bool Exists(string relative_path)
		{
			return this.Exists() && new DirectoryInfo(FileSystem.Normalize(Path.Combine(this.root, relative_path))).Exists;
		}

		public void GetTopLevelItems(List<FileSystemItem> file_system_items, string relative_root)
		{
			relative_root = relative_root ?? "";
			string text = FileSystem.Normalize(Path.Combine(this.root, relative_root));
			DirectoryInfo directoryInfo = new DirectoryInfo(text);
			if (!directoryInfo.Exists)
			{
				global::Debug.LogError("Cannot iterate over $" + text + ", this directory does not exist");
				return;
			}
			foreach (FileSystemInfo fileSystemInfo in directoryInfo.GetFileSystemInfos())
			{
				file_system_items.Add(new FileSystemItem
				{
					name = fileSystemInfo.Name,
					type = ((fileSystemInfo is DirectoryInfo) ? FileSystemItem.ItemType.Directory : FileSystemItem.ItemType.File)
				});
			}
		}

		public IFileDirectory GetFileSystem()
		{
			return this.file_system;
		}

		public bool TryCopyTo(string path, List<string> extensions = null)
		{
			bool flag;
			try
			{
				flag = Directory.CopyDirectory(this.root, path, extensions).error == Directory.CopyDirectoryResult.Error.None;
			}
			catch (UnauthorizedAccessException)
			{
				FileUtil.ErrorDialog(FileUtil.ErrorType.UnauthorizedAccess, path, null, null);
				flag = false;
			}
			catch (IOException)
			{
				FileUtil.ErrorDialog(FileUtil.ErrorType.IOError, path, null, null);
				flag = false;
			}
			catch
			{
				throw;
			}
			return flag;
		}

		public string Read(string relative_path)
		{
			string text;
			try
			{
				using (FileStream fileStream = File.OpenRead(Path.Combine(this.root, relative_path)))
				{
					byte[] array = new byte[fileStream.Length];
					fileStream.Read(array, 0, (int)fileStream.Length);
					text = Encoding.UTF8.GetString(array);
				}
			}
			catch
			{
				text = string.Empty;
			}
			return text;
		}

		private static Directory.CopyDirectoryResult CopyDirectory(string sourceDirName, string destDirName, List<string> extensions)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(sourceDirName);
			if (!directoryInfo.Exists)
			{
				return new Directory.CopyDirectoryResult
				{
					error = Directory.CopyDirectoryResult.Error.Read,
					fileCount = 0
				};
			}
			if (!FileUtil.CreateDirectory(destDirName, 0))
			{
				return new Directory.CopyDirectoryResult
				{
					error = Directory.CopyDirectoryResult.Error.Write,
					fileCount = 0
				};
			}
			FileInfo[] files = directoryInfo.GetFiles();
			Directory.CopyDirectoryResult copyDirectoryResult = new Directory.CopyDirectoryResult
			{
				error = Directory.CopyDirectoryResult.Error.None,
				fileCount = 0
			};
			foreach (FileInfo fileInfo in files)
			{
				bool flag = extensions == null || extensions.Count == 0;
				if (extensions != null)
				{
					using (List<string>.Enumerator enumerator = extensions.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current == Path.GetExtension(fileInfo.Name).ToLower())
							{
								flag = true;
								break;
							}
						}
					}
				}
				if (flag)
				{
					string text = Path.Combine(destDirName, fileInfo.Name);
					if (fileInfo.CopyTo(text, false) == null)
					{
						copyDirectoryResult.error = Directory.CopyDirectoryResult.Error.Write;
						return copyDirectoryResult;
					}
					copyDirectoryResult.fileCount++;
				}
			}
			foreach (DirectoryInfo directoryInfo2 in directoryInfo.GetDirectories())
			{
				string text2 = Path.Combine(destDirName, directoryInfo2.Name);
				Directory.CopyDirectoryResult copyDirectoryResult2 = Directory.CopyDirectory(directoryInfo2.FullName, text2, extensions);
				copyDirectoryResult.fileCount += copyDirectoryResult2.fileCount;
				if (copyDirectoryResult2.error != Directory.CopyDirectoryResult.Error.None)
				{
					copyDirectoryResult.error = copyDirectoryResult2.error;
					return copyDirectoryResult;
				}
			}
			if (copyDirectoryResult.fileCount == 0)
			{
				FileUtil.DeleteDirectory(destDirName, 0);
			}
			return copyDirectoryResult;
		}

		public void Dispose()
		{
		}

		private AliasDirectory file_system;

		private string root;

		private struct CopyDirectoryResult
		{
			public Directory.CopyDirectoryResult.Error error;

			public int fileCount;

			public enum Error
			{
				None,
				Read,
				Write
			}
		}
	}
}
