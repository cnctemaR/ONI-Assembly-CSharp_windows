using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
	public abstract class CriticalHandleMinusOneIsInvalid : CriticalHandle, IDisposable
	{
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		protected CriticalHandleMinusOneIsInvalid()
			: base((IntPtr)(-1))
		{
		}

		public override bool IsInvalid
		{
			get
			{
				return this.handle == (IntPtr)(-1);
			}
		}
	}
}
