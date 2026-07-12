using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace System.IO
{
	internal class KqueueMonitor : IDisposable
	{
		public int Connection
		{
			get
			{
				return this.conn;
			}
		}

		public KqueueMonitor(FileSystemWatcher fsw)
		{
			this.fsw = fsw;
			this.conn = -1;
			if (!KqueueMonitor.initialized)
			{
				KqueueMonitor.initialized = true;
				string environmentVariable = Environment.GetEnvironmentVariable("MONO_DARWIN_WATCHER_MAXFDS");
				int num;
				if (environmentVariable != null && int.TryParse(environmentVariable, out num))
				{
					this.maxFds = num;
				}
			}
		}

		public void Dispose()
		{
			this.CleanUp();
		}

		public void Start()
		{
			object obj = this.stateLock;
			lock (obj)
			{
				if (!this.started)
				{
					this.conn = KqueueMonitor.kqueue();
					if (this.conn == -1)
					{
						throw new IOException(string.Format("kqueue() error at init, error code = '{0}'", Marshal.GetLastWin32Error()));
					}
					this.thread = new Thread(delegate
					{
						this.DoMonitor();
					});
					this.thread.IsBackground = true;
					this.thread.Start();
					this.startedEvent.WaitOne();
					if (this.exc != null)
					{
						this.thread.Join();
						this.CleanUp();
						throw this.exc;
					}
					this.started = true;
				}
			}
		}

		public void Stop()
		{
			object obj = this.stateLock;
			lock (obj)
			{
				if (this.started)
				{
					this.requestStop = true;
					if (!this.inDispatch)
					{
						object obj2 = this.connLock;
						lock (obj2)
						{
							if (this.conn != -1)
							{
								KqueueMonitor.close(this.conn);
							}
							this.conn = -1;
							goto IL_0078;
						}
						IL_006D:
						this.thread.Interrupt();
						IL_0078:
						if (!this.thread.Join(2000))
						{
							goto IL_006D;
						}
						this.requestStop = false;
						this.started = false;
						if (this.exc != null)
						{
							throw this.exc;
						}
					}
				}
			}
		}

		private void CleanUp()
		{
			object obj = this.connLock;
			lock (obj)
			{
				if (this.conn != -1)
				{
					KqueueMonitor.close(this.conn);
				}
				this.conn = -1;
			}
			foreach (int num in this.fdsDict.Keys)
			{
				KqueueMonitor.close(num);
			}
			this.fdsDict.Clear();
			this.pathsDict.Clear();
		}

		private void DoMonitor()
		{
			try
			{
				this.Setup();
			}
			catch (Exception ex)
			{
				this.exc = ex;
			}
			finally
			{
				this.startedEvent.Set();
			}
			if (this.exc != null)
			{
				this.fsw.DispatchErrorEvents(new ErrorEventArgs(this.exc));
				return;
			}
			try
			{
				this.Monitor();
			}
			catch (Exception ex2)
			{
				this.exc = ex2;
			}
			finally
			{
				this.CleanUp();
				if (!this.requestStop)
				{
					this.started = false;
					this.inDispatch = false;
					this.fsw.EnableRaisingEvents = false;
				}
				if (this.exc != null)
				{
					this.fsw.DispatchErrorEvents(new ErrorEventArgs(this.exc));
				}
				this.requestStop = false;
			}
		}

		private void Setup()
		{
			List<int> list = new List<int>();
			if (this.fsw.FullPath != "/" && this.fsw.FullPath.EndsWith("/", StringComparison.Ordinal))
			{
				this.fullPathNoLastSlash = this.fsw.FullPath.Substring(0, this.fsw.FullPath.Length - 1);
			}
			else
			{
				this.fullPathNoLastSlash = this.fsw.FullPath;
			}
			StringBuilder stringBuilder = new StringBuilder(1024);
			if (KqueueMonitor.realpath(this.fsw.FullPath, stringBuilder) == IntPtr.Zero)
			{
				throw new IOException(string.Format("realpath({0}) failed, error code = '{1}'", this.fsw.FullPath, Marshal.GetLastWin32Error()));
			}
			string text = stringBuilder.ToString();
			if (text != this.fullPathNoLastSlash)
			{
				this.fixupPath = text;
			}
			else
			{
				this.fixupPath = null;
			}
			this.Scan(this.fullPathNoLastSlash, false, ref list);
			timespec timespec = new timespec
			{
				tv_sec = (IntPtr)0,
				tv_nsec = (IntPtr)0
			};
			kevent[] array = new kevent[0];
			kevent[] array2 = this.CreateChangeList(ref list);
			int num = 0;
			int num2;
			do
			{
				num2 = KqueueMonitor.kevent(this.conn, array2, array2.Length, array, array.Length, ref timespec);
				if (num2 == -1)
				{
					num = Marshal.GetLastWin32Error();
				}
			}
			while (num2 == -1 && num == 4);
			if (num2 == -1)
			{
				throw new IOException(string.Format("kevent() error at initial event registration, error code = '{0}'", num));
			}
		}

		private kevent[] CreateChangeList(ref List<int> FdList)
		{
			if (FdList.Count == 0)
			{
				return KqueueMonitor.emptyEventList;
			}
			List<kevent> list = new List<kevent>();
			foreach (int num in FdList)
			{
				kevent kevent = new kevent
				{
					ident = (UIntPtr)((ulong)((long)num)),
					filter = EventFilter.Vnode,
					flags = (EventFlags.Add | EventFlags.Enable | EventFlags.Clear),
					fflags = (FilterFlags.ReadLowWaterMark | FilterFlags.VNodeWrite | FilterFlags.VNodeExtend | FilterFlags.VNodeAttrib | FilterFlags.VNodeLink | FilterFlags.VNodeRename | FilterFlags.VNodeRevoke),
					data = IntPtr.Zero,
					udata = IntPtr.Zero
				};
				list.Add(kevent);
			}
			FdList.Clear();
			return list.ToArray();
		}

		private void Monitor()
		{
			kevent[] array = new kevent[32];
			List<int> newFds = new List<int>();
			List<PathData> list = new List<PathData>();
			List<string> list2 = new List<string>();
			int num = 0;
			Action<string> <>9__0;
			while (!this.requestStop)
			{
				kevent[] array2 = this.CreateChangeList(ref newFds);
				int num2 = Marshal.SizeOf<kevent>();
				IntPtr intPtr = Marshal.AllocHGlobal(num2 * array2.Length);
				for (int i = 0; i < array2.Length; i++)
				{
					Marshal.StructureToPtr<kevent>(array2[i], intPtr + i * num2, false);
				}
				IntPtr intPtr2 = Marshal.AllocHGlobal(num2 * array.Length);
				int num3 = KqueueMonitor.kevent_notimeout(ref this.conn, intPtr, array2.Length, intPtr2, array.Length);
				Marshal.FreeHGlobal(intPtr);
				for (int j = 0; j < num3; j++)
				{
					array[j] = Marshal.PtrToStructure<kevent>(intPtr2 + j * num2);
				}
				Marshal.FreeHGlobal(intPtr2);
				if (num3 == -1)
				{
					if (this.requestStop)
					{
						break;
					}
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (lastWin32Error != 4 && ++num == 3)
					{
						throw new IOException(string.Format("persistent kevent() error, error code = '{0}'", lastWin32Error));
					}
				}
				else
				{
					num = 0;
					for (int k = 0; k < num3; k++)
					{
						kevent kevent = array[k];
						if (this.fdsDict.ContainsKey((int)(uint)kevent.ident))
						{
							PathData pathData = this.fdsDict[(int)(uint)kevent.ident];
							if ((kevent.flags & EventFlags.Error) == EventFlags.Error)
							{
								string text = string.Format("kevent() error watching path '{0}', error code = '{1}'", pathData.Path, kevent.data);
								this.fsw.DispatchErrorEvents(new ErrorEventArgs(new IOException(text)));
							}
							else if ((kevent.fflags & FilterFlags.ReadLowWaterMark) == FilterFlags.ReadLowWaterMark || (kevent.fflags & FilterFlags.VNodeRevoke) == FilterFlags.VNodeRevoke)
							{
								if (pathData.Path == this.fullPathNoLastSlash)
								{
									return;
								}
								list.Add(pathData);
							}
							else
							{
								if ((kevent.fflags & FilterFlags.VNodeRename) == FilterFlags.VNodeRename)
								{
									this.UpdatePath(pathData);
								}
								if ((kevent.fflags & FilterFlags.VNodeWrite) == FilterFlags.VNodeWrite)
								{
									if (pathData.IsDirectory)
									{
										list2.Add(pathData.Path);
									}
									else
									{
										this.PostEvent(FileAction.Modified, pathData.Path, null);
									}
								}
								if ((kevent.fflags & FilterFlags.VNodeAttrib) == FilterFlags.VNodeAttrib || (kevent.fflags & FilterFlags.VNodeExtend) == FilterFlags.VNodeExtend)
								{
									this.PostEvent(FileAction.Modified, pathData.Path, null);
								}
							}
						}
					}
					list.ForEach(new Action<PathData>(this.Remove));
					list.Clear();
					List<string> list3 = list2;
					Action<string> action;
					if ((action = <>9__0) == null)
					{
						action = (<>9__0 = delegate(string path)
						{
							this.Scan(path, true, ref newFds);
						});
					}
					list3.ForEach(action);
					list2.Clear();
				}
			}
		}

		private PathData Add(string path, bool postEvents, ref List<int> fds)
		{
			PathData pathData;
			this.pathsDict.TryGetValue(path, out pathData);
			if (pathData != null)
			{
				return pathData;
			}
			if (this.fdsDict.Count >= this.maxFds)
			{
				throw new IOException("kqueue() FileSystemWatcher has reached the maximum number of files to watch.");
			}
			int num = KqueueMonitor.open(path, 32768, 0);
			if (num == -1)
			{
				this.fsw.DispatchErrorEvents(new ErrorEventArgs(new IOException(string.Format("open() error while attempting to process path '{0}', error code = '{1}'", path, Marshal.GetLastWin32Error()))));
				return null;
			}
			PathData pathData2;
			try
			{
				fds.Add(num);
				FileAttributes attributes = File.GetAttributes(path);
				pathData = new PathData
				{
					Path = path,
					Fd = num,
					IsDirectory = ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
				};
				this.pathsDict.Add(path, pathData);
				this.fdsDict.Add(num, pathData);
				if (postEvents)
				{
					this.PostEvent(FileAction.Added, path, null);
				}
				pathData2 = pathData;
			}
			catch (Exception ex)
			{
				KqueueMonitor.close(num);
				this.fsw.DispatchErrorEvents(new ErrorEventArgs(ex));
				pathData2 = null;
			}
			return pathData2;
		}

		private void Remove(PathData pathData)
		{
			this.fdsDict.Remove(pathData.Fd);
			this.pathsDict.Remove(pathData.Path);
			KqueueMonitor.close(pathData.Fd);
			this.PostEvent(FileAction.Removed, pathData.Path, null);
		}

		private void RemoveTree(PathData pathData)
		{
			List<PathData> list = new List<PathData>();
			list.Add(pathData);
			if (pathData.IsDirectory)
			{
				string text = pathData.Path + Path.DirectorySeparatorChar.ToString();
				foreach (string text2 in this.pathsDict.Keys)
				{
					if (text2.StartsWith(text))
					{
						list.Add(this.pathsDict[text2]);
					}
				}
			}
			list.ForEach(new Action<PathData>(this.Remove));
		}

		private void UpdatePath(PathData pathData)
		{
			string filenameFromFd = this.GetFilenameFromFd(pathData.Fd);
			if (!filenameFromFd.StartsWith(this.fullPathNoLastSlash))
			{
				this.RemoveTree(pathData);
				return;
			}
			List<PathData> list = new List<PathData>();
			string path = pathData.Path;
			list.Add(pathData);
			if (pathData.IsDirectory)
			{
				string text = path + Path.DirectorySeparatorChar.ToString();
				foreach (string text2 in this.pathsDict.Keys)
				{
					if (text2.StartsWith(text))
					{
						list.Add(this.pathsDict[text2]);
					}
				}
			}
			foreach (PathData pathData2 in list)
			{
				string path2 = pathData2.Path;
				string text3 = filenameFromFd + path2.Substring(path.Length);
				pathData2.Path = text3;
				this.pathsDict.Remove(path2);
				if (this.pathsDict.ContainsKey(text3))
				{
					PathData pathData3 = this.pathsDict[text3];
					if (this.GetFilenameFromFd(pathData2.Fd) == this.GetFilenameFromFd(pathData3.Fd))
					{
						this.Remove(pathData3);
					}
					else
					{
						this.UpdatePath(pathData3);
					}
				}
				this.pathsDict.Add(text3, pathData2);
			}
			this.PostEvent(FileAction.RenamedNewName, path, filenameFromFd);
		}

		private void Scan(string path, bool postEvents, ref List<int> fds)
		{
			if (this.requestStop)
			{
				return;
			}
			PathData pathData = this.Add(path, postEvents, ref fds);
			if (pathData == null)
			{
				return;
			}
			if (!pathData.IsDirectory)
			{
				return;
			}
			List<string> list = new List<string>();
			list.Add(path);
			while (list.Count > 0)
			{
				string text = list[0];
				list.RemoveAt(0);
				DirectoryInfo directoryInfo = new DirectoryInfo(text);
				FileSystemInfo[] array = null;
				try
				{
					array = directoryInfo.GetFileSystemInfos();
				}
				catch (IOException)
				{
					array = new FileSystemInfo[0];
				}
				foreach (FileSystemInfo fileSystemInfo in array)
				{
					if (((fileSystemInfo.Attributes & FileAttributes.Directory) != FileAttributes.Directory || this.fsw.IncludeSubdirectories) && ((fileSystemInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory || this.fsw.Pattern.IsMatch(fileSystemInfo.FullName)))
					{
						PathData pathData2 = this.Add(fileSystemInfo.FullName, postEvents, ref fds);
						if (pathData2 != null && pathData2.IsDirectory)
						{
							list.Add(fileSystemInfo.FullName);
						}
					}
				}
			}
		}

		private void PostEvent(FileAction action, string path, string newPath = null)
		{
			RenamedEventArgs e = null;
			if (this.requestStop || action == (FileAction)0)
			{
				return;
			}
			string text = ((path.Length > this.fullPathNoLastSlash.Length) ? path.Substring(this.fullPathNoLastSlash.Length + 1) : string.Empty);
			if (!this.fsw.Pattern.IsMatch(path) && (newPath == null || !this.fsw.Pattern.IsMatch(newPath)))
			{
				return;
			}
			if (action == FileAction.RenamedNewName)
			{
				string text2 = ((newPath.Length > this.fullPathNoLastSlash.Length) ? newPath.Substring(this.fullPathNoLastSlash.Length + 1) : string.Empty);
				e = new RenamedEventArgs(WatcherChangeTypes.Renamed, this.fsw.Path, text2, text);
			}
			this.fsw.DispatchEvents(action, text, ref e);
			if (this.fsw.Waiting)
			{
				FileSystemWatcher fileSystemWatcher = this.fsw;
				lock (fileSystemWatcher)
				{
					this.fsw.Waiting = false;
					global::System.Threading.Monitor.PulseAll(this.fsw);
				}
			}
		}

		private string GetFilenameFromFd(int fd)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			if (KqueueMonitor.fcntl(fd, 50, stringBuilder) != -1)
			{
				if (this.fixupPath != null)
				{
					stringBuilder.Replace(this.fixupPath, this.fullPathNoLastSlash, 0, this.fixupPath.Length);
				}
				return stringBuilder.ToString();
			}
			this.fsw.DispatchErrorEvents(new ErrorEventArgs(new IOException(string.Format("fcntl() error while attempting to get path for fd '{0}', error code = '{1}'", fd, Marshal.GetLastWin32Error()))));
			return string.Empty;
		}

		[DllImport("libc", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int fcntl(int file_names_by_descriptor, int cmd, StringBuilder sb);

		[DllImport("libc", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr realpath(string pathname, StringBuilder sb);

		[DllImport("libc", SetLastError = true)]
		private static extern int open(string path, int flags, int mode_t);

		[DllImport("libc")]
		private static extern int close(int fd);

		[DllImport("libc", SetLastError = true)]
		private static extern int kqueue();

		[DllImport("libc", SetLastError = true)]
		private static extern int kevent(int kq, [In] kevent[] ev, int nchanges, [Out] kevent[] evtlist, int nevents, [In] ref timespec time);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int kevent_notimeout(ref int kq, IntPtr ev, int nchanges, IntPtr evtlist, int nevents);

		private static bool initialized;

		private const int O_EVTONLY = 32768;

		private const int F_GETPATH = 50;

		private const int __DARWIN_MAXPATHLEN = 1024;

		private const int EINTR = 4;

		private static readonly kevent[] emptyEventList = new kevent[0];

		private int maxFds = int.MaxValue;

		private FileSystemWatcher fsw;

		private int conn;

		private Thread thread;

		private volatile bool requestStop;

		private AutoResetEvent startedEvent = new AutoResetEvent(false);

		private bool started;

		private bool inDispatch;

		private Exception exc;

		private object stateLock = new object();

		private object connLock = new object();

		private readonly Dictionary<string, PathData> pathsDict = new Dictionary<string, PathData>();

		private readonly Dictionary<int, PathData> fdsDict = new Dictionary<int, PathData>();

		private string fixupPath;

		private string fullPathNoLastSlash;
	}
}
