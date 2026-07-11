using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public sealed class RNGCryptoServiceProvider : RandomNumberGenerator
	{
		static RNGCryptoServiceProvider()
		{
			if (RNGCryptoServiceProvider.RngOpen())
			{
				RNGCryptoServiceProvider._lock = new object();
			}
		}

		public RNGCryptoServiceProvider()
		{
			this._handle = RNGCryptoServiceProvider.RngInitialize(null);
			this.Check();
		}

		public RNGCryptoServiceProvider(byte[] rgb)
		{
			this._handle = RNGCryptoServiceProvider.RngInitialize(rgb);
			this.Check();
		}

		public RNGCryptoServiceProvider(CspParameters cspParams)
		{
			this._handle = RNGCryptoServiceProvider.RngInitialize(null);
			this.Check();
		}

		public RNGCryptoServiceProvider(string str)
		{
			if (str == null)
			{
				this._handle = RNGCryptoServiceProvider.RngInitialize(null);
			}
			else
			{
				this._handle = RNGCryptoServiceProvider.RngInitialize(Encoding.UTF8.GetBytes(str));
			}
			this.Check();
		}

		private void Check()
		{
			if (this._handle == IntPtr.Zero)
			{
				throw new CryptographicException(Locale.GetText("Couldn't access random source."));
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool RngOpen();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr RngInitialize(byte[] seed);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr RngGetBytes(IntPtr handle, byte[] data);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RngClose(IntPtr handle);

		public override void GetBytes(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (RNGCryptoServiceProvider._lock == null)
			{
				this._handle = RNGCryptoServiceProvider.RngGetBytes(this._handle, data);
			}
			else
			{
				object @lock = RNGCryptoServiceProvider._lock;
				lock (@lock)
				{
					this._handle = RNGCryptoServiceProvider.RngGetBytes(this._handle, data);
				}
			}
			this.Check();
		}

		public override void GetNonZeroBytes(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			byte[] array = new byte[data.Length * 2];
			int i = 0;
			while (i < data.Length)
			{
				this._handle = RNGCryptoServiceProvider.RngGetBytes(this._handle, array);
				this.Check();
				int num = 0;
				while (num < array.Length && i != data.Length)
				{
					if (array[num] != 0)
					{
						data[i++] = array[num];
					}
					num++;
				}
			}
		}

		~RNGCryptoServiceProvider()
		{
			if (this._handle != IntPtr.Zero)
			{
				RNGCryptoServiceProvider.RngClose(this._handle);
				this._handle = IntPtr.Zero;
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		private static object _lock;

		private IntPtr _handle;
	}
}
