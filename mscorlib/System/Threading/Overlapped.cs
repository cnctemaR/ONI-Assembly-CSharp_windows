using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Threading
{
	[ComVisible(true)]
	public class Overlapped
	{
		public Overlapped()
		{
		}

		[Obsolete("Not 64bit compatible.  Please use the constructor that takes IntPtr for the event handle", false)]
		public Overlapped(int offsetLo, int offsetHi, int hEvent, IAsyncResult ar)
		{
			this.offsetL = offsetLo;
			this.offsetH = offsetHi;
			this.evt = hEvent;
			this.ares = ar;
		}

		public Overlapped(int offsetLo, int offsetHi, IntPtr hEvent, IAsyncResult ar)
		{
			this.offsetL = offsetLo;
			this.offsetH = offsetHi;
			this.evt_ptr = hEvent;
			this.ares = ar;
		}

		[CLSCompliant(false)]
		public unsafe static void Free(NativeOverlapped* nativeOverlappedPtr)
		{
			if ((IntPtr)((void*)nativeOverlappedPtr) == IntPtr.Zero)
			{
				throw new ArgumentNullException("nativeOverlappedPtr");
			}
			Marshal.FreeHGlobal((IntPtr)((void*)nativeOverlappedPtr));
		}

		[CLSCompliant(false)]
		public unsafe static Overlapped Unpack(NativeOverlapped* nativeOverlappedPtr)
		{
			if ((IntPtr)((void*)nativeOverlappedPtr) == IntPtr.Zero)
			{
				throw new ArgumentNullException("nativeOverlappedPtr");
			}
			return new Overlapped
			{
				offsetL = nativeOverlappedPtr->OffsetLow,
				offsetH = nativeOverlappedPtr->OffsetHigh,
				evt = (int)nativeOverlappedPtr->EventHandle
			};
		}

		[CLSCompliant(false)]
		[Obsolete("Use Pack(iocb, userData) instead", false)]
		[MonoTODO("Security - we need to propagate the call stack")]
		public unsafe NativeOverlapped* Pack(IOCompletionCallback iocb)
		{
			NativeOverlapped* ptr = (NativeOverlapped*)(void*)Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeOverlapped)));
			ptr->OffsetLow = this.offsetL;
			ptr->OffsetHigh = this.offsetH;
			ptr->EventHandle = (IntPtr)this.evt;
			return ptr;
		}

		[MonoTODO("handle userData")]
		[CLSCompliant(false)]
		[ComVisible(false)]
		public unsafe NativeOverlapped* Pack(IOCompletionCallback iocb, object userData)
		{
			NativeOverlapped* ptr = (NativeOverlapped*)(void*)Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeOverlapped)));
			ptr->OffsetLow = this.offsetL;
			ptr->OffsetHigh = this.offsetH;
			ptr->EventHandle = this.evt_ptr;
			return ptr;
		}

		[CLSCompliant(false)]
		[Obsolete("Use UnsafePack(iocb, userData) instead", false)]
		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlEvidence, ControlPolicy\"/>\n</PermissionSet>\n")]
		public unsafe NativeOverlapped* UnsafePack(IOCompletionCallback iocb)
		{
			return this.Pack(iocb);
		}

		[ComVisible(false)]
		[CLSCompliant(false)]
		public unsafe NativeOverlapped* UnsafePack(IOCompletionCallback iocb, object userData)
		{
			return this.Pack(iocb, userData);
		}

		public IAsyncResult AsyncResult
		{
			get
			{
				return this.ares;
			}
			set
			{
				this.ares = value;
			}
		}

		[Obsolete("Not 64bit compatible.  Use EventHandleIntPtr instead.", false)]
		public int EventHandle
		{
			get
			{
				return this.evt;
			}
			set
			{
				this.evt = value;
			}
		}

		[ComVisible(false)]
		public IntPtr EventHandleIntPtr
		{
			get
			{
				return this.evt_ptr;
			}
			set
			{
				this.evt_ptr = value;
			}
		}

		public int OffsetHigh
		{
			get
			{
				return this.offsetH;
			}
			set
			{
				this.offsetH = value;
			}
		}

		public int OffsetLow
		{
			get
			{
				return this.offsetL;
			}
			set
			{
				this.offsetL = value;
			}
		}

		private IAsyncResult ares;

		private int offsetL;

		private int offsetH;

		private int evt;

		private IntPtr evt_ptr;
	}
}
