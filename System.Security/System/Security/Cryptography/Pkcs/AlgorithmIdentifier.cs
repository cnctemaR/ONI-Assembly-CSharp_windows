using System;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class AlgorithmIdentifier
	{
		public AlgorithmIdentifier()
		{
			this._oid = new Oid("1.2.840.113549.3.7", "3des");
			this._params = new byte[0];
		}

		public AlgorithmIdentifier(Oid oid)
		{
			this._oid = oid;
			this._params = new byte[0];
		}

		public AlgorithmIdentifier(Oid oid, int keyLength)
		{
			this._oid = oid;
			this._length = keyLength;
			this._params = new byte[0];
		}

		public int KeyLength
		{
			get
			{
				return this._length;
			}
			set
			{
				this._length = value;
			}
		}

		public Oid Oid
		{
			get
			{
				return this._oid;
			}
			set
			{
				this._oid = value;
			}
		}

		public byte[] Parameters
		{
			get
			{
				return this._params;
			}
			set
			{
				this._params = value;
			}
		}

		private Oid _oid;

		private int _length;

		private byte[] _params;
	}
}
