using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.CoreFX;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO
{
	internal class CoreFXFileSystemWatcherProxy : IFileWatcher
	{
		protected void Operation(Action<IDictionary<object, FileSystemWatcher>, ConditionalWeakTable<object, FileSystemWatcher>, IDictionary<object, object>, object> map_op = null, Action<FileSystemWatcher, FileSystemWatcher> object_op = null, object handle = null, Action<FileSystemWatcher, FileSystemWatcher> cancel_op = null)
		{
			FileSystemWatcher internal_fsw = null;
			FileSystemWatcher fsw = null;
			bool flag2;
			if (cancel_op != null)
			{
				bool flag = Monitor.TryEnter(CoreFXFileSystemWatcherProxy.instance, 300);
				flag2 = handle != null && (CoreFXFileSystemWatcherProxy.internal_map.TryGetValue(handle, out internal_fsw) || CoreFXFileSystemWatcherProxy.external_map.TryGetValue(handle, out fsw));
				if (flag2 && flag)
				{
					try
					{
						cancel_op(internal_fsw, fsw);
					}
					catch (Exception)
					{
					}
				}
				if (flag)
				{
					Monitor.Exit(CoreFXFileSystemWatcherProxy.instance);
				}
				if (flag2 && !flag)
				{
					try
					{
						Task.Run<bool>(delegate
						{
							cancel_op(internal_fsw, fsw);
							return true;
						}).Wait(300);
					}
					catch (Exception)
					{
					}
				}
				return;
			}
			IFileWatcher fileWatcher;
			if (map_op != null && handle == null)
			{
				fileWatcher = CoreFXFileSystemWatcherProxy.instance;
				lock (fileWatcher)
				{
					try
					{
						map_op(CoreFXFileSystemWatcherProxy.internal_map, CoreFXFileSystemWatcherProxy.external_map, CoreFXFileSystemWatcherProxy.event_map, null);
					}
					catch (Exception ex)
					{
						throw new InvalidOperationException("map_op", ex);
					}
				}
				return;
			}
			if (handle == null)
			{
				return;
			}
			fileWatcher = CoreFXFileSystemWatcherProxy.instance;
			lock (fileWatcher)
			{
				flag2 = CoreFXFileSystemWatcherProxy.internal_map.TryGetValue(handle, out internal_fsw) && CoreFXFileSystemWatcherProxy.external_map.TryGetValue(handle, out fsw);
				if (flag2 && map_op != null)
				{
					try
					{
						map_op(CoreFXFileSystemWatcherProxy.internal_map, CoreFXFileSystemWatcherProxy.external_map, CoreFXFileSystemWatcherProxy.event_map, handle);
					}
					catch (Exception ex2)
					{
						throw new InvalidOperationException("map_op", ex2);
					}
				}
			}
			if (!flag2 || object_op == null)
			{
				return;
			}
			try
			{
				object_op(internal_fsw, fsw);
			}
			catch (Exception ex3)
			{
				throw new InvalidOperationException("object_op", ex3);
			}
		}

		protected void ProxyDispatch(object sender, FileAction action, FileSystemEventArgs args)
		{
			RenamedEventArgs renamed = ((action == FileAction.RenamedNewName) ? ((RenamedEventArgs)args) : null);
			object handle = null;
			this.Operation(delegate(IDictionary<object, FileSystemWatcher> in_map, ConditionalWeakTable<object, FileSystemWatcher> out_map, IDictionary<object, object> event_map, object h)
			{
				event_map.TryGetValue(sender, out handle);
			}, null, null, null);
			this.Operation(null, delegate(FileSystemWatcher _, FileSystemWatcher fsw)
			{
				if (!fsw.EnableRaisingEvents)
				{
					return;
				}
				fsw.DispatchEvents(action, args.Name, ref renamed);
				if (fsw.Waiting)
				{
					fsw.Waiting = false;
					Monitor.PulseAll(fsw);
				}
			}, handle, null);
		}

		protected void ProxyDispatchError(object sender, ErrorEventArgs args)
		{
			object handle = null;
			this.Operation(delegate(IDictionary<object, FileSystemWatcher> in_map, ConditionalWeakTable<object, FileSystemWatcher> out_map, IDictionary<object, object> event_map, object _)
			{
				event_map.TryGetValue(sender, out handle);
			}, null, null, null);
			this.Operation(null, delegate(FileSystemWatcher _, FileSystemWatcher fsw)
			{
				fsw.DispatchErrorEvents(args);
			}, handle, null);
		}

		public object NewWatcher(FileSystemWatcher fsw)
		{
			object handle = new object();
			FileSystemWatcher result = new FileSystemWatcher();
			result.Changed += delegate(object o, FileSystemEventArgs args)
			{
				Task.Run(delegate
				{
					this.ProxyDispatch(o, FileAction.Modified, args);
				});
			};
			result.Created += delegate(object o, FileSystemEventArgs args)
			{
				Task.Run(delegate
				{
					this.ProxyDispatch(o, FileAction.Added, args);
				});
			};
			result.Deleted += delegate(object o, FileSystemEventArgs args)
			{
				Task.Run(delegate
				{
					this.ProxyDispatch(o, FileAction.Removed, args);
				});
			};
			result.Renamed += delegate(object o, RenamedEventArgs args)
			{
				Task.Run(delegate
				{
					this.ProxyDispatch(o, FileAction.RenamedNewName, args);
				});
			};
			result.Error += delegate(object o, ErrorEventArgs args)
			{
				Task.Run(delegate
				{
					this.ProxyDispatchError(handle, args);
				});
			};
			this.Operation(delegate(IDictionary<object, FileSystemWatcher> in_map, ConditionalWeakTable<object, FileSystemWatcher> out_map, IDictionary<object, object> event_map, object _)
			{
				in_map.Add(handle, result);
				out_map.Add(handle, fsw);
				event_map.Add(result, handle);
			}, null, null, null);
			return handle;
		}

		public void StartDispatching(object handle)
		{
			if (handle == null)
			{
				return;
			}
			this.Operation(null, delegate(FileSystemWatcher internal_fsw, FileSystemWatcher fsw)
			{
				internal_fsw.Path = fsw.Path;
				internal_fsw.Filter = fsw.Filter;
				internal_fsw.IncludeSubdirectories = fsw.IncludeSubdirectories;
				internal_fsw.InternalBufferSize = fsw.InternalBufferSize;
				internal_fsw.NotifyFilter = fsw.NotifyFilter;
				internal_fsw.Site = fsw.Site;
				internal_fsw.EnableRaisingEvents = true;
			}, handle, null);
		}

		public void StopDispatching(object handle)
		{
			if (handle == null)
			{
				return;
			}
			this.Operation(null, null, handle, delegate(FileSystemWatcher internal_fsw, FileSystemWatcher fsw)
			{
				if (internal_fsw != null)
				{
					internal_fsw.EnableRaisingEvents = false;
				}
			});
		}

		public void Dispose(object handle)
		{
			if (handle == null)
			{
				return;
			}
			this.Operation(null, null, handle, delegate(FileSystemWatcher internal_fsw, FileSystemWatcher fsw)
			{
				if (internal_fsw != null)
				{
					internal_fsw.Dispose();
				}
				FileSystemWatcher fileSystemWatcher = CoreFXFileSystemWatcherProxy.internal_map[handle];
				CoreFXFileSystemWatcherProxy.internal_map.Remove(handle);
				CoreFXFileSystemWatcherProxy.external_map.Remove(handle);
				CoreFXFileSystemWatcherProxy.event_map.Remove(fileSystemWatcher);
				handle = null;
			});
		}

		public static bool GetInstance(out IFileWatcher watcher)
		{
			if (CoreFXFileSystemWatcherProxy.instance != null)
			{
				watcher = CoreFXFileSystemWatcherProxy.instance;
				return true;
			}
			CoreFXFileSystemWatcherProxy.internal_map = new ConcurrentDictionary<object, FileSystemWatcher>();
			CoreFXFileSystemWatcherProxy.external_map = new ConditionalWeakTable<object, FileSystemWatcher>();
			CoreFXFileSystemWatcherProxy.event_map = new ConcurrentDictionary<object, object>();
			IFileWatcher fileWatcher;
			watcher = (fileWatcher = new CoreFXFileSystemWatcherProxy());
			CoreFXFileSystemWatcherProxy.instance = fileWatcher;
			return true;
		}

		private static IFileWatcher instance;

		private static IDictionary<object, FileSystemWatcher> internal_map;

		private static ConditionalWeakTable<object, FileSystemWatcher> external_map;

		private static IDictionary<object, object> event_map;

		private const int INTERRUPT_MS = 300;
	}
}
