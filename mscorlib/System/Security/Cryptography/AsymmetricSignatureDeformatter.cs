using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public abstract class AsymmetricSignatureDeformatter
	{
		public abstract void SetHashAlgorithm(string strName);

		public abstract void SetKey(AsymmetricAlgorithm key);

		public abstract bool VerifySignature(byte[] rgbHash, byte[] rgbSignature);

		public virtual bool VerifySignature(HashAlgorithm hash, byte[] rgbSignature)
		{
			if (hash == null)
			{
				throw new ArgumentNullException("hash");
			}
			this.SetHashAlgorithm(hash.ToString());
			return this.VerifySignature(hash.Hash, rgbSignature);
		}
	}
}
