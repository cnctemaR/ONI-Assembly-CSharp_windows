using System;

namespace System.Security.Cryptography
{
	public sealed class CryptographicAttributeObject
	{
		public CryptographicAttributeObject(Oid oid)
			: this(oid, new AsnEncodedDataCollection())
		{
		}

		public CryptographicAttributeObject(Oid oid, AsnEncodedDataCollection values)
		{
			this._oid = new Oid(oid);
			if (values == null)
			{
				this.Values = new AsnEncodedDataCollection();
				return;
			}
			foreach (AsnEncodedData asnEncodedData in values)
			{
				if (!string.Equals(asnEncodedData.Oid.Value, oid.Value, StringComparison.Ordinal))
				{
					throw new InvalidOperationException(SR.Format("AsnEncodedData element in the collection has wrong Oid value: expected = '{0}', actual = '{1}'.", oid.Value, asnEncodedData.Oid.Value));
				}
			}
			this.Values = values;
		}

		public Oid Oid
		{
			get
			{
				return new Oid(this._oid);
			}
		}

		public AsnEncodedDataCollection Values { get; }

		private readonly Oid _oid;
	}
}
