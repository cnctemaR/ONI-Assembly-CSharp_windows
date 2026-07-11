using System;
using System.Globalization;
using System.Text;
using Mono.Security;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class Pkcs9SigningTime : Pkcs9AttributeObject
	{
		public Pkcs9SigningTime()
		{
			this.Oid = new Oid("1.2.840.113549.1.9.5", "Signing Time");
			this._signingTime = DateTime.Now;
			base.RawData = this.Encode();
		}

		public Pkcs9SigningTime(DateTime signingTime)
		{
			this.Oid = new Oid("1.2.840.113549.1.9.5", "Signing Time");
			this._signingTime = signingTime;
			base.RawData = this.Encode();
		}

		public Pkcs9SigningTime(byte[] encodedSigningTime)
		{
			if (encodedSigningTime == null)
			{
				throw new ArgumentNullException("encodedSigningTime");
			}
			this.Oid = new Oid("1.2.840.113549.1.9.5", "Signing Time");
			base.RawData = encodedSigningTime;
			this.Decode(encodedSigningTime);
		}

		public DateTime SigningTime
		{
			get
			{
				return this._signingTime;
			}
		}

		public override void CopyFrom(AsnEncodedData asnEncodedData)
		{
			if (asnEncodedData == null)
			{
				throw new ArgumentNullException("asnEncodedData");
			}
			this.Decode(asnEncodedData.RawData);
			base.Oid = asnEncodedData.Oid;
			base.RawData = asnEncodedData.RawData;
		}

		internal void Decode(byte[] attribute)
		{
			if (attribute[0] != 23)
			{
				throw new CryptographicException(Locale.GetText("Only UTCTIME is supported."));
			}
			ASN1 asn = new ASN1(attribute);
			byte[] value = asn.Value;
			string @string = Encoding.ASCII.GetString(value, 0, value.Length - 1);
			this._signingTime = DateTime.ParseExact(@string, "yyMMddHHmmss", null);
		}

		internal byte[] Encode()
		{
			if (this._signingTime.Year <= 1600)
			{
				throw new ArgumentOutOfRangeException("<= 1600");
			}
			if (this._signingTime.Year < 1950 || this._signingTime.Year >= 2050)
			{
				throw new CryptographicException("[1950,2049]");
			}
			string text = this._signingTime.ToString("yyMMddHHmmss", CultureInfo.InvariantCulture) + "Z";
			ASN1 asn = new ASN1(23, Encoding.ASCII.GetBytes(text));
			return asn.GetBytes();
		}

		internal const string oid = "1.2.840.113549.1.9.5";

		internal const string friendlyName = "Signing Time";

		private DateTime _signingTime;
	}
}
