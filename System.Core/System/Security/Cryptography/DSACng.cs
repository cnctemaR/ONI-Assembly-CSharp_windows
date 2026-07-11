using System;
using System.Security.Permissions;
using Unity;

namespace System.Security.Cryptography
{
	public sealed class DSACng : DSA
	{
		public DSACng()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public DSACng(int keySize)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public DSACng(CngKey key)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public CngKey Key
		{
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		[SecuritySafeCritical]
		public override byte[] CreateSignature(byte[] rgbHash)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}

		public override DSAParameters ExportParameters(bool includePrivateParameters)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return default(DSAParameters);
		}

		public override void ImportParameters(DSAParameters parameters)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		[SecuritySafeCritical]
		public override bool VerifySignature(byte[] rgbHash, byte[] rgbSignature)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}
	}
}
