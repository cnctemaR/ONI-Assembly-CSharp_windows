using System;
using Unity;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class PublicKeyInfo
	{
		internal PublicKeyInfo(AlgorithmIdentifier algorithm, byte[] keyValue)
		{
			this.Algorithm = algorithm;
			this.KeyValue = keyValue;
		}

		public AlgorithmIdentifier Algorithm { get; }

		public byte[] KeyValue { get; }

		internal PublicKeyInfo()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
