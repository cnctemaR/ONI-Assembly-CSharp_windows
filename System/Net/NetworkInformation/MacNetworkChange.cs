using System;
using System.Runtime.InteropServices;
using Mono.Util;

namespace System.Net.NetworkInformation
{
	internal sealed class MacNetworkChange : INetworkChange, IDisposable
	{
		[DllImport("/usr/lib/libSystem.dylib")]
		private static extern IntPtr dlopen(string path, int mode);

		[DllImport("/usr/lib/libSystem.dylib")]
		private static extern IntPtr dlsym(IntPtr handle, string symbol);

		[DllImport("/usr/lib/libSystem.dylib")]
		private static extern int dlclose(IntPtr handle);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern void CFRelease(IntPtr handle);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern IntPtr CFRunLoopGetMain();

		[DllImport("/System/Library/Frameworks/SystemConfiguration.framework/SystemConfiguration")]
		private static extern IntPtr SCNetworkReachabilityCreateWithAddress(IntPtr allocator, ref MacNetworkChange.sockaddr_in sockaddr);

		[DllImport("/System/Library/Frameworks/SystemConfiguration.framework/SystemConfiguration")]
		private static extern bool SCNetworkReachabilityGetFlags(IntPtr reachability, out MacNetworkChange.NetworkReachabilityFlags flags);

		[DllImport("/System/Library/Frameworks/SystemConfiguration.framework/SystemConfiguration")]
		private static extern bool SCNetworkReachabilitySetCallback(IntPtr reachability, MacNetworkChange.SCNetworkReachabilityCallback callback, ref MacNetworkChange.SCNetworkReachabilityContext context);

		[DllImport("/System/Library/Frameworks/SystemConfiguration.framework/SystemConfiguration")]
		private static extern bool SCNetworkReachabilityScheduleWithRunLoop(IntPtr reachability, IntPtr runLoop, IntPtr runLoopMode);

		[DllImport("/System/Library/Frameworks/SystemConfiguration.framework/SystemConfiguration")]
		private static extern bool SCNetworkReachabilityUnscheduleFromRunLoop(IntPtr reachability, IntPtr runLoop, IntPtr runLoopMode);

		private event NetworkAddressChangedEventHandler networkAddressChanged;

		private event NetworkAvailabilityChangedEventHandler networkAvailabilityChanged;

		public event NetworkAddressChangedEventHandler NetworkAddressChanged
		{
			add
			{
				value(null, EventArgs.Empty);
				this.networkAddressChanged += value;
			}
			remove
			{
				this.networkAddressChanged -= value;
			}
		}

		public event NetworkAvailabilityChangedEventHandler NetworkAvailabilityChanged
		{
			add
			{
				value(null, new NetworkAvailabilityEventArgs(this.IsAvailable));
				this.networkAvailabilityChanged += value;
			}
			remove
			{
				this.networkAvailabilityChanged -= value;
			}
		}

		private bool IsAvailable
		{
			get
			{
				return (this.flags & MacNetworkChange.NetworkReachabilityFlags.Reachable) != MacNetworkChange.NetworkReachabilityFlags.None && (this.flags & MacNetworkChange.NetworkReachabilityFlags.ConnectionRequired) == MacNetworkChange.NetworkReachabilityFlags.None;
			}
		}

		public bool HasRegisteredEvents
		{
			get
			{
				return this.networkAddressChanged != null || this.networkAvailabilityChanged != null;
			}
		}

		public MacNetworkChange()
		{
			MacNetworkChange.sockaddr_in sockaddr_in = MacNetworkChange.sockaddr_in.Create();
			this.handle = MacNetworkChange.SCNetworkReachabilityCreateWithAddress(IntPtr.Zero, ref sockaddr_in);
			if (this.handle == IntPtr.Zero)
			{
				throw new Exception("SCNetworkReachabilityCreateWithAddress returned NULL");
			}
			this.callback = new MacNetworkChange.SCNetworkReachabilityCallback(MacNetworkChange.HandleCallback);
			MacNetworkChange.SCNetworkReachabilityContext scnetworkReachabilityContext = new MacNetworkChange.SCNetworkReachabilityContext
			{
				info = GCHandle.ToIntPtr(GCHandle.Alloc(this))
			};
			MacNetworkChange.SCNetworkReachabilitySetCallback(this.handle, this.callback, ref scnetworkReachabilityContext);
			this.scheduledWithRunLoop = this.LoadRunLoopMode() && MacNetworkChange.SCNetworkReachabilityScheduleWithRunLoop(this.handle, MacNetworkChange.CFRunLoopGetMain(), this.runLoopMode);
			MacNetworkChange.SCNetworkReachabilityGetFlags(this.handle, out this.flags);
		}

