using System;

namespace System.Security.Cryptography
{
	public sealed class SHA256Cng : SHA256
	{
		[SecurityCritical]
		public SHA256Cng()
		{
			this.hash = new SHA256Managed();
		}

		[SecurityCritical]
		public override void Initialize()
		{
			this.hash.Initialize();
		}

		[SecurityCritical]
		protected override void HashCore(byte[] array, int ibStart, int cbSize)
		{
			this.hash.TransformBlock(array, ibStart, cbSize, null, 0);
		}

		[SecurityCritical]
		protected override byte[] HashFinal()
		{
			this.hash.TransformFinalBlock(SHA256Cng.Empty, 0, 0);
			this.HashValue = this.hash.Hash;
			return this.HashValue;
		}

		[SecurityCritical]
		protected override void Dispose(bool disposing)
		{
			((IDisposable)this.hash).Dispose();
			base.Dispose(disposing);
		}

		private static byte[] Empty = new byte[0];

		private SHA256 hash;
	}
}
