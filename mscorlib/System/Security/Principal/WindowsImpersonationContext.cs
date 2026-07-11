using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity;

namespace System.Security.Principal
{
	[ComVisible(true)]
	public class WindowsImpersonationContext : IDisposable
	{
		internal WindowsImpersonationContext(IntPtr token)
		{
			this._token = WindowsImpersonationContext.DuplicateToken(token);
			if (!WindowsImpersonationContext.SetCurrentToken(token))
			{
				throw new SecurityException("Couldn't impersonate token.");
			}
			this.undo = false;
		}

		[ComVisible(false)]
		public void Dispose()
		{
			if (!this.undo)
			{
				this.Undo();
			}
		}

		[ComVisible(false)]
		protected virtual void Dispose(bool disposing)
		{
			if (!this.undo)
			{
				this.Undo();
			}
			if (disposing)
			{
				GC.SuppressFinalize(this);
			}
		}

		public void Undo()
		{
			if (!WindowsImpersonationContext.RevertToSelf())
			{
				WindowsImpersonationContext.CloseToken(this._token);
				throw new SecurityException("Couldn't switch back to original token.");
			}
			WindowsImpersonationContext.CloseToken(this._token);
			this.undo = true;
			GC.SuppressFinalize(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CloseToken(IntPtr token);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr DuplicateToken(IntPtr token);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetCurrentToken(IntPtr token);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool RevertToSelf();

		internal WindowsImpersonationContext()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private IntPtr _token;

		private bool undo;
	}
}
