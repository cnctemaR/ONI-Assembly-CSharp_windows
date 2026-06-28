using System;
using System.Security.Cryptography.X509Certificates;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class CmsRecipient
	{
		public CmsRecipient(X509Certificate2 certificate)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			this._recipient = SubjectIdentifierType.IssuerAndSerialNumber;
			this._certificate = certificate;
		}

		public CmsRecipient(SubjectIdentifierType recipientIdentifierType, X509Certificate2 certificate)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			if (recipientIdentifierType == SubjectIdentifierType.Unknown)
			{
				this._recipient = SubjectIdentifierType.IssuerAndSerialNumber;
			}
			else
			{
				this._recipient = recipientIdentifierType;
			}
			this._certificate = certificate;
		}

		public X509Certificate2 Certificate
		{
			get
			{
				return this._certificate;
			}
		}

		public SubjectIdentifierType RecipientIdentifierType
		{
			get
			{
				return this._recipient;
			}
		}

		private SubjectIdentifierType _recipient;

		private X509Certificate2 _certificate;
	}
}
