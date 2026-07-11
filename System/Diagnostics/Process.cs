using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;

namespace System.Diagnostics
{
	[global::System.ComponentModel.DefaultEvent("Exited")]
	[MonitoringDescription("Represents a system process")]
	[global::System.ComponentModel.Designer("System.Diagnostics.Design.ProcessDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[global::System.ComponentModel.DefaultProperty("StartInfo")]
	[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	public class Process : global::System.ComponentModel.Component
	{
		private Process(IntPtr handle, int id)
		{
			this.process_handle = handle;
			this.pid = id;
		}

		public Process()
		{
		}

		[MonitoringDescription("Raised when it receives output data")]
		[global::System.ComponentModel.Browsable(true)]
		public event DataReceivedEventHandler OutputDataReceived;

		[MonitoringDescription("Raised when it receives error data")]
		[global::System.ComponentModel.Browsable(true)]
		public event DataReceivedEventHandler ErrorDataReceived;

		[MonitoringDescription("Raised when this process exits.")]
		[global::System.ComponentModel.Category("Behavior")]
		public event EventHandler Exited
		{
			add
			{
				if (this.process_handle != IntPtr.Zero && this.HasExited)
				{
					value.BeginInvoke(null, null, null, null);
				}
				else
				{
					this.exited_event = (EventHandler)Delegate.Combine(this.exited_event, value);
					if (this.exited_event != null)
					{
						this.StartExitCallbackIfNeeded();
					}
				}
			}
			remove
			{
				this.exited_event = (EventHandler)Delegate.Remove(this.exited_event, value);
			}
		}

		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("Base process priority.")]
		[global::System.MonoTODO]
		public int BasePriority
		{
			get
			{
				return 0;
			}
		}

		private void StartExitCallbackIfNeeded()
		{
			bool flag = !this.already_waiting && this.enableRaisingEvents && this.exited_event != null;
			if (flag && this.process_handle != IntPtr.Zero)
			{
				WaitOrTimerCallback waitOrTimerCallback = new WaitOrTimerCallback(Process.CBOnExit);
				Process.ProcessWaitHandle processWaitHandle = new Process.ProcessWaitHandle(this.process_handle);
				ThreadPool.RegisterWaitForSingleObject(processWaitHandle, waitOrTimerCallback, this, -1, true);
				this.already_waiting = true;
			}
		}

		[MonitoringDescription("Check for exiting of the process to raise the apropriate event.")]
		[global::System.ComponentModel.Browsable(false)]
		[global::System.ComponentModel.DefaultValue(false)]
		public bool EnableRaisingEvents
		{
			get
			{
				return this.enableRaisingEvents;
			}
			set
			{
				bool flag = this.enableRaisingEvents;
				this.enableRaisingEvents = value;
				if (this.enableRaisingEvents && !flag)
				{
					this.StartExitCallbackIfNeeded();
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ExitCode_internal(IntPtr handle);

		[MonitoringDescription("The exit code of the process.")]
		[global::System.ComponentModel.Browsable(false)]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public int ExitCode
		{
			get
			{
				if (this.process_handle == IntPtr.Zero)
				{
					throw new InvalidOperationException("Process has not been started.");
				}
				int num = Process.ExitCode_internal(this.process_handle);
				if (num == 259)
				{
					throw new InvalidOperationException("The process must exit before getting the requested information.");
				}
				return num;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long ExitTime_internal(IntPtr handle);

		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[global::System.ComponentModel.Browsable(false)]
		[MonitoringDescription("The exit time of the process.")]
		public DateTime ExitTime
		{
			get
			{
				if (this.process_handle == IntPtr.Zero)
				{
					throw new InvalidOperationException("Process has not been started.");
				}
				if (!this.HasExited)
				{
					throw new InvalidOperationException("The process must exit before getting the requested information.");
				}
				return DateTime.FromFileTime(Process.ExitTime_internal(this.process_handle));
			}
		}

		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("Handle for this process.")]
		[global::System.ComponentModel.Browsable(false)]
		public IntPtr Handle
		{
			get
			{
				return this.process_handle;
			}
		}

		[MonitoringDescription("Handles for this process.")]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[global::System.MonoTODO]
		public int HandleCount
		{
			get
			{
				return 0;
			}
		}

		[MonitoringDescription("Determines if the process is still running.")]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[global::System.ComponentModel.Browsable(false)]
		public bool HasExited
		{
			get
			{
				if (this.process_handle == IntPtr.Zero)
				{
					throw new InvalidOperationException("Process has not been started.");
				}
				int num = Process.ExitCode_internal(this.process_handle);
				return num != 259;
			}
		}

		[MonitoringDescription("Process identifier.")]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public int Id
		{
			get
			{
				if (this.pid == 0)
				{
					throw new InvalidOperationException("Process ID has not been set.");
				}
				return this.pid;
			}
		}

		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The name of the computer running the process.")]
		[global::System.MonoTODO]
		[global::System.ComponentModel.Browsable(false)]
		public string MachineName
		{
			get
			{
				return "localhost";
			}
		}

		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[global::System.ComponentModel.Browsable(false)]
		[MonitoringDescription("The main module of the process.")]
		public ProcessModule MainModule
		{
			get
			{
				return this.Modules[0];
			}
		}

		[MonitoringDescription("The handle of the main window of the process.")]
		[global::System.MonoTODO]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public IntPtr MainWindowHandle
		{
			get
			{
				return (IntPtr)0;
			}
		}

		[global::System.MonoTODO]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The title of the main window of the process.")]
		public string MainWindowTitle
		{
			get
			{
				return "null";
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetWorkingSet_internal(IntPtr handle, out int min, out int max);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetWorkingSet_internal(IntPtr handle, int min, int max, bool use_min);

		[MonitoringDescription("The maximum working set for this process.")]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public IntPtr MaxWorkingSet
		{
			get
			{
				if (this.HasExited)
				{
					throw new InvalidOperationException(string.Concat(new object[] { "The process ", this.ProcessName, " (ID ", this.Id, ") has exited" }));
				}
				int num;
				int num2;
				if (!Process.GetWorkingSet_internal(this.process_handle, out num, out num2))
				{
					throw new global::System.ComponentModel.Win32Exception();
				}
				return (IntPtr)num2;
			}
			set
			{
				if (this.HasExited)
				{
					throw new InvalidOperationException(string.Concat(new object[] { "The process ", this.ProcessName, " (ID ", this.Id, ") has exited" }));
				}
				if (!Process.SetWorkingSet_internal(this.process_handle, 0, value.ToInt32(), false))
				{
					throw new global::System.ComponentModel.Win32Exception();
				}
			}
		}

		[MonitoringDescription("The minimum working set for this process.")]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public IntPtr MinWorkingSet
		{
			get
			{
				if (this.HasExited)
				{
					throw new InvalidOperationException(string.Concat(new object[] { "The process ", this.ProcessName, " (ID ", this.Id, ") has exited" }));
				}
				int num;
				int num2;
				if (!Process.GetWorkingSet_internal(this.process_handle, out num, out num2))
				{
					throw new global::System.ComponentModel.Win32Exception();
				}
				return (IntPtr)num;
			}
			set
			{
				if (this.HasExited)
				{
					throw new InvalidOperationException(string.Concat(new object[] { "The process ", this.ProcessName, " (ID ", this.Id, ") has exited" }));
				}
				if (!Process.SetWorkingSet_internal(this.process_handle, value.ToInt32(), 0, true))
				{
					throw new global::System.ComponentModel.Win32Exception();
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern ProcessModule[] GetModules_internal(IntPtr handle);

		[MonitoringDescription("The modules that are loaded as part of this process.")]
		[global::System.ComponentModel.Browsable(false)]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public ProcessModuleCollection Modules
		{
			get
			{
				if (this.module_collection == null)
				{
					this.module_collection = new ProcessModuleCollection(this.GetModules_internal(this.process_handle));
				}
				return this.module_collection;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long GetProcessData(int pid, int data_type, out int error);

		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[Obsolete("Use NonpagedSystemMemorySize64")]
		[global::System.MonoTODO]
		[MonitoringDescription("The number of bytes that are not pageable.")]
		public int NonpagedSystemMemorySize
		{
			get
			{
				return 0;
			}
		}

		[global::System.MonoTODO]
		[MonitoringDescription("The number of bytes that are paged.")]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[Obsolete("Use PagedMemorySize64")]
		public int PagedMemorySize
		{
			get
			{
				return 0;
			}
		}

		[Obsolete("Use PagedSystemMemorySize64")]
		[MonitoringDescription("The amount of paged system memory in bytes.")]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[global::System.MonoTODO]
		public int PagedSystemMemorySize
		{
			get
			{
				return 0;
			}
		}

		[global::System.MonoTODO]
		[MonitoringDescription("The maximum amount of paged memory used by this process.")]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[Obsolete("Use PeakPagedMemorySize64")]
		public int PeakPagedMemorySize
		{
			get
			{
				return 0;
			}
		}

		[Obsolete("Use PeakVirtualMemorySize64")]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The maximum amount of virtual memory used by this process.")]
		public int PeakVirtualMemorySize
		{
			get
			{
				int num;
				return (int)Process.GetProcessData(this.pid, 8, out num);
			}
		}

		[MonitoringDescription("The maximum amount of system memory used by this process.")]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[Obsolete("Use PeakWorkingSet64")]
		public int PeakWorkingSet
		{
			get
			{
				int num;
				return (int)Process.GetProcessData(this.pid, 5, out num);
			}
		}

		[ComVisible(false)]
		[global::System.MonoTODO]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The number of bytes that are not pageable.")]
		public long NonpagedSystemMemorySize64
		{
			get
			{
				return 0L;
			}
		}

		[MonitoringDescription("The number of bytes that are paged.")]
		[ComVisible(false)]
		[global::System.MonoTODO]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public long PagedMemorySize64
		{
			get
			{
				return 0L;
			}
		}

		[global::System.MonoTODO]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The amount of paged system memory in bytes.")]
		[ComVisible(false)]
		public long PagedSystemMemorySize64
		{
			get
			{
				return 0L;
			}
		}

		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The maximum amount of paged memory used by this process.")]
		[ComVisible(false)]
		[global::System.MonoTODO]
		public long PeakPagedMemorySize64
		{
			get
			{
				return 0L;
			}
		}

		[ComVisible(false)]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The maximum amount of virtual memory used by this process.")]
		public long PeakVirtualMemorySize64
		{
			get
			{
				int num;
				return Process.GetProcessData(this.pid, 8, out num);
			}
		}

		[MonitoringDescription("The maximum amount of system memory used by this process.")]
		[ComVisible(false)]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public long PeakWorkingSet64
		{
			get
			{
				int num;
				return Process.GetProcessData(this.pid, 5, out num);
			}
		}

		[global::System.MonoTODO]
		[MonitoringDescription("Process will be of higher priority while it is actively used.")]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public bool PriorityBoostEnabled
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[global::System.MonoLimitation("Under Unix, only root is allowed to raise the priority.")]
		[MonitoringDescription("The relative process priority.")]
		public ProcessPriorityClass PriorityClass
		{
			get
			{
				if (this.process_handle == IntPtr.Zero)
				{
					throw new InvalidOperationException("Process has not been started.");
				}
				int num;
				int priorityClass = Process.GetPriorityClass(this.process_handle, out num);
				if (priorityClass == 0)
				{
					throw new global::System.ComponentModel.Win32Exception(num);
				}
				return (ProcessPriorityClass)priorityClass;
			}
			set
			{
				if (!Enum.IsDefined(typeof(ProcessPriorityClass), value))
				{
					throw new global::System.ComponentModel.InvalidEnumArgumentException("value", (int)value, typeof(ProcessPriorityClass));
				}
				if (this.process_handle == IntPtr.Zero)
				{
					throw new InvalidOperationException("Process has not been started.");
				}
				int num;
				if (!Process.SetPriorityClass(this.process_handle, (int)value, out num))
				{
					throw new global::System.ComponentModel.Win32Exception(num);
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetPriorityClass(IntPtr handle, out int error);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetPriorityClass(IntPtr handle, int priority, out int error);

		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The amount of memory exclusively used by this process.")]
		[Obsolete("Use PrivateMemorySize64")]
		public int PrivateMemorySize
		{
			get
			{
				int num;
				return (int)Process.GetProcessData(this.pid, 6, out num);
			}
		}

		[MonitoringDescription("The session ID for this process.")]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[global::System.MonoNotSupported("")]
		public int SessionId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long Times(IntPtr handle, int type);

		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The amount of processing time spent in the OS core for this process.")]
		public TimeSpan PrivilegedProcessorTime
		{
			get
			{
				return new TimeSpan(Process.Times(this.process_handle, 1));
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string ProcessName_internal(IntPtr handle);

		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The name of this process.")]
		public string ProcessName
		{
			get
			{
				if (this.process_name == null)
				{
					if (this.process_handle == IntPtr.Zero)
					{
						throw new InvalidOperationException("No process is associated with this object.");
					}
					this.process_name = Process.ProcessName_internal(this.process_handle);
					if (this.process_name == null)
					{
						throw new InvalidOperationException("Process has exited, so the requested information is not available.");
					}
					if (this.process_name.EndsWith(".exe") || this.process_name.EndsWith(".bat") || this.process_name.EndsWith(".com"))
					{
						this.process_name = this.process_name.Substring(0, this.process_name.Length - 4);
					}
				}
				return this.process_name;
			}
		}

		[MonitoringDescription("Allowed processor that can be used by this process.")]
		[global::System.MonoTODO]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public IntPtr ProcessorAffinity
		{
			get
			{
				return (IntPtr)0;
			}
			set
			{
			}
		}

		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[global::System.MonoTODO]
		[MonitoringDescription("Is this process responsive.")]
		public bool Responding
		{
			get
			{
				return false;
			}
		}

		[MonitoringDescription("The standard error stream of this process.")]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[global::System.ComponentModel.Browsable(false)]
		public StreamReader StandardError
		{
			get
			{
				if (this.error_stream == null)
				{
					throw new InvalidOperationException("Standard error has not been redirected");
				}
				if ((this.async_mode & Process.AsyncModes.AsyncError) != Process.AsyncModes.NoneYet)
				{
					throw new InvalidOperationException("Cannot mix asynchronous and synchonous reads.");
				}
				this.async_mode |= Process.AsyncModes.SyncError;
				return this.error_stream;
			}
		}

		[MonitoringDescription("The standard input stream of this process.")]
		[global::System.ComponentModel.Browsable(false)]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public StreamWriter StandardInput
		{
			get
			{
				if (this.input_stream == null)
				{
					throw new InvalidOperationException("Standard input has not been redirected");
				}
				return this.input_stream;
			}
		}

		[global::System.ComponentModel.Browsable(false)]
		[MonitoringDescription("The standard output stream of this process.")]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public StreamReader StandardOutput
		{
			get
			{
				if (this.output_stream == null)
				{
					throw new InvalidOperationException("Standard output has not been redirected");
				}
				if ((this.async_mode & Process.AsyncModes.AsyncOutput) != Process.AsyncModes.NoneYet)
				{
					throw new InvalidOperationException("Cannot mix asynchronous and synchonous reads.");
				}
				this.async_mode |= Process.AsyncModes.SyncOutput;
				return this.output_stream;
			}
		}

		[MonitoringDescription("Information for the start of this process.")]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Content)]
		[global::System.ComponentModel.Browsable(false)]
		public ProcessStartInfo StartInfo
		{
			get
			{
				if (this.start_info == null)
				{
					this.start_info = new ProcessStartInfo();
				}
				return this.start_info;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.start_info = value;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long StartTime_internal(IntPtr handle);

		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The time this process started.")]
		public DateTime StartTime
		{
			get
			{
				return DateTime.FromFileTime(Process.StartTime_internal(this.process_handle));
			}
		}

		[global::System.ComponentModel.DefaultValue(null)]
		[global::System.ComponentModel.Browsable(false)]
		[MonitoringDescription("The object that is used to synchronize event handler calls for this process.")]
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

		[global::System.ComponentModel.Browsable(false)]
		[global::System.MonoTODO]
		[MonitoringDescription("The number of threads of this process.")]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public ProcessThreadCollection Threads
		{
			get
			{
				return ProcessThreadCollection.GetEmpty();
			}
		}

		[MonitoringDescription("The total CPU time spent for this process.")]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public TimeSpan TotalProcessorTime
		{
			get
			{
				return new TimeSpan(Process.Times(this.process_handle, 2));
			}
		}

		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The CPU time spent for this process in user mode.")]
		public TimeSpan UserProcessorTime
		{
			get
			{
				return new TimeSpan(Process.Times(this.process_handle, 0));
			}
		}

		[MonitoringDescription("The amount of virtual memory currently used for this process.")]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[Obsolete("Use VirtualMemorySize64")]
		public int VirtualMemorySize
		{
			get
			{
				int num;
				return (int)Process.GetProcessData(this.pid, 7, out num);
			}
		}

		[MonitoringDescription("The amount of physical memory currently used for this process.")]
		[Obsolete("Use WorkingSet64")]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public int WorkingSet
		{
			get
			{
				int num;
				return (int)Process.GetProcessData(this.pid, 4, out num);
			}
		}

		[ComVisible(false)]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The amount of memory exclusively used by this process.")]
		public long PrivateMemorySize64
		{
			get
			{
				int num;
				return Process.GetProcessData(this.pid, 6, out num);
			}
		}

		[ComVisible(false)]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The amount of virtual memory currently used for this process.")]
		public long VirtualMemorySize64
		{
			get
			{
				int num;
				return Process.GetProcessData(this.pid, 7, out num);
			}
		}

		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[ComVisible(false)]
		[MonitoringDescription("The amount of physical memory currently used for this process.")]
		public long WorkingSet64
		{
			get
			{
				int num;
				return Process.GetProcessData(this.pid, 4, out num);
			}
		}

		public void Close()
		{
			this.Dispose(true);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Kill_internal(IntPtr handle, int signo);

		private bool Close(int signo)
		{
			if (this.process_handle == IntPtr.Zero)
			{
				throw new SystemException("No process to kill.");
			}
			int num = Process.ExitCode_internal(this.process_handle);
			if (num != 259)
			{
				throw new InvalidOperationException("The process already finished.");
			}
			return Process.Kill_internal(this.process_handle, signo);
		}

		public bool CloseMainWindow()
		{
			return this.Close(2);
		}

		[global::System.MonoTODO]
		public static void EnterDebugMode()
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetProcess_internal(int pid);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetPid_internal();

		public static Process GetCurrentProcess()
		{
			int pid_internal = Process.GetPid_internal();
			IntPtr process_internal = Process.GetProcess_internal(pid_internal);
			if (process_internal == IntPtr.Zero)
			{
				throw new SystemException("Can't find current process");
			}
			return new Process(process_internal, pid_internal);
		}

		public static Process GetProcessById(int processId)
		{
			IntPtr process_internal = Process.GetProcess_internal(processId);
			if (process_internal == IntPtr.Zero)
			{
				throw new ArgumentException("Can't find process with ID " + processId.ToString());
			}
			return new Process(process_internal, processId);
		}

		[global::System.MonoTODO("There is no support for retrieving process information from a remote machine")]
		public static Process GetProcessById(int processId, string machineName)
		{
			if (machineName == null)
			{
				throw new ArgumentNullException("machineName");
			}
			if (!Process.IsLocalMachine(machineName))
			{
				throw new NotImplementedException();
			}
			return Process.GetProcessById(processId);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int[] GetProcesses_internal();

		public static Process[] GetProcesses()
		{
			int[] processes_internal = Process.GetProcesses_internal();
			ArrayList arrayList = new ArrayList();
			if (processes_internal == null)
			{
				return new Process[0];
			}
			for (int i = 0; i < processes_internal.Length; i++)
			{
				try
				{
					arrayList.Add(Process.GetProcessById(processes_internal[i]));
				}
				catch (SystemException)
				{
				}
			}
			return (Process[])arrayList.ToArray(typeof(Process));
		}

		[global::System.MonoTODO("There is no support for retrieving process information from a remote machine")]
		public static Process[] GetProcesses(string machineName)
		{
			if (machineName == null)
			{
				throw new ArgumentNullException("machineName");
			}
			if (!Process.IsLocalMachine(machineName))
			{
				throw new NotImplementedException();
			}
			return Process.GetProcesses();
		}

		public static Process[] GetProcessesByName(string processName)
		{
			Process[] processes = Process.GetProcesses();
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < processes.Length; i++)
			{
				try
				{
					if (string.Compare(processName, processes[i].ProcessName, true) == 0)
					{
						arrayList.Add(processes[i]);
					}
				}
				catch (Exception)
				{
				}
			}
			return (Process[])arrayList.ToArray(typeof(Process));
		}

		[global::System.MonoTODO]
		public static Process[] GetProcessesByName(string processName, string machineName)
		{
			throw new NotImplementedException();
		}

		public void Kill()
		{
			this.Close(1);
		}

		[global::System.MonoTODO]
		public static void LeaveDebugMode()
		{
		}

		public void Refresh()
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ShellExecuteEx_internal(ProcessStartInfo startInfo, ref Process.ProcInfo proc_info);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CreateProcess_internal(ProcessStartInfo startInfo, IntPtr stdin, IntPtr stdout, IntPtr stderr, ref Process.ProcInfo proc_info);

		private static bool Start_shell(ProcessStartInfo startInfo, Process process)
		{
			Process.ProcInfo procInfo = default(Process.ProcInfo);
			if (startInfo.RedirectStandardInput || startInfo.RedirectStandardOutput || startInfo.RedirectStandardError)
			{
				throw new InvalidOperationException("UseShellExecute must be false when redirecting I/O.");
			}
			if (startInfo.HaveEnvVars)
			{
				throw new InvalidOperationException("UseShellExecute must be false in order to use environment variables.");
			}
			Process.FillUserInfo(startInfo, ref procInfo);
			bool flag;
			try
			{
				flag = Process.ShellExecuteEx_internal(startInfo, ref procInfo);
			}
			finally
			{
				if (procInfo.Password != IntPtr.Zero)
				{
					Marshal.FreeBSTR(procInfo.Password);
				}
				procInfo.Password = IntPtr.Zero;
			}
			if (!flag)
			{
				throw new global::System.ComponentModel.Win32Exception(-procInfo.pid);
			}
			process.process_handle = procInfo.process_handle;
			process.pid = procInfo.pid;
			process.StartExitCallbackIfNeeded();
			return flag;
		}

		private static bool Start_noshell(ProcessStartInfo startInfo, Process process)
		{
			Process.ProcInfo procInfo = default(Process.ProcInfo);
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			if (startInfo.HaveEnvVars)
			{
				string[] array = new string[startInfo.EnvironmentVariables.Count];
				startInfo.EnvironmentVariables.Keys.CopyTo(array, 0);
				procInfo.envKeys = array;
				array = new string[startInfo.EnvironmentVariables.Count];
				startInfo.EnvironmentVariables.Values.CopyTo(array, 0);
				procInfo.envValues = array;
			}
			bool flag;
			if (startInfo.RedirectStandardInput)
			{
				if (Process.IsWindows)
				{
					int num = 2;
					IntPtr intPtr3;
					flag = global::System.IO.MonoIO.CreatePipe(out intPtr, out intPtr3);
					if (flag)
					{
						flag = global::System.IO.MonoIO.DuplicateHandle(Process.GetCurrentProcess().Handle, intPtr3, Process.GetCurrentProcess().Handle, out intPtr2, 0, 0, num);
						global::System.IO.MonoIOError monoIOError;
						global::System.IO.MonoIO.Close(intPtr3, out monoIOError);
					}
				}
				else
				{
					flag = global::System.IO.MonoIO.CreatePipe(out intPtr, out intPtr2);
				}
				if (!flag)
				{
					throw new IOException("Error creating standard input pipe");
				}
			}
			else
			{
				intPtr = global::System.IO.MonoIO.ConsoleInput;
				intPtr2 = (IntPtr)0;
			}
			IntPtr consoleOutput;
			if (startInfo.RedirectStandardOutput)
			{
				IntPtr zero = IntPtr.Zero;
				if (Process.IsWindows)
				{
					int num2 = 2;
					IntPtr intPtr4;
					flag = global::System.IO.MonoIO.CreatePipe(out intPtr4, out consoleOutput);
					if (flag)
					{
						global::System.IO.MonoIO.DuplicateHandle(Process.GetCurrentProcess().Handle, intPtr4, Process.GetCurrentProcess().Handle, out zero, 0, 0, num2);
						global::System.IO.MonoIOError monoIOError;
						global::System.IO.MonoIO.Close(intPtr4, out monoIOError);
					}
				}
				else
				{
					flag = global::System.IO.MonoIO.CreatePipe(out zero, out consoleOutput);
				}
				process.stdout_rd = zero;
				if (!flag)
				{
					if (startInfo.RedirectStandardInput)
					{
						global::System.IO.MonoIOError monoIOError;
						global::System.IO.MonoIO.Close(intPtr, out monoIOError);
						global::System.IO.MonoIO.Close(intPtr2, out monoIOError);
					}
					throw new IOException("Error creating standard output pipe");
				}
			}
			else
			{
				process.stdout_rd = (IntPtr)0;
				consoleOutput = global::System.IO.MonoIO.ConsoleOutput;
			}
			IntPtr consoleError;
			if (startInfo.RedirectStandardError)
			{
				IntPtr zero2 = IntPtr.Zero;
				if (Process.IsWindows)
				{
					int num3 = 2;
					IntPtr intPtr5;
					flag = global::System.IO.MonoIO.CreatePipe(out intPtr5, out consoleError);
					if (flag)
					{
						global::System.IO.MonoIO.DuplicateHandle(Process.GetCurrentProcess().Handle, intPtr5, Process.GetCurrentProcess().Handle, out zero2, 0, 0, num3);
						global::System.IO.MonoIOError monoIOError;
						global::System.IO.MonoIO.Close(intPtr5, out monoIOError);
					}
				}
				else
				{
					flag = global::System.IO.MonoIO.CreatePipe(out zero2, out consoleError);
				}
				process.stderr_rd = zero2;
				if (!flag)
				{
					if (startInfo.RedirectStandardInput)
					{
						global::System.IO.MonoIOError monoIOError;
						global::System.IO.MonoIO.Close(intPtr, out monoIOError);
						global::System.IO.MonoIO.Close(intPtr2, out monoIOError);
					}
					if (startInfo.RedirectStandardOutput)
					{
						global::System.IO.MonoIOError monoIOError;
						global::System.IO.MonoIO.Close(process.stdout_rd, out monoIOError);
						global::System.IO.MonoIO.Close(consoleOutput, out monoIOError);
					}
					throw new IOException("Error creating standard error pipe");
				}
			}
			else
			{
				process.stderr_rd = (IntPtr)0;
				consoleError = global::System.IO.MonoIO.ConsoleError;
			}
			Process.FillUserInfo(startInfo, ref procInfo);
			try
			{
				flag = Process.CreateProcess_internal(startInfo, intPtr, consoleOutput, consoleError, ref procInfo);
			}
			finally
			{
				if (procInfo.Password != IntPtr.Zero)
				{
					Marshal.FreeBSTR(procInfo.Password);
				}
				procInfo.Password = IntPtr.Zero;
			}
			if (!flag)
			{
				if (startInfo.RedirectStandardInput)
				{
					global::System.IO.MonoIOError monoIOError;
					global::System.IO.MonoIO.Close(intPtr, out monoIOError);
					global::System.IO.MonoIO.Close(intPtr2, out monoIOError);
				}
				if (startInfo.RedirectStandardOutput)
				{
					global::System.IO.MonoIOError monoIOError;
					global::System.IO.MonoIO.Close(process.stdout_rd, out monoIOError);
					global::System.IO.MonoIO.Close(consoleOutput, out monoIOError);
				}
				if (startInfo.RedirectStandardError)
				{
					global::System.IO.MonoIOError monoIOError;
					global::System.IO.MonoIO.Close(process.stderr_rd, out monoIOError);
					global::System.IO.MonoIO.Close(consoleError, out monoIOError);
				}
				throw new global::System.ComponentModel.Win32Exception(-procInfo.pid, string.Concat(new string[] { "ApplicationName='", startInfo.FileName, "', CommandLine='", startInfo.Arguments, "', CurrentDirectory='", startInfo.WorkingDirectory, "'" }));
			}
			process.process_handle = procInfo.process_handle;
			process.pid = procInfo.pid;
			if (startInfo.RedirectStandardInput)
			{
				global::System.IO.MonoIOError monoIOError;
				global::System.IO.MonoIO.Close(intPtr, out monoIOError);
				process.input_stream = new StreamWriter(new global::System.IO.MonoSyncFileStream(intPtr2, FileAccess.Write, true, 8192), Console.Out.Encoding);
				process.input_stream.AutoFlush = true;
			}
			Encoding encoding = startInfo.StandardOutputEncoding ?? Console.Out.Encoding;
			Encoding encoding2 = startInfo.StandardErrorEncoding ?? Console.Out.Encoding;
			if (startInfo.RedirectStandardOutput)
			{
				global::System.IO.MonoIOError monoIOError;
				global::System.IO.MonoIO.Close(consoleOutput, out monoIOError);
				process.output_stream = new StreamReader(new global::System.IO.MonoSyncFileStream(process.stdout_rd, FileAccess.Read, true, 8192), encoding, true, 8192);
			}
			if (startInfo.RedirectStandardError)
			{
				global::System.IO.MonoIOError monoIOError;
				global::System.IO.MonoIO.Close(consoleError, out monoIOError);
				process.error_stream = new StreamReader(new global::System.IO.MonoSyncFileStream(process.stderr_rd, FileAccess.Read, true, 8192), encoding2, true, 8192);
			}
			process.StartExitCallbackIfNeeded();
			return flag;
		}

		private static void FillUserInfo(ProcessStartInfo startInfo, ref Process.ProcInfo proc_info)
		{
			if (startInfo.UserName != null)
			{
				proc_info.UserName = startInfo.UserName;
				proc_info.Domain = startInfo.Domain;
				if (startInfo.Password != null)
				{
					proc_info.Password = Marshal.SecureStringToBSTR(startInfo.Password);
				}
				else
				{
					proc_info.Password = IntPtr.Zero;
				}
				proc_info.LoadUserProfile = startInfo.LoadUserProfile;
			}
		}

		private static bool Start_common(ProcessStartInfo startInfo, Process process)
		{
			if (startInfo.FileName == null || startInfo.FileName.Length == 0)
			{
				throw new InvalidOperationException("File name has not been set");
			}
			if (startInfo.StandardErrorEncoding != null && !startInfo.RedirectStandardError)
			{
				throw new InvalidOperationException("StandardErrorEncoding is only supported when standard error is redirected");
			}
			if (startInfo.StandardOutputEncoding != null && !startInfo.RedirectStandardOutput)
			{
				throw new InvalidOperationException("StandardOutputEncoding is only supported when standard output is redirected");
			}
			if (!startInfo.UseShellExecute)
			{
				return Process.Start_noshell(startInfo, process);
			}
			if (!string.IsNullOrEmpty(startInfo.UserName))
			{
				throw new InvalidOperationException("UserShellExecute must be false if an explicit UserName is specified when starting a process");
			}
			return Process.Start_shell(startInfo, process);
		}

		public bool Start()
		{
			if (this.process_handle != IntPtr.Zero)
			{
				this.Process_free_internal(this.process_handle);
				this.process_handle = IntPtr.Zero;
			}
			return Process.Start_common(this.start_info, this);
		}

		public static Process Start(ProcessStartInfo startInfo)
		{
			if (startInfo == null)
			{
				throw new ArgumentNullException("startInfo");
			}
			Process process = new Process();
			process.StartInfo = startInfo;
			if (Process.Start_common(startInfo, process))
			{
				return process;
			}
			return null;
		}

		public static Process Start(string fileName)
		{
			return Process.Start(new ProcessStartInfo(fileName));
		}

		public static Process Start(string fileName, string arguments)
		{
			return Process.Start(new ProcessStartInfo(fileName, arguments));
		}

		public static Process Start(string fileName, string username, SecureString password, string domain)
		{
			return Process.Start(fileName, null, username, password, domain);
		}

		public static Process Start(string fileName, string arguments, string username, SecureString password, string domain)
		{
			return Process.Start(new ProcessStartInfo(fileName, arguments)
			{
				UserName = username,
				Password = password,
				Domain = domain,
				UseShellExecute = false
			});
		}

		public override string ToString()
		{
			return base.ToString() + " (" + this.ProcessName + ")";
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool WaitForExit_internal(IntPtr handle, int ms);

		public void WaitForExit()
		{
			this.WaitForExit(-1);
		}

		public bool WaitForExit(int milliseconds)
		{
			int num = milliseconds;
			if (num == 2147483647)
			{
				num = -1;
			}
			DateTime dateTime = DateTime.UtcNow;
			if (this.async_output != null && !this.async_output.IsCompleted)
			{
				if (!this.async_output.WaitHandle.WaitOne(num, false))
				{
					return false;
				}
				if (num >= 0)
				{
					DateTime utcNow = DateTime.UtcNow;
					num -= (int)(utcNow - dateTime).TotalMilliseconds;
					if (num <= 0)
					{
						return false;
					}
					dateTime = utcNow;
				}
			}
			if (this.async_error != null && !this.async_error.IsCompleted)
			{
				if (!this.async_error.WaitHandle.WaitOne(num, false))
				{
					return false;
				}
				if (num >= 0)
				{
					num -= (int)(DateTime.UtcNow - dateTime).TotalMilliseconds;
					if (num <= 0)
					{
						return false;
					}
				}
			}
			return this.WaitForExit_internal(this.process_handle, num);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool WaitForInputIdle_internal(IntPtr handle, int ms);

		[global::System.MonoTODO]
		public bool WaitForInputIdle()
		{
			return this.WaitForInputIdle(-1);
		}

		[global::System.MonoTODO]
		public bool WaitForInputIdle(int milliseconds)
		{
			return this.WaitForInputIdle_internal(this.process_handle, milliseconds);
		}

		private static bool IsLocalMachine(string machineName)
		{
			return machineName == "." || machineName.Length == 0 || string.Compare(machineName, Environment.MachineName, true) == 0;
		}

		private void OnOutputDataReceived(string str)
		{
			if (this.OutputDataReceived != null)
			{
				this.OutputDataReceived(this, new DataReceivedEventArgs(str));
			}
		}

		private void OnErrorDataReceived(string str)
		{
			if (this.ErrorDataReceived != null)
			{
				this.ErrorDataReceived(this, new DataReceivedEventArgs(str));
			}
		}

		[ComVisible(false)]
		public void BeginOutputReadLine()
		{
			if (this.process_handle == IntPtr.Zero || this.output_stream == null || !this.StartInfo.RedirectStandardOutput)
			{
				throw new InvalidOperationException("Standard output has not been redirected or process has not been started.");
			}
			if ((this.async_mode & Process.AsyncModes.SyncOutput) != Process.AsyncModes.NoneYet)
			{
				throw new InvalidOperationException("Cannot mix asynchronous and synchonous reads.");
			}
			this.async_mode |= Process.AsyncModes.AsyncOutput;
			this.output_canceled = false;
			if (this.async_output == null)
			{
				this.async_output = new Process.ProcessAsyncReader(this, this.stdout_rd, true);
				this.async_output.ReadHandler.BeginInvoke(null, this.async_output);
			}
		}

		[ComVisible(false)]
		public void CancelOutputRead()
		{
			if (this.process_handle == IntPtr.Zero || this.output_stream == null || !this.StartInfo.RedirectStandardOutput)
			{
				throw new InvalidOperationException("Standard output has not been redirected or process has not been started.");
			}
			if ((this.async_mode & Process.AsyncModes.SyncOutput) != Process.AsyncModes.NoneYet)
			{
				throw new InvalidOperationException("OutputStream is not enabled for asynchronous read operations.");
			}
			if (this.async_output == null)
			{
				throw new InvalidOperationException("No async operation in progress.");
			}
			this.output_canceled = true;
		}

		[ComVisible(false)]
		public void BeginErrorReadLine()
		{
			if (this.process_handle == IntPtr.Zero || this.error_stream == null || !this.StartInfo.RedirectStandardError)
			{
				throw new InvalidOperationException("Standard error has not been redirected or process has not been started.");
			}
			if ((this.async_mode & Process.AsyncModes.SyncError) != Process.AsyncModes.NoneYet)
			{
				throw new InvalidOperationException("Cannot mix asynchronous and synchonous reads.");
			}
			this.async_mode |= Process.AsyncModes.AsyncError;
			this.error_canceled = false;
			if (this.async_error == null)
			{
				this.async_error = new Process.ProcessAsyncReader(this, this.stderr_rd, false);
				this.async_error.ReadHandler.BeginInvoke(null, this.async_error);
			}
		}

		[ComVisible(false)]
		public void CancelErrorRead()
		{
			if (this.process_handle == IntPtr.Zero || this.output_stream == null || !this.StartInfo.RedirectStandardOutput)
			{
				throw new InvalidOperationException("Standard output has not been redirected or process has not been started.");
			}
			if ((this.async_mode & Process.AsyncModes.SyncOutput) != Process.AsyncModes.NoneYet)
			{
				throw new InvalidOperationException("OutputStream is not enabled for asynchronous read operations.");
			}
			if (this.async_error == null)
			{
				throw new InvalidOperationException("No async operation in progress.");
			}
			this.error_canceled = true;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Process_free_internal(IntPtr handle);

		protected override void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				this.disposed = true;
				if (disposing)
				{
					lock (this)
					{
						if (this.async_output != null)
						{
							this.async_output.Close();
						}
						if (this.async_error != null)
						{
							this.async_error.Close();
						}
					}
				}
				lock (this)
				{
					if (this.process_handle != IntPtr.Zero)
					{
						this.Process_free_internal(this.process_handle);
						this.process_handle = IntPtr.Zero;
					}
					if (this.input_stream != null)
					{
						this.input_stream.Close();
						this.input_stream = null;
					}
					if (this.output_stream != null)
					{
						this.output_stream.Close();
						this.output_stream = null;
					}
					if (this.error_stream != null)
					{
						this.error_stream.Close();
						this.error_stream = null;
					}
				}
			}
			base.Dispose(disposing);
		}

		~Process()
		{
			this.Dispose(false);
		}

		private static void CBOnExit(object state, bool unused)
		{
			Process process = (Process)state;
			process.OnExited();
		}

		protected void OnExited()
		{
			if (this.exited_event == null)
			{
				return;
			}
			if (this.synchronizingObject == null)
			{
				foreach (EventHandler eventHandler in this.exited_event.GetInvocationList())
				{
					try
					{
						eventHandler(this, EventArgs.Empty);
					}
					catch
					{
					}
				}
				return;
			}
			object[] array = new object[]
			{
				this,
				EventArgs.Empty
			};
			this.synchronizingObject.BeginInvoke(this.exited_event, array);
		}

		private static bool IsWindows
		{
			get
			{
				PlatformID platform = Environment.OSVersion.Platform;
				return platform == PlatformID.Win32S || platform == PlatformID.Win32Windows || platform == PlatformID.Win32NT || platform == PlatformID.WinCE;
			}
		}

		private IntPtr process_handle;

		private int pid;

		private bool enableRaisingEvents;

		private bool already_waiting;

		private global::System.ComponentModel.ISynchronizeInvoke synchronizingObject;

		private EventHandler exited_event;

		private IntPtr stdout_rd;

		private IntPtr stderr_rd;

		private ProcessModuleCollection module_collection;

		private string process_name;

		private StreamReader error_stream;

		private StreamWriter input_stream;

		private StreamReader output_stream;

		private ProcessStartInfo start_info;

		private Process.AsyncModes async_mode;

		private bool output_canceled;

		private bool error_canceled;

		private Process.ProcessAsyncReader async_output;

		private Process.ProcessAsyncReader async_error;

		private bool disposed;

		private struct ProcInfo
		{
			public IntPtr process_handle;

			public IntPtr thread_handle;

			public int pid;

			public int tid;

			public string[] envKeys;

			public string[] envValues;

			public string UserName;

			public string Domain;

			public IntPtr Password;

			public bool LoadUserProfile;
		}

		[Flags]
		private enum AsyncModes
		{
			NoneYet = 0,
			SyncOutput = 1,
			SyncError = 2,
			AsyncOutput = 4,
			AsyncError = 8
		}

		[StructLayout(LayoutKind.Sequential)]
		private sealed class ProcessAsyncReader
		{
			public ProcessAsyncReader(Process process, IntPtr handle, bool err_out)
			{
				if (err_out)
				{
					this.outputEncoding = process.StartInfo.StandardOutputEncoding ?? Console.Out.Encoding;
				}
				else
				{
					this.outputEncoding = process.StartInfo.StandardErrorEncoding ?? Console.Out.Encoding;
				}
				this.process = process;
				this.handle = handle;
				this.stream = new FileStream(handle, FileAccess.Read, false);
				this.ReadHandler = new Process.AsyncReadHandler(this.AddInput);
				this.err_out = err_out;
			}

			public void AddInput()
			{
				lock (this)
				{
					int num = this.stream.Read(this.buffer, 0, this.buffer.Length);
					if (num == 0)
					{
						this.completed = true;
						if (this.wait_handle != null)
						{
							this.wait_handle.Set();
						}
						this.FlushLast();
					}
					else
					{
						try
						{
							this.sb.Append(this.outputEncoding.GetString(this.buffer, 0, num));
						}
						catch
						{
							for (int i = 0; i < num; i++)
							{
								this.sb.Append((char)this.buffer[i]);
							}
						}
						this.Flush(false);
						this.ReadHandler.BeginInvoke(null, this);
					}
				}
			}

			private void FlushLast()
			{
				this.Flush(true);
				if (this.err_out)
				{
					this.process.OnOutputDataReceived(null);
				}
				else
				{
					this.process.OnErrorDataReceived(null);
				}
			}

			private void Flush(bool last)
			{
				if (this.sb.Length == 0 || (this.err_out && this.process.output_canceled) || (!this.err_out && this.process.error_canceled))
				{
					return;
				}
				string text = this.sb.ToString();
				this.sb.Length = 0;
				string[] array = text.Split(new char[] { '\n' });
				int num = array.Length;
				if (num == 0)
				{
					return;
				}
				for (int i = 0; i < num - 1; i++)
				{
					if (this.err_out)
					{
						this.process.OnOutputDataReceived(array[i]);
					}
					else
					{
						this.process.OnErrorDataReceived(array[i]);
					}
				}
				string text2 = array[num - 1];
				if (last || (num == 1 && text2 == string.Empty))
				{
					if (this.err_out)
					{
						this.process.OnOutputDataReceived(text2);
					}
					else
					{
						this.process.OnErrorDataReceived(text2);
					}
				}
				else
				{
					this.sb.Append(text2);
				}
			}

			public bool IsCompleted
			{
				get
				{
					return this.completed;
				}
			}

			public WaitHandle WaitHandle
			{
				get
				{
					WaitHandle waitHandle;
					lock (this)
					{
						if (this.wait_handle == null)
						{
							this.wait_handle = new ManualResetEvent(this.completed);
						}
						waitHandle = this.wait_handle;
					}
					return waitHandle;
				}
			}

			public void Close()
			{
				this.stream.Close();
			}

			public object Sock;

			public IntPtr handle;

			public object state;

			public AsyncCallback callback;

			public ManualResetEvent wait_handle;

			public Exception delayedException;

			public object EndPoint;

			private byte[] buffer = new byte[4196];

			public int Offset;

			public int Size;

			public int SockFlags;

			public object AcceptSocket;

			public object[] Addresses;

			public int port;

			public object Buffers;

			public bool ReuseSocket;

			public object acc_socket;

			public int total;

			public bool completed_sync;

			private bool completed;

			private bool err_out;

			internal int error;

			public int operation = 8;

			public object ares;

			public int EndCalled;

			private Process process;

			private Stream stream;

			private StringBuilder sb = new StringBuilder();

			private Encoding outputEncoding;

			public Process.AsyncReadHandler ReadHandler;
		}

		private class ProcessWaitHandle : WaitHandle
		{
			public ProcessWaitHandle(IntPtr handle)
			{
				this.Handle = Process.ProcessWaitHandle.ProcessHandle_duplicate(handle);
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern IntPtr ProcessHandle_duplicate(IntPtr handle);
		}

		private delegate void AsyncReadHandler();
	}
}
