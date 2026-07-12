using System;
using System.Security.Cryptography.X509Certificates;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class Pkcs12CertBag : Pkcs12SafeBag
	{
		public ReadOnlyMemory<byte> EncodedCertificate
		{
			get
			{
				throw new PlatformNotSupportedException();
			}
		}

		public bool IsX509Certificate
		{
			get
			{
				throw new PlatformNotSupportedException();
			}
		}

		public Pkcs12CertBag(Oid certificateType, ReadOnlyMemory<byte> encodedCertificate)
			: base(null, default(ReadOnlyMemory<byte>), false)
		{
			throw new PlatformNotSupportedException();
		}

		public X509Certificate2 GetCertificate()
		{
			throw new PlatformNotSupportedException();
		}

		public Oid GetCertificateType()
		{
			throw new PlatformNotSupportedException();
		}
	}
}
