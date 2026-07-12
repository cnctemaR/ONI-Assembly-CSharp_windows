using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Enumeration;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace System.IO.CoreFX
{
	public class FileSystemWatcher : Component, ISupportInitialize
	{
		private void StartRaisingEvents()
		{
			if (this.IsSuspended())
			{
				this._enabled = true;
				return;
			}
			if (!FileSystemWatcher.IsHandleInvalid(this._directoryHandle))
			{
				return;
			}
			this._directoryHandle = global::Interop.Kernel32.CreateFile(this._directory, 1, FileShare.Read | FileShare.Write | FileShare.Delete, FileMode.Open, 1107296256);
			if (FileSystemWatcher.IsHandleInvalid(this._directoryHandle))
			{
				this._directoryHandle = null;
				throw new FileNotFoundException(SR.Format("Error reading the {0} directory.", this._directory));
			}
			FileSystemWatcher.AsyncReadState asyncReadState;
			try
			{
				int num = Interlocked.Increment(ref this._currentSession);
				byte[] array = this.AllocateBuffer();
				asyncReadState = new FileSystemWatcher.AsyncReadState(num, array, this._directoryHandle, ThreadPoolBoundHandle.BindHandle(this._directoryHandle));
				asyncReadState.PreAllocatedOverlapped = new PreAllocatedOverlapped(new IOCompletionCallback(this.ReadDirectoryChangesCallback), asyncReadState, array);
			}
			catch
			{
				this._directoryHandle.Dispose();
				this._directoryHandle = null;
				throw;
			}
			this._enabled = true;
			this.Monitor(asyncReadState);
		}

		private void StopRaisingEvents()
		{
			this._enabled = false;
			if (this.IsSuspended())
			{
				return;
			}
			if (FileSystemWatcher.IsHandleInvalid(this._directoryHandle))
			{
				return;
			}
			Interlocked.Increment(ref this._currentSession);
			this._directoryHandle.Dispose();
			this._directoryHandle = null;
		}

		private void FinalizeDispose()
		{
			if (!FileSystemWatcher.IsHandleInvalid(this._directoryHandle))
			{
				this._directoryHandle.Dispose();
			}
		}

		private static bool IsHandleInvalid(SafeFileHandle handle)
		{
			return handle == null || handle.IsInvalid || handle.IsClosed;
		}

		private unsafe void Monitor(FileSystemWatcher.AsyncReadState state)
		{
			NativeOverlapped* ptr = null;
			bool flag = false;
			try
			{
				if (this._enabled && !FileSystemWatcher.IsHandleInvalid(state.DirectoryHandle))
				{
					ptr = state.ThreadPoolBinding.AllocateNativeOverlapped(state.PreAllocatedOverlapped);
					int num;
					flag = global::Interop.Kernel32.ReadDirectoryChangesW(state.DirectoryHandle, state.Buffer, this._internalBufferSize, this._includeSubdirectories, (int)this._notifyFilters, out num, ptr, IntPtr.Zero);
				}
			}
			catch (ObjectDisposedException)
			{
			}
			catch (ArgumentNullException)
			{
			}
			finally
			{
				if (!flag)
				{
					if (ptr != null)
					{
						state.ThreadPoolBinding.FreeNativeOverlapped(ptr);
					}
					state.PreAllocatedOverlapped.Dispose();
					state.ThreadPoolBinding.Dispose();
					if (!FileSystemWatcher.IsHandleInvalid(state.DirectoryHandle))
					{
						this.OnError(new ErrorEventArgs(new Win32Exception()));
					}
				}
			}
		}

		private unsafe void ReadDirectoryChangesCallback(uint errorCode, uint numBytes, NativeOverlapped* overlappedPointer)
		{
			FileSystemWatcher.AsyncReadState asyncReadState = (FileSystemWatcher.AsyncReadState)ThreadPoolBoundHandle.GetNativeOverlappedState(overlappedPointer);
			try
			{
				if (!FileSystemWatcher.IsHandleInvalid(asyncReadState.DirectoryHandle))
				{
					if (errorCode != 0U)
					{
						if (errorCode != 995U)
						{
							this.OnError(new ErrorEventArgs(new Win32Exception((int)errorCode)));
							this.EnableRaisingEvents = false;
						}
					}
					else if (asyncReadState.Session == Volatile.Read(ref this._currentSession))
					{
						if (numBytes == 0U)
						{
							this.NotifyInternalBufferOverflowEvent();
						}
						else
						{
							this.ParseEventBufferAndNotifyForEach(asyncReadState.Buffer);
						}
					}
				}
			}
			finally
			{
				asyncReadState.ThreadPoolBinding.FreeNativeOverlapped(overlappedPointer);
				this.Monitor(asyncReadState);
			}
		}

		private unsafe void ParseEventBufferAndNotifyForEach(byte[] buffer)
		{
			int num = 0;
			string text = null;
			int num2;
			do
			{
				int num3;
				string text2;
				fixed (byte* ptr = &buffer[0])
				{
					byte* ptr2 = ptr;
					num2 = *(int*)(ptr2 + num);
					num3 = *(int*)(ptr2 + num + 4);
					int num4 = *(int*)(ptr2 + num + 8);
					text2 = new string((char*)(ptr2 + num + 12), 0, num4 / 2);
				}
				if (num3 == 4)
				{
					text = text2;
				}
				else if (num3 == 5)
				{
					this.NotifyRenameEventArgs(WatcherChangeTypes.Renamed, text2, text);
					text = null;
				}
				else
				{
					if (text != null)
					{
						this.NotifyRenameEventArgs(WatcherChangeTypes.Renamed, null, text);
						text = null;
					}
					switch (num3)
					{
					case 1:
						this.NotifyFileSystemEventArgs(WatcherChangeTypes.Created, text2);
						break;
					case 2:
						this.NotifyFileSystemEventArgs(WatcherChangeTypes.Deleted, text2);
						break;
					case 3:
						this.NotifyFileSystemEventArgs(WatcherChangeTypes.Changed, text2);
						break;
					}
				}
				num += num2;
			}
			while (num2 != 0);
			if (text != null)
			{
				this.NotifyRenameEventArgs(WatcherChangeTypes.Renamed, null, text);
			}
		}

		public FileSystemWatcher()
		{
			this._directory = string.Empty;
		}

		public FileSystemWatcher(string path)
		{
			FileSystemWatcher.CheckPathValidity(path);
			this._directory = path;
		}

		public FileSystemWatcher(string path, string filter)
		{
			FileSystemWatcher.CheckPathValidity(path);
			this._directory = path;
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			this.Filter = filter;
		}

		public NotifyFilters NotifyFilter
		{
			get
			{
				return this._notifyFilters;
			}
			set
			{
				if ((value & ~(NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size)) != (NotifyFilters)0)
				{
					throw new ArgumentException(SR.Format("The value of argument '{0}' ({1}) is invalid for Enum type '{2}'.", "value", (int)value, "NotifyFilters"));
				}
				if (this._notifyFilters != value)
				{
					this._notifyFilters = value;
					this.Restart();
				}
			}
		}

		public Collection<string> Filters
		{
			get
			{
				return this._filters;
			}
		}

		public bool EnableRaisingEvents
		{
			get
			{
				return this._enabled;
			}
			set
			{
				if (this._enabled == value)
				{
					return;
				}
				if (this.IsSuspended())
				{
					this._enabled = value;
					return;
				}
				if (value)
				{
					this.StartRaisingEventsIfNotDisposed();
					return;
				}
				this.StopRaisingEvents();
			}
		}

		public string Filter
		{
			get
			{
				if (this.Filters.Count != 0)
				{
					return this.Filters[0];
				}
				return "*";
			}
			set
			{
				this.Filters.Clear();
				this.Filters.Add(value);
			}
		}

		public bool IncludeSubdirectories
		{
			get
			{
				return this._includeSubdirectories;
			}
			set
			{
				if (this._includeSubdirectories != value)
				{
					this._includeSubdirectories = value;
					this.Restart();
				}
			}
		}

		public int InternalBufferSize
		{
			get
			{
				return (int)this._internalBufferSize;
			}
			set
			{
				if ((ulong)this._internalBufferSize != (ulong)((long)value))
				{
					if (value < 4096)
					{
						this._internalBufferSize = 4096U;
					}
					else
					{
						this._internalBufferSize = (uint)value;
					}
					this.Restart();
				}
			}
		}

		private byte[] AllocateBuffer()
		{
			byte[] array;
			try
			{
				array = new byte[this._internalBufferSize];
			}
			catch (OutOfMemoryException)
			{
				throw new OutOfMemoryException(SR.Format("The specified buffer size is too large. FileSystemWatcher cannot allocate {0} bytes for the internal buffer.", this._internalBufferSize));
			}
			return array;
		}

		public string Path
		{
			get
			{
				return this._directory;
			}
			set
			{
				value = ((value == null) ? string.Empty : value);
				if (!string.Equals(this._directory, value, PathInternal.StringComparison))
				{
					if (value.Length == 0)
					{
						throw new ArgumentException(SR.Format("The directory name {0} is invalid.", value), "Path");
					}
					if (!Directory.Exists(value))
					{
						throw new ArgumentException(SR.Format("The directory name '{0}' does not exist.", value), "Path");
					}
					this._directory = value;
					this.Restart();
				}
			}
		}

		public event FileSystemEventHandler Changed
		{
			add
			{
				this._onChangedHandler = (FileSystemEventHandler)Delegate.Combine(this._onChangedHandler, value);
			}
			remove
			{
				this._onChangedHandler = (FileSystemEventHandler)Delegate.Remove(this._onChangedHandler, value);
			}
		}

		public event FileSystemEventHandler Created
		{
			add
			{
				this._onCreatedHandler = (FileSystemEventHandler)Delegate.Combine(this._onCreatedHandler, value);
			}
			remove
			{
				this._onCreatedHandler = (FileSystemEventHandler)Delegate.Remove(this._onCreatedHandler, value);
			}
		}

		public event FileSystemEventHandler Deleted
		{
			add
			{
				this._onDeletedHandler = (FileSystemEventHandler)Delegate.Combine(this._onDeletedHandler, value);
			}
			remove
			{
				this._onDeletedHandler = (FileSystemEventHandler)Delegate.Remove(this._onDeletedHandler, value);
			}
		}

		public event ErrorEventHandler Error
		{
			add
			{
				this._onErrorHandler = (ErrorEventHandler)Delegate.Combine(this._onErrorHandler, value);
			}
			remove
			{
				this._onErrorHandler = (ErrorEventHandler)Delegate.Remove(this._onErrorHandler, value);
			}
		}

		public event RenamedEventHandler Renamed
		{
			add
			{
				this._onRenamedHandler = (RenamedEventHandler)Delegate.Combine(this._onRenamedHandler, value);
			}
			remove
			{
				this._onRenamedHandler = (RenamedEventHandler)Delegate.Remove(this._onRenamedHandler, value);
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					this.StopRaisingEvents();
					this._onChangedHandler = null;
					this._onCreatedHandler = null;
					this._onDeletedHandler = null;
					this._onRenamedHandler = null;
					this._onErrorHandler = null;
				}
				else
				{
					this.FinalizeDispose();
				}
			}
			finally
			{
				this._disposed = true;
				base.Dispose(disposing);
			}
		}

		private static void CheckPathValidity(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(SR.Format("The directory name {0} is invalid.", path), "path");
			}
			if (!Directory.Exists(path))
			{
				throw new ArgumentException(SR.Format("The directory name '{0}' does not exist.", path), "path");
			}
		}

		private bool MatchPattern(ReadOnlySpan<char> relativePath)
		{
			if (relativePath.IsWhiteSpace())
			{
				return false;
			}
			ReadOnlySpan<char> fileName = global::System.IO.Path.GetFileName(relativePath);
			if (fileName.Length == 0)
			{
				return false;
			}
			string[] filters = this._filters.GetFilters();
			if (filters.Length == 0)
			{
				return true;
			}
			string[] array = filters;
			for (int i = 0; i < array.Length; i++)
			{
				if (FileSystemName.MatchesSimpleExpression(array[i], fileName, !PathInternal.IsCaseSensitive))
				{
					return true;
				}
			}
			return false;
		}

		private void NotifyInternalBufferOverflowEvent()
		{
			ErrorEventHandler onErrorHandler = this._onErrorHandler;
			if (onErrorHandler == null)
			{
				return;
			}
			onErrorHandler(this, new ErrorEventArgs(new InternalBufferOverflowException(SR.Format("Too many changes at once in directory:{0}.", this._directory))));
		}

		private void NotifyRenameEventArgs(WatcherChangeTypes action, ReadOnlySpan<char> name, ReadOnlySpan<char> oldName)
		{
			RenamedEventHandler onRenamedHandler = this._onRenamedHandler;
			if (onRenamedHandler != null && (this.MatchPattern(name) || this.MatchPattern(oldName)))
			{
				onRenamedHandler(this, new RenamedEventArgs(action, this._directory, name.IsEmpty ? null : name.ToString(), oldName.IsEmpty ? null : oldName.ToString()));
			}
		}

		private FileSystemEventHandler GetHandler(WatcherChangeTypes changeType)
		{
			switch (changeType)
			{
			case WatcherChangeTypes.Created:
				return this._onCreatedHandler;
			case WatcherChangeTypes.Deleted:
				return this._onDeletedHandler;
			case WatcherChangeTypes.Changed:
				return this._onChangedHandler;
			}
			return null;
		}

		private void NotifyFileSystemEventArgs(WatcherChangeTypes changeType, ReadOnlySpan<char> name)
		{
			FileSystemEventHandler handler = this.GetHandler(changeType);
			if (handler != null && this.MatchPattern(name.IsEmpty ? this._directory : name))
			{
				handler(this, new FileSystemEventArgs(changeType, this._directory, name.IsEmpty ? null : name.ToString()));
			}
		}

		private void NotifyFileSystemEventArgs(WatcherChangeTypes changeType, string name)
		{
			FileSystemEventHandler handler = this.GetHandler(changeType);
			if (handler != null && this.MatchPattern(string.IsNullOrEmpty(name) ? this._directory : name))
			{
				handler(this, new FileSystemEventArgs(changeType, this._directory, name));
			}
		}

		protected void OnChanged(FileSystemEventArgs e)
		{
			this.InvokeOn(e, this._onChangedHandler);
		}

		protected void OnCreated(FileSystemEventArgs e)
		{
			this.InvokeOn(e, this._onCreatedHandler);
		}

		protected void OnDeleted(FileSystemEventArgs e)
		{
			this.InvokeOn(e, this._onDeletedHandler);
		}

		private void InvokeOn(FileSystemEventArgs e, FileSystemEventHandler handler)
		{
			if (handler != null)
			{
				ISynchronizeInvoke synchronizingObject = this.SynchronizingObject;
				if (synchronizingObject != null && synchronizingObject.InvokeRequired)
				{
					synchronizingObject.BeginInvoke(handler, new object[] { this, e });
					return;
				}
				handler(this, e);
			}
		}

		protected void OnError(ErrorEventArgs e)
		{
			ErrorEventHandler onErrorHandler = this._onErrorHandler;
			if (onErrorHandler != null)
			{
				ISynchronizeInvoke synchronizingObject = this.SynchronizingObject;
				if (synchronizingObject != null && synchronizingObject.InvokeRequired)
				{
					synchronizingObject.BeginInvoke(onErrorHandler, new object[] { this, e });
					return;
				}
				onErrorHandler(this, e);
			}
		}

		protected void OnRenamed(RenamedEventArgs e)
		{
			RenamedEventHandler onRenamedHandler = this._onRenamedHandler;
			if (onRenamedHandler != null)
			{
				ISynchronizeInvoke synchronizingObject = this.SynchronizingObject;
				if (synchronizingObject != null && synchronizingObject.InvokeRequired)
				{
					synchronizingObject.BeginInvoke(onRenamedHandler, new object[] { this, e });
					return;
				}
				onRenamedHandler(this, e);
			}
		}

		public WaitForChangedResult WaitForChanged(WatcherChangeTypes changeType)
		{
			return this.WaitForChanged(changeType, -1);
		}

		public WaitForChangedResult WaitForChanged(WatcherChangeTypes changeType, int timeout)
		{
			TaskCompletionSource<WaitForChangedResult> tcs = new TaskCompletionSource<WaitForChangedResult>();
			FileSystemEventHandler fileSystemEventHandler = null;
			RenamedEventHandler renamedEventHandler = null;
			if ((changeType & (WatcherChangeTypes.Changed | WatcherChangeTypes.Created | WatcherChangeTypes.Deleted)) != (WatcherChangeTypes)0)
			{
				fileSystemEventHandler = delegate(object s, FileSystemEventArgs e)
				{
					if ((e.ChangeType & changeType) != (WatcherChangeTypes)0)
					{
						tcs.TrySetResult(new WaitForChangedResult(e.ChangeType, e.Name, null, false));
					}
				};
				if ((changeType & WatcherChangeTypes.Created) != (WatcherChangeTypes)0)
				{
					this.Created += fileSystemEventHandler;
				}
				if ((changeType & WatcherChangeTypes.Deleted) != (WatcherChangeTypes)0)
				{
					this.Deleted += fileSystemEventHandler;
				}
				if ((changeType & WatcherChangeTypes.Changed) != (WatcherChangeTypes)0)
				{
					this.Changed += fileSystemEventHandler;
				}
			}
			if ((changeType & WatcherChangeTypes.Renamed) != (WatcherChangeTypes)0)
			{
				renamedEventHandler = delegate(object s, RenamedEventArgs e)
				{
					if ((e.ChangeType & changeType) != (WatcherChangeTypes)0)
					{
						tcs.TrySetResult(new WaitForChangedResult(e.ChangeType, e.Name, e.OldName, false));
					}
				};
				this.Renamed += renamedEventHandler;
			}
			try
			{
				bool enableRaisingEvents = this.EnableRaisingEvents;
				if (!enableRaisingEvents)
				{
					this.EnableRaisingEvents = true;
				}
				tcs.Task.Wait(timeout);
				this.EnableRaisingEvents = enableRaisingEvents;
			}
			finally
			{
				if (renamedEventHandler != null)
				{
					this.Renamed -= renamedEventHandler;
				}
				if (fileSystemEventHandler != null)
				{
					if ((changeType & WatcherChangeTypes.Changed) != (WatcherChangeTypes)0)
					{
						this.Changed -= fileSystemEventHandler;
					}
					if ((changeType & WatcherChangeTypes.Deleted) != (WatcherChangeTypes)0)
					{
						this.Deleted -= fileSystemEventHandler;
					}
					if ((changeType & WatcherChangeTypes.Created) != (WatcherChangeTypes)0)
					{
						this.Created -= fileSystemEventHandler;
					}
				}
			}
			if (tcs.Task.Status != TaskStatus.RanToCompletion)
			{
				return WaitForChangedResult.TimedOutResult;
			}
			return tcs.Task.Result;
		}

		private void Restart()
		{
			if (!this.IsSuspended() && this._enabled)
			{
				this.StopRaisingEvents();
				this.StartRaisingEventsIfNotDisposed();
			}
		}

		private void StartRaisingEventsIfNotDisposed()
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			this.StartRaisingEvents();
		}

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

		public ISynchronizeInvoke SynchronizingObject { get; set; }

		public void BeginInit()
		{
			bool enabled = this._enabled;
			this.StopRaisingEvents();
			this._enabled = enabled;
			this._initializing = true;
		}

		public void EndInit()
		{
			this._initializing = false;
			if (this._directory.Length != 0 && this._enabled)
			{
				this.StartRaisingEvents();
			}
		}

		private bool IsSuspended()
		{
			return this._initializing || base.DesignMode;
		}

		private int _currentSession;

		private SafeFileHandle _directoryHandle;

		private readonly FileSystemWatcher.NormalizedFilterCollection _filters = new FileSystemWatcher.NormalizedFilterCollection();

		private string _directory;

		private const NotifyFilters c_defaultNotifyFilters = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite;

		private NotifyFilters _notifyFilters = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite;

		private bool _includeSubdirectories;

		private bool _enabled;

		private bool _initializing;

		private uint _internalBufferSize = 8192U;

		private bool _disposed;

		private FileSystemEventHandler _onChangedHandler;

		private FileSystemEventHandler _onCreatedHandler;

		private FileSystemEventHandler _onDeletedHandler;

		private RenamedEventHandler _onRenamedHandler;

		private ErrorEventHandler _onErrorHandler;

		private static readonly char[] s_wildcards = new char[] { '?', '*' };

		private const int c_notifyFiltersValidMask = 383;

		private sealed class AsyncReadState
		{
			internal AsyncReadState(int session, byte[] buffer, SafeFileHandle handle, ThreadPoolBoundHandle binding)
			{
				this.Session = session;
				this.Buffer = buffer;
				this.DirectoryHandle = handle;
				this.ThreadPoolBinding = binding;
			}

			internal int Session { get; private set; }

			internal byte[] Buffer { get; private set; }

			internal SafeFileHandle DirectoryHandle { get; private set; }

			internal ThreadPoolBoundHandle ThreadPoolBinding { get; private set; }

			internal PreAllocatedOverlapped PreAllocatedOverlapped { get; set; }
		}

		private sealed class NormalizedFilterCollection : Collection<string>
		{
			internal NormalizedFilterCollection()
				: base(new FileSystemWatcher.NormalizedFilterCollection.ImmutableStringList())
			{
			}

			protected override void InsertItem(int index, string item)
			{
				base.InsertItem(index, (string.IsNullOrEmpty(item) || item == "*.*") ? "*" : item);
			}

			protected override void SetItem(int index, string item)
			{
				base.SetItem(index, (string.IsNullOrEmpty(item) || item == "*.*") ? "*" : item);
			}

			internal string[] GetFilters()
			{
				return ((FileSystemWatcher.NormalizedFilterCollection.ImmutableStringList)base.Items).Items;
			}

			private sealed class ImmutableStringList : IList<string>, ICollection<string>, IEnumerable<string>, IEnumerable
			{
				public string this[int index]
				{
					get
					{
						string[] items = this.Items;
						if (index >= items.Length)
						{
							throw new ArgumentOutOfRangeException("index");
						}
						return items[index];
					}
					set
					{
						string[] array = (string[])this.Items.Clone();
						array[index] = value;
						this.Items = array;
					}
				}

				public int Count
				{
					get
					{
						return this.Items.Length;
					}
				}

				public bool IsReadOnly
				{
					get
					{
						return false;
					}
				}

				public void Add(string item)
				{
					throw new NotSupportedException();
				}

				public void Clear()
				{
					this.Items = Array.Empty<string>();
				}

				public bool Contains(string item)
				{
					return Array.IndexOf<string>(this.Items, item) != -1;
				}

				public void CopyTo(string[] array, int arrayIndex)
				{
					this.Items.CopyTo(array, arrayIndex);
				}

				public IEnumerator<string> GetEnumerator()
				{
					return ((IEnumerable<string>)this.Items).GetEnumerator();
				}

				public int IndexOf(string item)
				{
					return Array.IndexOf<string>(this.Items, item);
				}

				public void Insert(int index, string item)
				{
					string[] items = this.Items;
					string[] array = new string[items.Length + 1];
					items.AsSpan<string>(0, index).CopyTo(array);
					items.AsSpan<string>(index).CopyTo(array.AsSpan<string>(index + 1));
					array[index] = item;
					this.Items = array;
				}

				public bool Remove(string item)
				{
					throw new NotSupportedException();
				}

				public void RemoveAt(int index)
				{
					string[] items = this.Items;
					string[] array = new string[items.Length - 1];
					items.AsSpan<string>(0, index).CopyTo(array);
					items.AsSpan<string>(index + 1).CopyTo(array.AsSpan<string>(index));
					this.Items = array;
				}

				IEnumerator IEnumerable.GetEnumerator()
				{
					return this.GetEnumerator();
				}

				public string[] Items = Array.Empty<string>();
			}
		}
	}
}
