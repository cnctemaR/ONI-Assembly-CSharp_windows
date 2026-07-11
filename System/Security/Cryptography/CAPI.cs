using System;

namespace System.Security.Cryptography
{
	internal static class CAPI
	{
		public static string CryptFindOIDInfoNameFromKey(string key, OidGroup oidGroup)
		{
			uint num = global::<PrivateImplementationDetails>.ComputeStringHash(key);
			if (num <= 2236143101U)
			{
				if (num <= 668701924U)
				{
					if (num <= 587661398U)
					{
						if (num <= 510574318U)
						{
							if (num != 489691311U)
							{
								if (num != 510574318U)
								{
									goto IL_0612;
								}
								if (!(key == "1.2.840.10040.4.3"))
								{
									goto IL_0612;
								}
								goto IL_0564;
							}
							else
							{
								if (!(key == "1.2.840.10045.4.1"))
								{
									goto IL_0612;
								}
								return "sha1ECDSA";
							}
						}
						else if (num != 523246549U)
						{
							if (num != 587661398U)
							{
								goto IL_0612;
							}
							if (!(key == "1.2.840.10045.4.3.3"))
							{
								goto IL_0612;
							}
							return "sha384ECDSA";
						}
						else
						{
							if (!(key == "1.2.840.10045.4.3"))
							{
								goto IL_0612;
							}
							return "specifiedECDSA";
						}
					}
					else if (num <= 604439017U)
					{
						if (num != 601591448U)
						{
							if (num != 604439017U)
							{
								goto IL_0612;
							}
							if (!(key == "1.2.840.10045.4.3.2"))
							{
								goto IL_0612;
							}
							return "sha256ECDSA";
						}
						else if (!(key == "1.2.840.113549.1.1.5"))
						{
							goto IL_0612;
						}
					}
					else if (num != 618369067U)
					{
						if (num != 637994255U)
						{
							if (num != 668701924U)
							{
								goto IL_0612;
							}
							if (!(key == "1.2.840.113549.1.1.1"))
							{
								goto IL_0612;
							}
							return "RSA";
						}
						else
						{
							if (!(key == "1.2.840.10045.4.3.4"))
							{
								goto IL_0612;
							}
							return "sha512ECDSA";
						}
					}
					else
					{
						if (!(key == "1.2.840.113549.1.1.4"))
						{
							goto IL_0612;
						}
						goto IL_055E;
					}
				}
				else if (num <= 858759237U)
				{
					if (num <= 719034781U)
					{
						if (num != 702257162U)
						{
							if (num != 719034781U)
							{
								goto IL_0612;
							}
							if (!(key == "1.2.840.113549.1.1.2"))
							{
								goto IL_0612;
							}
							goto IL_056A;
						}
						else
						{
							if (!(key == "1.2.840.113549.1.1.3"))
							{
								goto IL_0612;
							}
							return "md4RSA";
						}
					}
					else if (num != 841981618U)
					{
						if (num != 858759237U)
						{
							goto IL_0612;
						}
						if (!(key == "1.3.14.3.2.26"))
						{
							goto IL_0612;
						}
						return "sha1";
					}
					else
					{
						if (!(key == "1.3.14.3.2.27"))
						{
							goto IL_0612;
						}
						return "dsaSHA1";
					}
				}
				else if (num <= 1746622805U)
				{
					if (num != 875536856U)
					{
						if (num != 1746622805U)
						{
							goto IL_0612;
						}
						if (!(key == "2.16.840.1.101.2.1.1.19"))
						{
							goto IL_0612;
						}
						return "mosaicUpdatedSig";
					}
					else if (!(key == "1.3.14.3.2.29"))
					{
						goto IL_0612;
					}
				}
				else if (num != 1775291187U)
				{
					if (num != 2095896238U)
					{
						if (num != 2236143101U)
						{
							goto IL_0612;
						}
						if (!(key == "1.2.840.113549.3.7"))
						{
							goto IL_0612;
						}
						return "3des";
					}
					else
					{
						if (!(key == "1.2.840.113549.1.7.1"))
						{
							goto IL_0612;
						}
						return "PKCS 7 Data";
					}
				}
				else
				{
					if (!(key == "2.16.840.1.101.3.4.1.2"))
					{
						goto IL_0612;
					}
					return "aes128";
				}
			}
			else if (num <= 2940934855U)
			{
				if (num <= 2511031925U)
				{
					if (num <= 2477476687U)
					{
						if (num != 2460699068U)
						{
							if (num != 2477476687U)
							{
								goto IL_0612;
							}
							if (!(key == "1.2.840.113549.1.1.11"))
							{
								goto IL_0612;
							}
							return "sha256RSA";
						}
						else
						{
							if (!(key == "1.2.840.113549.1.1.10"))
							{
								goto IL_0612;
							}
							return "RSASSA-PSS";
						}
					}
					else if (num != 2494254306U)
					{
						if (num != 2511031925U)
						{
							goto IL_0612;
						}
						if (!(key == "1.2.840.113549.1.1.13"))
						{
							goto IL_0612;
						}
						return "sha512RSA";
					}
					else
					{
						if (!(key == "1.2.840.113549.1.1.12"))
						{
							goto IL_0612;
						}
						return "sha384RSA";
					}
				}
				else if (num <= 2629740981U)
				{
					if (num != 2524208222U)
					{
						if (num != 2629740981U)
						{
							goto IL_0612;
						}
						if (!(key == "2.16.840.1.101.3.4.1.42"))
						{
							goto IL_0612;
						}
						return "aes256";
					}
					else
					{
						if (!(key == "1.3.14.3.2.3"))
						{
							goto IL_0612;
						}
						goto IL_055E;
					}
				}
				else if (num != 2883603088U)
				{
					if (num != 2900380707U)
					{
						if (num != 2940934855U)
						{
							goto IL_0612;
						}
						if (!(key == "1.3.14.7.2.3.1"))
						{
							goto IL_0612;
						}
						goto IL_056A;
					}
					else
					{
						if (!(key == "1.2.840.113549.1.9.4"))
						{
							goto IL_0612;
						}
						return "Message Digest";
					}
				}
				else
				{
					if (!(key == "1.2.840.113549.1.9.5"))
					{
						goto IL_0612;
					}
					return "Signing Time";
				}
			}
			else if (num <= 3507107587U)
			{
				if (num <= 3190303825U)
				{
					if (num != 2984268802U)
					{
						if (num != 3190303825U)
						{
							goto IL_0612;
						}
						if (!(key == "1.3.14.3.2.13"))
						{
							goto IL_0612;
						}
						goto IL_0564;
					}
					else
					{
						if (!(key == "1.2.840.113549.1.9.3"))
						{
							goto IL_0612;
						}
						return "Content Type";
					}
				}
				else if (num != 3223859063U)
				{
					if (num != 3507107587U)
					{
						goto IL_0612;
					}
					if (!(key == "2.5.29.17"))
					{
						goto IL_0612;
					}
					return "Subject Alternative Name";
				}
				else if (!(key == "1.3.14.3.2.15"))
				{
					goto IL_0612;
				}
			}
			else if (num <= 3700541871U)
			{
				if (num != 3691389006U)
				{
					if (num != 3700541871U)
					{
						goto IL_0612;
					}
					if (!(key == "2.16.840.1.101.3.4.2.1"))
					{
						goto IL_0612;
					}
					return "sha256";
				}
				else
				{
					if (!(key == "2.16.840.1.113730.1.1"))
					{
						goto IL_0612;
					}
					return "Netscape Cert Type";
				}
			}
			else if (num != 3717319490U)
			{
				if (num != 3734097109U)
				{
					if (num != 3760531456U)
					{
						goto IL_0612;
					}
					if (!(key == "1.2.840.113549.2.5"))
					{
						goto IL_0612;
					}
					return "md5";
				}
				else
				{
					if (!(key == "2.16.840.1.101.3.4.2.3"))
					{
						goto IL_0612;
					}
					return "sha512";
				}
			}
			else
			{
				if (!(key == "2.16.840.1.101.3.4.2.2"))
				{
					goto IL_0612;
				}
				return "sha384";
			}
			return "sha1RSA";
			IL_055E:
			return "md5RSA";
			IL_0564:
			return "sha1DSA";
			IL_056A:
			return "md2RSA";
			IL_0612:
			return null;
		}

