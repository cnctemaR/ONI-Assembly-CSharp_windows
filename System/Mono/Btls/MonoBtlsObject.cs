using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Threading;

namespace Mono.Btls
{
	internal abstract class MonoBtlsObject : IDisposable
	{
		internal MonoBtlsObject(MonoBtlsObject.MonoBtlsHandle handle)
		{
			this.handle = handle;
		}

		internal MonoBtlsObject.MonoBtlsHandle Handle
		{
			get
			{
				this.CheckThrow();
				return this.handle;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.handle != null && !this.handle.IsInvalid;
			}
		}

		protected void CheckThrow()
		{
			if (this.lastError != null)
			{
				throw this.lastError;
			}
			if (this.handle == null || this.handle.IsInvalid)
			{
				throw new ObjectDisposedException("MonoBtlsSsl");
			}
		}

		protected Exception SetException(Exception ex)
		{
			if (this.lastError == null)
			{
				this.lastError = ex;
			}
			return ex;
		}

		protected void CheckError(bool ok, [CallerMemberName] string callerName = null)
		{
			if (ok)
			{
				return;
			}
			if (callerName != null)
			{
				throw new CryptographicException(string.Concat(new string[]
				{
					"`",
					base.GetType().Name,
					".",
					callerName,
					"` failed."
				}));
			}
			throw new CryptographicException();
		}

		protected void CheckError(int ret, [CallerMemberName] string callerName = null)
		{
			this.CheckError(ret == 1, callerName);
		}

		protected internal void CheckLastError([CallerMemberName] string callerName = null)
		{
			Exception ex = Interlocked.Exchange<Exception>(ref this.lastError, null);
			if (ex == null)
			{
				return;
			}
			if (ex is AuthenticationException || ex is NotSupportedException)
			{
				throw ex;
			}
			string text;
			if (callerName != null)
			{
				text = string.Concat(new string[]
				{
					"Caught unhandled exception in `",
					base.GetType().Name,
					".",
					callerName,
					"`."
				});
			}
			else
			{
				text = "Caught unhandled exception.";
			}
			throw new CryptographicException(text, ex);
		}

		[DllImport("libmono-btls-shared")]
		private static extern void mono_btls_free(IntPtr data);

		protected void FreeDataPtr(IntPtr data)
		{
			MonoBtlsObject.mono_btls_free(data);
		}

		protected virtual void Close()
		{
		}

		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				try
				{
					if (this.handle != null)
					{
						this.Close();
						this.handle.Dispose();
						this.handle = null;
					}
				}
				finally
				{
					ObjectDisposedException ex = new ObjectDisposedException(base.GetType().Name);
					Interlocked.CompareExchange<Exception>(ref this.lastError, ex, null);
				}
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~MonoBtlsObject()
		{
			this.Dispose(false);
		}

		internal const string BTLS_DYLIB = "libmono-btls-shared";

		private MonoBtlsObject.MonoBtlsHandle handle;

		private Exception lastError;

		protected internal abstract class MonoBtlsHandle : SafeHandle
		{
			internal MonoBtlsHandle()
				: base(IntPtr.Zero, true)
			{
			}

			internal MonoBtlsHandle(IntPtr handle, bool ownsHandle)
				: base(handle, ownsHandle)
			{
			}

			public override bool IsInvalid
			{
				get
				{
					return this.handle == IntPtr.Zero;
				}
			}
		}
	}
}
