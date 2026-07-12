using System;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;

namespace Internal.Cryptography
{
	internal abstract class DecryptorPal : IDisposable
	{
		internal DecryptorPal(RecipientInfoCollection recipientInfos)
		{
			this.RecipientInfos = recipientInfos;
		}

		public RecipientInfoCollection RecipientInfos { get; }

		public abstract ContentInfo TryDecrypt(RecipientInfo recipientInfo, X509Certificate2 cert, X509Certificate2Collection originatorCerts, X509Certificate2Collection extraStore, out Exception exception);

		public abstract void Dispose();
	}
}
