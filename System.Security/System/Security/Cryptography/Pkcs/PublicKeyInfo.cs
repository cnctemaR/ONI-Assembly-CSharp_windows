using System;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class PublicKeyInfo
	{
		internal PublicKeyInfo(AlgorithmIdentifier algorithm, byte[] key)
		{
			this._algorithm = algorithm;
			this._key = key;
		}

		public AlgorithmIdentifier Algorithm
		{
			get
			{
				return this._algorithm;
			}
		}

		public byte[] KeyValue
		{
			get
			{
				return this._key;
			}
		}

		private AlgorithmIdentifier _algorithm;

		private byte[] _key;
	}
}
