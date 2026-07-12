using System;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using Mono.Security.Cryptography;

namespace Mono.Security.X509
{
	public class X509Certificate : ISerializable
	{
		private void Parse(byte[] data)
		{
			try
			{
				this.decoder = new ASN1(data);
				if (this.decoder.Tag != 48)
				{
					throw new CryptographicException(X509Certificate.encoding_error);
				}
				if (this.decoder[0].Tag != 48)
				{
					throw new CryptographicException(X509Certificate.encoding_error);
				}
				ASN1 asn = this.decoder[0];
				int num = 0;
				ASN1 asn2 = this.decoder[0][num];
				this.version = 1;
				if (asn2.Tag == 160 && asn2.Count > 0)
				{
					this.version += (int)asn2[0].Value[0];
					num++;
				}
				ASN1 asn3 = this.decoder[0][num++];
				if (asn3.Tag != 2)
				{
					throw new CryptographicException(X509Certificate.encoding_error);
				}
				this.serialnumber = asn3.Value;
				Array.Reverse<byte>(this.serialnumber, 0, this.serialnumber.Length);
				num++;
				this.issuer = asn.Element(num++, 48);
				this.m_issuername = X501.ToString(this.issuer);
				ASN1 asn4 = asn.Element(num++, 48);
				ASN1 asn5 = asn4[0];
				this.m_from = ASN1Convert.ToDateTime(asn5);
				ASN1 asn6 = asn4[1];
				this.m_until = ASN1Convert.ToDateTime(asn6);
				this.subject = asn.Element(num++, 48);
				this.m_subject = X501.ToString(this.subject);
				ASN1 asn7 = asn.Element(num++, 48);
				ASN1 asn8 = asn7.Element(0, 48);
				ASN1 asn9 = asn8.Element(0, 6);
				this.m_keyalgo = ASN1Convert.ToOid(asn9);
				ASN1 asn10 = asn8[1];
				this.m_keyalgoparams = ((asn8.Count > 1) ? asn10.GetBytes() : null);
				ASN1 asn11 = asn7.Element(1, 3);
				int num2 = asn11.Length - 1;
				this.m_publickey = new byte[num2];
				Buffer.BlockCopy(asn11.Value, 1, this.m_publickey, 0, num2);
				byte[] value = this.decoder[2].Value;
				this.signature = new byte[value.Length - 1];
				Buffer.BlockCopy(value, 1, this.signature, 0, this.signature.Length);
				asn8 = this.decoder[1];
				asn9 = asn8.Element(0, 6);
				this.m_signaturealgo = ASN1Convert.ToOid(asn9);
				asn10 = asn8[1];
				if (asn10 != null)
				{
					this.m_signaturealgoparams = asn10.GetBytes();
				}
				else
				{
					this.m_signaturealgoparams = null;
				}
				ASN1 asn12 = asn.Element(num, 129);
				if (asn12 != null)
				{
					num++;
					this.issuerUniqueID = asn12.Value;
				}
				ASN1 asn13 = asn.Element(num, 130);
				if (asn13 != null)
				{
					num++;
					this.subjectUniqueID = asn13.Value;
				}
				ASN1 asn14 = asn.Element(num, 163);
				if (asn14 != null && asn14.Count == 1)
				{
					this.extensions = new X509ExtensionCollection(asn14[0]);
				}
				else
				{
					this.extensions = new X509ExtensionCollection(null);
				}
				this.m_encodedcert = (byte[])data.Clone();
			}
			catch (Exception ex)
			{
				throw new CryptographicException(X509Certificate.encoding_error, ex);
			}
		}

		public X509Certificate(byte[] data)
		{
			if (data != null)
			{
				if (data.Length != 0 && data[0] != 48)
				{
					try
					{
						data = X509Certificate.PEM("CERTIFICATE", data);
					}
					catch (Exception ex)
					{
						throw new CryptographicException(X509Certificate.encoding_error, ex);
					}
				}
				this.Parse(data);
			}
		}

		private byte[] GetUnsignedBigInteger(byte[] integer)
		{
			if (integer[0] == 0)
			{
				int num = integer.Length - 1;
				byte[] array = new byte[num];
				Buffer.BlockCopy(integer, 1, array, 0, num);
				return array;
			}
			return integer;
		}

