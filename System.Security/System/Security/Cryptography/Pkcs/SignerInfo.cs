using System;
using System.Security.Cryptography.X509Certificates;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class SignerInfo
	{
		internal SignerInfo(string hashName, X509Certificate2 certificate, SubjectIdentifierType type, object o, int version)
		{
			this._digest = new Oid(CryptoConfig.MapNameToOID(hashName));
			this._certificate = certificate;
			this._counter = new SignerInfoCollection();
			this._signed = new CryptographicAttributeObjectCollection();
			this._unsigned = new CryptographicAttributeObjectCollection();
			this._signer = new SubjectIdentifier(type, o);
			this._version = version;
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
		}

		public SignerInfoCollection CounterSignerInfos
		{
			get
			{
				return this._counter;
			}
		}

		public Oid DigestAlgorithm
		{
			get
			{
				return this._digest;
			}
		}

		public SubjectIdentifier SignerIdentifier
		{
			get
			{
				return this._signer;
			}
		}

		public CryptographicAttributeObjectCollection UnsignedAttributes
		{
			get
			{
				return this._unsigned;
			}
		}

		public int Version
		{
			get
			{
				return this._version;
			}
		}

		[MonoTODO]
		public void CheckHash()
		{
		}

		[MonoTODO]
		public void CheckSignature(bool verifySignatureOnly)
		{
		}

		[MonoTODO]
		public void CheckSignature(X509Certificate2Collection extraStore, bool verifySignatureOnly)
		{
		}

		[MonoTODO]
		public void ComputeCounterSignature()
		{
		}

		[MonoTODO]
		public void ComputeCounterSignature(CmsSigner signer)
		{
		}

		[MonoTODO]
		public void RemoveCounterSignature(SignerInfo counterSignerInfo)
		{
		}

		[MonoTODO]
		public void RemoveCounterSignature(int index)
		{
		}

		private SubjectIdentifier _signer;

		private X509Certificate2 _certificate;

		private Oid _digest;

		private SignerInfoCollection _counter;

		private CryptographicAttributeObjectCollection _signed;

		private CryptographicAttributeObjectCollection _unsigned;

		private int _version;
	}
}
