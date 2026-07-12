using System;
using System.Runtime.InteropServices;

namespace System.Net.Security
{
	internal abstract class SafeFreeCredentials : SafeHandle
	{
		protected SafeFreeCredentials()
			: base(IntPtr.Zero, true)
		{
			this._handle = default(global::Interop.SspiCli.CredHandle);
		}

		public override bool IsInvalid
		{
			get
			{
				return base.IsClosed || this._handle.IsZero;
			}
		}

		public static int AcquireCredentialsHandle(string package, global::Interop.SspiCli.CredentialUse intent, ref global::Interop.SspiCli.SEC_WINNT_AUTH_IDENTITY_W authdata, out SafeFreeCredentials outCredential)
		{
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Enter(null, package, intent, authdata, "AcquireCredentialsHandle");
			}
			outCredential = new SafeFreeCredential_SECURITY();
			long num2;
			int num = global::Interop.SspiCli.AcquireCredentialsHandleW(null, package, (int)intent, null, ref authdata, null, null, ref outCredential._handle, out num2);
			if (num != 0)
			{
				outCredential.SetHandleAsInvalid();
			}
			return num;
		}

		public static int AcquireDefaultCredential(string package, global::Interop.SspiCli.CredentialUse intent, out SafeFreeCredentials outCredential)
		{
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Enter(null, package, intent, "AcquireDefaultCredential");
			}
			outCredential = new SafeFreeCredential_SECURITY();
			long num2;
			int num = global::Interop.SspiCli.AcquireCredentialsHandleW(null, package, (int)intent, null, IntPtr.Zero, null, null, ref outCredential._handle, out num2);
			if (num != 0)
			{
				outCredential.SetHandleAsInvalid();
			}
			return num;
		}

		public static int AcquireCredentialsHandle(string package, global::Interop.SspiCli.CredentialUse intent, ref SafeSspiAuthDataHandle authdata, out SafeFreeCredentials outCredential)
		{
			outCredential = new SafeFreeCredential_SECURITY();
			long num2;
			int num = global::Interop.SspiCli.AcquireCredentialsHandleW(null, package, (int)intent, null, authdata, null, null, ref outCredential._handle, out num2);
			if (num != 0)
			{
				outCredential.SetHandleAsInvalid();
			}
			return num;
		}

		public unsafe static int AcquireCredentialsHandle(string package, global::Interop.SspiCli.CredentialUse intent, ref global::Interop.SspiCli.SCHANNEL_CRED authdata, out SafeFreeCredentials outCredential)
		{
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Enter(null, package, intent, authdata, "AcquireCredentialsHandle");
			}
			int num = -1;
			IntPtr paCred = authdata.paCred;
			try
			{
				IntPtr intPtr = new IntPtr((void*)(&paCred));
				if (paCred != IntPtr.Zero)
				{
					authdata.paCred = intPtr;
				}
				outCredential = new SafeFreeCredential_SECURITY();
				long num2;
				num = global::Interop.SspiCli.AcquireCredentialsHandleW(null, package, (int)intent, null, ref authdata, null, null, ref outCredential._handle, out num2);
			}
			finally
			{
				authdata.paCred = paCred;
			}
			if (num != 0)
			{
				outCredential.SetHandleAsInvalid();
			}
			return num;
		}

		internal global::Interop.SspiCli.CredHandle _handle;
	}
}
