using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class CryptoConfig
	{
		public static void AddOID(string oid, params string[] names)
		{
			throw new PlatformNotSupportedException();
		}

		public static object CreateFromName(string name)
		{
			return CryptoConfig.CreateFromName(name, null);
		}

		[PreserveDependency(".ctor()", "System.Security.Cryptography.X509Certificates.X509SubjectKeyIdentifierExtension", "System")]
		[PreserveDependency(".ctor()", "System.Security.Cryptography.X509Certificates.X509EnhancedKeyUsageExtension", "System")]
		[PreserveDependency(".ctor()", "System.Security.Cryptography.X509Certificates.X509BasicConstraintsExtension", "System")]
		[PreserveDependency(".ctor()", "System.Security.Cryptography.X509Certificates.X509KeyUsageExtension", "System")]
		[PreserveDependency(".ctor()", "System.Security.Cryptography.X509Certificates.X509Chain", "System")]
		[PreserveDependency(".ctor()", "System.Security.Cryptography.AesCryptoServiceProvider", "System.Core")]
		public static object CreateFromName(string name, params object[] args)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			Type type = null;
			string text = name.ToLowerInvariant();
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			if (num <= 2442086578U)
			{
				if (num <= 1318943838U)
				{
					if (num <= 589079309U)
					{
						if (num <= 401798778U)
						{
							if (num <= 294650258U)
							{
								if (num != 97835172U)
								{
									if (num != 289646596U)
									{
										if (num != 294650258U)
										{
											goto IL_0FA9;
										}
										if (!(text == "3des"))
										{
											goto IL_0FA9;
										}
										goto IL_0F57;
									}
									else
									{
										if (!(text == "system.security.cryptography.sha1cryptoserviceprovider"))
										{
											goto IL_0FA9;
										}
										goto IL_0F39;
									}
								}
								else
								{
									if (!(text == "http://www.w3.org/2001/04/xmldsig-more#sha384"))
									{
										goto IL_0FA9;
									}
									goto IL_0F4B;
								}
							}
							else if (num != 373238979U)
							{
								if (num != 381964475U)
								{
									if (num != 401798778U)
									{
										goto IL_0FA9;
									}
									if (!(text == "system.security.cryptography.dsasignaturedescription"))
									{
										goto IL_0FA9;
									}
								}
								else
								{
									if (!(text == "hmacsha1"))
									{
										goto IL_0FA9;
									}
									goto IL_0ED3;
								}
							}
							else
							{
								if (!(text == "system.security.cryptography.sha1cng"))
								{
									goto IL_0FA9;
								}
								goto IL_0F39;
							}
						}
						else if (num <= 550229268U)
						{
							if (num != 418711287U)
							{
								if (num != 524624695U)
								{
									if (num != 550229268U)
									{
										goto IL_0FA9;
									}
									if (!(text == "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256"))
									{
										goto IL_0FA9;
									}
									goto IL_0ED9;
								}
								else
								{
									if (!(text == "system.security.cryptography.hashalgorithm"))
									{
										goto IL_0FA9;
									}
									goto IL_0F39;
								}
							}
							else
							{
								if (!(text == "system.security.cryptography.mactripledes"))
								{
									goto IL_0FA9;
								}
								goto IL_0EEB;
							}
						}
						else if (num != 572088812U)
						{
							if (num != 585245684U)
							{
								if (num != 589079309U)
								{
									goto IL_0FA9;
								}
								if (!(text == "system.security.cryptography.hmac"))
								{
									goto IL_0FA9;
								}
								goto IL_0ED3;
							}
							else
							{
								if (!(text == "system.security.cryptography.sha1managed"))
								{
									goto IL_0FA9;
								}
								return new SHA1Managed();
							}
						}
						else
						{
							if (!(text == "http://www.w3.org/2001/04/xmldsig-more#hmac-sha384"))
							{
								goto IL_0FA9;
							}
							goto IL_0EDF;
						}
					}
					else if (num <= 900295094U)
					{
						if (num <= 734683829U)
						{
							if (num != 699966473U)
							{
								if (num != 708523592U)
								{
									if (num != 734683829U)
									{
										goto IL_0FA9;
									}
									if (!(text == "hmacsha256"))
									{
										goto IL_0FA9;
									}
									goto IL_0ED9;
								}
								else
								{
									if (!(text == "aes"))
									{
										goto IL_0FA9;
									}
									type = Type.GetType("System.Security.Cryptography.AesCryptoServiceProvider, System.Core");
									goto IL_0FA9;
								}
							}
							else if (!(text == "http://www.w3.org/2000/09/xmldsig#dsa-sha1"))
							{
								goto IL_0FA9;
							}
						}
						else if (num != 853553133U)
						{
							if (num != 877368883U)
							{
								if (num != 900295094U)
								{
									goto IL_0FA9;
								}
								if (!(text == "system.security.cryptography.dsasignatureformatter"))
								{
									goto IL_0FA9;
								}
								return new DSASignatureFormatter();
							}
							else
							{
								if (!(text == "http://www.w3.org/2000/09/xmldsig#rsa-sha1"))
								{
									goto IL_0FA9;
								}
								goto IL_0F21;
							}
						}
						else
						{
							if (!(text == "system.security.cryptography.rsapkcs1sha384signaturedescription"))
							{
								goto IL_0FA9;
							}
							goto IL_0F2D;
						}
					}
					else if (num <= 1104969097U)
					{
						if (num != 965923590U)
						{
							if (num != 999454301U)
							{
								if (num != 1104969097U)
								{
									goto IL_0FA9;
								}
								if (!(text == "rsa"))
								{
									goto IL_0FA9;
								}
								goto IL_0F0F;
							}
							else
							{
								if (!(text == "system.security.cryptography.rsapkcs1sha256signaturedescription"))
								{
									goto IL_0FA9;
								}
								goto IL_0F27;
							}
						}
						else
						{
							if (!(text == "http://www.w3.org/2000/09/xmldsig#sha1"))
							{
								goto IL_0FA9;
							}
							goto IL_0F39;
						}
					}
					else if (num <= 1168228931U)
					{
						if (num != 1147401626U)
						{
							if (num != 1168228931U)
							{
								goto IL_0FA9;
							}
							if (!(text == "system.security.cryptography.ripemd160managed"))
							{
								goto IL_0FA9;
							}
							goto IL_0F03;
						}
						else
						{
							if (!(text == "system.security.cryptography.rsapkcs1signatureformatter"))
							{
								goto IL_0FA9;
							}
							return new RSAPKCS1SignatureFormatter();
						}
					}
					else if (num != 1279198866U)
					{
						if (num != 1318943838U)
						{
							goto IL_0FA9;
						}
						if (!(text == "http://www.w3.org/2000/09/xmldsig#hmac-sha1"))
						{
							goto IL_0FA9;
						}
						goto IL_0ED3;
					}
					else
					{
						if (!(text == "system.security.cryptography.tripledes"))
						{
							goto IL_0FA9;
						}
						goto IL_0F57;
					}
					return new DSASignatureDescription();
				}
				if (num <= 1862521808U)
				{
					if (num <= 1664836558U)
					{
						if (num <= 1604759256U)
						{
							if (num != 1495151835U)
							{
								if (num != 1600607069U)
								{
									if (num != 1604759256U)
									{
										goto IL_0FA9;
									}
									if (!(text == "sha-512"))
									{
										goto IL_0FA9;
									}
									goto IL_0F51;
								}
								else
								{
									if (!(text == "system.security.cryptography.sha384cng"))
									{
										goto IL_0FA9;
									}
									goto IL_0F4B;
								}
							}
							else
							{
								if (!(text == "system.security.cryptography.descryptoserviceprovider"))
								{
									goto IL_0FA9;
								}
								goto IL_0EC1;
							}
						}
						else if (num != 1610008198U)
						{
							if (num != 1629735498U)
							{
								if (num != 1664836558U)
								{
									goto IL_0FA9;
								}
								if (!(text == "system.security.cryptography.sha256cryptoserviceprovider"))
								{
									goto IL_0FA9;
								}
								goto IL_0F45;
							}
							else
							{
								if (!(text == "system.security.cryptography.rc2cryptoserviceprovider"))
								{
									goto IL_0FA9;
								}
								goto IL_0EF7;
							}
						}
						else
						{
							if (!(text == "system.security.cryptography.rijndaelmanaged"))
							{
								goto IL_0FA9;
							}
							goto IL_0EFD;
						}
					}
					else if (num <= 1720406050U)
					{
						if (num != 1686995390U)
						{
							if (num != 1688024611U)
							{
								if (num != 1720406050U)
								{
									goto IL_0FA9;
								}
								if (!(text == "x509chain"))
								{
									goto IL_0FA9;
								}
								type = Type.GetType("System.Security.Cryptography.X509Certificates.X509Chain, System");
								goto IL_0FA9;
							}
							else
							{
								if (!(text == "http://www.w3.org/2001/04/xmlenc#sha256"))
								{
									goto IL_0FA9;
								}
								goto IL_0F45;
							}
						}
						else if (!(text == "system.security.cryptography.md5"))
						{
							goto IL_0FA9;
						}
					}
					else if (num != 1778503857U)
					{
						if (num != 1820576144U)
						{
							if (num != 1862521808U)
							{
								goto IL_0FA9;
							}
							if (!(text == "rc2"))
							{
								goto IL_0FA9;
							}
							goto IL_0EF7;
						}
						else
						{
							if (!(text == "hmacsha512"))
							{
								goto IL_0FA9;
							}
							goto IL_0EE5;
						}
					}
					else
					{
						if (!(text == "system.security.cryptography.randomnumbergenerator"))
						{
							goto IL_0FA9;
						}
						goto IL_0F09;
					}
				}
				else if (num <= 2246759783U)
				{
					if (num <= 2120664437U)
					{
						if (num != 2070555668U)
						{
							if (num != 2102584679U)
							{
								if (num != 2120664437U)
								{
									goto IL_0FA9;
								}
								if (!(text == "sha-384"))
								{
									goto IL_0FA9;
								}
								goto IL_0F4B;
							}
							else
							{
								if (!(text == "ripemd160"))
								{
									goto IL_0FA9;
								}
								goto IL_0F03;
							}
						}
						else
						{
							if (!(text == "sha1"))
							{
								goto IL_0FA9;
							}
							goto IL_0F39;
						}
					}
					else if (num != 2131651891U)
					{
						if (num != 2214607313U)
						{
							if (num != 2246759783U)
							{
								goto IL_0FA9;
							}
							if (!(text == "system.security.cryptography.dsasignaturedeformatter"))
							{
								goto IL_0FA9;
							}
							return new DSASignatureDeformatter();
						}
						else
						{
							if (!(text == "system.security.cryptography.sha512cryptoserviceprovider"))
							{
								goto IL_0FA9;
							}
							goto IL_0F51;
						}
					}
					else
					{
						if (!(text == "tripledes"))
						{
							goto IL_0FA9;
						}
						goto IL_0F57;
					}
				}
				else if (num <= 2346491937U)
				{
					if (num != 2269936011U)
					{
						if (num != 2340547105U)
						{
							if (num != 2346491937U)
							{
								goto IL_0FA9;
							}
							if (!(text == "system.security.cryptography.sha256"))
							{
								goto IL_0FA9;
							}
							goto IL_0F45;
						}
						else
						{
							if (!(text == "system.security.cryptography.keyedhashalgorithm"))
							{
								goto IL_0FA9;
							}
							goto IL_0ED3;
						}
					}
					else if (!(text == "system.security.cryptography.md5cryptoserviceprovider"))
					{
						goto IL_0FA9;
					}
				}
				else if (num <= 2394616414U)
				{
					if (num != 2393554675U)
					{
						if (num != 2394616414U)
						{
							goto IL_0FA9;
						}
						if (!(text == "system.security.cryptography.sha384cryptoserviceprovider"))
						{
							goto IL_0FA9;
						}
						goto IL_0F4B;
					}
					else if (!(text == "md5"))
					{
						goto IL_0FA9;
					}
				}
				else if (num != 2415328530U)
				{
					if (num != 2442086578U)
					{
						goto IL_0FA9;
					}
					if (!(text == "hmacripemd160"))
					{
						goto IL_0FA9;
					}
					goto IL_0ECD;
				}
				else
				{
					if (!(text == "system.security.cryptography.hmacsha256"))
					{
						goto IL_0FA9;
					}
					goto IL_0ED9;
				}
				return new MD5CryptoServiceProvider();
				IL_0ED9:
				return new HMACSHA256();
			}
			if (num <= 3339968437U)
			{
				if (num <= 2920802226U)
				{
					if (num <= 2685283101U)
					{
						if (num <= 2484875538U)
						{
							if (num != 2451404838U)
							{
								if (num != 2478224771U)
								{
									if (num != 2484875538U)
									{
										goto IL_0FA9;
									}
									if (!(text == "system.security.cryptography.sha256managed"))
									{
										goto IL_0FA9;
									}
									goto IL_0F45;
								}
								else
								{
									if (!(text == "system.security.cryptography.hmacsha512"))
									{
										goto IL_0FA9;
									}
									goto IL_0EE5;
								}
							}
							else
							{
								if (!(text == "system.security.cryptography.ripemd160"))
								{
									goto IL_0FA9;
								}
								goto IL_0F03;
							}
						}
						else if (num != 2631153146U)
						{
							if (num != 2661179293U)
							{
								if (num != 2685283101U)
								{
									goto IL_0FA9;
								}
								if (!(text == "system.security.cryptography.rc2"))
								{
									goto IL_0FA9;
								}
								goto IL_0EF7;
							}
							else
							{
								if (!(text == "sha-256"))
								{
									goto IL_0FA9;
								}
								goto IL_0F45;
							}
						}
						else
						{
							if (!(text == "sha256"))
							{
								goto IL_0FA9;
							}
							goto IL_0F45;
						}
					}
					else if (num <= 2803157229U)
					{
						if (num != 2694049387U)
						{
							if (num != 2700614742U)
							{
								if (num != 2803157229U)
								{
									goto IL_0FA9;
								}
								if (!(text == "system.security.cryptography.sha256cng"))
								{
									goto IL_0FA9;
								}
								goto IL_0F45;
							}
							else
							{
								if (!(text == "sha384"))
								{
									goto IL_0FA9;
								}
								goto IL_0F4B;
							}
						}
						else
						{
							if (!(text == "sha512"))
							{
								goto IL_0FA9;
							}
							goto IL_0F51;
						}
					}
					else if (num != 2824063256U)
					{
						if (num != 2855136637U)
						{
							if (num != 2920802226U)
							{
								goto IL_0FA9;
							}
							if (!(text == "rijndael"))
							{
								goto IL_0FA9;
							}
							goto IL_0EFD;
						}
						else
						{
							if (!(text == "hmacsha384"))
							{
								goto IL_0FA9;
							}
							goto IL_0EDF;
						}
					}
					else if (!(text == "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512"))
					{
						goto IL_0FA9;
					}
				}
				else if (num <= 3106619289U)
				{
					if (num <= 3024233790U)
					{
						if (num != 2930374873U)
						{
							if (num != 2930817943U)
							{
								if (num != 3024233790U)
								{
									goto IL_0FA9;
								}
								if (!(text == "system.security.cryptography.hmacsha384"))
								{
									goto IL_0FA9;
								}
								goto IL_0EDF;
							}
							else
							{
								if (!(text == "system.security.cryptography.sha1"))
								{
									goto IL_0FA9;
								}
								goto IL_0F39;
							}
						}
						else
						{
							if (!(text == "http://www.w3.org/2001/04/xmldsig-more#hmac-sha512"))
							{
								goto IL_0FA9;
							}
							goto IL_0EE5;
						}
					}
					else if (num != 3071220272U)
					{
						if (num != 3091284687U)
						{
							if (num != 3106619289U)
							{
								goto IL_0FA9;
							}
							if (!(text == "http://www.w3.org/2001/04/xmldsig-more#hmac-ripemd160"))
							{
								goto IL_0FA9;
							}
							goto IL_0ECD;
						}
						else
						{
							if (!(text == "system.security.cryptography.rsapkcs1sha1signaturedescription"))
							{
								goto IL_0FA9;
							}
							goto IL_0F21;
						}
					}
					else if (!(text == "system.security.cryptography.rsapkcs1sha512signaturedescription"))
					{
						goto IL_0FA9;
					}
				}
				else if (num <= 3193284448U)
				{
					if (num != 3155186700U)
					{
						if (num != 3177008669U)
						{
							if (num != 3193284448U)
							{
								goto IL_0FA9;
							}
							if (!(text == "system.security.cryptography.rngcryptoserviceprovider"))
							{
								goto IL_0FA9;
							}
							goto IL_0F09;
						}
						else
						{
							if (!(text == "triple des"))
							{
								goto IL_0FA9;
							}
							goto IL_0F57;
						}
					}
					else
					{
						if (!(text == "system.security.cryptography.hmacsha1"))
						{
							goto IL_0FA9;
						}
						goto IL_0ED3;
					}
				}
				else if (num <= 3271835900U)
				{
					if (num != 3223542963U)
					{
						if (num != 3271835900U)
						{
							goto IL_0FA9;
						}
						if (!(text == "randomnumbergenerator"))
						{
							goto IL_0FA9;
						}
						goto IL_0F09;
					}
					else
					{
						if (!(text == "dsa"))
						{
							goto IL_0FA9;
						}
						goto IL_0EA9;
					}
				}
				else if (num != 3295766687U)
				{
					if (num != 3339968437U)
					{
						goto IL_0FA9;
					}
					if (!(text == "http://www.w3.org/2001/04/xmldsig-more#rsa-sha384"))
					{
						goto IL_0FA9;
					}
					goto IL_0F2D;
				}
				else
				{
					if (!(text == "system.security.cryptography.hmacripemd160"))
					{
						goto IL_0FA9;
					}
					goto IL_0ECD;
				}
				return new RSAPKCS1SHA512SignatureDescription();
			}
			if (num <= 3841402386U)
			{
				if (num <= 3529085699U)
				{
					if (num <= 3457114317U)
					{
						if (num != 3416134926U)
						{
							if (num != 3442835812U)
							{
								if (num != 3457114317U)
								{
									goto IL_0FA9;
								}
								if (!(text == "system.security.cryptography.sha512managed"))
								{
									goto IL_0FA9;
								}
								goto IL_0F51;
							}
							else
							{
								if (!(text == "system.security.cryptography.rsa"))
								{
									goto IL_0FA9;
								}
								goto IL_0F0F;
							}
						}
						else
						{
							if (!(text == "system.security.cryptography.des"))
							{
								goto IL_0FA9;
							}
							goto IL_0EC1;
						}
					}
					else if (num != 3506813397U)
					{
						if (num != 3523885206U)
						{
							if (num != 3529085699U)
							{
								goto IL_0FA9;
							}
							if (!(text == "des"))
							{
								goto IL_0FA9;
							}
							goto IL_0EC1;
						}
						else
						{
							if (!(text == "2.5.29.14"))
							{
								goto IL_0FA9;
							}
							type = Type.GetType("System.Security.Cryptography.X509Certificates.X509SubjectKeyIdentifierExtension, System");
							goto IL_0FA9;
						}
					}
					else
					{
						if (!(text == "2.5.29.37"))
						{
							goto IL_0FA9;
						}
						type = Type.GetType("System.Security.Cryptography.X509Certificates.X509EnhancedKeyUsageExtension, System");
						goto IL_0FA9;
					}
				}
				else if (num <= 3708241362U)
				{
					if (num != 3540662825U)
					{
						if (num != 3569803917U)
						{
							if (num != 3708241362U)
							{
								goto IL_0FA9;
							}
							if (!(text == "system.security.cryptography.sha512cng"))
							{
								goto IL_0FA9;
							}
							goto IL_0F51;
						}
						else
						{
							if (!(text == "system.security.cryptography.asymmetricalgorithm"))
							{
								goto IL_0FA9;
							}
							goto IL_0F0F;
						}
					}
					else
					{
						if (!(text == "2.5.29.15"))
						{
							goto IL_0FA9;
						}
						type = Type.GetType("System.Security.Cryptography.X509Certificates.X509KeyUsageExtension, System");
						goto IL_0FA9;
					}
				}
				else if (num <= 3741994253U)
				{
					if (num != 3711531707U)
					{
						if (num != 3741994253U)
						{
							goto IL_0FA9;
						}
						if (!(text == "2.5.29.19"))
						{
							goto IL_0FA9;
						}
						type = Type.GetType("System.Security.Cryptography.X509Certificates.X509BasicConstraintsExtension, System");
						goto IL_0FA9;
					}
					else if (!(text == "system.security.cryptography.hmacmd5"))
					{
						goto IL_0FA9;
					}
				}
				else if (num != 3772560434U)
				{
					if (num != 3841402386U)
					{
						goto IL_0FA9;
					}
					if (!(text == "ripemd-160"))
					{
						goto IL_0FA9;
					}
					goto IL_0F03;
				}
				else
				{
					if (!(text == "http://www.w3.org/2001/04/xmlenc#sha512"))
					{
						goto IL_0FA9;
					}
					goto IL_0F51;
				}
			}
			else if (num <= 4070407701U)
			{
				if (num <= 3979214893U)
				{
					if (num != 3849984186U)
					{
						if (num != 3880483293U)
						{
							if (num != 3979214893U)
							{
								goto IL_0FA9;
							}
							if (!(text == "sha"))
							{
								goto IL_0FA9;
							}
							goto IL_0F39;
						}
						else
						{
							if (!(text == "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256"))
							{
								goto IL_0FA9;
							}
							goto IL_0F27;
						}
					}
					else
					{
						if (!(text == "system.security.cryptography.dsa"))
						{
							goto IL_0FA9;
						}
						goto IL_0EA9;
					}
				}
				else if (num != 3991591796U)
				{
					if (num != 4003337351U)
					{
						if (num != 4070407701U)
						{
							goto IL_0FA9;
						}
						if (!(text == "system.security.cryptography.rijndael"))
						{
							goto IL_0FA9;
						}
						goto IL_0EFD;
					}
					else
					{
						if (!(text == "system.security.cryptography.dsacryptoserviceprovider"))
						{
							goto IL_0FA9;
						}
						goto IL_0EA9;
					}
				}
				else
				{
					if (!(text == "system.security.cryptography.sha512"))
					{
						goto IL_0FA9;
					}
					goto IL_0F51;
				}
			}
			else if (num <= 4109837759U)
			{
				if (num != 4092224578U)
				{
					if (num != 4106266752U)
					{
						if (num != 4109837759U)
						{
							goto IL_0FA9;
						}
						if (!(text == "system.security.cryptography.tripledescryptoserviceprovider"))
						{
							goto IL_0FA9;
						}
						goto IL_0F57;
					}
					else
					{
						if (!(text == "mactripledes"))
						{
							goto IL_0FA9;
						}
						goto IL_0EEB;
					}
				}
				else if (!(text == "hmacmd5"))
				{
					goto IL_0FA9;
				}
			}
			else if (num <= 4181060418U)
			{
				if (num != 4120168715U)
				{
					if (num != 4181060418U)
					{
						goto IL_0FA9;
					}
					if (!(text == "system.security.cryptography.sha384managed"))
					{
						goto IL_0FA9;
					}
					goto IL_0F4B;
				}
				else
				{
					if (!(text == "system.security.cryptography.rsapkcs1signaturedeformatter"))
					{
						goto IL_0FA9;
					}
					return new RSAPKCS1SignatureDeformatter();
				}
			}
			else if (num != 4199782769U)
			{
				if (num != 4265317454U)
				{
					goto IL_0FA9;
				}
				if (!(text == "system.security.cryptography.symmetricalgorithm"))
				{
					goto IL_0FA9;
				}
				goto IL_0EFD;
			}
			else
			{
				if (!(text == "system.security.cryptography.sha384"))
				{
					goto IL_0FA9;
				}
				goto IL_0F4B;
			}
			return new HMACMD5();
			IL_0EA9:
			return new DSACryptoServiceProvider();
			IL_0EC1:
			return new DESCryptoServiceProvider();
			IL_0ECD:
			return new HMACRIPEMD160();
			IL_0ED3:
			return new HMACSHA1();
			IL_0EDF:
			return new HMACSHA384();
			IL_0EE5:
			return new HMACSHA512();
			IL_0EEB:
			return new MACTripleDES();
			IL_0EF7:
			return new RC2CryptoServiceProvider();
			IL_0EFD:
			return new RijndaelManaged();
			IL_0F03:
			return new RIPEMD160Managed();
			IL_0F09:
			return new RNGCryptoServiceProvider();
			IL_0F0F:
			return new RSACryptoServiceProvider();
			IL_0F21:
			return new RSAPKCS1SHA1SignatureDescription();
			IL_0F27:
			return new RSAPKCS1SHA256SignatureDescription();
			IL_0F2D:
			return new RSAPKCS1SHA384SignatureDescription();
			IL_0F39:
			return new SHA1CryptoServiceProvider();
			IL_0F45:
			return new SHA256Managed();
			IL_0F4B:
			return new SHA384Managed();
			IL_0F51:
			return new SHA512Managed();
			IL_0F57:
			return new TripleDESCryptoServiceProvider();
			IL_0FA9:
			if (type == null)
			{
				object obj = CryptoConfig.lockObject;
				lock (obj)
				{
					Dictionary<string, Type> dictionary = CryptoConfig.algorithms;
					if (dictionary != null && dictionary.TryGetValue(name, out type))
					{
						try
						{
							return Activator.CreateInstance(type, args);
						}
						catch
						{
						}
					}
				}
				type = Type.GetType(name);
			}
			object obj2;
			try
			{
				obj2 = Activator.CreateInstance(type, args);
			}
			catch
			{
				obj2 = null;
			}
			return obj2;
		}

		internal static string MapNameToOID(string name, object arg)
		{
			return CryptoConfig.MapNameToOID(name);
		}

		public static string MapNameToOID(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			string text = name.ToLowerInvariant();
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			if (num <= 2393554675U)
			{
				if (num <= 1686995390U)
				{
					if (num <= 585245684U)
					{
						if (num != 289646596U)
						{
							if (num != 373238979U)
							{
								if (num != 585245684U)
								{
									goto IL_04A8;
								}
								if (!(text == "system.security.cryptography.sha1managed"))
								{
									goto IL_04A8;
								}
								goto IL_046C;
							}
							else
							{
								if (!(text == "system.security.cryptography.sha1cng"))
								{
									goto IL_04A8;
								}
								goto IL_046C;
							}
						}
						else
						{
							if (!(text == "system.security.cryptography.sha1cryptoserviceprovider"))
							{
								goto IL_04A8;
							}
							goto IL_046C;
						}
					}
					else if (num <= 1600607069U)
					{
						if (num != 1168228931U)
						{
							if (num != 1600607069U)
							{
								goto IL_04A8;
							}
							if (!(text == "system.security.cryptography.sha384cng"))
							{
								goto IL_04A8;
							}
							goto IL_047E;
						}
						else
						{
							if (!(text == "system.security.cryptography.ripemd160managed"))
							{
								goto IL_04A8;
							}
							goto IL_048A;
						}
					}
					else if (num != 1664836558U)
					{
						if (num != 1686995390U)
						{
							goto IL_04A8;
						}
						if (!(text == "system.security.cryptography.md5"))
						{
							goto IL_04A8;
						}
					}
					else
					{
						if (!(text == "system.security.cryptography.sha256cryptoserviceprovider"))
						{
							goto IL_04A8;
						}
						goto IL_0478;
					}
				}
				else if (num <= 2131651891U)
				{
					if (num <= 2070555668U)
					{
						if (num != 1862521808U)
						{
							if (num != 2070555668U)
							{
								goto IL_04A8;
							}
							if (!(text == "sha1"))
							{
								goto IL_04A8;
							}
							goto IL_046C;
						}
						else
						{
							if (!(text == "rc2"))
							{
								goto IL_04A8;
							}
							return "1.2.840.113549.3.2";
						}
					}
					else if (num != 2102584679U)
					{
						if (num != 2131651891U)
						{
							goto IL_04A8;
						}
						if (!(text == "tripledes"))
						{
							goto IL_04A8;
						}
						return "1.2.840.113549.3.7";
					}
					else
					{
						if (!(text == "ripemd160"))
						{
							goto IL_04A8;
						}
						goto IL_048A;
					}
				}
				else if (num <= 2269936011U)
				{
					if (num != 2214607313U)
					{
						if (num != 2269936011U)
						{
							goto IL_04A8;
						}
						if (!(text == "system.security.cryptography.md5cryptoserviceprovider"))
						{
							goto IL_04A8;
						}
					}
					else
					{
						if (!(text == "system.security.cryptography.sha512cryptoserviceprovider"))
						{
							goto IL_04A8;
						}
						goto IL_0484;
					}
				}
				else if (num != 2346491937U)
				{
					if (num != 2393554675U)
					{
						goto IL_04A8;
					}
					if (!(text == "md5"))
					{
						goto IL_04A8;
					}
				}
				else
				{
					if (!(text == "system.security.cryptography.sha256"))
					{
						goto IL_04A8;
					}
					goto IL_0478;
				}
				return "1.2.840.113549.2.5";
			}
			if (num <= 2700614742U)
			{
				if (num <= 2484875538U)
				{
					if (num != 2394616414U)
					{
						if (num != 2451404838U)
						{
							if (num != 2484875538U)
							{
								goto IL_04A8;
							}
							if (!(text == "system.security.cryptography.sha256managed"))
							{
								goto IL_04A8;
							}
							goto IL_0478;
						}
						else
						{
							if (!(text == "system.security.cryptography.ripemd160"))
							{
								goto IL_04A8;
							}
							goto IL_048A;
						}
					}
					else
					{
						if (!(text == "system.security.cryptography.sha384cryptoserviceprovider"))
						{
							goto IL_04A8;
						}
						goto IL_047E;
					}
				}
				else if (num <= 2672027822U)
				{
					if (num != 2631153146U)
					{
						if (num != 2672027822U)
						{
							goto IL_04A8;
						}
						if (!(text == "tripledeskeywrap"))
						{
							goto IL_04A8;
						}
						return "1.2.840.113549.1.9.16.3.6";
					}
					else
					{
						if (!(text == "sha256"))
						{
							goto IL_04A8;
						}
						goto IL_0478;
					}
				}
				else if (num != 2694049387U)
				{
					if (num != 2700614742U)
					{
						goto IL_04A8;
					}
					if (!(text == "sha384"))
					{
						goto IL_04A8;
					}
					goto IL_047E;
				}
				else
				{
					if (!(text == "sha512"))
					{
						goto IL_04A8;
					}
					goto IL_0484;
				}
			}
			else if (num <= 3529085699U)
			{
				if (num <= 2930817943U)
				{
					if (num != 2803157229U)
					{
						if (num != 2930817943U)
						{
							goto IL_04A8;
						}
						if (!(text == "system.security.cryptography.sha1"))
						{
							goto IL_04A8;
						}
					}
					else
					{
						if (!(text == "system.security.cryptography.sha256cng"))
						{
							goto IL_04A8;
						}
						goto IL_0478;
					}
				}
				else if (num != 3457114317U)
				{
					if (num != 3529085699U)
					{
						goto IL_04A8;
					}
					if (!(text == "des"))
					{
						goto IL_04A8;
					}
					return "1.3.14.3.2.7";
				}
				else
				{
					if (!(text == "system.security.cryptography.sha512managed"))
					{
						goto IL_04A8;
					}
					goto IL_0484;
				}
			}
			else if (num <= 3991591796U)
			{
				if (num != 3708241362U)
				{
					if (num != 3991591796U)
					{
						goto IL_04A8;
					}
					if (!(text == "system.security.cryptography.sha512"))
					{
						goto IL_04A8;
					}
					goto IL_0484;
				}
				else
				{
					if (!(text == "system.security.cryptography.sha512cng"))
					{
						goto IL_04A8;
					}
					goto IL_0484;
				}
			}
			else if (num != 4181060418U)
			{
				if (num != 4199782769U)
				{
					goto IL_04A8;
				}
				if (!(text == "system.security.cryptography.sha384"))
				{
					goto IL_04A8;
				}
				goto IL_047E;
			}
			else
			{
				if (!(text == "system.security.cryptography.sha384managed"))
				{
					goto IL_04A8;
				}
				goto IL_047E;
			}
			IL_046C:
			return "1.3.14.3.2.26";
			IL_0478:
			return "2.16.840.1.101.3.4.2.1";
			IL_047E:
			return "2.16.840.1.101.3.4.2.2";
			IL_0484:
			return "2.16.840.1.101.3.4.2.3";
			IL_048A:
			return "1.3.36.3.2.1";
			IL_04A8:
			return null;
		}

		private static void Initialize()
		{
			CryptoConfig.algorithms = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
		}

		public static void AddAlgorithm(Type algorithm, params string[] names)
		{
			if (algorithm == null)
			{
				throw new ArgumentNullException("algorithm");
			}
			if (!algorithm.IsVisible)
			{
				throw new ArgumentException("Algorithms added to CryptoConfig must be accessable from outside their assembly.", "algorithm");
			}
			if (names == null)
			{
				throw new ArgumentNullException("names");
			}
			string[] array = new string[names.Length];
			Array.Copy(names, array, array.Length);
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				if (string.IsNullOrEmpty(array2[i]))
				{
					throw new ArgumentException("CryptoConfig cannot add a mapping for a null or empty name.");
				}
			}
			object obj = CryptoConfig.lockObject;
			lock (obj)
			{
				if (CryptoConfig.algorithms == null)
				{
					CryptoConfig.Initialize();
				}
				foreach (string text in array)
				{
					CryptoConfig.algorithms[text] = algorithm;
				}
			}
		}

		public static byte[] EncodeOID(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			char[] array = new char[] { '.' };
			string[] array2 = str.Split(array);
			if (array2.Length < 2)
			{
				throw new CryptographicUnexpectedOperationException(Locale.GetText("OID must have at least two parts"));
			}
			byte[] array3 = new byte[str.Length];
			try
			{
				byte b = Convert.ToByte(array2[0]);
				byte b2 = Convert.ToByte(array2[1]);
				array3[2] = Convert.ToByte((int)(b * 40 + b2));
			}
			catch
			{
				throw new CryptographicUnexpectedOperationException(Locale.GetText("Invalid OID"));
			}
			int num = 3;
			for (int i = 2; i < array2.Length; i++)
			{
				long num2 = Convert.ToInt64(array2[i]);
				if (num2 > 127L)
				{
					byte[] array4 = CryptoConfig.EncodeLongNumber(num2);
					Buffer.BlockCopy(array4, 0, array3, num, array4.Length);
					num += array4.Length;
				}
				else
				{
					array3[num++] = Convert.ToByte(num2);
				}
			}
			int num3 = 2;
			byte[] array5 = new byte[num];
			array5[0] = 6;
			if (num > 127)
			{
				throw new CryptographicUnexpectedOperationException(Locale.GetText("OID > 127 bytes"));
			}
			array5[1] = Convert.ToByte(num - 2);
			Buffer.BlockCopy(array3, num3, array5, num3, num - num3);
			return array5;
		}

		private static byte[] EncodeLongNumber(long x)
		{
			if (x > 2147483647L || x < -2147483648L)
			{
				throw new OverflowException(Locale.GetText("Part of OID doesn't fit in Int32"));
			}
			long num = x;
			int num2 = 1;
			while (num > 127L)
			{
				num >>= 7;
				num2++;
			}
			byte[] array = new byte[num2];
			for (int i = 0; i < num2; i++)
			{
				num = x >> 7 * i;
				num &= 127L;
				if (i != 0)
				{
					num += 128L;
				}
				array[num2 - i - 1] = Convert.ToByte(num);
			}
			return array;
		}

		[MonoLimitation("nothing is FIPS certified so it never make sense to restrict to this (empty) subset")]
		public static bool AllowOnlyFipsAlgorithms
		{
			get
			{
				return false;
			}
		}

		private static readonly object lockObject = new object();

		private static Dictionary<string, Type> algorithms;
	}
}
