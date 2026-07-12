using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Net
{
	internal class NegotiationInfoClass
	{
		internal unsafe NegotiationInfoClass(SafeHandle safeHandle, int negotiationState)
		{
			if (safeHandle.IsInvalid)
			{
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Info(this, FormattableStringFactory.Create("Invalid handle:{0}", new object[] { safeHandle }), ".ctor");
				}
				return;
			}
			IntPtr intPtr = safeHandle.DangerousGetHandle();
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Info(this, FormattableStringFactory.Create("packageInfo:{0} negotiationState:{1:x}", new object[] { intPtr, negotiationState }), ".ctor");
			}
			if (negotiationState == 0 || negotiationState == 1)
			{
				string text = null;
				IntPtr name = ((SecurityPackageInfo*)(void*)intPtr)->Name;
				if (name != IntPtr.Zero)
				{
					text = Marshal.PtrToStringUni(name);
				}
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Info(this, FormattableStringFactory.Create("packageInfo:{0} negotiationState:{1:x} name:{2}", new object[] { intPtr, negotiationState, text }), ".ctor");
				}
				if (string.Compare(text, "Kerberos", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.AuthenticationPackage = "Kerberos";
					return;
				}
				if (string.Compare(text, "NTLM", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.AuthenticationPackage = "NTLM";
					return;
				}
				this.AuthenticationPackage = text;
			}
		}

		internal string AuthenticationPackage;

		internal const string NTLM = "NTLM";

		internal const string Kerberos = "Kerberos";

		internal const string Negotiate = "Negotiate";

		internal const string Basic = "Basic";
	}
}
