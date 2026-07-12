using System;

namespace System.Security.Cryptography.Xml
{
	internal class RSAPKCS1SHA1SignatureDescription : RSAPKCS1SignatureDescription
	{
		public RSAPKCS1SHA1SignatureDescription()
			: base("SHA1")
		{
		}

		public sealed override HashAlgorithm CreateDigest()
		{
			return SHA1.Create();
		}
	}
}
