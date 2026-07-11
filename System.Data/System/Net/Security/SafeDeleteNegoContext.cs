using System;
using Microsoft.Win32.SafeHandles;

namespace System.Net.Security
{
	internal sealed class SafeDeleteNegoContext : SafeDeleteContext
	{
		public SafeGssNameHandle TargetName
		{
			get
			{
				return this._targetName;
			}
		}

		public bool IsNtlmUsed
		{
			get
			{
				return this._isNtlmUsed;
			}
		}

		public SafeGssContextHandle GssContext
		{
			get
			{
				return this._context;
			}
		}

		public SafeDeleteNegoContext(SafeFreeNegoCredentials credential, string targetName)
			: base(credential)
		{
			try
			{
				this._targetName = SafeGssNameHandle.CreatePrincipal(targetName);
			}
			catch
			{
				base.Dispose();
				throw;
			}
		}

		public void SetGssContext(SafeGssContextHandle context)
		{
			this._context = context;
		}

		public void SetAuthenticationPackage(bool isNtlmUsed)
		{
			this._isNtlmUsed = isNtlmUsed;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this._context != null)
				{
					this._context.Dispose();
					this._context = null;
				}
				if (this._targetName != null)
				{
					this._targetName.Dispose();
					this._targetName = null;
				}
			}
			base.Dispose(disposing);
		}

		private SafeGssNameHandle _targetName;

		private SafeGssContextHandle _context;

		private bool _isNtlmUsed;
	}
}
