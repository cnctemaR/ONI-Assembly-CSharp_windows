using System;
using System.Collections.Generic;
using System.Text;
using Mono.Security;

namespace System.Security.Cryptography.X509Certificates
{
	public sealed class X509EnhancedKeyUsageExtension : X509Extension
	{
		public X509EnhancedKeyUsageExtension()
		{
			this._oid = new Oid("2.5.29.37", "Enhanced Key Usage");
		}

		public X509EnhancedKeyUsageExtension(AsnEncodedData encodedEnhancedKeyUsages, bool critical)
		{
			this._oid = new Oid("2.5.29.37", "Enhanced Key Usage");
			this._raw = encodedEnhancedKeyUsages.RawData;
			base.Critical = critical;
			this._status = this.Decode(base.RawData);
		}

		public X509EnhancedKeyUsageExtension(OidCollection enhancedKeyUsages, bool critical)
		{
			if (enhancedKeyUsages == null)
			{
				throw new ArgumentNullException("enhancedKeyUsages");
			}
			this._oid = new Oid("2.5.29.37", "Enhanced Key Usage");
			base.Critical = critical;
			this._enhKeyUsage = enhancedKeyUsages.ReadOnlyCopy();
			base.RawData = this.Encode();
		}

		public OidCollection EnhancedKeyUsages
		{
			get
			{
				AsnDecodeStatus status = this._status;
				if (status != AsnDecodeStatus.Ok && status != AsnDecodeStatus.InformationNotAvailable)
				{
					throw new CryptographicException("Badly encoded extension.");
				}
				if (this._enhKeyUsage == null)
				{
					this._enhKeyUsage = new OidCollection();
				}
				this._enhKeyUsage.ReadOnly = true;
				return this._enhKeyUsage;
			}
		}

		public override void CopyFrom(AsnEncodedData asnEncodedData)
		{
			if (asnEncodedData == null)
			{
				throw new ArgumentNullException("encodedData");
			}
			X509Extension x509Extension = asnEncodedData as X509Extension;
			if (x509Extension == null)
			{
				throw new ArgumentException(global::Locale.GetText("Wrong type."), "asnEncodedData");
			}
			if (x509Extension._oid == null)
			{
				this._oid = new Oid("2.5.29.37", "Enhanced Key Usage");
			}
			else
			{
				this._oid = new Oid(x509Extension._oid);
			}
			base.RawData = x509Extension.RawData;
			base.Critical = x509Extension.Critical;
			this._status = this.Decode(base.RawData);
		}

		internal AsnDecodeStatus Decode(byte[] extension)
		{
			if (extension == null || extension.Length == 0)
			{
				return AsnDecodeStatus.BadAsn;
			}
			if (extension[0] != 48)
			{
				return AsnDecodeStatus.BadTag;
			}
			if (this._enhKeyUsage == null)
			{
				this._enhKeyUsage = new OidCollection();
			}
			try
			{
				ASN1 asn = new ASN1(extension);
				if (asn.Tag != 48)
				{
					throw new CryptographicException(global::Locale.GetText("Invalid ASN.1 Tag"));
				}
				for (int i = 0; i < asn.Count; i++)
				{
					this._enhKeyUsage.Add(new Oid(ASN1Convert.ToOid(asn[i])));
				}
			}
			catch
			{
				return AsnDecodeStatus.BadAsn;
			}
			return AsnDecodeStatus.Ok;
		}

		internal byte[] Encode()
		{
			ASN1 asn = new ASN1(48);
			foreach (Oid oid in this._enhKeyUsage)
			{
				asn.Add(ASN1Convert.FromOid(oid.Value));
			}
			return asn.GetBytes();
		}

		internal override string ToString(bool multiLine)
		{
			switch (this._status)
			{
			case AsnDecodeStatus.BadAsn:
				return string.Empty;
			case AsnDecodeStatus.BadTag:
			case AsnDecodeStatus.BadLength:
				return base.FormatUnkownData(this._raw);
			case AsnDecodeStatus.InformationNotAvailable:
				return "Information Not Available";
			default:
			{
				if (this._oid.Value != "2.5.29.37")
				{
					return string.Format("Unknown Key Usage ({0})", this._oid.Value);
				}
				if (this._enhKeyUsage.Count == 0)
				{
					return "Information Not Available";
				}
				StringBuilder stringBuilder = new StringBuilder();
				int i = 0;
				while (i < this._enhKeyUsage.Count)
				{
					Oid oid = this._enhKeyUsage[i];
					string value = oid.Value;
					if (value == null)
					{
						goto IL_0102;
					}
					if (X509EnhancedKeyUsageExtension.<>f__switch$map1A == null)
					{
						X509EnhancedKeyUsageExtension.<>f__switch$map1A = new Dictionary<string, int>(1) { { "1.3.6.1.5.5.7.3.1", 0 } };
					}
					int num;
					if (!X509EnhancedKeyUsageExtension.<>f__switch$map1A.TryGetValue(value, out num))
					{
						goto IL_0102;
					}
					if (num != 0)
					{
						goto IL_0102;
					}
					stringBuilder.Append("Server Authentication (");
					IL_0113:
					stringBuilder.Append(oid.Value);
					stringBuilder.Append(")");
					if (multiLine)
					{
						stringBuilder.Append(Environment.NewLine);
					}
					else if (i != this._enhKeyUsage.Count - 1)
					{
						stringBuilder.Append(", ");
					}
					i++;
					continue;
					IL_0102:
					stringBuilder.Append("Unknown Key Usage (");
					goto IL_0113;
				}
				return stringBuilder.ToString();
			}
			}
		}

		internal const string oid = "2.5.29.37";

		internal const string friendlyName = "Enhanced Key Usage";

		private OidCollection _enhKeyUsage;

		private AsnDecodeStatus _status;
	}
}
