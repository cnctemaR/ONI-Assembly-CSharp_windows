using System;

namespace System.Security.Cryptography.Xml
{
	internal static class CryptoHelpers
	{
		public static object CreateFromKnownName(string name)
		{
			uint num = global::<PrivateImplementationDetails>.ComputeStringHash(name);
			if (num <= 1735592375U)
			{
				if (num <= 877368883U)
				{
					if (num <= 334230854U)
					{
						if (num != 59013920U)
						{
							if (num != 118971470U)
							{
								if (num == 334230854U)
								{
									if (name == "http://www.w3.org/2000/09/xmldsig#enveloped-signature")
									{
										return new XmlDsigEnvelopedSignatureTransform();
									}
								}
							}
							else if (name == "http://www.w3.org/2000/09/xmldsig# RetrievalMethod")
							{
								return new KeyInfoRetrievalMethod();
							}
						}
						else if (name == "System.Security.Cryptography.RSASignatureDescription")
						{
							return new RSAPKCS1SHA1SignatureDescription();
						}
					}
					else if (num != 635407831U)
					{
						if (num != 699966473U)
						{
							if (num == 877368883U)
							{
								if (name == "http://www.w3.org/2000/09/xmldsig#rsa-sha1")
								{
									return new RSAPKCS1SHA1SignatureDescription();
								}
							}
						}
						else if (name == "http://www.w3.org/2000/09/xmldsig#dsa-sha1")
						{
							return new DSASignatureDescription();
						}
					}
					else if (name == "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments")
					{
						return new XmlDsigC14NWithCommentsTransform();
					}
				}
				else if (num <= 983015964U)
				{
					if (num != 944534123U)
					{
						if (num != 965923590U)
						{
							if (num == 983015964U)
							{
								if (name == "http://www.w3.org/2000/09/xmldsig# KeyValue/RSAKeyValue")
								{
									return new RSAKeyValue();
								}
							}
						}
						else if (name == "http://www.w3.org/2000/09/xmldsig#sha1")
						{
							return SHA1.Create();
						}
					}
					else if (name == "http://www.w3.org/2001/10/xml-exc-c14n#WithComments")
					{
						return new XmlDsigExcC14NWithCommentsTransform();
					}
				}
				else if (num <= 1059040347U)
				{
					if (num != 1000591612U)
					{
						if (num == 1059040347U)
						{
							if (name == "urn:mpeg:mpeg21:2003:01-REL-R-NS:licenseTransform")
							{
								return new XmlLicenseTransform();
							}
						}
					}
					else if (name == "http://www.w3.org/2000/09/xmldsig#base64")
					{
						return new XmlDsigBase64Transform();
					}
				}
				else if (num != 1596384045U)
				{
					if (num == 1735592375U)
					{
						if (name == "http://www.w3.org/2001/04/xmlenc#tripledes-cbc")
						{
							return TripleDES.Create();
						}
					}
				}
				else if (name == "http://www.w3.org/2001/04/xmldsig-more#hmac-md5")
				{
					return new HMACMD5();
				}
			}
			else if (num <= 2824063256U)
			{
				if (num <= 2297147784U)
				{
					if (num != 1762848122U)
					{
						if (num != 1935726387U)
						{
							if (num == 2297147784U)
							{
								if (name == "http://www.w3.org/TR/2001/REC-xml-c14n-20010315")
								{
									return new XmlDsigC14NTransform();
								}
							}
						}
						else if (name == "MD5")
						{
							return MD5.Create();
						}
					}
					else if (name == "System.Security.Cryptography.DSASignatureDescription")
					{
						return new DSASignatureDescription();
					}
				}
				else if (num <= 2540099133U)
				{
					if (num != 2468637947U)
					{
						if (num == 2540099133U)
						{
							if (name == "http://www.w3.org/2002/07/decrypt#XML")
							{
								return new XmlDecryptionTransform();
							}
						}
					}
					else if (name == "http://www.w3.org/2000/09/xmldsig# KeyName")
					{
						return new KeyInfoName();
					}
				}
				else if (num != 2587789385U)
				{
					if (num == 2824063256U)
					{
						if (name == "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512")
						{
							return new RSAPKCS1SHA512SignatureDescription();
						}
					}
				}
				else if (name == "http://www.w3.org/TR/1999/REC-xpath-19991116")
				{
					return new XmlDsigXPathTransform();
				}
			}
			else if (num <= 3339968437U)
			{
				if (num != 2918801765U)
				{
					if (num != 3058940555U)
					{
						if (num == 3339968437U)
						{
							if (name == "http://www.w3.org/2001/04/xmldsig-more#rsa-sha384")
							{
								return new RSAPKCS1SHA384SignatureDescription();
							}
						}
					}
					else if (name == "http://www.w3.org/TR/1999/REC-xslt-19991116")
					{
						return new XmlDsigXsltTransform();
					}
				}
				else if (name == "http://www.w3.org/2001/10/xml-exc-c14n#")
				{
					return new XmlDsigExcC14NTransform();
				}
			}
			else if (num <= 3880483293U)
			{
				if (num != 3756823938U)
				{
					if (num == 3880483293U)
					{
						if (name == "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256")
						{
							return new RSAPKCS1SHA256SignatureDescription();
						}
					}
				}
				else if (name == "http://www.w3.org/2000/09/xmldsig# KeyValue/DSAKeyValue")
				{
					return new DSAKeyValue();
				}
			}
			else if (num != 4131741765U)
			{
				if (num == 4253998307U)
				{
					if (name == "http://www.w3.org/2001/04/xmlenc# EncryptedKey")
					{
						return new KeyInfoEncryptedKey();
					}
				}
			}
			else if (name == "http://www.w3.org/2000/09/xmldsig# X509Data")
			{
				return new KeyInfoX509Data();
			}
			return null;
		}

		public static T CreateFromName<T>(string name) where T : class
		{
			T t;
			if (name == null || name.IndexOfAny(CryptoHelpers._invalidChars) >= 0)
			{
				t = default(T);
				return t;
			}
			try
			{
				t = (CryptoHelpers.CreateFromKnownName(name) ?? CryptoConfig.CreateFromName(name)) as T;
			}
			catch (Exception)
			{
				t = default(T);
			}
			return t;
		}

		private static readonly char[] _invalidChars = new char[] { ',', '`', '[', '*', '&' };
	}
}