		public DSA DSA
		{
			get
			{
				if (this.m_keyalgoparams == null)
				{
					throw new CryptographicException("Missing key algorithm parameters.");
				}
				if (this._dsa == null && this.m_keyalgo == "1.2.840.10040.4.1")
				{
					DSAParameters dsaparameters = default(DSAParameters);
					ASN1 asn = new ASN1(this.m_publickey);
					if (asn == null || asn.Tag != 2)
					{
						return null;
					}
					dsaparameters.Y = this.GetUnsignedBigInteger(asn.Value);
					ASN1 asn2 = new ASN1(this.m_keyalgoparams);
					if (asn2 == null || asn2.Tag != 48 || asn2.Count < 3)
					{
						return null;
					}
					if (asn2[0].Tag != 2 || asn2[1].Tag != 2 || asn2[2].Tag != 2)
					{
						return null;
					}
					dsaparameters.P = this.GetUnsignedBigInteger(asn2[0].Value);
					dsaparameters.Q = this.GetUnsignedBigInteger(asn2[1].Value);
					dsaparameters.G = this.GetUnsignedBigInteger(asn2[2].Value);
					this._dsa = new DSACryptoServiceProvider(dsaparameters.Y.Length << 3);
					this._dsa.ImportParameters(dsaparameters);
				}
				return this._dsa;
			}
			set
			{
				this._dsa = value;
				if (value != null)
				{
					this._rsa = null;
				}
			}
		}

		public X509ExtensionCollection Extensions
		{
			get
			{
				return this.extensions;
			}
		}

		public byte[] Hash
		{
			get
			{
				if (this.certhash == null)
				{
					if (this.decoder == null || this.decoder.Count < 1)
					{
						return null;
					}
					string text = PKCS1.HashNameFromOid(this.m_signaturealgo, false);
					if (text == null)
					{
						return null;
					}
					byte[] bytes = this.decoder[0].GetBytes();
					using (HashAlgorithm hashAlgorithm = PKCS1.CreateFromName(text))
					{
						this.certhash = hashAlgorithm.ComputeHash(bytes, 0, bytes.Length);
					}
				}
				return (byte[])this.certhash.Clone();
			}
		}

		public virtual string IssuerName
		{
			get
			{
				return this.m_issuername;
			}
		}

		public virtual string KeyAlgorithm
		{
			get
			{
				return this.m_keyalgo;
			}
		}

		public virtual byte[] KeyAlgorithmParameters
		{
			get
			{
				if (this.m_keyalgoparams == null)
				{
					return null;
				}
				return (byte[])this.m_keyalgoparams.Clone();
			}
			set
			{
				this.m_keyalgoparams = value;
			}
		}

		public virtual byte[] PublicKey
		{
			get
			{
				if (this.m_publickey == null)
				{
					return null;
				}
				return (byte[])this.m_publickey.Clone();
			}
		}

		public virtual RSA RSA
		{
			get
			{
				if (this._rsa == null && this.m_keyalgo == "1.2.840.113549.1.1.1")
				{
					RSAParameters rsaparameters = default(RSAParameters);
					ASN1 asn = new ASN1(this.m_publickey);
					ASN1 asn2 = asn[0];
					if (asn2 == null || asn2.Tag != 2)
					{
						return null;
					}
					ASN1 asn3 = asn[1];
					if (asn3.Tag != 2)
					{
						return null;
					}
					rsaparameters.Modulus = this.GetUnsignedBigInteger(asn2.Value);
					rsaparameters.Exponent = asn3.Value;
					int num = rsaparameters.Modulus.Length << 3;
					this._rsa = new RSACryptoServiceProvider(num);
					this._rsa.ImportParameters(rsaparameters);
				}
				return this._rsa;
			}
			set
			{
				if (value != null)
				{
					this._dsa = null;
				}
				this._rsa = value;
			}
		}

		public virtual byte[] RawData
		{
			get
			{
				if (this.m_encodedcert == null)
				{
					return null;
				}
				return (byte[])this.m_encodedcert.Clone();
			}
		}

