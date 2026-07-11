using System;

namespace System.Security.Cryptography.Xml
{
	public sealed class DataReference : EncryptedReference
	{
		public DataReference()
		{
			base.ReferenceType = "DataReference";
		}

		public DataReference(string uri)
			: base(uri)
		{
			base.ReferenceType = "DataReference";
		}

		public DataReference(string uri, TransformChain transformChain)
			: base(uri, transformChain)
		{
			base.ReferenceType = "DataReference";
		}
	}
}
