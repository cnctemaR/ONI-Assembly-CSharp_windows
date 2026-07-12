using System;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class AlgorithmIdentifier
	{
		public AlgorithmIdentifier()
			: this(Oid.FromOidValue("1.2.840.113549.3.7", OidGroup.EncryptionAlgorithm), 0)
		{
		}

		public AlgorithmIdentifier(Oid oid)
			: this(oid, 0)
		{
		}

		public AlgorithmIdentifier(Oid oid, int keyLength)
		{
			this.Oid = oid;
			this.KeyLength = keyLength;
		}

		public Oid Oid { get; set; }

		public int KeyLength { get; set; }

		public byte[] Parameters { get; set; } = Array.Empty<byte>();
	}
}