		public static string CryptFindOIDInfoKeyFromName(string name, OidGroup oidGroup)
		{
			uint num = global::<PrivateImplementationDetails>.ComputeStringHash(name);
			if (num <= 2469887870U)
			{
				if (num <= 1500038902U)
				{
					if (num <= 611118286U)
					{
						if (num <= 294650258U)
						{
							if (num != 149012944U)
							{
								if (num == 294650258U)
								{
									if (name == "3des")
									{
										return "1.2.840.113549.3.7";
									}
								}
							}
							else if (name == "sha256RSA")
							{
								return "1.2.840.113549.1.1.11";
							}
						}
						else if (num != 561569601U)
						{
							if (num == 611118286U)
							{
								if (name == "sha256ECDSA")
								{
									return "1.2.840.10045.4.3.2";
								}
							}
						}
						else if (name == "shaRSA")
						{
							return "1.3.14.3.2.29";
						}
					}
					else if (num <= 1066307707U)
					{
						if (num != 738683793U)
						{
							if (num == 1066307707U)
							{
								if (name == "sha512RSA")
								{
									return "1.2.840.113549.1.1.13";
								}
							}
						}
						else if (name == "sha512ECDSA")
						{
							return "1.2.840.10045.4.3.4";
						}
					}
					else if (num != 1440264225U)
					{
						if (num == 1500038902U)
						{
							if (name == "sha1RSA")
							{
								return "1.2.840.113549.1.1.5";
							}
						}
					}
					else if (name == "PKCS 7 Data")
					{
						return "1.2.840.113549.1.7.1";
					}
				}
				else if (num <= 2070555668U)
				{
					if (num <= 1698200840U)
					{
						if (num != 1597341437U)
						{
							if (num == 1698200840U)
							{
								if (name == "sha1DSA")
								{
									return "1.2.840.10040.4.3";
								}
							}
						}
						else if (name == "Signing Time")
						{
							return "1.2.840.113549.1.9.5";
						}
					}
					else if (num != 1813874674U)
					{
						if (num == 2070555668U)
						{
							if (name == "sha1")
							{
								return "1.3.14.3.2.26";
							}
						}
					}
					else if (name == "sha384ECDSA")
					{
						return "1.2.840.10045.4.3.3";
					}
				}
				else if (num <= 2226558121U)
				{
					if (num != 2158592553U)
					{
						if (num == 2226558121U)
						{
							if (name == "RSA")
							{
								return "1.2.840.113549.1.1.1";
							}
						}
					}
					else if (name == "aes128")
					{
						return "2.16.840.1.101.3.4.1.2";
					}
				}
				else if (num != 2393554675U)
				{
					if (num == 2469887870U)
					{
						if (name == "dsaSHA1")
						{
							return "1.3.14.3.2.27";
						}
					}
				}
				else if (name == "md5")
				{
					return "1.2.840.113549.2.5";
				}
			}
			else if (num <= 3014620826U)
			{
				if (num <= 2631153146U)
				{
					if (num <= 2503680566U)
					{
						if (num != 2493627064U)
						{
							if (num == 2503680566U)
							{
								if (name == "Content Type")
								{
									return "1.2.840.113549.1.9.3";
								}
							}
						}
						else if (name == "md2RSA")
						{
							return "1.2.840.113549.1.1.2";
						}
					}
					else if (num != 2505692187U)
					{
						if (num == 2631153146U)
						{
							if (name == "sha256")
							{
								return "2.16.840.1.101.3.4.2.1";
							}
						}
					}
					else if (name == "specifiedECDSA")
					{
						return "1.2.840.10045.4.3";
					}
				}
				else if (num <= 2694049387U)
				{
					if (num != 2647597108U)
					{
						if (num == 2694049387U)
						{
							if (name == "sha512")
							{
								return "2.16.840.1.101.3.4.2.3";
							}
						}
					}
					else if (name == "Netscape Cert Type")
					{
						return "2.16.840.1.113730.1.1";
					}
				}
				else if (num != 2700614742U)
				{
					if (num == 3014620826U)
					{
						if (name == "md4RSA")
						{
							return "1.2.840.113549.1.1.3";
						}
					}
				}
				else if (name == "sha384")
				{
					return "2.16.840.1.101.3.4.2.2";
				}
			}
			else if (num <= 3621965885U)
			{
				if (num <= 3315115577U)
				{
					if (num != 3023050339U)
					{
						if (num == 3315115577U)
						{
							if (name == "mosaicUpdatedSig")
							{
								return "2.16.840.1.101.2.1.1.19";
							}
						}
					}
					else if (name == "Subject Alternative Name")
					{
						return "2.5.29.17";
					}
				}
				else if (num != 3434299060U)
				{
					if (num == 3621965885U)
					{
						if (name == "RSASSA-PSS")
						{
							return "1.2.840.113549.1.1.10";
						}
					}
				}
				else if (name == "Message Digest")
				{
					return "1.2.840.113549.1.9.4";
				}
			}
			else if (num <= 3760740595U)
			{
				if (num != 3631158884U)
				{
					if (num == 3760740595U)
					{
						if (name == "md5RSA")
						{
							return "1.2.840.113549.1.1.4";
						}
					}
				}
				else if (name == "sha384RSA")
				{
					return "1.2.840.113549.1.1.12";
				}
			}
			else if (num != 3900422261U)
			{
				if (num == 4027207524U)
				{
					if (name == "sha1ECDSA")
					{
						return "1.2.840.10045.4.1";
					}
				}
			}
			else if (name == "aes256")
			{
				return "2.16.840.1.101.3.4.1.42";
			}
			return null;
		}

		internal const uint CRYPT_OID_INFO_OID_KEY = 1U;

		internal const uint CRYPT_OID_INFO_NAME_KEY = 2U;

		internal const uint CRYPT_OID_INFO_ALGID_KEY = 3U;

		internal const uint CRYPT_OID_INFO_SIGN_KEY = 4U;
	}
}
