using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO
{
	[IODescription("")]
	[DefaultEvent("Changed")]
	public class FileSystemWatcher : Component, ISupportInitialize
	{
		public FileSystemWatcher()
		{
			this.notifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite;
			this.enableRaisingEvents = false;
			this.filter = "*";
			this.includeSubdirectories = false;
			this.internalBufferSize = 8192;
			this.path = "";
			this.InitWatcher();
		}

		public FileSystemWatcher(string path)
			: this(path, "*")
		{
		}

		public FileSystemWatcher(string path, string filter)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			if (path == string.Empty)
			{
				throw new ArgumentException("Empty path", "path");
			}
			if (!Directory.Exists(path))
			{
				throw new ArgumentException("Directory does not exist", "path");
			}
			this.inited = false;
			this.start_requested = false;
			this.enableRaisingEvents = false;
			this.filter = filter;
			if (this.filter == "*.*")
			{
				this.filter = "*";
			}
			this.includeSubdirectories = false;
			this.internalBufferSize = 8192;
			this.notifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite;
			this.path = path;
			this.synchronizingObject = null;
			this.InitWatcher();
		}

		[EnvironmentPermission(SecurityAction.Assert, Read = "MONO_MANAGED_WATCHER")]
		private void InitWatcher()
		{
			object obj = FileSystemWatcher.lockobj;
			lock (obj)
			{
				if (this.watcher_handle == null)
				{
					string environmentVariable = Environment.GetEnvironmentVariable("MONO_MANAGED_WATCHER");
					int num = 0;
					bool flag2 = false;
					if (environmentVariable == null)
					{
						num = FileSystemWatcher.InternalSupportsFSW();
					}
					switch (num)
					{
					case 1:
						flag2 = DefaultWatcher.GetInstance(out this.watcher);
						this.watcher_handle = this;
						break;
					case 2:
						flag2 = FAMWatcher.GetInstance(out this.watcher, false);
						this.watcher_handle = this;
						break;
					case 3:
						flag2 = KeventWatcher.GetInstance(out this.watcher);
						this.watcher_handle = this;
						break;
					case 4:
						flag2 = FAMWatcher.GetInstance(out this.watcher, true);
						this.watcher_handle = this;
						break;
					case 6:
						flag2 = CoreFXFileSystemWatcherProxy.GetInstance(out this.watcher);
						this.watcher_handle = (this.watcher as CoreFXFileSystemWatcherProxy).NewWatcher(this);
						break;
					}
					if (num == 0 || !flag2)
					{
						if (string.Compare(environmentVariable, "disabled", true) == 0)
						{
							NullFileWatcher.GetInstance(out this.watcher);
						}
						else
						{
							DefaultWatcher.GetInstance(out this.watcher);
							this.watcher_handle = this;
						}
					}
					this.inited = true;
				}
			}
		}

		[Conditional("DEBUG")]
		[Conditional("TRACE")]
		private void ShowWatcherInfo()
		{
			Console.WriteLine("Watcher implementation: {0}", (this.watcher != null) ? this.watcher.GetType().ToString() : "<none>");
		}

		internal bool Waiting
		{
			get
			{
				return this.waiting;
			}
			set
			{
				this.waiting = value;
			}
		}

		internal string MangledFilter
		{
			get
			{
				if (this.filter != "*.*")
				{
					return this.filter;
				}
				if (this.mangledFilter != null)
				{
					return this.mangledFilter;
				}
				return "*.*";
			}
		}

		internal SearchPattern2 Pattern
		{
			get
			{
				if (this.pattern == null)
				{
					IFileWatcher fileWatcher = this.watcher;
					if (((fileWatcher != null) ? fileWatcher.GetType() : null) == typeof(KeventWatcher))
					{
						this.pattern = new SearchPattern2(this.MangledFilter, true);
					}
					else
					{
						this.pattern = new SearchPattern2(this.MangledFilter);
					}
				}
				return this.pattern;
			}
		}

		internal string FullPath
		{
			get
			{
				if (this.fullpath == null)
				{
					if (this.path == null || this.path == "")
					{
						this.fullpath = Environment.CurrentDirectory;
					}
					else
					{
						this.fullpath = global::System.IO.Path.GetFullPath(this.path);
					}
				}
				return this.fullpath;
			}
		}

		[IODescription("Flag to indicate if this instance is active")]
		[DefaultValue(false)]
		public bool EnableRaisingEvents
		{
			get
			{
				return this.enableRaisingEvents;
			}
			set
			{
				if (this.disposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				this.start_requested = true;
				if (!this.inited)
				{
					return;
				}
				if (value == this.enableRaisingEvents)
				{
					return;
				}
				this.enableRaisingEvents = value;
				if (value)
				{
					this.Start();
					return;
				}
				this.Stop();
				this.start_requested = false;
			}
		}

		[TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[SettingsBindable(true)]
		[IODescription("File name filter pattern")]
		[DefaultValue("*.*")]
		public string Filter
		{
			get
			{
				return this.filter;
			}
			set
			{
				if (value == null || value == "")
				{
					value = "*";
				}
				if (!string.Equals(this.filter, value, PathInternal.StringComparison))
				{
					this.filter = ((value == "*.*") ? "*" : value);
					this.pattern = null;
					this.mangledFilter = null;
				}
			}
		}

		[IODescription("Flag to indicate we want to watch subdirectories")]
		[DefaultValue(false)]
		public bool IncludeSubdirectories
		{
			get
			{
				return this.includeSubdirectories;
			}
			set
			{
				if (this.includeSubdirectories == value)
				{
					return;
				}
				this.includeSubdirectories = value;
				if (value && this.enableRaisingEvents)
				{
					this.Stop();
					this.Start();
				}
			}
		}

		[Browsable(false)]
		[DefaultValue(8192)]
		public int InternalBufferSize
		{
			get
			{
				return this.internalBufferSize;
			}
			set
			{
				if (this.internalBufferSize == value)
				{
					return;
				}
				if (value < 4096)
				{
					value = 4096;
				}
				this.internalBufferSize = value;
				if (this.enableRaisingEvents)
				{
					this.Stop();
					this.Start();
				}
			}
		}

		[DefaultValue(NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite)]
		[IODescription("Flag to indicate which change event we want to monitor")]
		public NotifyFilters NotifyFilter
		{
			get
			{
				return this.notifyFilter;
			}
			set
			{
				if (this.notifyFilter == value)
				{
					return;
				}
				this.notifyFilter = value;
				if (this.enableRaisingEvents)
				{
					this.Stop();
					this.Start();
				}
			}
		}

		[IODescription("The directory to monitor")]
		[Editor("System.Diagnostics.Design.FSWPathEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[DefaultValue("")]
		[TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[SettingsBindable(true)]
		public string Path
		{
			get
			{
				return this.path;
			}
			set
			{
				if (this.disposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				value = ((value == null) ? string.Empty : value);
				if (string.Equals(this.path, value, PathInternal.StringComparison))
				{
					return;
				}
				bool flag = false;
				Exception ex = null;
				try
				{
					flag = Directory.Exists(value);
				}
				catch (Exception ex)
				{
				}
				if (ex != null)
				{
					throw new ArgumentException(SR.Format("The directory name {0} is invalid.", value), "Path");
				}
				if (!flag)
				{
					throw new ArgumentException(SR.Format("The directory name '{0}' does not exist.", value), "Path");
				}
				this.path = value;
				this.fullpath = null;
				if (this.enableRaisingEvents)
				{
					this.Stop();
					this.Start();
				}
			}
		}

		[Browsable(false)]
		public override ISite Site
		{
			get
			{
				return base.Site;
			}
			set
			{
				base.Site = value;
				if (this.Site != null && this.Site.DesignMode)
				{
					this.EnableRaisingEvents = true;
				}
			}
		}

		[Browsable(false)]
		[IODescription("The object used to marshal the event handler calls resulting from a directory change")]
		[DefaultValue(null)]
		public ISynchronizeInvoke SynchronizingObject
		{
			get
			{
				return this.synchronizingObject;
			}
			set
			{
				this.synchronizingObject = value;
			}
		}

		public void BeginInit()
		{
			this.inited = false;
		}

		protected override void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}
			try
			{
				IFileWatcher fileWatcher = this.watcher;
				if (fileWatcher != null)
				{
					fileWatcher.StopDispatching(this.watcher_handle);
				}
				IFileWatcher fileWatcher2 = this.watcher;
				if (fileWatcher2 != null)
				{
					fileWatcher2.Dispose(this.watcher_handle);
				}
			}
			catch (Exception)
			{
			}
			this.watcher_handle = null;
			this.watcher = null;
			this.disposed = true;
			base.Dispose(disposing);
			GC.SuppressFinalize(this);
		}

		~FileSystemWatcher()
		{
			if (!this.disposed)
			{
				this.Dispose(false);
			}
		}

		public void EndInit()
		{
			this.inited = true;
			if (this.start_requested)
			{
				this.EnableRaisingEvents = true;
			}
		}

		private void RaiseEvent(Delegate ev, EventArgs arg, FileSystemWatcher.EventType evtype)
		{
			if (this.disposed)
			{
				return;
			}
			if (ev == null)
			{
				return;
			}
			if (this.synchronizingObject == null)
			{
				foreach (Delegate @delegate in ev.GetInvocationList())
				{
					switch (evtype)
					{
					case FileSystemWatcher.EventType.FileSystemEvent:
						((FileSystemEventHandler)@delegate)(this, (FileSystemEventArgs)arg);
						break;
					case FileSystemWatcher.EventType.ErrorEvent:
						((ErrorEventHandler)@delegate)(this, (ErrorEventArgs)arg);
						break;
					case FileSystemWatcher.EventType.RenameEvent:
						((RenamedEventHandler)@delegate)(this, (RenamedEventArgs)arg);
						break;
					}
				}
				return;
			}
			this.synchronizingObject.BeginInvoke(ev, new object[] { this, arg });
		}

		protected void OnChanged(FileSystemEventArgs e)
		{
			this.RaiseEvent(this.Changed, e, FileSystemWatcher.EventType.FileSystemEvent);
		}

		protected void OnCreated(FileSystemEventArgs e)
		{
			this.RaiseEvent(this.Created, e, FileSystemWatcher.EventType.FileSystemEvent);
		}

		protected void OnDeleted(FileSystemEventArgs e)
		{
			this.RaiseEvent(this.Deleted, e, FileSystemWatcher.EventType.FileSystemEvent);
		}

		protected void OnError(ErrorEventArgs e)
		{
			this.RaiseEvent(this.Error, e, FileSystemWatcher.EventType.ErrorEvent);
		}

		protected void OnRenamed(RenamedEventArgs e)
		{
			this.RaiseEvent(this.Renamed, e, FileSystemWatcher.EventType.RenameEvent);
		}

		public WaitForChangedResult WaitForChanged(WatcherChangeTypes changeType)
		{
			return this.WaitForChanged(changeType, -1);
		}

		public WaitForChangedResult WaitForChanged(WatcherChangeTypes changeType, int timeout)
		{
			WaitForChangedResult waitForChangedResult = default(WaitForChangedResult);
			bool flag = this.EnableRaisingEvents;
			if (!flag)
			{
				this.EnableRaisingEvents = true;
			}
			bool flag3;
			lock (this)
			{
				this.waiting = true;
				flag3 = Monitor.Wait(this, timeout);
				if (flag3)
				{
					waitForChangedResult = this.lastData;
				}
			}
			this.EnableRaisingEvents = flag;
			if (!flag3)
			{
				waitForChangedResult.TimedOut = true;
			}
			return waitForChangedResult;
		}

		internal void DispatchErrorEvents(ErrorEventArgs args)
		{
			if (this.disposed)
			{
				return;
			}
			this.OnError(args);
		}

		internal void DispatchEvents(FileAction act, string filename, ref RenamedEventArgs renamed)
		{
			FileSystemWatcher.<>c__DisplayClass70_0 CS$<>8__locals1 = new FileSystemWatcher.<>c__DisplayClass70_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.filename = filename;
			if (this.disposed)
			{
				return;
			}
			if (this.waiting)
			{
				this.lastData = default(WaitForChangedResult);
			}
			switch (act)
			{
			case FileAction.Added:
				this.lastData.Name = CS$<>8__locals1.filename;
				this.lastData.ChangeType = WatcherChangeTypes.Created;
				Task.Run(delegate
				{
					CS$<>8__locals1.<>4__this.OnCreated(new FileSystemEventArgs(WatcherChangeTypes.Created, CS$<>8__locals1.<>4__this.path, CS$<>8__locals1.filename));
				});
				return;
			case FileAction.Removed:
				this.lastData.Name = CS$<>8__locals1.filename;
				this.lastData.ChangeType = WatcherChangeTypes.Deleted;
				Task.Run(delegate
				{
					CS$<>8__locals1.<>4__this.OnDeleted(new FileSystemEventArgs(WatcherChangeTypes.Deleted, CS$<>8__locals1.<>4__this.path, CS$<>8__locals1.filename));
				});
				return;
			case FileAction.Modified:
				this.lastData.Name = CS$<>8__locals1.filename;
				this.lastData.ChangeType = WatcherChangeTypes.Changed;
				Task.Run(delegate
				{
					CS$<>8__locals1.<>4__this.OnChanged(new FileSystemEventArgs(WatcherChangeTypes.Changed, CS$<>8__locals1.<>4__this.path, CS$<>8__locals1.filename));
				});
				return;
			case FileAction.RenamedOldName:
				if (renamed != null)
				{
					this.OnRenamed(renamed);
				}
				this.lastData.OldName = CS$<>8__locals1.filename;
				this.lastData.ChangeType = WatcherChangeTypes.Renamed;
				renamed = new RenamedEventArgs(WatcherChangeTypes.Renamed, this.path, CS$<>8__locals1.filename, "");
				return;
			case FileAction.RenamedNewName:
			{
				this.lastData.Name = CS$<>8__locals1.filename;
				this.lastData.ChangeType = WatcherChangeTypes.Renamed;
				if (renamed == null)
				{
					renamed = new RenamedEventArgs(WatcherChangeTypes.Renamed, this.path, "", CS$<>8__locals1.filename);
				}
				RenamedEventArgs renamed_ref = renamed;
				Task.Run(delegate
				{
					CS$<>8__locals1.<>4__this.OnRenamed(renamed_ref);
				});
				renamed = null;
				return;
			}
			default:
				return;
			}
		}

		private void Start()
		{
			if (this.disposed)
			{
				return;
			}
			if (this.watcher_handle == null)
			{
				return;
			}
			IFileWatcher fileWatcher = this.watcher;
			if (fileWatcher == null)
			{
				return;
			}
			fileWatcher.StartDispatching(this.watcher_handle);
		}

		private void Stop()
		{
			if (this.disposed)
			{
				return;
			}
			if (this.watcher_handle == null)
			{
				return;
			}
			IFileWatcher fileWatcher = this.watcher;
			if (fileWatcher == null)
			{
				return;
			}
			fileWatcher.StopDispatching(this.watcher_handle);
		}

		[IODescription("Occurs when a file/directory change matches the filter")]
		public event FileSystemEventHandler Changed;

		[IODescription("Occurs when a file/directory creation matches the filter")]
		public event FileSystemEventHandler Created;

		[IODescription("Occurs when a file/directory deletion matches the filter")]
		public event FileSystemEventHandler Deleted;

		[Browsable(false)]
		public event ErrorEventHandler Error;

		[IODescription("Occurs when a file/directory rename matches the filter")]
		public event RenamedEventHandler Renamed;

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int InternalSupportsFSW();

		private bool inited;

		private bool start_requested;

		private bool enableRaisingEvents;

		private string filter;

		private bool includeSubdirectories;

		private int internalBufferSize;

		private NotifyFilters notifyFilter;

		private string path;

		private string fullpath;

		private ISynchronizeInvoke synchronizingObject;

		private WaitForChangedResult lastData;

		private bool waiting;

		private SearchPattern2 pattern;

		private bool disposed;

		private string mangledFilter;

		private IFileWatcher watcher;

		private object watcher_handle;

		private static object lockobj = new object();

		private enum EventType
		{
			FileSystemEvent,
			ErrorEvent,
			RenameEvent
		}
	}
}
