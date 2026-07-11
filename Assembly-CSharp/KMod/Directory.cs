using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Klei;
using STRINGS;
using UnityEngine;

namespace KMod
{
	internal struct Directory : IFileSource
	{
		public Directory(string root)
		{
			this.root = root;
			this.file_system = new PrefixFileSystem(root, root, Application.streamingAssetsPath);
		}

		public string GetRoot()
		{
			return this.root;
		}

		public bool Exists()
		{
			return Directory.Exists(this.GetRoot());
		}

		public void GetTopLevelItems(List<FileSystemItem> file_system_items)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(this.root);
			foreach (FileSystemInfo fileSystemInfo in directoryInfo.GetFileSystemInfos())
			{
				file_system_items.Add(new FileSystemItem
				{
					name = fileSystemInfo.Name,
					type = ((!(fileSystemInfo is DirectoryInfo)) ? FileSystemItem.ItemType.File : FileSystemItem.ItemType.Directory)
				});
			}
		}

		public IFileSystem GetFileSystem()
		{
			return this.file_system;
		}

		public void CopyTo(string path, List<string> extensions = null)
		{
			try
			{
				Directory.CopyDirectory(this.root, path, extensions);
			}
			catch (UnauthorizedAccessException)
			{
				FileUtil.ErrorDialog(string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_UNAUTHORIZED, path));
			}
			catch (IOException)
			{
				FileUtil.ErrorDialog(string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_SUFFICIENT_SPACE, path));
			}
			catch (Exception ex)
			{
				FileUtil.ErrorDialog(string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_UNAUTHORIZED, ex.Message));
			}
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

		private static int CopyDirectory(string sourceDirName, string destDirName, List<string> extensions)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(sourceDirName);
			if (!directoryInfo.Exists)
			{
				return 0;
			}
			if (!FileUtil.CreateDirectory(destDirName))
			{
				return 0;
			}
			FileInfo[] files = directoryInfo.GetFiles();
			int num = 0;
			foreach (FileInfo fileInfo in files)
			{
				bool flag = extensions == null || extensions.Count == 0;
				if (extensions != null)
				{
					foreach (string text in extensions)
					{
						if (text == Path.GetExtension(fileInfo.Name).ToLower())
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					string text2 = Path.Combine(destDirName, fileInfo.Name);
					fileInfo.CopyTo(text2, false);
					num++;
				}
			}
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			foreach (DirectoryInfo directoryInfo2 in directories)
			{
				string text3 = Path.Combine(destDirName, directoryInfo2.Name);
				num += Directory.CopyDirectory(directoryInfo2.FullName, text3, extensions);
			}
			if (num == 0)
			{
				FileUtil.DeleteDirectory(destDirName);
			}
			return num;
		}

		private PrefixFileSystem file_system;

		private string root;
	}
}
