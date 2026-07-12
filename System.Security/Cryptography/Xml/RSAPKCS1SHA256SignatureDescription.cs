using System;

namespace System.Security.Cryptography.Xml
{
	internal class RSAPKCS1SHA256SignatureDescription : RSAPKCS1SignatureDescription
	{
		public RSAPKCS1SHA256SignatureDescription()
			: base("SHA256")
		{
		}

		public sealed override HashAlgorithm CreateDigest()
		{
			return SHA256.Create();
		}
	}
}
