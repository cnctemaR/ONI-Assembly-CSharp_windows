using System;
using System.Security.Cryptography.X509Certificates;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class CmsSigner
	{
		public CmsSigner()
		{
			this._signer = SubjectIdentifierType.IssuerAndSerialNumber;
			this._digest = new Oid("1.3.14.3.2.26");
			this._options = X509IncludeOption.ExcludeRoot;
			this._signed = new CryptographicAttributeObjectCollection();
			this._unsigned = new CryptographicAttributeObjectCollection();
			this._coll = new X509Certificate2Collection();
		}

		public CmsSigner(SubjectIdentifierType signerIdentifierType)
			: this()
		{
			if (signerIdentifierType == SubjectIdentifierType.Unknown)
			{
				this._signer = SubjectIdentifierType.IssuerAndSerialNumber;
			}
			else
			{
				this._signer = signerIdentifierType;
			}
		}

		public CmsSigner(SubjectIdentifierType signerIdentifierType, X509Certificate2 certificate)
			: this(signerIdentifierType)
		{
			this._certificate = certificate;
		}

		public CmsSigner(X509Certificate2 certificate)
			: this()
		{
			this._certificate = certificate;
		}

		[MonoTODO]
		public CmsSigner(CspParameters parameters)
			: this()
		{
		}

		public CryptographicAttributeObjectCollection SignedAttributes
		{
			get
			{
				return this._signed;
			}
		}

		public X509Certificate2 Certificate
		{
			get
			{
				return this._certificate;
			}
			set
			{
				this._certificate = value;
			}
		}

		public X509Certificate2Collection Certificates
		{
			get
			{
				return this._coll;
			}
		}

		public Oid DigestAlgorithm
		{
			get
			{
				return this._digest;
			}
			set
			{
				this._digest = value;
			}
		}

		public X509IncludeOption IncludeOption
		{
			get
			{
				return this._options;
			}
			set
			{
				this._options = value;
			}
		}

		public SubjectIdentifierType SignerIdentifierType
		{
			get
			{
				return this._signer;
			}
			set
			{
				if (value == SubjectIdentifierType.Unknown)
				{
					throw new ArgumentException("value");
				}
				this._signer = value;
			}
		}

		public CryptographicAttributeObjectCollection UnsignedAttributes
		{
			get
			{
				return this._unsigned;
			}
		}

		private SubjectIdentifierType _signer;

		private X509Certificate2 _certificate;

		private X509Certificate2Collection _coll;

		private Oid _digest;

		private X509IncludeOption _options;

		private CryptographicAttributeObjectCollection _signed;

		private CryptographicAttributeObjectCollection _unsigned;
	}
}
