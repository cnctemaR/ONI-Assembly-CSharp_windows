using System;
using System.Collections;
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
					defaultWatcherData.DisabledTime = DateTime.Now;
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
					foreach (object obj in hashtable2.Values)
					{
						DefaultWatcherData defaultWatcherData = (DefaultWatcherData)obj;
						bool flag = this.UpdateDataAndDispatch(defaultWatcherData, true);
						if (flag)
						{
							Hashtable hashtable3 = DefaultWatcher.watches;
							lock (hashtable3)
							{
								DefaultWatcher.watches.Remove(defaultWatcherData.FSW);
							}
						}
					}
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
				return data.DisabledTime != DateTime.MaxValue && (DateTime.Now - data.DisabledTime).TotalSeconds > 5.0;
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
			string[] array = null;
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
			foreach (object obj in data.Files.Keys)
			{
				string text2 = (string)obj;
				FileData fileData = (FileData)data.Files[text2];
				if (fileData.Directory == directory)
				{
					fileData.NotExists = true;
				}
			}
			foreach (string text3 in array)
			{
				FileData fileData2 = (FileData)data.Files[text3];
				if (fileData2 == null)
				{
					try
					{
						data.Files.Add(text3, DefaultWatcher.CreateFileData(directory, text3));
					}
					catch
					{
						data.Files.Remove(text3);
						goto IL_01BD;
					}
					if (dispatch)
					{
						DefaultWatcher.DispatchEvents(data.FSW, FileAction.Added, text3);
					}
				}
				else if (fileData2.Directory == directory)
				{
					fileData2.NotExists = false;
				}
				IL_01BD:;
			}
			if (!dispatch)
			{
				return;
			}
			ArrayList arrayList = null;
			foreach (object obj2 in data.Files.Keys)
			{
				string text4 = (string)obj2;
				FileData fileData3 = (FileData)data.Files[text4];
				if (fileData3.NotExists)
				{
					if (arrayList == null)
					{
						arrayList = new ArrayList();
					}
					arrayList.Add(text4);
					DefaultWatcher.DispatchEvents(data.FSW, FileAction.Removed, text4);
				}
			}
			if (arrayList != null)
			{
				foreach (object obj3 in arrayList)
				{
					string text5 = (string)obj3;
					data.Files.Remove(text5);
				}
				arrayList = null;
			}
			foreach (object obj4 in data.Files.Keys)
			{
				string text6 = (string)obj4;
				FileData fileData4 = (FileData)data.Files[text6];
				DateTime creationTime;
				DateTime lastWriteTime;
				try
				{
					creationTime = File.GetCreationTime(text6);
					lastWriteTime = File.GetLastWriteTime(text6);
				}
				catch
				{
					if (arrayList == null)
					{
						arrayList = new ArrayList();
					}
					arrayList.Add(text6);
					DefaultWatcher.DispatchEvents(data.FSW, FileAction.Removed, text6);
					continue;
				}
				if (creationTime != fileData4.CreationTime || lastWriteTime != fileData4.LastWriteTime)
				{
					fileData4.CreationTime = creationTime;
					fileData4.LastWriteTime = lastWriteTime;
					DefaultWatcher.DispatchEvents(data.FSW, FileAction.Modified, text6);
				}
			}
			if (arrayList != null)
			{
				foreach (object obj5 in arrayList)
				{
					string text7 = (string)obj5;
					data.Files.Remove(text7);
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
