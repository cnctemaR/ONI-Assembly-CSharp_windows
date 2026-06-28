using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using System.Security.Principal;

namespace System.Threading
{
	[ComDefaultInterface(typeof(_Thread))]
	[ClassInterface(ClassInterfaceType.None)]
	[ComVisible(true)]
	public sealed class Thread : CriticalFinalizerObject, _Thread
	{
		public Thread(ThreadStart start)
		{
			if (start == null)
			{
				throw new ArgumentNullException("Null ThreadStart");
			}
			this.threadstart = start;
			this.Thread_init();
		}

		public Thread(ThreadStart start, int maxStackSize)
		{
			if (start == null)
			{
				throw new ArgumentNullException("start");
			}
			if (maxStackSize < 131072)
			{
				throw new ArgumentException("< 128 kb", "maxStackSize");
			}
			this.threadstart = start;
			this.stack_size = maxStackSize;
			this.Thread_init();
		}

		public Thread(ParameterizedThreadStart start)
		{
			if (start == null)
			{
				throw new ArgumentNullException("start");
			}
			this.threadstart = start;
			this.Thread_init();
		}

		public Thread(ParameterizedThreadStart start, int maxStackSize)
		{
			if (start == null)
			{
				throw new ArgumentNullException("start");
			}
			if (maxStackSize < 131072)
			{
				throw new ArgumentException("< 128 kb", "maxStackSize");
			}
			this.threadstart = start;
			this.stack_size = maxStackSize;
			this.Thread_init();
		}

		void _Thread.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _Thread.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _Thread.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _Thread.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		public static Context CurrentContext
		{
			[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"Infrastructure\"/>\n</PermissionSet>\n")]
			get
			{
				return AppDomain.InternalGetContext();
			}
		}

