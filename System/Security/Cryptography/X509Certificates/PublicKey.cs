using System;
using System.Collections.Generic;
using Mono.Security;
using Mono.Security.Cryptography;
using Mono.Security.X509;

namespace System.Security.Cryptography.X509Certificates
{
	public sealed class PublicKey
	{
		public PublicKey(Oid oid, AsnEncodedData parameters, AsnEncodedData keyValue)
		{
			if (oid == null)
			{
				throw new ArgumentNullException("oid");
			}
			if (parameters == null)
			{
				throw new ArgumentNullException("parameters");
			}
			if (keyValue == null)
			{
				throw new ArgumentNullException("keyValue");
			}
			this._oid = new Oid(oid);
			this._params = new AsnEncodedData(parameters);
			this._keyValue = new AsnEncodedData(keyValue);
		}

		internal PublicKey(X509Certificate certificate)
		{
			bool flag = true;
			if (certificate.KeyAlgorithm == "1.2.840.113549.1.1.1")
			{
				RSACryptoServiceProvider rsacryptoServiceProvider = certificate.RSA as RSACryptoServiceProvider;
				if (rsacryptoServiceProvider != null && rsacryptoServiceProvider.PublicOnly)
				{
					this._key = certificate.RSA;
					flag = false;
				}
				else
				{
					RSAManaged rsamanaged = certificate.RSA as RSAManaged;
					if (rsamanaged != null && rsamanaged.PublicOnly)
					{
						this._key = certificate.RSA;
						flag = false;
					}
				}
				if (flag)
				{
					RSAParameters rsaparameters = certificate.RSA.ExportParameters(false);
					this._key = RSA.Create();
					(this._key as RSA).ImportParameters(rsaparameters);
				}
			}
			else
			{
				DSACryptoServiceProvider dsacryptoServiceProvider = certificate.DSA as DSACryptoServiceProvider;
				if (dsacryptoServiceProvider != null && dsacryptoServiceProvider.PublicOnly)
				{
					this._key = certificate.DSA;
					flag = false;
				}
				if (flag)
				{
					DSAParameters dsaparameters = certificate.DSA.ExportParameters(false);
					this._key = DSA.Create();
					(this._key as DSA).ImportParameters(dsaparameters);
				}
			}
			this._oid = new Oid(certificate.KeyAlgorithm);
			this._keyValue = new AsnEncodedData(this._oid, certificate.PublicKey);
			this._params = new AsnEncodedData(this._oid, certificate.KeyAlgorithmParameters);
		}

		public AsnEncodedData EncodedKeyValue
		{
			get
			{
				return this._keyValue;
			}
		}

		public AsnEncodedData EncodedParameters
		{
			get
			{
				return this._params;
			}
		}

		public AsymmetricAlgorithm Key
		{
			get
			{
				if (this._key == null)
				{
					string value = this._oid.Value;
					if (value != null)
					{
						if (PublicKey.<>f__switch$map16 == null)
						{
							PublicKey.<>f__switch$map16 = new Dictionary<string, int>(2)
							{
								{ "1.2.840.113549.1.1.1", 0 },
								{ "1.2.840.10040.4.1", 1 }
							};
						}
						int num;
						if (PublicKey.<>f__switch$map16.TryGetValue(value, out num))
						{
							if (num == 0)
							{
								this._key = PublicKey.DecodeRSA(this._keyValue.RawData);
								goto IL_00D7;
							}
							if (num == 1)
							{
								this._key = PublicKey.DecodeDSA(this._keyValue.RawData, this._params.RawData);
								goto IL_00D7;
							}
						}
					}
					string text = global::Locale.GetText("Cannot decode public key from unknown OID '{0}'.", new object[] { this._oid.Value });
					throw new NotSupportedException(text);
				}
				IL_00D7:
				return this._key;
			}
		}

		public Oid Oid
		{
			get
			{
				return this._oid;
			}
		}

		private static byte[] GetUnsignedBigInteger(byte[] integer)
		{
			if (integer[0] != 0)
			{
				return integer;
			}
			int num = integer.Length - 1;
			byte[] array = new byte[num];
			Buffer.BlockCopy(integer, 1, array, 0, num);
			return array;
		}

		internal static DSA DecodeDSA(byte[] rawPublicKey, byte[] rawParameters)
		{
			DSAParameters dsaparameters = default(DSAParameters);
			try
			{
				ASN1 asn = new ASN1(rawPublicKey);
				if (asn.Tag != 2)
				{
					throw new CryptographicException(global::Locale.GetText("Missing DSA Y integer."));
				}
				dsaparameters.Y = PublicKey.GetUnsignedBigInteger(asn.Value);
				ASN1 asn2 = new ASN1(rawParameters);
				if (asn2 == null || asn2.Tag != 48 || asn2.Count < 3)
				{
					throw new CryptographicException(global::Locale.GetText("Missing DSA parameters."));
				}
				if (asn2[0].Tag != 2 || asn2[1].Tag != 2 || asn2[2].Tag != 2)
				{
					throw new CryptographicException(global::Locale.GetText("Invalid DSA parameters."));
				}
				dsaparameters.P = PublicKey.GetUnsignedBigInteger(asn2[0].Value);
				dsaparameters.Q = PublicKey.GetUnsignedBigInteger(asn2[1].Value);
				dsaparameters.G = PublicKey.GetUnsignedBigInteger(asn2[2].Value);
			}
			catch (Exception ex)
			{
				string text = global::Locale.GetText("Error decoding the ASN.1 structure.");
				throw new CryptographicException(text, ex);
			}
			DSA dsa = new DSACryptoServiceProvider(dsaparameters.Y.Length << 3);
			dsa.ImportParameters(dsaparameters);
			return dsa;
		}

		internal static RSA DecodeRSA(byte[] rawPublicKey)
		{
			RSAParameters rsaparameters = default(RSAParameters);
			try
			{
				ASN1 asn = new ASN1(rawPublicKey);
				if (asn.Count == 0)
				{
					throw new CryptographicException(global::Locale.GetText("Missing RSA modulus and exponent."));
				}
				ASN1 asn2 = asn[0];
				if (asn2 == null || asn2.Tag != 2)
				{
					throw new CryptographicException(global::Locale.GetText("Missing RSA modulus."));
				}
				ASN1 asn3 = asn[1];
				if (asn3.Tag != 2)
				{
					throw new CryptographicException(global::Locale.GetText("Missing RSA public exponent."));
				}
				rsaparameters.Modulus = PublicKey.GetUnsignedBigInteger(asn2.Value);
				rsaparameters.Exponent = asn3.Value;
			}
			catch (Exception ex)
			{
				string text = global::Locale.GetText("Error decoding the ASN.1 structure.");
				throw new CryptographicException(text, ex);
			}
			int num = rsaparameters.Modulus.Length << 3;
			RSA rsa = new RSACryptoServiceProvider(num);
			rsa.ImportParameters(rsaparameters);
			return rsa;
		}

		private const string rsaOid = "1.2.840.113549.1.1.1";

		private const string dsaOid = "1.2.840.10040.4.1";

		private AsymmetricAlgorithm _key;

		private AsnEncodedData _keyValue;

		private AsnEncodedData _params;

		private Oid _oid;
	}
}
