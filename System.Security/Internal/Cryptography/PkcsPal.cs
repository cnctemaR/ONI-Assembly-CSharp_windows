using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using Internal.Cryptography.Pal.AnyOS;

namespace Internal.Cryptography
{
	internal abstract class PkcsPal
	{
		public abstract byte[] Encrypt(CmsRecipientCollection recipients, ContentInfo contentInfo, AlgorithmIdentifier contentEncryptionAlgorithm, X509Certificate2Collection originatorCerts, CryptographicAttributeObjectCollection unprotectedAttributes);

		public abstract DecryptorPal Decode(byte[] encodedMessage, out int version, out ContentInfo contentInfo, out AlgorithmIdentifier contentEncryptionAlgorithm, out X509Certificate2Collection originatorCerts, out CryptographicAttributeObjectCollection unprotectedAttributes);

		public abstract byte[] EncodeOctetString(byte[] octets);

		public abstract byte[] DecodeOctetString(byte[] encodedOctets);

		public abstract byte[] EncodeUtcTime(DateTime utcTime);

		public abstract DateTime DecodeUtcTime(byte[] encodedUtcTime);

		public abstract string DecodeOid(byte[] encodedOid);

		public abstract Oid GetEncodedMessageType(byte[] encodedMessage);

		public abstract void AddCertsFromStoreForDecryption(X509Certificate2Collection certs);

		public abstract Exception CreateRecipientsNotFoundException();

		public abstract Exception CreateRecipientInfosAfterEncryptException();

		public abstract Exception CreateDecryptAfterEncryptException();

		public abstract Exception CreateDecryptTwiceException();

		public abstract byte[] GetSubjectKeyIdentifier(X509Certificate2 certificate);

		public abstract T GetPrivateKeyForSigning<T>(X509Certificate2 certificate, bool silent) where T : AsymmetricAlgorithm;

		public abstract T GetPrivateKeyForDecryption<T>(X509Certificate2 certificate, bool silent) where T : AsymmetricAlgorithm;

		public static PkcsPal Instance
		{
			get
			{
				return PkcsPal.s_instance;
			}
		}

		private static readonly PkcsPal s_instance = new ManagedPkcsPal();
	}
}
