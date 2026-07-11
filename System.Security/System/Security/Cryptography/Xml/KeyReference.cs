using System;

namespace System.Security.Cryptography.Xml
{
	public sealed class KeyReference : EncryptedReference
	{
		public KeyReference()
		{
			base.ReferenceType = "KeyReference";
		}

		public KeyReference(string uri)
			: base(uri)
		{
			base.ReferenceType = "KeyReference";
		}

		public KeyReference(string uri, TransformChain transformChain)
			: base(uri, transformChain)
		{
			base.ReferenceType = "KeyReference";
		}
	}
}
