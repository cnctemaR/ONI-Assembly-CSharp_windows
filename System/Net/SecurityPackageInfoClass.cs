using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Net
{
	internal class SecurityPackageInfoClass
	{
		internal unsafe SecurityPackageInfoClass(SafeHandle safeHandle, int index)
		{
			if (safeHandle.IsInvalid)
			{
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Info(this, FormattableStringFactory.Create("Invalid handle: {0}", new object[] { safeHandle }), ".ctor");
				}
				return;
			}
			IntPtr intPtr = safeHandle.DangerousGetHandle() + sizeof(SecurityPackageInfo) * index;
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Info(this, FormattableStringFactory.Create("unmanagedAddress: {0}", new object[] { intPtr }), ".ctor");
			}
			SecurityPackageInfo* ptr = (SecurityPackageInfo*)(void*)intPtr;
			this.Capabilities = ptr->Capabilities;
			this.Version = ptr->Version;
			this.RPCID = ptr->RPCID;
			this.MaxToken = ptr->MaxToken;
			IntPtr intPtr2 = ptr->Name;
			if (intPtr2 != IntPtr.Zero)
			{
				this.Name = Marshal.PtrToStringUni(intPtr2);
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Info(this, FormattableStringFactory.Create("Name: {0}", new object[] { this.Name }), ".ctor");
				}
			}
			intPtr2 = ptr->Comment;
			if (intPtr2 != IntPtr.Zero)
			{
				this.Comment = Marshal.PtrToStringUni(intPtr2);
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Info(this, FormattableStringFactory.Create("Comment: {0}", new object[] { this.Comment }), ".ctor");
				}
			}
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Info(this, this.ToString(), ".ctor");
			}
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"Capabilities:",
				string.Format(CultureInfo.InvariantCulture, "0x{0:x}", this.Capabilities),
				" Version:",
				this.Version.ToString(NumberFormatInfo.InvariantInfo),
				" RPCID:",
				this.RPCID.ToString(NumberFormatInfo.InvariantInfo),
				" MaxToken:",
				this.MaxToken.ToString(NumberFormatInfo.InvariantInfo),
				" Name:",
				(this.Name == null) ? "(null)" : this.Name,
				" Comment:",
				(this.Comment == null) ? "(null)" : this.Comment
			});
		}

		internal int Capabilities;

		internal short Version;

		internal short RPCID;

		internal int MaxToken;

		internal string Name;

		internal string Comment;
	}
}
