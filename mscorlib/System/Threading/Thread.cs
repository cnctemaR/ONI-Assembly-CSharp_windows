using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;

namespace System.Threading
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	[ComDefaultInterface(typeof(_Thread))]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class Thread : CriticalFinalizerObject, _Thread
	{
		private static void AsyncLocalSetCurrentCulture(AsyncLocalValueChangedArgs<CultureInfo> args)
		{
			Thread.m_CurrentCulture = args.CurrentValue;
		}

		private static void AsyncLocalSetCurrentUICulture(AsyncLocalValueChangedArgs<CultureInfo> args)
		{
			Thread.m_CurrentUICulture = args.CurrentValue;
		}

		[SecuritySafeCritical]
		public Thread(ThreadStart start)
		{
			if (start == null)
			{
				throw new ArgumentNullException("start");
			}
			this.SetStartHelper(start, 0);
		}

		[SecuritySafeCritical]
		public Thread(ThreadStart start, int maxStackSize)
		{
			if (start == null)
			{
				throw new ArgumentNullException("start");
			}
			if (0 > maxStackSize)
			{
				throw new ArgumentOutOfRangeException("maxStackSize", Environment.GetResourceString("Non-negative number required."));
			}
			this.SetStartHelper(start, maxStackSize);
		}

		[SecuritySafeCritical]
		public Thread(ParameterizedThreadStart start)
		{
			if (start == null)
			{
				throw new ArgumentNullException("start");
			}
			this.SetStartHelper(start, 0);
		}

		[SecuritySafeCritical]
		public Thread(ParameterizedThreadStart start, int maxStackSize)
		{
			if (start == null)
			{
				throw new ArgumentNullException("start");
			}
			if (0 > maxStackSize)
			{
				throw new ArgumentOutOfRangeException("maxStackSize", Environment.GetResourceString("Non-negative number required."));
			}
			this.SetStartHelper(start, maxStackSize);
		}

		[HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public void Start()
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			this.Start(ref stackCrawlMark);
		}

		[HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public void Start(object parameter)
		{
			if (this.m_Delegate is ThreadStart)
			{
				throw new InvalidOperationException(Environment.GetResourceString("The thread was created with a ThreadStart delegate that does not accept a parameter."));
			}
			this.m_ThreadStartArg = parameter;
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			this.Start(ref stackCrawlMark);
		}

		[SecuritySafeCritical]
		private void Start(ref StackCrawlMark stackMark)
		{
			if (this.m_Delegate != null)
			{
				ThreadHelper threadHelper = (ThreadHelper)this.m_Delegate.Target;
				ExecutionContext executionContext = ExecutionContext.Capture(ref stackMark, ExecutionContext.CaptureOptions.IgnoreSyncCtx);
				threadHelper.SetExecutionContextHelper(executionContext);
			}
			IPrincipal principal = null;
			this.StartInternal(principal, ref stackMark);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		internal ExecutionContext.Reader GetExecutionContextReader()
		{
			return new ExecutionContext.Reader(this.m_ExecutionContext);
		}

		internal bool ExecutionContextBelongsToCurrentScope
		{
			get
			{
				return !this.m_ExecutionContextBelongsToOuterScope;
			}
			set
			{
				this.m_ExecutionContextBelongsToOuterScope = !value;
			}
		}

		public ExecutionContext ExecutionContext
		{
			[SecuritySafeCritical]
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
			get
			{
				ExecutionContext executionContext;
				if (this == Thread.CurrentThread)
				{
					executionContext = this.GetMutableExecutionContext();
				}
				else
				{
					executionContext = this.m_ExecutionContext;
				}
				return executionContext;
			}
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		internal ExecutionContext GetMutableExecutionContext()
		{
			if (this.m_ExecutionContext == null)
			{
				this.m_ExecutionContext = new ExecutionContext();
			}
			else if (!this.ExecutionContextBelongsToCurrentScope)
			{
				ExecutionContext executionContext = this.m_ExecutionContext.CreateMutableCopy();
				this.m_ExecutionContext = executionContext;
			}
			this.ExecutionContextBelongsToCurrentScope = true;
			return this.m_ExecutionContext;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[SecurityCritical]
		internal void SetExecutionContext(ExecutionContext value, bool belongsToCurrentScope)
		{
			this.m_ExecutionContext = value;
			this.ExecutionContextBelongsToCurrentScope = belongsToCurrentScope;
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		internal void SetExecutionContext(ExecutionContext.Reader value, bool belongsToCurrentScope)
		{
			this.m_ExecutionContext = value.DangerousGetRawExecutionContext();
			this.ExecutionContextBelongsToCurrentScope = belongsToCurrentScope;
		}

		[Obsolete("Thread.SetCompressedStack is no longer supported. Please use the System.Threading.CompressedStack class")]
		[SecurityCritical]
		public void SetCompressedStack(CompressedStack stack)
		{
			throw new InvalidOperationException(Environment.GetResourceString("Use CompressedStack.(Capture/Run) or ExecutionContext.(Capture/Run) APIs instead."));
		}

		[SecurityCritical]
		[Obsolete("Thread.GetCompressedStack is no longer supported. Please use the System.Threading.CompressedStack class")]
		public CompressedStack GetCompressedStack()
		{
			throw new InvalidOperationException(Environment.GetResourceString("Use CompressedStack.(Capture/Run) or ExecutionContext.(Capture/Run) APIs instead."));
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Demand, ControlThread = true)]
		public static void ResetAbort()
		{
			Thread currentThread = Thread.CurrentThread;
			if ((currentThread.ThreadState & ThreadState.AbortRequested) == ThreadState.Running)
			{
				throw new ThreadStateException(Environment.GetResourceString("Unable to reset abort because no abort was requested."));
			}
			currentThread.ResetAbortNative();
			currentThread.ClearAbortReason();
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ResetAbortNative();

		[Obsolete("Thread.Suspend has been deprecated.  Please use other classes in System.Threading, such as Monitor, Mutex, Event, and Semaphore, to synchronize Threads or protect resources.  http://go.microsoft.com/fwlink/?linkid=14202", false)]
		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Demand, ControlThread = true)]
		[SecurityPermission(SecurityAction.Demand, ControlThread = true)]
		public void Suspend()
		{
			this.SuspendInternal();
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SuspendInternal();

		[Obsolete("Thread.Resume has been deprecated.  Please use other classes in System.Threading, such as Monitor, Mutex, Event, and Semaphore, to synchronize Threads or protect resources.  http://go.microsoft.com/fwlink/?linkid=14202", false)]
		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Demand, ControlThread = true)]
		public void Resume()
		{
			this.ResumeInternal();
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ResumeInternal();

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Demand, ControlThread = true)]
		public void Interrupt()
		{
			this.InterruptInternal();
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InterruptInternal();

		public ThreadPriority Priority
		{
			[SecuritySafeCritical]
			get
			{
				return (ThreadPriority)this.GetPriorityNative();
			}
			[SecuritySafeCritical]
			[HostProtection(SecurityAction.LinkDemand, SelfAffectingThreading = true)]
			set
			{
				this.SetPriorityNative((int)value);
			}
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetPriorityNative();

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetPriorityNative(int priority);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool JoinInternal(int millisecondsTimeout);

		[SecuritySafeCritical]
		[HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
		public void Join()
		{
			this.JoinInternal(-1);
		}

		[SecuritySafeCritical]
		[HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
		public bool Join(int millisecondsTimeout)
		{
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout", Environment.GetResourceString("Number must be either non-negative and less than or equal to Int32.MaxValue or -1."));
			}
			return this.JoinInternal(millisecondsTimeout);
		}

		[HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
		public bool Join(TimeSpan timeout)
		{
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1L || num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("timeout", Environment.GetResourceString("Number must be either non-negative and less than or equal to Int32.MaxValue or -1."));
			}
			return this.Join((int)num);
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SleepInternal(int millisecondsTimeout);

		[SecuritySafeCritical]
		public static void Sleep(int millisecondsTimeout)
		{
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout", Environment.GetResourceString("Number must be either non-negative and less than or equal to Int32.MaxValue or -1."));
			}
			Thread.SleepInternal(millisecondsTimeout);
		}

		public static void Sleep(TimeSpan timeout)
		{
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1L || num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("timeout", Environment.GetResourceString("Number must be either non-negative and less than or equal to Int32.MaxValue or -1."));
			}
			Thread.Sleep((int)num);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool YieldInternal();

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[SecuritySafeCritical]
		[HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
		public static bool Yield()
		{
			return Thread.YieldInternal();
		}

		[SecurityCritical]
		private void SetStartHelper(Delegate start, int maxStackSize)
		{
			maxStackSize = Thread.GetProcessDefaultStackSize(maxStackSize);
			ThreadHelper threadHelper = new ThreadHelper(start);
			if (start is ThreadStart)
			{
				this.SetStart(new ThreadStart(threadHelper.ThreadStart), maxStackSize);
				return;
			}
			this.SetStart(new ParameterizedThreadStart(threadHelper.ThreadStart), maxStackSize);
		}

		[HostProtection(SecurityAction.LinkDemand, SharedState = true, ExternalThreading = true)]
		public static LocalDataStoreSlot AllocateDataSlot()
		{
			return Thread.LocalDataStoreManager.AllocateDataSlot();
		}

		[HostProtection(SecurityAction.LinkDemand, SharedState = true, ExternalThreading = true)]
		public static LocalDataStoreSlot AllocateNamedDataSlot(string name)
		{
			return Thread.LocalDataStoreManager.AllocateNamedDataSlot(name);
		}

		[HostProtection(SecurityAction.LinkDemand, SharedState = true, ExternalThreading = true)]
		public static LocalDataStoreSlot GetNamedDataSlot(string name)
		{
			return Thread.LocalDataStoreManager.GetNamedDataSlot(name);
		}

		[HostProtection(SecurityAction.LinkDemand, SharedState = true, ExternalThreading = true)]
		public static void FreeNamedDataSlot(string name)
		{
			Thread.LocalDataStoreManager.FreeNamedDataSlot(name);
		}

		[HostProtection(SecurityAction.LinkDemand, SharedState = true, ExternalThreading = true)]
		public static object GetData(LocalDataStoreSlot slot)
		{
			LocalDataStoreHolder localDataStoreHolder = Thread.s_LocalDataStore;
			if (localDataStoreHolder == null)
			{
				Thread.LocalDataStoreManager.ValidateSlot(slot);
				return null;
			}
			return localDataStoreHolder.Store.GetData(slot);
		}

		[HostProtection(SecurityAction.LinkDemand, SharedState = true, ExternalThreading = true)]
		public static void SetData(LocalDataStoreSlot slot, object data)
		{
			LocalDataStoreHolder localDataStoreHolder = Thread.s_LocalDataStore;
			if (localDataStoreHolder == null)
			{
				localDataStoreHolder = Thread.LocalDataStoreManager.CreateLocalDataStore();
				Thread.s_LocalDataStore = localDataStoreHolder;
			}
			localDataStoreHolder.Store.SetData(slot, data);
		}

		public CultureInfo CurrentUICulture
		{
			get
			{
				return this.GetCurrentUICultureNoAppX();
			}
			[SecuritySafeCritical]
			[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				CultureInfo.VerifyCultureName(value, true);
				if (Thread.m_CurrentUICulture == null && Thread.m_CurrentCulture == null)
				{
					Thread.nativeInitCultureAccessors();
				}
				if (!AppContextSwitches.NoAsyncCurrentCulture)
				{
					if (Thread.s_asyncLocalCurrentUICulture == null)
					{
						Interlocked.CompareExchange<AsyncLocal<CultureInfo>>(ref Thread.s_asyncLocalCurrentUICulture, new AsyncLocal<CultureInfo>(new Action<AsyncLocalValueChangedArgs<CultureInfo>>(Thread.AsyncLocalSetCurrentUICulture)), null);
					}
					Thread.s_asyncLocalCurrentUICulture.Value = value;
					return;
				}
				Thread.m_CurrentUICulture = value;
			}
		}

		internal CultureInfo GetCurrentUICultureNoAppX()
		{
			if (Thread.m_CurrentUICulture != null)
			{
				return Thread.m_CurrentUICulture;
			}
			CultureInfo defaultThreadCurrentUICulture = CultureInfo.DefaultThreadCurrentUICulture;
			if (defaultThreadCurrentUICulture == null)
			{
				return CultureInfo.UserDefaultUICulture;
			}
			return defaultThreadCurrentUICulture;
		}

		public CultureInfo CurrentCulture
		{
			get
			{
				return this.GetCurrentCultureNoAppX();
			}
			[SecuritySafeCritical]
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (Thread.m_CurrentCulture == null && Thread.m_CurrentUICulture == null)
				{
					Thread.nativeInitCultureAccessors();
				}
				if (!AppContextSwitches.NoAsyncCurrentCulture)
				{
					if (Thread.s_asyncLocalCurrentCulture == null)
					{
						Interlocked.CompareExchange<AsyncLocal<CultureInfo>>(ref Thread.s_asyncLocalCurrentCulture, new AsyncLocal<CultureInfo>(new Action<AsyncLocalValueChangedArgs<CultureInfo>>(Thread.AsyncLocalSetCurrentCulture)), null);
					}
					Thread.s_asyncLocalCurrentCulture.Value = value;
					return;
				}
				Thread.m_CurrentCulture = value;
			}
		}

		private CultureInfo GetCurrentCultureNoAppX()
		{
			if (Thread.m_CurrentCulture != null)
			{
				return Thread.m_CurrentCulture;
			}
			CultureInfo defaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentCulture;
			if (defaultThreadCurrentCulture == null)
			{
				return CultureInfo.UserDefaultCulture;
			}
			return defaultThreadCurrentCulture;
		}

		private static void nativeInitCultureAccessors()
		{
			Thread.m_CurrentCulture = CultureInfo.ConstructCurrentCulture();
			Thread.m_CurrentUICulture = CultureInfo.ConstructCurrentUICulture();
		}

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void MemoryBarrier();

		private static LocalDataStoreMgr LocalDataStoreManager
		{
			get
			{
				if (Thread.s_LocalDataStoreMgr == null)
				{
					Interlocked.CompareExchange<LocalDataStoreMgr>(ref Thread.s_LocalDataStoreMgr, new LocalDataStoreMgr(), null);
				}
				return Thread.s_LocalDataStoreMgr;
			}
		}

		void _Thread.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _Thread.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _Thread.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _Thread.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ConstructInternalThread();

		private InternalThread Internal
		{
			get
			{
				if (this.internal_thread == null)
				{
					this.ConstructInternalThread();
				}
				return this.internal_thread;
			}
		}

		public static Context CurrentContext
		{
			[SecurityPermission(SecurityAction.LinkDemand, Infrastructure = true)]
			get
			{
				return AppDomain.InternalGetContext();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern byte[] ByteArrayToRootDomain(byte[] arr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern byte[] ByteArrayToCurrentDomain(byte[] arr);

		private static void DeserializePrincipal(Thread th)
		{
			MemoryStream memoryStream = new MemoryStream(Thread.ByteArrayToCurrentDomain(th.Internal._serialized_principal));
			int num = memoryStream.ReadByte();
			if (num == 0)
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				th.principal = (IPrincipal)binaryFormatter.Deserialize(memoryStream);
				th.principal_version = th.Internal._serialized_principal_version;
				return;
			}
			if (num == 1)
			{
				BinaryReader binaryReader = new BinaryReader(memoryStream);
				string text = binaryReader.ReadString();
				string text2 = binaryReader.ReadString();
				int num2 = binaryReader.ReadInt32();
				string[] array = null;
				if (num2 >= 0)
				{
					array = new string[num2];
					for (int i = 0; i < num2; i++)
					{
						array[i] = binaryReader.ReadString();
					}
				}
				th.principal = new GenericPrincipal(new GenericIdentity(text, text2), array);
				return;
			}
			if (num == 2 || num == 3)
			{
				string[] array2 = ((num == 2) ? null : new string[0]);
				th.principal = new GenericPrincipal(new GenericIdentity("", ""), array2);
			}
		}

		private static void SerializePrincipal(Thread th, IPrincipal value)
		{
			MemoryStream memoryStream = new MemoryStream();
			bool flag = false;
			if (value.GetType() == typeof(GenericPrincipal))
			{
				GenericPrincipal genericPrincipal = (GenericPrincipal)value;
				if (genericPrincipal.Identity != null && genericPrincipal.Identity.GetType() == typeof(GenericIdentity))
				{
					GenericIdentity genericIdentity = (GenericIdentity)genericPrincipal.Identity;
					if (genericIdentity.Name == "" && genericIdentity.AuthenticationType == "")
					{
						if (genericPrincipal.Roles == null)
						{
							memoryStream.WriteByte(2);
							flag = true;
						}
						else if (genericPrincipal.Roles.Length == 0)
						{
							memoryStream.WriteByte(3);
							flag = true;
						}
					}
					else
					{
						memoryStream.WriteByte(1);
						BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
						binaryWriter.Write(genericPrincipal.Identity.Name);
						binaryWriter.Write(genericPrincipal.Identity.AuthenticationType);
						string[] roles = genericPrincipal.Roles;
						if (roles == null)
						{
							binaryWriter.Write(-1);
						}
						else
						{
							binaryWriter.Write(roles.Length);
							foreach (string text in roles)
							{
								binaryWriter.Write(text);
							}
						}
						binaryWriter.Flush();
						flag = true;
					}
				}
			}
			if (!flag)
			{
				memoryStream.WriteByte(0);
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				try
				{
					binaryFormatter.Serialize(memoryStream, value);
				}
				catch
				{
				}
			}
			th.Internal._serialized_principal = Thread.ByteArrayToRootDomain(memoryStream.ToArray());
		}

		public static IPrincipal CurrentPrincipal
		{
			get
			{
				Thread currentThread = Thread.CurrentThread;
				if (currentThread.principal_version != currentThread.Internal._serialized_principal_version)
				{
					currentThread.principal = null;
				}
				if (currentThread.principal != null)
				{
					return currentThread.principal;
				}
				if (currentThread.Internal._serialized_principal != null)
				{
					try
					{
						Thread.DeserializePrincipal(currentThread);
						return currentThread.principal;
					}
					catch
					{
					}
				}
				currentThread.principal = Thread.GetDomain().DefaultPrincipal;
				currentThread.principal_version = currentThread.Internal._serialized_principal_version;
				return currentThread.principal;
			}
			[SecurityPermission(SecurityAction.Demand, ControlPrincipal = true)]
			set
			{
				Thread currentThread = Thread.CurrentThread;
				if (value != Thread.GetDomain().DefaultPrincipal)
				{
					currentThread.Internal._serialized_principal_version++;
					try
					{
						Thread.SerializePrincipal(currentThread, value);
					}
					catch (Exception)
					{
						currentThread.Internal._serialized_principal = null;
					}
					currentThread.principal_version = currentThread.Internal._serialized_principal_version;
				}
				else
				{
					currentThread.Internal._serialized_principal = null;
				}
				currentThread.principal = value;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Thread GetCurrentThread();

		public static Thread CurrentThread
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
			get
			{
				Thread thread = Thread.current_thread;
				if (thread != null)
				{
					return thread;
				}
				return Thread.GetCurrentThread();
			}
		}

		internal static int CurrentThreadId
		{
			get
			{
				return (int)Thread.CurrentThread.internal_thread.thread_id;
			}
		}

		public static AppDomain GetDomain()
		{
			return AppDomain.CurrentDomain;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetDomainID();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern IntPtr Thread_internal(MulticastDelegate start);

		private Thread(InternalThread it)
		{
			this.internal_thread = it;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		~Thread()
		{
		}

		[Obsolete("Deprecated in favor of GetApartmentState, SetApartmentState and TrySetApartmentState.")]
		public ApartmentState ApartmentState
		{
			get
			{
				this.ValidateThreadState();
				return (ApartmentState)this.Internal.apartment_state;
			}
			set
			{
				this.ValidateThreadState();
				this.TrySetApartmentState(value);
			}
		}

		public bool IsThreadPoolThread
		{
			get
			{
				return this.IsThreadPoolThreadInternal;
			}
		}

		internal bool IsThreadPoolThreadInternal
		{
			get
			{
				return this.Internal.threadpool_thread;
			}
			set
			{
				this.Internal.threadpool_thread = value;
			}
		}

		public bool IsAlive
		{
			get
			{
				ThreadState state = Thread.GetState(this.Internal);
				return (state & ThreadState.Aborted) == ThreadState.Running && (state & ThreadState.Stopped) == ThreadState.Running && (state & ThreadState.Unstarted) == ThreadState.Running;
			}
		}

		public bool IsBackground
		{
			get
			{
				return (this.ValidateThreadState() & ThreadState.Background) > ThreadState.Running;
			}
			set
			{
				this.ValidateThreadState();
				if (value)
				{
					Thread.SetState(this.Internal, ThreadState.Background);
					return;
				}
				Thread.ClrState(this.Internal, ThreadState.Background);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetName_internal(InternalThread thread);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetName_internal(InternalThread thread, string name);

		public string Name
		{
			get
			{
				return Thread.GetName_internal(this.Internal);
			}
			set
			{
				Thread.SetName_internal(this.Internal, value);
			}
		}

		public ThreadState ThreadState
		{
			get
			{
				return Thread.GetState(this.Internal);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Abort_internal(InternalThread thread, object stateInfo);

		[SecurityPermission(SecurityAction.Demand, ControlThread = true)]
		public void Abort()
		{
			Thread.Abort_internal(this.Internal, null);
		}

		[SecurityPermission(SecurityAction.Demand, ControlThread = true)]
		public void Abort(object stateInfo)
		{
			Thread.Abort_internal(this.Internal, stateInfo);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern object GetAbortExceptionState();

		internal object AbortReason
		{
			get
			{
				return this.GetAbortExceptionState();
			}
		}

		private void ClearAbortReason()
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SpinWait_nop();

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static void SpinWait(int iterations)
		{
			if (iterations < 0)
			{
				return;
			}
			while (iterations-- > 0)
			{
				Thread.SpinWait_nop();
			}
		}

		private void StartInternal(IPrincipal principal, ref StackCrawlMark stackMark)
		{
			this.Internal._serialized_principal = Thread.CurrentThread.Internal._serialized_principal;
			if (this.Thread_internal(this.m_Delegate) == IntPtr.Zero)
			{
				throw new SystemException("Thread creation failed.");
			}
			this.m_ThreadStartArg = null;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetState(InternalThread thread, ThreadState set);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClrState(InternalThread thread, ThreadState clr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ThreadState GetState(InternalThread thread);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern byte VolatileRead(ref byte address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double VolatileRead(ref double address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern short VolatileRead(ref short address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int VolatileRead(ref int address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long VolatileRead(ref long address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr VolatileRead(ref IntPtr address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern object VolatileRead(ref object address);

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern sbyte VolatileRead(ref sbyte address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float VolatileRead(ref float address);

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ushort VolatileRead(ref ushort address);

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint VolatileRead(ref uint address);

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ulong VolatileRead(ref ulong address);

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UIntPtr VolatileRead(ref UIntPtr address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref byte address, byte value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref double address, double value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref short address, short value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref int address, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref long address, long value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref IntPtr address, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref object address, object value);

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref sbyte address, sbyte value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref float address, float value);

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref ushort address, ushort value);

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref uint address, uint value);

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref ulong address, ulong value);

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref UIntPtr address, UIntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int SystemMaxStackStize();

		private static int GetProcessDefaultStackSize(int maxStackSize)
		{
			if (maxStackSize == 0)
			{
				return 0;
			}
			if (maxStackSize < 131072)
			{
				return 131072;
			}
			int pageSize = Environment.GetPageSize();
			if (maxStackSize % pageSize != 0)
			{
				maxStackSize = maxStackSize / (pageSize - 1) * pageSize;
			}
			return Math.Min(maxStackSize, Thread.SystemMaxStackStize());
		}

		private void SetStart(MulticastDelegate start, int maxStackSize)
		{
			this.m_Delegate = start;
			this.Internal.stack_size = maxStackSize;
		}

		public int ManagedThreadId
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				return this.Internal.managed_id;
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static void BeginCriticalRegion()
		{
			Thread.CurrentThread.Internal.critical_region_level++;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static void EndCriticalRegion()
		{
			Thread.CurrentThread.Internal.critical_region_level--;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static void BeginThreadAffinity()
		{
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static void EndThreadAffinity()
		{
		}

		public ApartmentState GetApartmentState()
		{
			this.ValidateThreadState();
			return (ApartmentState)this.Internal.apartment_state;
		}

		public void SetApartmentState(ApartmentState state)
		{
			if (!this.TrySetApartmentState(state))
			{
				throw new InvalidOperationException("Failed to set the specified COM apartment state.");
			}
		}

		public bool TrySetApartmentState(ApartmentState state)
		{
			if ((this.ThreadState & ThreadState.Unstarted) == ThreadState.Running)
			{
				throw new ThreadStateException("Thread was in an invalid state for the operation being executed.");
			}
			if (this.Internal.apartment_state != 2 && (ApartmentState)this.Internal.apartment_state != state)
			{
				return false;
			}
			this.Internal.apartment_state = (byte)state;
			return true;
		}

		[ComVisible(false)]
		public override int GetHashCode()
		{
			return this.ManagedThreadId;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void GetStackTraces(out Thread[] threads, out object[] stack_frames);

		internal static Dictionary<Thread, StackTrace> Mono_GetStackTraces()
		{
			Thread[] array;
			object[] array2;
			Thread.GetStackTraces(out array, out array2);
			Dictionary<Thread, StackTrace> dictionary = new Dictionary<Thread, StackTrace>();
			for (int i = 0; i < array.Length; i++)
			{
				dictionary[array[i]] = new StackTrace((StackFrame[])array2[i]);
			}
			return dictionary;
		}

		public void DisableComObjectEagerCleanup()
		{
			throw new PlatformNotSupportedException();
		}

		private ThreadState ValidateThreadState()
		{
			ThreadState state = Thread.GetState(this.Internal);
			if ((state & ThreadState.Stopped) != ThreadState.Running)
			{
				throw new ThreadStateException("Thread is dead; state can not be accessed.");
			}
			return state;
		}

		private static LocalDataStoreMgr s_LocalDataStoreMgr;

		[ThreadStatic]
		private static LocalDataStoreHolder s_LocalDataStore;

		[ThreadStatic]
		internal static CultureInfo m_CurrentCulture;

		[ThreadStatic]
		internal static CultureInfo m_CurrentUICulture;

		private static AsyncLocal<CultureInfo> s_asyncLocalCurrentCulture;

		private static AsyncLocal<CultureInfo> s_asyncLocalCurrentUICulture;

		private InternalThread internal_thread;

		private object m_ThreadStartArg;

		private object pending_exception;

		private IPrincipal principal;

		private int principal_version;

		[ThreadStatic]
		private static Thread current_thread;

		private MulticastDelegate m_Delegate;

		private ExecutionContext m_ExecutionContext;

		private bool m_ExecutionContextBelongsToOuterScope;
	}
}
