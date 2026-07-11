using System;
using System.Runtime.InteropServices;
using Mono.Net;
using ObjCRuntimeInternal;

namespace Mono.AppleTls
{
	internal class SecAccess : INativeObject, IDisposable
	{
		public SecAccess(IntPtr handle, bool owns = false)
		{
			this.handle = handle;
			if (!owns)
			{
				CFObject.CFRetain(handle);
			}
		}

		~SecAccess()
		{
			this.Dispose(false);
		}

		public IntPtr Handle
		{
			get
			{
				return this.handle;
			}
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SecStatusCode SecAccessCreate(IntPtr descriptor, IntPtr trustedList, out IntPtr accessRef);

		public static SecAccess Create(string descriptor)
		{
			CFString cfstring = CFString.Create(descriptor);
			if (cfstring == null)
			{
				throw new InvalidOperationException();
			}
			SecAccess secAccess;
			try
			{
				IntPtr intPtr;
				SecStatusCode secStatusCode = SecAccess.SecAccessCreate(cfstring.Handle, IntPtr.Zero, out intPtr);
				if (secStatusCode != SecStatusCode.Success)
				{
					throw new InvalidOperationException(secStatusCode.ToString());
				}
				secAccess = new SecAccess(intPtr, true);
			}
			finally
			{
				cfstring.Dispose();
			}
			return secAccess;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.handle != IntPtr.Zero)
			{
				CFObject.CFRelease(this.handle);
				this.handle = IntPtr.Zero;
			}
		}

		internal IntPtr handle;
	}
}
