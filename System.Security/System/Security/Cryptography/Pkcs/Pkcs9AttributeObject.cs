using System;

namespace System.Security.Cryptography.Pkcs
{
	public class Pkcs9AttributeObject : AsnEncodedData
	{
		public Pkcs9AttributeObject()
		{
		}

		public Pkcs9AttributeObject(AsnEncodedData asnEncodedData)
			: base(asnEncodedData)
		{
		}

		public Pkcs9AttributeObject(Oid oid, byte[] encodedData)
		{
			if (oid == null)
			{
				throw new ArgumentNullException("oid");
			}
			base.Oid = oid;
			base.RawData = encodedData;
		}

		public Pkcs9AttributeObject(string oid, byte[] encodedData)
			: base(oid, encodedData)
		{
		}

		public new Oid Oid
		{
			get
			{
				return base.Oid;
			}
			internal set
			{
				base.Oid = value;
			}
		}

		public override void CopyFrom(AsnEncodedData asnEncodedData)
		{
			if (asnEncodedData == null)
			{
				throw new ArgumentNullException("asnEncodedData");
			}
			throw new ArgumentException("Cannot convert the PKCS#9 attribute.");
		}
	}
}
