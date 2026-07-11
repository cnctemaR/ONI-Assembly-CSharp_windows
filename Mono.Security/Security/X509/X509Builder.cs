using System;
using System.Globalization;
using System.Security.Cryptography;

namespace Mono.Security.X509
{
	public abstract class X509Builder
	{
		protected X509Builder()
		{
			this.hashName = "SHA1";
		}

		protected abstract ASN1 ToBeSigned(string hashName);

		protected string GetOid(string hashName)
		{
			string text = hashName.ToLower(CultureInfo.InvariantCulture);
			switch (text)
			{
			case "md2":
				return "1.2.840.113549.1.1.2";
			case "md4":
				return "1.2.840.113549.1.1.3";
			case "md5":
				return "1.2.840.113549.1.1.4";
			case "sha1":
				return "1.2.840.113549.1.1.5";
			case "sha256":
				return "1.2.840.113549.1.1.11";
			case "sha384":
				return "1.2.840.113549.1.1.12";
			case "sha512":
				return "1.2.840.113549.1.1.13";
			}
			throw new NotSupportedException("Unknown hash algorithm " + hashName);
		}

		public string Hash
		{
			get
			{
				return this.hashName;
			}
			set
			{
				if (this.hashName == null)
				{
					this.hashName = "SHA1";
				}
				else
				{
					this.hashName = value;
				}
			}
		}

		public virtual byte[] Sign(AsymmetricAlgorithm aa)
		{
			if (aa is RSA)
			{
				return this.Sign(aa as RSA);
			}
			if (aa is DSA)
			{
				return this.Sign(aa as DSA);
			}
			throw new NotSupportedException("Unknown Asymmetric Algorithm " + aa.ToString());
		}

		private byte[] Build(ASN1 tbs, string hashoid, byte[] signature)
		{
			ASN1 asn = new ASN1(48);
			asn.Add(tbs);
			asn.Add(PKCS7.AlgorithmIdentifier(hashoid));
			byte[] array = new byte[signature.Length + 1];
			Buffer.BlockCopy(signature, 0, array, 1, signature.Length);
			asn.Add(new ASN1(3, array));
			return asn.GetBytes();
		}

		public virtual byte[] Sign(RSA key)
		{
			string oid = this.GetOid(this.hashName);
			ASN1 asn = this.ToBeSigned(oid);
			HashAlgorithm hashAlgorithm = HashAlgorithm.Create(this.hashName);
			byte[] array = hashAlgorithm.ComputeHash(asn.GetBytes());
			RSAPKCS1SignatureFormatter rsapkcs1SignatureFormatter = new RSAPKCS1SignatureFormatter(key);
			rsapkcs1SignatureFormatter.SetHashAlgorithm(this.hashName);
			byte[] array2 = rsapkcs1SignatureFormatter.CreateSignature(array);
			return this.Build(asn, oid, array2);
		}

		public virtual byte[] Sign(DSA key)
		{
			string text = "1.2.840.10040.4.3";
			ASN1 asn = this.ToBeSigned(text);
			HashAlgorithm hashAlgorithm = HashAlgorithm.Create(this.hashName);
			if (!(hashAlgorithm is SHA1))
			{
				throw new NotSupportedException("Only SHA-1 is supported for DSA");
			}
			byte[] array = hashAlgorithm.ComputeHash(asn.GetBytes());
			DSASignatureFormatter dsasignatureFormatter = new DSASignatureFormatter(key);
			dsasignatureFormatter.SetHashAlgorithm(this.hashName);
			byte[] array2 = dsasignatureFormatter.CreateSignature(array);
			byte[] array3 = new byte[20];
			Buffer.BlockCopy(array2, 0, array3, 0, 20);
			byte[] array4 = new byte[20];
			Buffer.BlockCopy(array2, 20, array4, 0, 20);
			ASN1 asn2 = new ASN1(48);
			asn2.Add(new ASN1(2, array3));
			asn2.Add(new ASN1(2, array4));
			return this.Build(asn, text, asn2.GetBytes());
		}

		private const string defaultHash = "SHA1";

		private string hashName;
	}
}
