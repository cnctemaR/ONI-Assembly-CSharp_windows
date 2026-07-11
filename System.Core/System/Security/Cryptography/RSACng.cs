using System;

namespace System.Security.Cryptography
{
	public sealed class RSACng : RSA
	{
		public RSACng()
			: this(2048)
		{
		}

		public RSACng(int keySize)
		{
			throw new NotImplementedException();
		}

		public RSACng(CngKey key)
		{
			throw new NotImplementedException();
		}

		public CngKey Key
		{
			[SecuritySafeCritical]
			get
			{
				throw new NotImplementedException();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		public override RSAParameters ExportParameters(bool includePrivateParameters)
		{
			throw new NotImplementedException();
		}

		public override void ImportParameters(RSAParameters parameters)
		{
			throw new NotImplementedException();
		}
	}
}
