using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace System.IO
{
	internal class DefaultWatcher : IFileWatcher
	{
		private DefaultWatcher()
		{
		}

		public static bool GetInstance(out IFileWatcher watcher)
		{
			if (DefaultWatcher.instance != null)
			{
				watcher = DefaultWatcher.instance;
				return true;
			}
			DefaultWatcher.instance = new DefaultWatcher();
			watcher = DefaultWatcher.instance;
			return true;
		}

		public void StartDispatching(FileSystemWatcher fsw)
		{
			lock (this)
			{
				if (DefaultWatcher.watches == null)
				{
					DefaultWatcher.watches = new Hashtable();
				}
				if (DefaultWatcher.thread == null)
				{
					DefaultWatcher.thread = new Thread(new ThreadStart(this.Monitor));
					DefaultWatcher.thread.IsBackground = true;
					DefaultWatcher.thread.Start();
				}
			}
			Hashtable hashtable = DefaultWatcher.watches;
			lock (hashtable)
			{
				DefaultWatcherData defaultWatcherData = (DefaultWatcherData)DefaultWatcher.watches[fsw];
				if (defaultWatcherData == null)
				{
					defaultWatcherData = new DefaultWatcherData();
					defaultWatcherData.Files = new Hashtable();
					DefaultWatcher.watches[fsw] = defaultWatcherData;
				}
				defaultWatcherData.FSW = fsw;
				defaultWatcherData.Directory = fsw.FullPath;
				defaultWatcherData.NoWildcards = !fsw.Pattern.HasWildcard;
				if (defaultWatcherData.NoWildcards)
				{
					defaultWatcherData.FileMask = Path.Combine(defaultWatcherData.Directory, fsw.MangledFilter);
				}
				else
				{
					defaultWatcherData.FileMask = fsw.MangledFilter;
				}
				defaultWatcherData.IncludeSubdirs = fsw.IncludeSubdirectories;
				defaultWatcherData.Enabled = true;
				defaultWatcherData.DisabledTime = DateTime.MaxValue;
				this.UpdateDataAndDispatch(defaultWatcherData, false);
			}
		}

		public void StopDispatching(FileSystemWatcher fsw)
		{
			lock (this)
			{
				if (DefaultWatcher.watches == null)
				{
					return;
				}
			}
			Hashtable hashtable = DefaultWatcher.watches;
			lock (hashtable)
			{
				DefaultWatcherData defaultWatcherData = (DefaultWatcherData)DefaultWatcher.watches[fsw];
				if (defaultWatcherData != null)
				{
					defaultWatcherData.Enabled = false;
					defaultWatcherData.DisabledTime = DateTime.UtcNow;
				}
			}
		}

		private void Monitor()
		{
			int num = 0;
			for (;;)
			{
				Thread.Sleep(750);
				Hashtable hashtable = DefaultWatcher.watches;
				Hashtable hashtable2;
				lock (hashtable)
				{
					if (DefaultWatcher.watches.Count == 0)
					{
						if (++num == 20)
						{
							break;
						}
						continue;
					}
					else
					{
						hashtable2 = (Hashtable)DefaultWatcher.watches.Clone();
					}
				}
				if (hashtable2.Count != 0)
				{
					num = 0;
					using (IEnumerator enumerator = hashtable2.Values.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							DefaultWatcherData defaultWatcherData = (DefaultWatcherData)obj;
							if (this.UpdateDataAndDispatch(defaultWatcherData, true))
							{
								hashtable = DefaultWatcher.watches;
								lock (hashtable)
								{
									DefaultWatcher.watches.Remove(defaultWatcherData.FSW);
								}
							}
						}
						continue;
					}
					break;
				}
			}
			lock (this)
			{
				DefaultWatcher.thread = null;
			}
		}

		private bool UpdateDataAndDispatch(DefaultWatcherData data, bool dispatch)
		{
			if (!data.Enabled)
			{
				return data.DisabledTime != DateTime.MaxValue && (DateTime.UtcNow - data.DisabledTime).TotalSeconds > 5.0;
			}
			this.DoFiles(data, data.Directory, dispatch);
			return false;
		}

		private static void DispatchEvents(FileSystemWatcher fsw, FileAction action, string filename)
		{
			RenamedEventArgs e = null;
			lock (fsw)
			{
				fsw.DispatchEvents(action, filename, ref e);
				if (fsw.Waiting)
				{
					fsw.Waiting = false;
					global::System.Threading.Monitor.PulseAll(fsw);
				}
			}
		}

		private void DoFiles(DefaultWatcherData data, string directory, bool dispatch)
		{
			bool flag = Directory.Exists(directory);
			if (flag && data.IncludeSubdirs)
			{
				foreach (string text in Directory.GetDirectories(directory))
				{
					this.DoFiles(data, text, dispatch);
				}
			}
			string[] array;
			if (!flag)
			{
				array = DefaultWatcher.NoStringsArray;
			}
			else if (!data.NoWildcards)
			{
				array = Directory.GetFileSystemEntries(directory, data.FileMask);
			}
			else if (File.Exists(data.FileMask) || Directory.Exists(data.FileMask))
			{
				array = new string[] { data.FileMask };
			}
			else
			{
				array = DefaultWatcher.NoStringsArray;
			}
			object filesLock = data.FilesLock;
			lock (filesLock)
			{
				this.IterateAndModifyFilesData(data, directory, dispatch, array);
			}
		}

		private void IterateAndModifyFilesData(DefaultWatcherData data, string directory, bool dispatch, string[] files)
		{
			foreach (object obj in data.Files.Keys)
			{
				string text = (string)obj;
				FileData fileData = (FileData)data.Files[text];
				if (fileData.Directory == directory)
				{
					fileData.NotExists = true;
				}
			}
			foreach (string text2 in files)
			{
				FileData fileData2 = (FileData)data.Files[text2];
				if (fileData2 == null)
				{
					try
					{
						data.Files.Add(text2, DefaultWatcher.CreateFileData(directory, text2));
					}
					catch
					{
						data.Files.Remove(text2);
						goto IL_00DD;
					}
					if (dispatch)
					{
						DefaultWatcher.DispatchEvents(data.FSW, FileAction.Added, text2);
					}
				}
				else if (fileData2.Directory == directory)
				{
					fileData2.NotExists = false;
				}
				IL_00DD:;
			}
			if (!dispatch)
			{
				return;
			}
			List<string> list = null;
			foreach (object obj2 in data.Files.Keys)
			{
				string text3 = (string)obj2;
				if (((FileData)data.Files[text3]).NotExists)
				{
					if (list == null)
					{
						list = new List<string>();
					}
					list.Add(text3);
					DefaultWatcher.DispatchEvents(data.FSW, FileAction.Removed, text3);
				}
			}
			if (list != null)
			{
				foreach (string text4 in list)
				{
					data.Files.Remove(text4);
				}
				list = null;
			}
			foreach (object obj3 in data.Files.Keys)
			{
				string text5 = (string)obj3;
				FileData fileData3 = (FileData)data.Files[text5];
				DateTime creationTime;
				DateTime lastWriteTime;
				try
				{
					creationTime = File.GetCreationTime(text5);
					lastWriteTime = File.GetLastWriteTime(text5);
				}
				catch
				{
					if (list == null)
					{
						list = new List<string>();
					}
					list.Add(text5);
					DefaultWatcher.DispatchEvents(data.FSW, FileAction.Removed, text5);
					continue;
				}
				if (creationTime != fileData3.CreationTime || lastWriteTime != fileData3.LastWriteTime)
				{
					fileData3.CreationTime = creationTime;
					fileData3.LastWriteTime = lastWriteTime;
					DefaultWatcher.DispatchEvents(data.FSW, FileAction.Modified, text5);
				}
			}
			if (list != null)
			{
				foreach (string text6 in list)
				{
					data.Files.Remove(text6);
				}
			}
		}

		private static FileData CreateFileData(string directory, string filename)
		{
			FileData fileData = new FileData();
			string text = Path.Combine(directory, filename);
			fileData.Directory = directory;
			fileData.Attributes = File.GetAttributes(text);
			fileData.CreationTime = File.GetCreationTime(text);
			fileData.LastWriteTime = File.GetLastWriteTime(text);
			return fileData;
		}

		private static DefaultWatcher instance;

		private static Thread thread;

		private static Hashtable watches;

		private static string[] NoStringsArray = new string[0];
	}
}
