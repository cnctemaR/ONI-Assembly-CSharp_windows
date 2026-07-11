using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
	public abstract class SafeHandleZeroOrMinusOneIsInvalid : SafeHandle, IDisposable
	{
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		protected SafeHandleZeroOrMinusOneIsInvalid(bool ownsHandle)
			: base((IntPtr)0, ownsHandle)
		{
		}

		public override bool IsInvalid
		{
			get
			{
				return this.handle == (IntPtr)(-1) || this.handle == (IntPtr)0;
			}
		}
	}
}