		public virtual byte[] SerialNumber
		{
			get
			{
				if (this.serialnumber == null)
				{
					return null;
				}
				return (byte[])this.serialnumber.Clone();
			}
		}

		public virtual byte[] Signature
		{
			get
			{
				if (this.signature == null)
				{
					return null;
				}
				string signaturealgo = this.m_signaturealgo;
				uint num = global::<PrivateImplementationDetails>.ComputeStringHash(signaturealgo);
				if (num <= 719034781U)
				{
					if (num <= 601591448U)
					{
						if (num != 510574318U)
						{
							if (num != 601591448U)
							{
								goto IL_022D;
							}
							if (!(signaturealgo == "1.2.840.113549.1.1.5"))
							{
								goto IL_022D;
							}
						}
						else
						{
							if (!(signaturealgo == "1.2.840.10040.4.3"))
							{
								goto IL_022D;
							}
							ASN1 asn = new ASN1(this.signature);
							if (asn == null || asn.Count != 2)
							{
								return null;
							}
							byte[] value = asn[0].Value;
							byte[] value2 = asn[1].Value;
							byte[] array = new byte[40];
							int num2 = Math.Max(0, value.Length - 20);
							int num3 = Math.Max(0, 20 - value.Length);
							Buffer.BlockCopy(value, num2, array, num3, value.Length - num2);
							int num4 = Math.Max(0, value2.Length - 20);
							int num5 = Math.Max(20, 40 - value2.Length);
							Buffer.BlockCopy(value2, num4, array, num5, value2.Length - num4);
							return array;
						}
					}
					else if (num != 618369067U)
					{
						if (num != 702257162U)
						{
							if (num != 719034781U)
							{
								goto IL_022D;
							}
							if (!(signaturealgo == "1.2.840.113549.1.1.2"))
							{
								goto IL_022D;
							}
						}
						else if (!(signaturealgo == "1.2.840.113549.1.1.3"))
						{
							goto IL_022D;
						}
					}
					else if (!(signaturealgo == "1.2.840.113549.1.1.4"))
					{
						goto IL_022D;
					}
				}
				else if (num <= 2477476687U)
				{
					if (num != 875536856U)
					{
						if (num != 2477476687U)
						{
							goto IL_022D;
						}
						if (!(signaturealgo == "1.2.840.113549.1.1.11"))
						{
							goto IL_022D;
						}
					}
					else if (!(signaturealgo == "1.3.14.3.2.29"))
					{
						goto IL_022D;
					}
				}
				else if (num != 2494254306U)
				{
					if (num != 2511031925U)
					{
						if (num != 3493391575U)
						{
							goto IL_022D;
						}
						if (!(signaturealgo == "1.3.36.3.3.1.2"))
						{
							goto IL_022D;
						}
					}
					else if (!(signaturealgo == "1.2.840.113549.1.1.13"))
					{
						goto IL_022D;
					}
				}
				else if (!(signaturealgo == "1.2.840.113549.1.1.12"))
				{
					goto IL_022D;
				}
				return (byte[])this.signature.Clone();
				IL_022D:
				throw new CryptographicException("Unsupported hash algorithm: " + this.m_signaturealgo);
			}
		}

		public virtual string SignatureAlgorithm
		{
			get
			{
				return this.m_signaturealgo;
			}
		}

		public virtual byte[] SignatureAlgorithmParameters
		{
			get
			{
				if (this.m_signaturealgoparams == null)
				{
					return this.m_signaturealgoparams;
				}
				return (byte[])this.m_signaturealgoparams.Clone();
			}
		}

		public virtual string SubjectName
		{
			get
			{
				return this.m_subject;
			}
		}

		public virtual DateTime ValidFrom
		{
			get
			{
				return this.m_from;
			}
		}

		public virtual DateTime ValidUntil
		{
			get
			{
				return this.m_until;
			}
		}

		public int Version
		{
			get
			{
				return this.version;
			}
		}

		public bool IsCurrent
		{
			get
			{
				return this.WasCurrent(DateTime.UtcNow);
			}
		}

		public bool WasCurrent(DateTime instant)
		{
			return instant > this.ValidFrom && instant <= this.ValidUntil;
		}

