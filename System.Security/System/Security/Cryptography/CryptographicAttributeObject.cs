using System;

namespace System.Security.Cryptography
{
	public sealed class CryptographicAttributeObject
	{
		public CryptographicAttributeObject(Oid oid)
		{
			if (oid == null)
			{
				throw new ArgumentNullException("oid");
			}
			this._oid = new Oid(oid);
			this._list = new AsnEncodedDataCollection();
		}

		public CryptographicAttributeObject(Oid oid, AsnEncodedDataCollection values)
		{
			if (oid == null)
			{
				throw new ArgumentNullException("oid");
			}
			this._oid = new Oid(oid);
			if (values == null)
			{
				this._list = new AsnEncodedDataCollection();
			}
			else
			{
				this._list = values;
			}
		}

		public Oid Oid
		{
			get
			{
				return this._oid;
			}
		}

		public AsnEncodedDataCollection Values
		{
			get
			{
				return this._list;
			}
		}

		private Oid _oid;

		private AsnEncodedDataCollection _list;
	}
}
