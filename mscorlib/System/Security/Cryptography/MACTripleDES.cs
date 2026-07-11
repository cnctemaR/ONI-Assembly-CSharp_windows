using System;
using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class MACTripleDES : KeyedHashAlgorithm
	{
		public MACTripleDES()
		{
			this.Setup("TripleDES", null);
		}

		public MACTripleDES(byte[] rgbKey)
		{
			if (rgbKey == null)
			{
				throw new ArgumentNullException("rgbKey");
			}
			this.Setup("TripleDES", rgbKey);
		}

		public MACTripleDES(string strTripleDES, byte[] rgbKey)
		{
			if (rgbKey == null)
			{
				throw new ArgumentNullException("rgbKey");
			}
			if (strTripleDES == null)
			{
				this.Setup("TripleDES", rgbKey);
			}
			else
			{
				this.Setup(strTripleDES, rgbKey);
			}
		}

		private void Setup(string strTripleDES, byte[] rgbKey)
		{
			this.tdes = TripleDES.Create(strTripleDES);
			this.tdes.Padding = PaddingMode.Zeros;
			if (rgbKey != null)
			{
				this.tdes.Key = rgbKey;
			}
			this.HashSizeValue = this.tdes.BlockSize;
			this.Key = this.tdes.Key;
			this.mac = new MACAlgorithm(this.tdes);
			this.m_disposed = false;
		}

		~MACTripleDES()
		{
			this.Dispose(false);
		}

		[ComVisible(false)]
		public PaddingMode Padding
		{
			get
			{
				return this.tdes.Padding;
			}
			set
			{
				this.tdes.Padding = value;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (!this.m_disposed)
			{
				if (this.KeyValue != null)
				{
					Array.Clear(this.KeyValue, 0, this.KeyValue.Length);
				}
				if (this.tdes != null)
				{
					this.tdes.Clear();
				}
				if (disposing)
				{
					this.KeyValue = null;
					this.tdes = null;
				}
				base.Dispose(disposing);
				this.m_disposed = true;
			}
		}

		public override void Initialize()
		{
			if (this.m_disposed)
			{
				throw new ObjectDisposedException("MACTripleDES");
			}
			this.State = 0;
			this.mac.Initialize(this.KeyValue);
		}

		protected override void HashCore(byte[] rgbData, int ibStart, int cbSize)
		{
			if (this.m_disposed)
			{
				throw new ObjectDisposedException("MACTripleDES");
			}
			if (this.State == 0)
			{
				this.Initialize();
				this.State = 1;
			}
			this.mac.Core(rgbData, ibStart, cbSize);
		}

		protected override byte[] HashFinal()
		{
			if (this.m_disposed)
			{
				throw new ObjectDisposedException("MACTripleDES");
			}
			this.State = 0;
			return this.mac.Final();
		}

		private TripleDES tdes;

		private MACAlgorithm mac;

		private bool m_disposed;
	}
}
