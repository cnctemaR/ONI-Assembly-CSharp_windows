using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Mono.Security;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	public class AsnEncodedData
	{
		protected AsnEncodedData()
		{
		}

		public AsnEncodedData(string oid, byte[] rawData)
		{
			this._oid = new Oid(oid);
			this.RawData = rawData;
		}

		public AsnEncodedData(Oid oid, byte[] rawData)
		{
			this.Oid = oid;
			this.RawData = rawData;
		}

		public AsnEncodedData(AsnEncodedData asnEncodedData)
		{
			if (asnEncodedData == null)
			{
				throw new ArgumentNullException("asnEncodedData");
			}
			if (asnEncodedData._oid != null)
			{
				this.Oid = new Oid(asnEncodedData._oid);
			}
			this.RawData = asnEncodedData._raw;
		}

		public AsnEncodedData(byte[] rawData)
		{
			this.RawData = rawData;
		}

		public Oid Oid
		{
			get
			{
				return this._oid;
			}
			set
			{
				if (value == null)
				{
					this._oid = null;
				}
				else
				{
					this._oid = new Oid(value);
				}
			}
		}

		public byte[] RawData
		{
			get
			{
				return this._raw;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("RawData");
				}
				this._raw = (byte[])value.Clone();
			}
		}

		public virtual void CopyFrom(AsnEncodedData asnEncodedData)
		{
			if (asnEncodedData == null)
			{
				throw new ArgumentNullException("asnEncodedData");
			}
			if (asnEncodedData._oid == null)
			{
				this.Oid = null;
			}
			else
			{
				this.Oid = new Oid(asnEncodedData._oid);
			}
			this.RawData = asnEncodedData._raw;
		}

		public virtual string Format(bool multiLine)
		{
			if (this._raw == null)
			{
				return string.Empty;
			}
			if (this._oid == null)
			{
				return this.Default(multiLine);
			}
			return this.ToString(multiLine);
		}

		internal virtual string ToString(bool multiLine)
		{
			string value = this._oid.Value;
			switch (value)
			{
			case "2.5.29.19":
				return this.BasicConstraintsExtension(multiLine);
			case "2.5.29.37":
				return this.EnhancedKeyUsageExtension(multiLine);
			case "2.5.29.15":
				return this.KeyUsageExtension(multiLine);
			case "2.5.29.14":
				return this.SubjectKeyIdentifierExtension(multiLine);
			case "2.5.29.17":
				return this.SubjectAltName(multiLine);
			case "2.16.840.1.113730.1.1":
				return this.NetscapeCertType(multiLine);
			}
			return this.Default(multiLine);
		}

		internal string Default(bool multiLine)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this._raw.Length; i++)
			{
				stringBuilder.Append(this._raw[i].ToString("x2"));
				if (i != this._raw.Length - 1)
				{
					stringBuilder.Append(" ");
				}
			}
			return stringBuilder.ToString();
		}

		internal string BasicConstraintsExtension(bool multiLine)
		{
			string text;
			try
			{
				global::System.Security.Cryptography.X509Certificates.X509BasicConstraintsExtension x509BasicConstraintsExtension = new global::System.Security.Cryptography.X509Certificates.X509BasicConstraintsExtension(this, false);
				text = x509BasicConstraintsExtension.ToString(multiLine);
			}
			catch
			{
				text = string.Empty;
			}
			return text;
		}

		internal string EnhancedKeyUsageExtension(bool multiLine)
		{
			string text;
			try
			{
				global::System.Security.Cryptography.X509Certificates.X509EnhancedKeyUsageExtension x509EnhancedKeyUsageExtension = new global::System.Security.Cryptography.X509Certificates.X509EnhancedKeyUsageExtension(this, false);
				text = x509EnhancedKeyUsageExtension.ToString(multiLine);
			}
			catch
			{
				text = string.Empty;
			}
			return text;
		}

		internal string KeyUsageExtension(bool multiLine)
		{
			string text;
			try
			{
				global::System.Security.Cryptography.X509Certificates.X509KeyUsageExtension x509KeyUsageExtension = new global::System.Security.Cryptography.X509Certificates.X509KeyUsageExtension(this, false);
				text = x509KeyUsageExtension.ToString(multiLine);
			}
			catch
			{
				text = string.Empty;
			}
			return text;
		}

		internal string SubjectKeyIdentifierExtension(bool multiLine)
		{
			string text;
			try
			{
				global::System.Security.Cryptography.X509Certificates.X509SubjectKeyIdentifierExtension x509SubjectKeyIdentifierExtension = new global::System.Security.Cryptography.X509Certificates.X509SubjectKeyIdentifierExtension(this, false);
				text = x509SubjectKeyIdentifierExtension.ToString(multiLine);
			}
			catch
			{
				text = string.Empty;
			}
			return text;
		}

		internal string SubjectAltName(bool multiLine)
		{
			if (this._raw.Length < 5)
			{
				return "Information Not Available";
			}
			string text3;
			try
			{
				ASN1 asn = new ASN1(this._raw);
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < asn.Count; i++)
				{
					ASN1 asn2 = asn[i];
					byte tag = asn2.Tag;
					string text;
					string text2;
					if (tag != 129)
					{
						if (tag != 130)
						{
							text = string.Format("Unknown ({0})=", asn2.Tag);
							text2 = CryptoConvert.ToHex(asn2.Value);
						}
						else
						{
							text = "DNS Name=";
							text2 = Encoding.ASCII.GetString(asn2.Value);
						}
					}
					else
					{
						text = "RFC822 Name=";
						text2 = Encoding.ASCII.GetString(asn2.Value);
					}
					stringBuilder.Append(text);
					stringBuilder.Append(text2);
					if (multiLine)
					{
						stringBuilder.Append(Environment.NewLine);
					}
					else if (i < asn.Count - 1)
					{
						stringBuilder.Append(", ");
					}
				}
				text3 = stringBuilder.ToString();
			}
			catch
			{
				text3 = string.Empty;
			}
			return text3;
		}

		internal string NetscapeCertType(bool multiLine)
		{
			if (this._raw.Length < 4 || this._raw[0] != 3 || this._raw[1] != 2)
			{
				return "Information Not Available";
			}
			int num = this._raw[3] >> (int)this._raw[2] << (int)this._raw[2];
			StringBuilder stringBuilder = new StringBuilder();
			if ((num & 128) == 128)
			{
				stringBuilder.Append("SSL Client Authentication");
			}
			if ((num & 64) == 64)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append("SSL Server Authentication");
			}
			if ((num & 32) == 32)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append("SMIME");
			}
			if ((num & 16) == 16)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append("Signature");
			}
			if ((num & 8) == 8)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append("Unknown cert type");
			}
			if ((num & 4) == 4)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append("SSL CA");
			}
			if ((num & 2) == 2)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append("SMIME CA");
			}
			if ((num & 1) == 1)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append("Signature CA");
			}
			stringBuilder.AppendFormat(" ({0})", num.ToString("x2"));
			return stringBuilder.ToString();
		}

		internal Oid _oid;

		internal byte[] _raw;
	}
}