		private bool LoadRunLoopMode()
		{
			IntPtr intPtr = MacNetworkChange.dlopen("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation", 0);
			if (intPtr == IntPtr.Zero)
			{
				return false;
			}
			try
			{
				this.runLoopMode = MacNetworkChange.dlsym(intPtr, "kCFRunLoopDefaultMode");
				if (this.runLoopMode != IntPtr.Zero)
				{
					this.runLoopMode = Marshal.ReadIntPtr(this.runLoopMode);
					return this.runLoopMode != IntPtr.Zero;
				}
			}
			finally
			{
				MacNetworkChange.dlclose(intPtr);
			}
			return false;
		}

		public void Dispose()
		{
			lock (this)
			{
				if (!(this.handle == IntPtr.Zero))
				{
					if (this.scheduledWithRunLoop)
					{
						MacNetworkChange.SCNetworkReachabilityUnscheduleFromRunLoop(this.handle, MacNetworkChange.CFRunLoopGetMain(), this.runLoopMode);
					}
					MacNetworkChange.CFRelease(this.handle);
					this.handle = IntPtr.Zero;
					this.callback = null;
					this.flags = MacNetworkChange.NetworkReachabilityFlags.None;
					this.scheduledWithRunLoop = false;
				}
			}
		}

		[MonoPInvokeCallback(typeof(MacNetworkChange.SCNetworkReachabilityCallback))]
		private static void HandleCallback(IntPtr reachability, MacNetworkChange.NetworkReachabilityFlags flags, IntPtr info)
		{
			if (info == IntPtr.Zero)
			{
				return;
			}
			MacNetworkChange macNetworkChange = GCHandle.FromIntPtr(info).Target as MacNetworkChange;
			if (macNetworkChange == null || macNetworkChange.flags == flags)
			{
				return;
			}
			macNetworkChange.flags = flags;
			NetworkAddressChangedEventHandler networkAddressChangedEventHandler = macNetworkChange.networkAddressChanged;
			if (networkAddressChangedEventHandler != null)
			{
				networkAddressChangedEventHandler(null, EventArgs.Empty);
			}
			NetworkAvailabilityChangedEventHandler networkAvailabilityChangedEventHandler = macNetworkChange.networkAvailabilityChanged;
			if (networkAvailabilityChangedEventHandler != null)
			{
				networkAvailabilityChangedEventHandler(null, new NetworkAvailabilityEventArgs(macNetworkChange.IsAvailable));
			}
		}

		private const string DL_LIB = "/usr/lib/libSystem.dylib";

		private const string CORE_SERVICES_LIB = "/System/Library/Frameworks/SystemConfiguration.framework/SystemConfiguration";

		private const string CORE_FOUNDATION_LIB = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";

		private IntPtr handle;

		private IntPtr runLoopMode;

		private MacNetworkChange.SCNetworkReachabilityCallback callback;

		private bool scheduledWithRunLoop;

		private MacNetworkChange.NetworkReachabilityFlags flags;

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void SCNetworkReachabilityCallback(IntPtr target, MacNetworkChange.NetworkReachabilityFlags flags, IntPtr info);

		[StructLayout(LayoutKind.Explicit, Size = 28)]
		private struct sockaddr_in
		{
			public static MacNetworkChange.sockaddr_in Create()
			{
				return new MacNetworkChange.sockaddr_in
				{
					sin_len = 28,
					sin_family = 2
				};
			}

			[FieldOffset(0)]
			public byte sin_len;

			[FieldOffset(1)]
			public byte sin_family;
		}

		private struct SCNetworkReachabilityContext
		{
			public IntPtr version;

			public IntPtr info;

			public IntPtr retain;

			public IntPtr release;

			public IntPtr copyDescription;
		}

		[Flags]
		private enum NetworkReachabilityFlags
		{
			None = 0,
			TransientConnection = 1,
			Reachable = 2,
			ConnectionRequired = 4,
			ConnectionOnTraffic = 8,
			InterventionRequired = 16,
			ConnectionOnDemand = 32,
			IsLocalAddress = 65536,
			IsDirect = 131072,
			IsWWAN = 262144,
			ConnectionAutomatic = 8
		}
	}
}
