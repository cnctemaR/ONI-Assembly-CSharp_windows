using System;

namespace Microsoft.Win32.SafeHandles
{
	public sealed class SafeX509ChainHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		internal SafeX509ChainHandle(IntPtr handle)
			: base(true)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		protected override bool ReleaseHandle()
		{
			throw new NotImplementedException();
		}
	}
}
