using System;
using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class Pkcs9DocumentName : Pkcs9AttributeObject
	{
		public Pkcs9DocumentName()
			: base(new Oid("1.3.6.1.4.1.311.88.2.1"))
		{
		}

		public Pkcs9DocumentName(string documentName)
			: base("1.3.6.1.4.1.311.88.2.1", Pkcs9DocumentName.Encode(documentName))
		{
			this._lazyDocumentName = documentName;
		}

		public Pkcs9DocumentName(byte[] encodedDocumentName)
			: base("1.3.6.1.4.1.311.88.2.1", encodedDocumentName)
		{
		}

		public string DocumentName
		{
			get
			{
				string text;
				if ((text = this._lazyDocumentName) == null)
				{
					text = (this._lazyDocumentName = Pkcs9DocumentName.Decode(base.RawData));
				}
				return text;
			}
		}

		public override void CopyFrom(AsnEncodedData asnEncodedData)
		{
			base.CopyFrom(asnEncodedData);
			this._lazyDocumentName = null;
		}

		private static string Decode(byte[] rawData)
		{
			if (rawData == null)
			{
				return null;
			}
			return PkcsPal.Instance.DecodeOctetString(rawData).OctetStringToUnicode();
		}

		private static byte[] Encode(string documentName)
		{
			if (documentName == null)
			{
				throw new ArgumentNullException("documentName");
			}
			byte[] array = documentName.UnicodeToOctetString();
			return PkcsPal.Instance.EncodeOctetString(array);
		}

		private volatile string _lazyDocumentName;
	}
}
