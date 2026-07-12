using System;
using System.Security.Cryptography.X509Certificates;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class CmsRecipient
	{
		public CmsRecipient(X509Certificate2 certificate)
			: this(SubjectIdentifierType.IssuerAndSerialNumber, certificate)
		{
		}

		public CmsRecipient(SubjectIdentifierType recipientIdentifierType, X509Certificate2 certificate)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			switch (recipientIdentifierType)
			{
			case SubjectIdentifierType.Unknown:
				recipientIdentifierType = SubjectIdentifierType.IssuerAndSerialNumber;
				break;
			case SubjectIdentifierType.IssuerAndSerialNumber:
			case SubjectIdentifierType.SubjectKeyIdentifier:
				break;
			default:
				throw new CryptographicException(SR.Format("The subject identifier type {0} is not valid.", recipientIdentifierType));
			}
			this.RecipientIdentifierType = recipientIdentifierType;
			this.Certificate = certificate;
		}

		public SubjectIdentifierType RecipientIdentifierType { get; }

		public X509Certificate2 Certificate { get; }
	}
}
