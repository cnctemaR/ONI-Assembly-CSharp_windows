using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Threading;

namespace System.IO
{
	[global::System.ComponentModel.DefaultEvent("Changed")]
	[IODescription("")]
	public class FileSystemWatcher : global::System.ComponentModel.Component, global::System.ComponentModel.ISupportInitialize
	{
		public FileSystemWatcher()
		{
			this.notifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite;
			this.enableRaisingEvents = false;
			this.filter = "*.*";
			this.includeSubdirectories = false;
			this.internalBufferSize = 8192;
			this.path = string.Empty;
			this.InitWatcher();
		}

		public FileSystemWatcher(string path)
			: this(path, "*.*")
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
				throw new ArgumentException("Directory does not exists", "path");
			}
			this.enableRaisingEvents = false;
			this.filter = filter;
			this.includeSubdirectories = false;
			this.internalBufferSize = 8192;
			this.notifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite;
			this.path = path;
			this.synchronizingObject = null;
			this.InitWatcher();
		}

		[IODescription("Occurs when a file/directory change matches the filter")]
		public event FileSystemEventHandler Changed;

		[IODescription("Occurs when a file/directory creation matches the filter")]
		public event FileSystemEventHandler Created;

		[IODescription("Occurs when a file/directory deletion matches the filter")]
		public event FileSystemEventHandler Deleted;

		[global::System.ComponentModel.Browsable(false)]
		public event ErrorEventHandler Error;

		[IODescription("Occurs when a file/directory rename matches the filter")]
		public event RenamedEventHandler Renamed;

		[PermissionSet(SecurityAction.Assert, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nRead=\"MONO_MANAGED_WATCHER\"\nWrite=\"\"/>\n</PermissionSet>\n")]
		private void InitWatcher()
		{
			object obj = FileSystemWatcher.lockobj;
			lock (obj)
			{
				if (FileSystemWatcher.watcher == null)
				{
					string environmentVariable = Environment.GetEnvironmentVariable("MONO_MANAGED_WATCHER");
					int num = 0;
					if (environmentVariable == null)
					{
						num = FileSystemWatcher.InternalSupportsFSW();
					}
					bool flag = false;
					switch (num)
					{
					case 1:
						flag = DefaultWatcher.GetInstance(out FileSystemWatcher.watcher);
						break;
					case 2:
						flag = FAMWatcher.GetInstance(out FileSystemWatcher.watcher, false);
						break;
					case 3:
						flag = KeventWatcher.GetInstance(out FileSystemWatcher.watcher);
						break;
					case 4:
						flag = FAMWatcher.GetInstance(out FileSystemWatcher.watcher, true);
						break;
					case 5:
						flag = InotifyWatcher.GetInstance(out FileSystemWatcher.watcher, true);
						break;
					}
					if (num == 0 || !flag)
					{
						if (string.Compare(environmentVariable, "disabled", true) == 0)
						{
							NullFileWatcher.GetInstance(out FileSystemWatcher.watcher);
						}
						else
						{
							DefaultWatcher.GetInstance(out FileSystemWatcher.watcher);
						}
					}
				}
			}
		}

		[Conditional("DEBUG")]
		[Conditional("TRACE")]
		private void ShowWatcherInfo()
		{
			Console.WriteLine("Watcher implementation: {0}", (FileSystemWatcher.watcher == null) ? "<none>" : FileSystemWatcher.watcher.GetType().ToString());
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
				string text = "*.*";
				if (FileSystemWatcher.watcher.GetType() != typeof(WindowsWatcher))
				{
					text = "*";
				}
				return text;
			}
		}

		internal SearchPattern2 Pattern
		{
			get
			{
				if (this.pattern == null)
				{
					this.pattern = new SearchPattern2(this.MangledFilter);
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
					if (this.path == null || this.path == string.Empty)
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
		[global::System.ComponentModel.DefaultValue(false)]
		public bool EnableRaisingEvents
		{
			get
			{
				return this.enableRaisingEvents;
			}
			set
			{
				if (value == this.enableRaisingEvents)
				{
					return;
				}
				this.enableRaisingEvents = value;
				if (value)
				{
					this.Start();
				}
				else
				{
					this.Stop();
				}
			}
		}

		[global::System.ComponentModel.RecommendedAsConfigurable(true)]
		[global::System.ComponentModel.TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[global::System.ComponentModel.DefaultValue("*.*")]
		[IODescription("File name filter pattern")]
		public string Filter
		{
			get
			{
				return this.filter;
			}
			set
			{
				if (value == null || value == string.Empty)
				{
					value = "*.*";
				}
				if (this.filter != value)
				{
					this.filter = value;
					this.pattern = null;
					this.mangledFilter = null;
				}
			}
		}

		[global::System.ComponentModel.DefaultValue(false)]
		[IODescription("Flag to indicate we want to watch subdirectories")]
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

		[global::System.ComponentModel.DefaultValue(8192)]
		[global::System.ComponentModel.Browsable(false)]
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
				if (value < 4196)
				{
					value = 4196;
				}
				this.internalBufferSize = value;
				if (this.enableRaisingEvents)
				{
					this.Stop();
					this.Start();
				}
			}
		}

		[global::System.ComponentModel.DefaultValue(NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite)]
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

		[global::System.ComponentModel.DefaultValue("")]
		[global::System.ComponentModel.RecommendedAsConfigurable(true)]
		[global::System.ComponentModel.Editor("System.Diagnostics.Design.FSWPathEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[global::System.ComponentModel.TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[IODescription("The directory to monitor")]
		public string Path
		{
			get
			{
				return this.path;
			}
			set
			{
				if (this.path == value)
				{
					return;
				}
				bool flag = false;
				Exception ex = null;
				try
				{
					flag = Directory.Exists(value);
				}
				catch (Exception ex2)
				{
					ex = ex2;
				}
				if (ex != null)
				{
					throw new ArgumentException("Invalid directory name", "value", ex);
				}
				if (!flag)
				{
					throw new ArgumentException("Directory does not exists", "value");
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

		[global::System.ComponentModel.Browsable(false)]
		public override global::System.ComponentModel.ISite Site
		{
			get
			{
				return base.Site;
			}
			set
			{
				base.Site = value;
			}
		}

		[global::System.ComponentModel.Browsable(false)]
		[global::System.ComponentModel.DefaultValue(null)]
		[IODescription("The object used to marshal the event handler calls resulting from a directory change")]
		public global::System.ComponentModel.ISynchronizeInvoke SynchronizingObject
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
		}

		protected override void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				this.disposed = true;
				this.Stop();
			}
			base.Dispose(disposing);
		}

		~FileSystemWatcher()
		{
			this.disposed = true;
			this.Stop();
		}

		public void EndInit()
		{
		}

		private void RaiseEvent(Delegate ev, EventArgs arg, FileSystemWatcher.EventType evtype)
		{
			if (ev == null)
			{
				return;
			}
			if (this.synchronizingObject == null)
			{
				switch (evtype)
				{
				case FileSystemWatcher.EventType.FileSystemEvent:
					((FileSystemEventHandler)ev).BeginInvoke(this, (FileSystemEventArgs)arg, null, null);
					break;
				case FileSystemWatcher.EventType.ErrorEvent:
					((ErrorEventHandler)ev).BeginInvoke(this, (ErrorEventArgs)arg, null, null);
					break;
				case FileSystemWatcher.EventType.RenameEvent:
					((RenamedEventHandler)ev).BeginInvoke(this, (RenamedEventArgs)arg, null, null);
					break;
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
			bool flag2;
			lock (this)
			{
				this.waiting = true;
				flag2 = Monitor.Wait(this, timeout);
				if (flag2)
				{
					waitForChangedResult = this.lastData;
				}
			}
			this.EnableRaisingEvents = flag;
			if (!flag2)
			{
				waitForChangedResult.TimedOut = true;
			}
			return waitForChangedResult;
		}

		internal void DispatchEvents(FileAction act, string filename, ref RenamedEventArgs renamed)
		{
			if (this.waiting)
			{
				this.lastData = default(WaitForChangedResult);
			}
			switch (act)
			{
			case FileAction.Added:
				this.lastData.Name = filename;
				this.lastData.ChangeType = WatcherChangeTypes.Created;
				this.OnCreated(new FileSystemEventArgs(WatcherChangeTypes.Created, this.path, filename));
				break;
			case FileAction.Removed:
				this.lastData.Name = filename;
				this.lastData.ChangeType = WatcherChangeTypes.Deleted;
				this.OnDeleted(new FileSystemEventArgs(WatcherChangeTypes.Deleted, this.path, filename));
				break;
			case FileAction.Modified:
				this.lastData.Name = filename;
				this.lastData.ChangeType = WatcherChangeTypes.Changed;
				this.OnChanged(new FileSystemEventArgs(WatcherChangeTypes.Changed, this.path, filename));
				break;
			case FileAction.RenamedOldName:
				if (renamed != null)
				{
					this.OnRenamed(renamed);
				}
				this.lastData.OldName = filename;
				this.lastData.ChangeType = WatcherChangeTypes.Renamed;
				renamed = new RenamedEventArgs(WatcherChangeTypes.Renamed, this.path, filename, string.Empty);
				break;
			case FileAction.RenamedNewName:
				this.lastData.Name = filename;
				this.lastData.ChangeType = WatcherChangeTypes.Renamed;
				if (renamed == null)
				{
					renamed = new RenamedEventArgs(WatcherChangeTypes.Renamed, this.path, string.Empty, filename);
				}
				this.OnRenamed(renamed);
				renamed = null;
				break;
			}
		}

		private void Start()
		{
			FileSystemWatcher.watcher.StartDispatching(this);
		}

		private void Stop()
		{
			FileSystemWatcher.watcher.StopDispatching(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int InternalSupportsFSW();

		private bool enableRaisingEvents;

		private string filter;

		private bool includeSubdirectories;

		private int internalBufferSize;

		private NotifyFilters notifyFilter;

		private string path;

		private string fullpath;

		private global::System.ComponentModel.ISynchronizeInvoke synchronizingObject;

		private WaitForChangedResult lastData;

		private bool waiting;

		private SearchPattern2 pattern;

		private bool disposed;

		private string mangledFilter;

		private static IFileWatcher watcher;

		private static object lockobj = new object();

		private enum EventType
		{
			FileSystemEvent,
			ErrorEvent,
			RenameEvent
		}
	}
}
