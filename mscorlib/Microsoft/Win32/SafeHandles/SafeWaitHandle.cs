using System;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Threading;

namespace Microsoft.Win32.SafeHandles
{
	[SecurityCritical]
	public sealed class SafeWaitHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		private SafeWaitHandle()
			: base(true)
		{
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public SafeWaitHandle(IntPtr existingHandle, bool ownsHandle)
			: base(ownsHandle)
		{
			base.SetHandle(existingHandle);
		}

		[SecurityCritical]
		protected override bool ReleaseHandle()
		{
			NativeEventCalls.CloseEvent_internal(this.handle);
			return true;
		}
	}
}