		public byte[] IssuerUniqueIdentifier
		{
			get
			{
				if (this.issuerUniqueID == null)
				{
					return null;
				}
				return (byte[])this.issuerUniqueID.Clone();
			}
		}

		public byte[] SubjectUniqueIdentifier
		{
			get
			{
				if (this.subjectUniqueID == null)
				{
					return null;
				}
				return (byte[])this.subjectUniqueID.Clone();
			}
		}

		internal bool VerifySignature(DSA dsa)
		{
			DSASignatureDeformatter dsasignatureDeformatter = new DSASignatureDeformatter(dsa);
			dsasignatureDeformatter.SetHashAlgorithm("SHA1");
			return dsasignatureDeformatter.VerifySignature(this.Hash, this.Signature);
		}

		internal bool VerifySignature(RSA rsa)
		{
			if (this.m_signaturealgo == "1.2.840.10040.4.3")
			{
				return false;
			}
			RSAPKCS1SignatureDeformatter rsapkcs1SignatureDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
			rsapkcs1SignatureDeformatter.SetHashAlgorithm(PKCS1.HashNameFromOid(this.m_signaturealgo, true));
			return rsapkcs1SignatureDeformatter.VerifySignature(this.Hash, this.Signature);
		}

		public bool VerifySignature(AsymmetricAlgorithm aa)
		{
			if (aa == null)
			{
				throw new ArgumentNullException("aa");
			}
			if (aa is RSA)
			{
				return this.VerifySignature(aa as RSA);
			}
			if (aa is DSA)
			{
				return this.VerifySignature(aa as DSA);
			}
			throw new NotSupportedException("Unknown Asymmetric Algorithm " + aa.ToString());
		}

		public bool CheckSignature(byte[] hash, string hashAlgorithm, byte[] signature)
		{
			return ((RSACryptoServiceProvider)this.RSA).VerifyHash(hash, hashAlgorithm, signature);
		}

		public bool IsSelfSigned
		{
			get
			{
				if (this.m_issuername != this.m_subject)
				{
					return false;
				}
				bool flag;
				try
				{
					if (this.RSA != null)
					{
						flag = this.VerifySignature(this.RSA);
					}
					else if (this.DSA != null)
					{
						flag = this.VerifySignature(this.DSA);
					}
					else
					{
						flag = false;
					}
				}
				catch (CryptographicException)
				{
					flag = false;
				}
				return flag;
			}
		}

		public ASN1 GetIssuerName()
		{
			return this.issuer;
		}

		public ASN1 GetSubjectName()
		{
			return this.subject;
		}

		protected X509Certificate(SerializationInfo info, StreamingContext context)
		{
			this.Parse((byte[])info.GetValue("raw", typeof(byte[])));
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("raw", this.m_encodedcert);
		}

		private static byte[] PEM(string type, byte[] data)
		{
			string @string = Encoding.ASCII.GetString(data);
			string text = string.Format("-----BEGIN {0}-----", type);
			string text2 = string.Format("-----END {0}-----", type);
			int num = @string.IndexOf(text) + text.Length;
			int num2 = @string.IndexOf(text2, num);
			return Convert.FromBase64String(@string.Substring(num, num2 - num));
		}

		private ASN1 decoder;

		private byte[] m_encodedcert;

		private DateTime m_from;

		private DateTime m_until;

		private ASN1 issuer;

		private string m_issuername;

		private string m_keyalgo;

		private byte[] m_keyalgoparams;

		private ASN1 subject;

		private string m_subject;

		private byte[] m_publickey;

		private byte[] signature;

		private string m_signaturealgo;

		private byte[] m_signaturealgoparams;

		private byte[] certhash;

		private RSA _rsa;

		private DSA _dsa;

		internal const string OID_DSA = "1.2.840.10040.4.1";

		internal const string OID_RSA = "1.2.840.113549.1.1.1";

		internal const string OID_ECC = "1.2.840.10045.2.1";

		private int version;

		private byte[] serialnumber;

		private byte[] issuerUniqueID;

		private byte[] subjectUniqueID;

		private X509ExtensionCollection extensions;

		private static string encoding_error = Locale.GetText("Input data cannot be coded as a valid certificate.");
	}
}