		public static IPrincipal CurrentPrincipal
		{
			get
			{
				IPrincipal principal = null;
				Thread currentThread = Thread.CurrentThread;
				Thread thread = currentThread;
				lock (thread)
				{
					principal = currentThread._principal;
					if (principal == null)
					{
						principal = Thread.GetDomain().DefaultPrincipal;
					}
				}
				return principal;
			}
			[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlPrincipal\"/>\n</PermissionSet>\n")]
			set
			{
				Thread currentThread = Thread.CurrentThread;
				Thread thread = currentThread;
				lock (thread)
				{
					currentThread._principal = value;
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Thread CurrentThread_internal();

		public static Thread CurrentThread
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
			get
			{
				return Thread.CurrentThread_internal();
			}
		}

		internal static int CurrentThreadId
		{
			get
			{
				return (int)Thread.CurrentThread.thread_id;
			}
		}

		private static void InitDataStoreHash()
		{
			object obj = Thread.datastore_lock;
			lock (obj)
			{
				if (Thread.datastorehash == null)
				{
					Thread.datastorehash = Hashtable.Synchronized(new Hashtable());
				}
			}
		}

		public static LocalDataStoreSlot AllocateNamedDataSlot(string name)
		{
			object obj = Thread.datastore_lock;
			LocalDataStoreSlot localDataStoreSlot2;
			lock (obj)
			{
				if (Thread.datastorehash == null)
				{
					Thread.InitDataStoreHash();
				}
				LocalDataStoreSlot localDataStoreSlot = (LocalDataStoreSlot)Thread.datastorehash[name];
				if (localDataStoreSlot != null)
				{
					throw new ArgumentException("Named data slot already added");
				}
				localDataStoreSlot = Thread.AllocateDataSlot();
				Thread.datastorehash.Add(name, localDataStoreSlot);
				localDataStoreSlot2 = localDataStoreSlot;
			}
			return localDataStoreSlot2;
		}

		public static void FreeNamedDataSlot(string name)
		{
			object obj = Thread.datastore_lock;
			lock (obj)
			{
				if (Thread.datastorehash != null)
				{
					Thread.datastorehash.Remove(name);
				}
			}
		}

		public static LocalDataStoreSlot AllocateDataSlot()
		{
			return new LocalDataStoreSlot(true);
		}

		public static object GetData(LocalDataStoreSlot slot)
		{
			object[] array = Thread.local_slots;
			if (slot == null)
			{
				throw new ArgumentNullException("slot");
			}
			if (array != null && slot.slot < array.Length)
			{
				return array[slot.slot];
			}
			return null;
		}

		public static void SetData(LocalDataStoreSlot slot, object data)
		{
			object[] array = Thread.local_slots;
			if (slot == null)
			{
				throw new ArgumentNullException("slot");
			}
			if (array == null)
			{
				array = new object[slot.slot + 2];
				Thread.local_slots = array;
			}
			else if (slot.slot >= array.Length)
			{
				object[] array2 = new object[slot.slot + 2];
				array.CopyTo(array2, 0);
				array = array2;
				Thread.local_slots = array;
			}
			array[slot.slot] = data;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void FreeLocalSlotValues(int slot, bool thread_local);

		public static LocalDataStoreSlot GetNamedDataSlot(string name)
		{
			object obj = Thread.datastore_lock;
			LocalDataStoreSlot localDataStoreSlot2;
			lock (obj)
			{
				if (Thread.datastorehash == null)
				{
					Thread.InitDataStoreHash();
				}
				LocalDataStoreSlot localDataStoreSlot = (LocalDataStoreSlot)Thread.datastorehash[name];
				if (localDataStoreSlot == null)
				{
					localDataStoreSlot = Thread.AllocateNamedDataSlot(name);
				}
				localDataStoreSlot2 = localDataStoreSlot;
			}
			return localDataStoreSlot2;
		}

		public static AppDomain GetDomain()
		{
			return AppDomain.CurrentDomain;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetDomainID();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetAbort_internal();

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlThread\"/>\n</PermissionSet>\n")]
		public static void ResetAbort()
		{
			Thread.ResetAbort_internal();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Sleep_internal(int ms);

		public static void Sleep(int millisecondsTimeout)
		{
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout", "Negative timeout");
			}
			Thread.Sleep_internal(millisecondsTimeout);
		}

		public static void Sleep(TimeSpan timeout)
		{
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1L || num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("timeout", "timeout out of range");
			}
			Thread.Sleep_internal((int)num);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern IntPtr Thread_internal(MulticastDelegate start);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Thread_init();

		[Obsolete("Deprecated in favor of GetApartmentState, SetApartmentState and TrySetApartmentState.")]
		public ApartmentState ApartmentState
		{
			get
			{
				if ((this.ThreadState & ThreadState.Stopped) != ThreadState.Running)
				{
					throw new ThreadStateException("Thread is dead; state can not be accessed.");
				}
				return (ApartmentState)this.apartment_state;
			}
			set
			{
				this.TrySetApartmentState(value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern CultureInfo GetCachedCurrentCulture();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern byte[] GetSerializedCurrentCulture();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetCachedCurrentCulture(CultureInfo culture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetSerializedCurrentCulture(byte[] culture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern CultureInfo GetCachedCurrentUICulture();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern byte[] GetSerializedCurrentUICulture();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetCachedCurrentUICulture(CultureInfo culture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetSerializedCurrentUICulture(byte[] culture);

		public CultureInfo CurrentCulture
		{
			get
			{
				if (this.in_currentculture)
				{
					return CultureInfo.InvariantCulture;
				}
				CultureInfo cultureInfo = this.GetCachedCurrentCulture();
				if (cultureInfo != null)
				{
					return cultureInfo;
				}
				byte[] serializedCurrentCulture = this.GetSerializedCurrentCulture();
				if (serializedCurrentCulture == null)
				{
					object obj = Thread.culture_lock;
					lock (obj)
					{
						this.in_currentculture = true;
						cultureInfo = CultureInfo.ConstructCurrentCulture();
						this.SetCachedCurrentCulture(cultureInfo);
						this.in_currentculture = false;
						NumberFormatter.SetThreadCurrentCulture(cultureInfo);
						return cultureInfo;
					}
				}
				this.in_currentculture = true;
				try
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					MemoryStream memoryStream = new MemoryStream(serializedCurrentCulture);
					cultureInfo = (CultureInfo)binaryFormatter.Deserialize(memoryStream);
					this.SetCachedCurrentCulture(cultureInfo);
				}
				finally
				{
					this.in_currentculture = false;
				}
				NumberFormatter.SetThreadCurrentCulture(cultureInfo);
				return cultureInfo;
			}
			[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlThread\"/>\n</PermissionSet>\n")]
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				CultureInfo cachedCurrentCulture = this.GetCachedCurrentCulture();
				if (cachedCurrentCulture == value)
				{
					return;
				}
				value.CheckNeutral();
				this.in_currentculture = true;
				try
				{
					this.SetCachedCurrentCulture(value);
					byte[] array;
					if (value.IsReadOnly && value.cached_serialized_form != null)
					{
						array = value.cached_serialized_form;
					}
					else
					{
						BinaryFormatter binaryFormatter = new BinaryFormatter();
						MemoryStream memoryStream = new MemoryStream();
						binaryFormatter.Serialize(memoryStream, value);
						array = memoryStream.GetBuffer();
						if (value.IsReadOnly)
						{
							value.cached_serialized_form = array;
						}
					}
					this.SetSerializedCurrentCulture(array);
				}
				finally
				{
					this.in_currentculture = false;
				}
				NumberFormatter.SetThreadCurrentCulture(value);
			}
		}

		public CultureInfo CurrentUICulture
		{
			get
			{
				if (this.in_currentculture)
				{
					return CultureInfo.InvariantCulture;
				}
				CultureInfo cultureInfo = this.GetCachedCurrentUICulture();
				if (cultureInfo != null)
				{
					return cultureInfo;
				}
				byte[] serializedCurrentUICulture = this.GetSerializedCurrentUICulture();
				if (serializedCurrentUICulture == null)
				{
					object obj = Thread.culture_lock;
					lock (obj)
					{
						this.in_currentculture = true;
						cultureInfo = CultureInfo.ConstructCurrentUICulture();
						this.SetCachedCurrentUICulture(cultureInfo);
						this.in_currentculture = false;
						return cultureInfo;
					}
				}
				this.in_currentculture = true;
				try
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					MemoryStream memoryStream = new MemoryStream(serializedCurrentUICulture);
					cultureInfo = (CultureInfo)binaryFormatter.Deserialize(memoryStream);
					this.SetCachedCurrentUICulture(cultureInfo);
				}
				finally
				{
					this.in_currentculture = false;
				}
				return cultureInfo;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				CultureInfo cachedCurrentUICulture = this.GetCachedCurrentUICulture();
				if (cachedCurrentUICulture == value)
				{
					return;
				}
				this.in_currentculture = true;
				try
				{
					this.SetCachedCurrentUICulture(value);
					byte[] array;
					if (value.IsReadOnly && value.cached_serialized_form != null)
					{
						array = value.cached_serialized_form;
					}
					else
					{
						BinaryFormatter binaryFormatter = new BinaryFormatter();
						MemoryStream memoryStream = new MemoryStream();
						binaryFormatter.Serialize(memoryStream, value);
						array = memoryStream.GetBuffer();
						if (value.IsReadOnly)
						{
							value.cached_serialized_form = array;
						}
					}
					this.SetSerializedCurrentUICulture(array);
				}
				finally
				{
					this.in_currentculture = false;
				}
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
				return this.threadpool_thread;
			}
			set
			{
				this.threadpool_thread = value;
			}
		}

		public bool IsAlive
		{
			get
			{
				ThreadState threadState = this.GetState();
				return (threadState & ThreadState.Aborted) == ThreadState.Running && (threadState & ThreadState.Stopped) == ThreadState.Running && (threadState & ThreadState.Unstarted) == ThreadState.Running;
			}
		}

		public bool IsBackground
		{
			get
			{
				ThreadState threadState = this.GetState();
				if ((threadState & ThreadState.Stopped) != ThreadState.Running)
				{
					throw new ThreadStateException("Thread is dead; state can not be accessed.");
				}
				return (threadState & ThreadState.Background) != ThreadState.Running;
			}
			set
			{
				if (value)
				{
					this.SetState(ThreadState.Background);
				}
				else
				{
					this.ClrState(ThreadState.Background);
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string GetName_internal();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetName_internal(string name);

		public string Name
		{
			get
			{
				return this.GetName_internal();
			}
			set
			{
				this.SetName_internal(value);
			}
		}

		public ThreadPriority Priority
		{
			get
			{
				return ThreadPriority.Lowest;
			}
			set
			{
			}
		}

		public ThreadState ThreadState
		{
			get
			{
				return this.GetState();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Abort_internal(object stateInfo);

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlThread\"/>\n</PermissionSet>\n")]
		public void Abort()
		{
			this.Abort_internal(null);
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlThread\"/>\n</PermissionSet>\n")]
		public void Abort(object stateInfo)
		{
			this.Abort_internal(stateInfo);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern object GetAbortExceptionState();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Interrupt_internal();

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlThread\"/>\n</PermissionSet>\n")]
		public void Interrupt()
		{
			this.Interrupt_internal();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Join_internal(int ms, IntPtr handle);

		public void Join()
		{
			this.Join_internal(-1, this.system_thread_handle);
		}

		public bool Join(int millisecondsTimeout)
		{
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout", "Timeout less than zero");
			}
			return this.Join_internal(millisecondsTimeout, this.system_thread_handle);
		}

		public bool Join(TimeSpan timeout)
		{
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1L || num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("timeout", "timeout out of range");
			}
			return this.Join_internal((int)num, this.system_thread_handle);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void MemoryBarrier();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Resume_internal();

		[Obsolete("")]
		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlThread\"/>\n</PermissionSet>\n")]
		public void Resume()
		{
			this.Resume_internal();
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

		public void Start()
		{
			if (!ExecutionContext.IsFlowSuppressed())
			{
				this.ec_to_set = ExecutionContext.Capture();
			}
			if (Thread.CurrentThread._principal != null)
			{
				this._principal = Thread.CurrentThread._principal;
			}
			if (this.Thread_internal(this.threadstart) == (IntPtr)0)
			{
				throw new SystemException("Thread creation failed.");
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Suspend_internal();

		[Obsolete("")]
		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlThread\"/>\n</PermissionSet>\n")]
		public void Suspend()
		{
			this.Suspend_internal();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Thread_free_internal(IntPtr handle);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		~Thread()
		{
			this.Thread_free_internal(this.system_thread_handle);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetState(ThreadState set);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ClrState(ThreadState clr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern ThreadState GetState();

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

		private static int GetNewManagedId()
		{
			return Thread.GetNewManagedId_internal();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetNewManagedId_internal();

		[MonoTODO("limited to CompressedStack support")]
		public ExecutionContext ExecutionContext
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
			get
			{
				if (Thread._ec == null)
				{
					Thread._ec = new ExecutionContext();
				}
				return Thread._ec;
			}
		}

		public int ManagedThreadId
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				if (this.managed_id == 0)
				{
					int newManagedId = Thread.GetNewManagedId();
					Interlocked.CompareExchange(ref this.managed_id, newManagedId, 0);
				}
				return this.managed_id;
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static void BeginCriticalRegion()
		{
			Thread.CurrentThread.critical_region_level++;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static void EndCriticalRegion()
		{
			Thread.CurrentThread.critical_region_level--;
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
			return (ApartmentState)this.apartment_state;
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
			if (this != Thread.CurrentThread && (this.ThreadState & ThreadState.Unstarted) == ThreadState.Running)
			{
				throw new ThreadStateException("Thread was in an invalid state for the operation being executed.");
			}
			if (this.apartment_state != 2)
			{
				return false;
			}
			this.apartment_state = (byte)state;
			return true;
		}

		[ComVisible(false)]
		public override int GetHashCode()
		{
			return this.ManagedThreadId;
		}

		public void Start(object parameter)
		{
			this.start_obj = parameter;
			this.Start();
		}

		[Obsolete("see CompressedStack class")]
		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n   <IPermission class=\"System.Security.Permissions.StrongNameIdentityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                PublicKeyBlob=\"00000000000000000400000000000000\"/>\n</PermissionSet>\n")]
		public CompressedStack GetCompressedStack()
		{
			CompressedStack compressedStack = this.ExecutionContext.SecurityContext.CompressedStack;
			return (compressedStack != null && !compressedStack.IsEmpty()) ? compressedStack.CreateCopy() : null;
		}

		[Obsolete("see CompressedStack class")]
		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n   <IPermission class=\"System.Security.Permissions.StrongNameIdentityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                PublicKeyBlob=\"00000000000000000400000000000000\"/>\n</PermissionSet>\n")]
		public void SetCompressedStack(CompressedStack stack)
		{
			this.ExecutionContext.SecurityContext.CompressedStack = stack;
		}

		private int lock_thread_id;

		private IntPtr system_thread_handle;

		private object cached_culture_info;

		private IntPtr unused0;

		private bool threadpool_thread;

		private IntPtr name;

		private int name_len;

		private ThreadState state = ThreadState.Unstarted;

		private object abort_exc;

		private int abort_state_handle;

		private long thread_id;

		private IntPtr start_notify;

		private IntPtr stack_ptr;

		private UIntPtr static_data;

		private IntPtr jit_data;

		private IntPtr lock_data;

		private object current_appcontext;

		private int stack_size;

		private object start_obj;

		private IntPtr appdomain_refs;

		private int interruption_requested;

		private IntPtr suspend_event;

		private IntPtr suspended_event;

		private IntPtr resume_event;

		private IntPtr synch_cs;

		private IntPtr serialized_culture_info;

		private int serialized_culture_info_len;

		private IntPtr serialized_ui_culture_info;

		private int serialized_ui_culture_info_len;

		private bool thread_dump_requested;

		private IntPtr end_stack;

		private bool thread_interrupt_requested;

		private byte apartment_state = 2;

		private volatile int critical_region_level;

		private int small_id;

		private IntPtr manage_callback;

		private object pending_exception;

		private ExecutionContext ec_to_set;

		private IntPtr interrupt_on_stop;

		private IntPtr unused3;

		private IntPtr unused4;

		private IntPtr unused5;

		private IntPtr unused6;

		[ThreadStatic]
		private static object[] local_slots;

		[ThreadStatic]
		private static ExecutionContext _ec;

		private MulticastDelegate threadstart;

		private int managed_id;

		private IPrincipal _principal;

		private static Hashtable datastorehash;

		private static object datastore_lock = new object();

		private bool in_currentculture;

		private static object culture_lock = new object();
	}
}
