using System;
using System.Runtime.InteropServices;

namespace System.Data.SqlClient
{
	internal sealed class SNIHandle : SafeHandle
	{
		internal SNIHandle(SNINativeMethodWrapper.ConsumerInfo myInfo, string serverName, byte[] spnBuffer, bool ignoreSniOpenTimeout, int timeout, out byte[] instanceName, bool flushCache, bool fSync, bool fParallel)
			: base(IntPtr.Zero, true)
		{
			try
			{
			}
			finally
			{
				this._fSync = fSync;
				instanceName = new byte[256];
				if (ignoreSniOpenTimeout)
				{
					timeout = -1;
				}
				this._status = SNINativeMethodWrapper.SNIOpenSyncEx(myInfo, serverName, ref this.handle, spnBuffer, instanceName, flushCache, fSync, timeout, fParallel);
			}
		}

		internal SNIHandle(SNINativeMethodWrapper.ConsumerInfo myInfo, SNIHandle parent)
			: base(IntPtr.Zero, true)
		{
			try
			{
			}
			finally
			{
				this._status = SNINativeMethodWrapper.SNIOpenMarsSession(myInfo, parent, ref this.handle, parent._fSync);
			}
		}

		public override bool IsInvalid
		{
			get
			{
				return IntPtr.Zero == this.handle;
			}
		}

		protected override bool ReleaseHandle()
		{
			IntPtr handle = this.handle;
			this.handle = IntPtr.Zero;
			return !(IntPtr.Zero != handle) || SNINativeMethodWrapper.SNIClose(handle) == 0U;
		}

		internal uint Status
		{
			get
			{
				return this._status;
			}
		}

		private readonly uint _status = uint.MaxValue;

		private readonly bool _fSync;
	}
}
