using System;

namespace Mono.Security.Protocol.Tls
{
	internal static class CipherSuiteFactory
	{
		public static CipherSuiteCollection GetSupportedCiphers(bool server, SecurityProtocolType protocol)
		{
			if (protocol <= SecurityProtocolType.Ssl2)
			{
				if (protocol != SecurityProtocolType.Default)
				{
					if (protocol != SecurityProtocolType.Ssl2)
					{
						goto IL_002D;
					}
					goto IL_002D;
				}
			}
			else
			{
				if (protocol == SecurityProtocolType.Ssl3)
				{
					return CipherSuiteFactory.GetSsl3SupportedCiphers();
				}
				if (protocol != SecurityProtocolType.Tls)
				{
					goto IL_002D;
				}
			}
			return CipherSuiteFactory.GetTls1SupportedCiphers();
			IL_002D:
			throw new NotSupportedException("Unsupported security protocol type");
		}

		private static CipherSuiteCollection GetTls1SupportedCiphers()
		{
			return new CipherSuiteCollection(SecurityProtocolType.Tls)
			{
				{
					53,
					"TLS_RSA_WITH_AES_256_CBC_SHA",
					CipherAlgorithmType.Rijndael,
					HashAlgorithmType.Sha1,
					ExchangeAlgorithmType.RsaKeyX,
					false,
					true,
					32,
					32,
					256,
					16,
					16
				},
				{
					47,
					"TLS_RSA_WITH_AES_128_CBC_SHA",
					CipherAlgorithmType.Rijndael,
					HashAlgorithmType.Sha1,
					ExchangeAlgorithmType.RsaKeyX,
					false,
					true,
					16,
					16,
					128,
					16,
					16
				},
				{
					10,
					"TLS_RSA_WITH_3DES_EDE_CBC_SHA",
					CipherAlgorithmType.TripleDes,
					HashAlgorithmType.Sha1,
					ExchangeAlgorithmType.RsaKeyX,
					false,
					true,
					24,
					24,
					168,
					8,
					8
				},
				{
					5,
					"TLS_RSA_WITH_RC4_128_SHA",
					CipherAlgorithmType.Rc4,
					HashAlgorithmType.Sha1,
					ExchangeAlgorithmType.RsaKeyX,
					false,
					false,
					16,
					16,
					128,
					0,
					0
				},
				{
					4,
					"TLS_RSA_WITH_RC4_128_MD5",
					CipherAlgorithmType.Rc4,
					HashAlgorithmType.Md5,
					ExchangeAlgorithmType.RsaKeyX,
					false,
					false,
					16,
					16,
					128,
					0,
					0
				},
				{
					9,
					"TLS_RSA_WITH_DES_CBC_SHA",
					CipherAlgorithmType.Des,
					HashAlgorithmType.Sha1,
					ExchangeAlgorithmType.RsaKeyX,
					false,
					true,
					8,
					8,
					56,
					8,
					8
				}
			};
		}

		private static CipherSuiteCollection GetSsl3SupportedCiphers()
		{
			return new CipherSuiteCollection(SecurityProtocolType.Ssl3)
			{
				{
					53,
					"SSL_RSA_WITH_AES_256_CBC_SHA",
					CipherAlgorithmType.Rijndael,
					HashAlgorithmType.Sha1,
					ExchangeAlgorithmType.RsaKeyX,
					false,
					true,
					32,
					32,
					256,
					16,
					16
				},
				{
					47,
					"SSL_RSA_WITH_AES_128_CBC_SHA",
					CipherAlgorithmType.Rijndael,
					HashAlgorithmType.Sha1,
					ExchangeAlgorithmType.RsaKeyX,
					false,
					true,
					16,
					16,
					128,
					16,
					16
				},
				{
					10,
					"SSL_RSA_WITH_3DES_EDE_CBC_SHA",
					CipherAlgorithmType.TripleDes,
					HashAlgorithmType.Sha1,
					ExchangeAlgorithmType.RsaKeyX,
					false,
					true,
					24,
					24,
					168,
					8,
					8
				},
				{
					5,
					"SSL_RSA_WITH_RC4_128_SHA",
					CipherAlgorithmType.Rc4,
					HashAlgorithmType.Sha1,
					ExchangeAlgorithmType.RsaKeyX,
					false,
					false,
					16,
					16,
					128,
					0,
					0
				},
				{
					4,
					"SSL_RSA_WITH_RC4_128_MD5",
					CipherAlgorithmType.Rc4,
					HashAlgorithmType.Md5,
					ExchangeAlgorithmType.RsaKeyX,
					false,
					false,
					16,
					16,
					128,
					0,
					0
				},
				{
					9,
					"SSL_RSA_WITH_DES_CBC_SHA",
					CipherAlgorithmType.Des,
					HashAlgorithmType.Sha1,
					ExchangeAlgorithmType.RsaKeyX,
					false,
					true,
					8,
					8,
					56,
					8,
					8
				}
			};
		}
	}
}
