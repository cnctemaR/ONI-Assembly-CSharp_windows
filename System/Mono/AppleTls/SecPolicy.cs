using System;
using System.Runtime.InteropServices;
using Mono.Net;
using ObjCRuntimeInternal;

namespace Mono.AppleTls
{
	internal class SecPolicy : INativeObject, IDisposable
	{
		internal SecPolicy(IntPtr handle, bool owns = false)
		{
			if (handle == IntPtr.Zero)
			{
				throw new Exception("Invalid handle");
			}
			this.handle = handle;
			if (!owns)
			{
				CFObject.CFRetain(handle);
			}
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern IntPtr SecPolicyCreateSSL(bool server, IntPtr hostname);

		public static SecPolicy CreateSslPolicy(bool server, string hostName)
		{
			CFString cfstring = ((hostName == null) ? null : CFString.Create(hostName));
			IntPtr intPtr = ((cfstring == null) ? IntPtr.Zero : cfstring.Handle);
			SecPolicy secPolicy = new SecPolicy(SecPolicy.SecPolicyCreateSSL(server, intPtr), true);
			if (cfstring != null)
			{
				cfstring.Dispose();
			}
			return secPolicy;
		}

		~SecPolicy()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public IntPtr Handle
		{
			get
			{
				return this.handle;
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.handle != IntPtr.Zero)
			{
				CFObject.CFRelease(this.handle);
				this.handle = IntPtr.Zero;
			}
		}

		private IntPtr handle;
	}
}
