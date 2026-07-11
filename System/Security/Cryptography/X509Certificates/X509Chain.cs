using System;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography.X509Certificates
{
	public class X509Chain : IDisposable
	{
		internal X509ChainImpl Impl
		{
			get
			{
				X509Helper2.ThrowIfContextInvalid(this.impl);
				return this.impl;
			}
		}

		internal bool IsValid
		{
			get
			{
				return X509Helper2.IsValid(this.impl);
			}
		}

		internal void ThrowIfContextInvalid()
		{
			X509Helper2.ThrowIfContextInvalid(this.impl);
		}

		public X509Chain()
			: this(false)
		{
		}

		public X509Chain(bool useMachineContext)
		{
			this.impl = X509Helper2.CreateChainImpl(useMachineContext);
		}

		internal X509Chain(X509ChainImpl impl)
		{
			X509Helper2.ThrowIfContextInvalid(impl);
			this.impl = impl;
		}

		[MonoTODO("Mono's X509Chain is fully managed. All handles are invalid.")]
		public X509Chain(IntPtr chainContext)
		{
			throw new NotSupportedException();
		}

		[MonoTODO("Mono's X509Chain is fully managed. Always returns IntPtr.Zero.")]
		public IntPtr ChainContext
		{
			get
			{
				if (this.impl != null && this.impl.IsValid)
				{
					return this.impl.Handle;
				}
				return IntPtr.Zero;
			}
		}

		public X509ChainElementCollection ChainElements
		{
			get
			{
				return this.Impl.ChainElements;
			}
		}

		public X509ChainPolicy ChainPolicy
		{
			get
			{
				return this.Impl.ChainPolicy;
			}
			set
			{
				this.Impl.ChainPolicy = value;
			}
		}

		public X509ChainStatus[] ChainStatus
		{
			get
			{
				return this.Impl.ChainStatus;
			}
		}

		public SafeX509ChainHandle SafeHandle
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		[MonoTODO("Not totally RFC3280 compliant, but neither is MS implementation...")]
		public bool Build(X509Certificate2 certificate)
		{
			return this.Impl.Build(certificate);
		}

		public void Reset()
		{
			this.Impl.Reset();
		}

		public static X509Chain Create()
		{
			return (X509Chain)CryptoConfig.CreateFromName("X509Chain");
		}

		[SecuritySafeCritical]
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.impl != null)
			{
				this.impl.Dispose();
				this.impl = null;
			}
		}

		~X509Chain()
		{
			this.Dispose(false);
		}

		private X509ChainImpl impl;
	}
}
